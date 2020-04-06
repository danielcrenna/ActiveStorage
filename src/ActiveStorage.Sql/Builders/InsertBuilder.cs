// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using TypeKitchen;

namespace ActiveStorage.Sql.Builders
{
	internal static class InsertBuilder
	{
		public static string InsertInto(this ISqlDialect d, AccessorMembers members, IEnumerable<AccessorMember> columns, int columnCount, bool returnKeys)
		{
			return Pooling.StringBuilderPool.Scoped(sb =>
			{
				sb.Append("INSERT INTO ");
				
				sb.AppendTable(d, members).Append(" (")
					// ReSharper disable once PossibleMultipleEnumeration
					.AppendColumnNames(d, columns, columnCount)
					.Append(") ");

				if (returnKeys &&
				    d.TryFetchInsertedKey(FetchInsertedKeyLocation.BeforeValues, out var fetchBeforeValues))
					sb.Append(fetchBeforeValues).Append(" ");

				sb.Append("VALUES (")
					// ReSharper disable once PossibleMultipleEnumeration
					.AppendColumnNames(d, columns, columnCount)
					.Append(") ");

				if (returnKeys && d.TryFetchInsertedKey(FetchInsertedKeyLocation.AfterStatement, out var fetchAfterStatement))
				{
					sb.Append("; ");
					sb.Append(fetchAfterStatement);
				}
			});
		}
	}
}