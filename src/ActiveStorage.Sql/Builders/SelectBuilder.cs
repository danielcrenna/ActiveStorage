// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using ActiveStorage.Sql.Internal;
using TypeKitchen;

namespace ActiveStorage.Sql.Builders
{
	internal static class SelectBuilder
	{
		public static string Select(this ISqlDialect d, AccessorMembers members, object example)
		{
			return d.Select(members, members.ToHash(example));
		}

		private static string Select(this ISqlDialect d, AccessorMembers members, IReadOnlyDictionary<AccessorMember, object> hash)
		{
			return Pooling.StringBuilderPool.Scoped(sb =>
			{
				sb.Append("SELECT ");
				sb.AppendColumnNames(d, hash.Keys, hash.Count);
				sb.Append(" FROM ");
				sb.AppendTable(d, members);
				sb.AppendWhereClause(d, hash);
			});
		}
	}
}