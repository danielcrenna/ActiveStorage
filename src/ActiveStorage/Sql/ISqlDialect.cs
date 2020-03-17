// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	public interface ISqlDialect
	{
		char? StartIdentifier { get; }
		char? EndIdentifier { get; }
		char? Separator { get; }
		char? Parameter { get; }
		char? Quote { get; }

		Task<IEnumerable<T>> QueryAsync<T>(string connectionString, string sql,
			Dictionary<string, object> parameters = null);

		Task<T> QuerySingleAsync<T>(string connectionString, string sql, Dictionary<string, object> parameters = null);
		Task<int> ExecuteAsync(string connectionString, string sql, Dictionary<string, object> parameters = null);

		bool TryFetchInsertedKey(FetchInsertedKeyLocation location, out string sql);

		string ResolveTableName(AccessorMembers members)
		{
			return !members.DeclaringType.TryGetAttribute(true, out TableAttribute attribute)
				? members.DeclaringType.GetNonGenericName()
				: attribute.Name;
		}

		string ResolveSchemaName(AccessorMembers members)
		{
			return !members.DeclaringType.TryGetAttribute(true, out TableAttribute attribute) ? null : attribute.Schema;
		}

		string ResolveColumnName(AccessorMember member)
		{
			return !member.TryGetAttribute(out ColumnAttribute attribute) ? member.Name : attribute.Name;
		}
	}
}