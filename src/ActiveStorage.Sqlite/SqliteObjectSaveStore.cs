// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveStorage.Sql;
using ActiveStorage.Sqlite.Internal;
using Dapper;
using Microsoft.Data.Sqlite;
using TypeKitchen;

namespace ActiveStorage.Sqlite
{
	public class SqliteObjectSaveStore : IObjectSaveStore
	{
		private readonly string _connectionString;
		private readonly ISqlDialect _dialect;
		private readonly IEnumerable<IFieldTransform> _fieldTransforms;
		private readonly IDataInfoProvider _provider;

		static SqliteObjectSaveStore() => SqlMapper.AddTypeHandler(DateTimeOffsetHandler.Default);

		public SqliteObjectSaveStore(string connectionString, IEnumerable<IFieldTransform> fieldTransforms,
			IDataInfoProvider provider = null, ISqlDialect dialect = null)
		{
			_connectionString = connectionString;
			_fieldTransforms = fieldTransforms;

			_provider = provider ?? new AttributeDataInfoProvider();
			_dialect = dialect ?? new SqliteDialect();
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

			var columns = new List<AccessorMember>();

			if (fields?.Length > 0)
			{
				foreach (var field in fields)
				{
					if (members.TryGetValue(field, out var member) && !_provider.IsIgnored(member) && _provider.IsSaved(member))
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
			
			await using var db = new SqliteConnection(_connectionString);

			var sql = _dialect.InsertInto(members, columns, false);
			var parameters = columns.ToDictionary(k => $"{_dialect.Parameter}{_dialect.ResolveColumnName(k)}", v => hash[v.Name]);
			var inserted = await db.ExecuteAsync(sql, parameters);
			Debug.Assert(inserted == 1);

			return Operation.FromResult(ObjectSave.Created);
		}
	}
}