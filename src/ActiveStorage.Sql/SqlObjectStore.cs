// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ActiveLogging;
using ActiveStorage.Sql.Builders;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	public abstract class SqlObjectStore
	{
		protected readonly string ConnectionString;
		protected readonly ISqlDialect Dialect;
		protected readonly IEnumerable<IFieldTransform> FieldTransforms;
		protected readonly IDataInfoProvider Provider;
		protected readonly ISafeLogger Logger;

		protected SqlObjectStore(string connectionString, ISqlDialect dialect, IDataInfoProvider provider, ISafeLogger logger = null, params IFieldTransform[] fieldTransforms)
		{
			ConnectionString = connectionString;
			Dialect = dialect;
			FieldTransforms = fieldTransforms;
			Provider = provider;
			Logger = logger;
		}

		protected AccessorMembers BeforeCreate(object @object, out Dictionary<string, object> hash)
		{
			var accessor = ReadAccessor.Create(@object, AccessorMemberTypes.Properties, AccessorMemberScope.Public,
				out var members);

			hash = members.ToDictionary(k => k.Name, v =>
			{
				foreach (var field in FieldTransforms)
					if (field.TryTransform(accessor, @object, v, out var transformed))
						return transformed;

				accessor.TryGetValue(@object, v.Name, out var untransformed);
				return untransformed;
			});
			return members;
		}

		protected async Task InsertAsync(IReadOnlyDictionary<string, object> hash, IReadOnlyCollection<string> fields, AccessorMembers members)
		{
			var columns = Pooling.ListPool<AccessorMember>.Get();
			try
			{
				if (fields?.Count > 0)
				{
					foreach (var field in fields)
					{
						if (members.TryGetValue(field, out var member) && !Provider.IsIgnored(member) &&
						    Provider.IsSaved(member))
						{
							columns.Add(member);
						}
					}
				}
				else
				{
					foreach (var member in members)
					{
						if (Provider.IsIgnored(member))
						{
							columns.Remove(member);
							continue;
						}

						if (Provider.IsSaved(member))
						{
							columns.Add(member);
						}
					}
				}

				var sql = Dialect.InsertInto(members, columns, false);
				var parameters = columns.ToDictionary(k => $"{Dialect.Parameter}{Dialect.ResolveColumnName(k)}", v => hash[v.Name]);
				var inserted = await Dialect.ExecuteAsync(ConnectionString, sql, parameters);
				Debug.Assert(inserted == 1);
			}
			finally
			{
				Pooling.ListPool<AccessorMember>.Return(columns);
			}
		}
	}
}