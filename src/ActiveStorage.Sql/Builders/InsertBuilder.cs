// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using TypeKitchen;

namespace ActiveStorage.Sql.Builders
{
	internal static class InsertBuilder
	{
		public static string InsertInto(this ISqlDialect d, AccessorMembers members, IList<AccessorMember> include,
			bool returnKeys)
		{
			return Pooling.StringBuilderPool.Scoped(sb =>
			{
				sb.Append("INSERT INTO ");

				var tableName = d.ResolveTableName(members);
				var schemaName = d.ResolveSchemaName(members);

				sb.AppendTable(d, tableName, schemaName).Append(" (");

				for (var i = 0; i < include.Count; i++)
				{
					var columnName = members.TryGetValue(include[i].Name, out var member)
						? d.ResolveColumnName(member)
						: include[i].Name;

					sb.AppendName(d, columnName);
					if (i < include.Count - 1)
						sb.Append(", ");
				}

				sb.Append(") ");

				if (returnKeys &&
				    d.TryFetchInsertedKey(FetchInsertedKeyLocation.BeforeValues, out var fetchBeforeValues))
					sb.Append(fetchBeforeValues).Append(" ");

				sb.Append("VALUES (");

				for (var i = 0; i < include.Count; i++)
				{
					var columnName = members.TryGetValue(include[i].Name, out var member)
						? d.ResolveColumnName(member)
						: include[i].Name;

					sb.AppendParameter(d, columnName);
					if (i < include.Count - 1)
						sb.Append(",");
				}

				sb.Append(")");

				if (returnKeys &&
				    d.TryFetchInsertedKey(FetchInsertedKeyLocation.AfterStatement, out var fetchAfterStatement))
					sb.Append("; ").Append(fetchAfterStatement);
			});
		}
	}
}