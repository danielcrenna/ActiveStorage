// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveStorage.Sql.Builders;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	public sealed class SqlObjectSaveStore : IObjectSaveStore
	{
		private readonly string _connectionString;
		private readonly ISqlDialect _dialect;

		private readonly IEnumerable<IFieldTransform> _fieldTransforms;
		private readonly IDataInfoProvider _provider;

		public SqlObjectSaveStore(string connectionString, ISqlDialect dialect, IDataInfoProvider provider,
			params IFieldTransform[] fieldTransforms)
		{
			_connectionString = connectionString;
			_dialect = dialect;
			_fieldTransforms = fieldTransforms;
			_provider = provider;
		}

		public async Task<Operation<ObjectSave>> SaveAsync(object @object, params string[] fields)
		{
			var accessor = ReadAccessor.Create(@object, AccessorMemberTypes.Properties, AccessorMemberScope.Public,
				out var members);

			var hash = members.ToDictionary(k => k.Name, v =>
			{
				foreach (var field in _fieldTransforms)
					if (field.TryTransform(accessor, @object, v, out var transformed))
						return transformed;

				accessor.TryGetValue(@object, v.Name, out var untransformed);
				return untransformed;
			});

			var columns = Pooling.ListPool<AccessorMember>.Get();
			try
			{
				if (fields?.Length > 0)
				{
					foreach (var field in fields)
					{
						if (members.TryGetValue(field, out var member) && !_provider.IsIgnored(member) &&
						    _provider.IsSaved(member))
						{
							columns.Add(member);
						}
					}
				}
				else
				{
					foreach (var member in members)
					{
						if (_provider.IsIgnored(member))
						{
							columns.Remove(member);
							continue;
						}

						if (_provider.IsSaved(member))
						{
							columns.Add(member);
						}
					}
				}

				var sql = _dialect.InsertInto(members, columns, false);
				var parameters = columns.ToDictionary(k => $"{_dialect.Parameter}{_dialect.ResolveColumnName(k)}",
					v => hash[v.Name]);
				var inserted = await _dialect.ExecuteAsync(_connectionString, sql, parameters);

				Debug.Assert(inserted == 1);
			}
			finally
			{
				Pooling.ListPool<AccessorMember>.Return(columns);
			}

			return Operation.FromResult(ObjectSave.Created);
		}
	}
}