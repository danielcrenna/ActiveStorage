// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using TypeKitchen;

namespace ActiveStorage.Sql.Builders
{
	public static class CountBuilder
	{
		public static string Count(this ISqlDialect d, AccessorMembers members)
		{
			return Pooling.StringBuilderPool.Scoped(sb =>
			{
				sb.Append("SELECT ");
				sb.Append("COUNT(1) FROM ");
				sb.AppendTable(d, d.ResolveTableName(members), d.ResolveSchemaName(members));
			});
		}
	}
}