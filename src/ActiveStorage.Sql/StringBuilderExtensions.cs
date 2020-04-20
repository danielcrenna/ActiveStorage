// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	internal static class StringBuilderExtensions
	{
		public static StringBuilder Append(this StringBuilder sb, int tabs, string value) => sb.Append(TabString(tabs)).Append(value);
		public static StringBuilder AppendLine(this StringBuilder sb, int tabs, string value) => sb.Append(Enumerable.Repeat("    ", tabs)).AppendLine(value);

		public static StringBuilder AppendTable(this StringBuilder sb, ISqlDialect d, AccessorMembers members) => sb.AppendTable(d, d.ResolveTableName(members), d.ResolveSchemaName(members));
		public static StringBuilder AppendTable(this StringBuilder sb, ISqlDialect d, string table, string schema)
		{
			if (!string.IsNullOrWhiteSpace(schema))
				sb.AppendName(d, schema).Append(d.Separator);
			sb.AppendName(d, table);
			return sb;
		}

		public static StringBuilder AppendColumnNames(this StringBuilder sb, ISqlDialect d, IEnumerable<AccessorMember> columns, int columnCount)
		{
			var i = 0;
			foreach (var column in columns)
			{
				sb.AppendName(d, d.ResolveColumnName(column));
				if (i < columnCount - 1)
					sb.Append(", ");
				i++;
			}
			return sb;
		}

		public static StringBuilder AppendColumnParameters(this StringBuilder sb, ISqlDialect d, IEnumerable<AccessorMember> columns, int columnCount)
		{
			var i = 0;
			foreach (var column in columns)
			{
				sb.AppendParameter(d, d.ResolveColumnName(column));
				if (i < columnCount - 1)
					sb.Append(", ");
				i++;
			}
			return sb;
		}

		public static StringBuilder AppendName(this StringBuilder sb, ISqlDialect d, object value)
		{
			return sb.Append(d.StartIdentifier).Append(value).Append(d.EndIdentifier);
		}

		public static StringBuilder AppendParameter(this StringBuilder sb, ISqlDialect d, object value)
		{
			return sb.Append(d.Parameter).Append(value);
		}

		public static StringBuilder AppendWhereClause(this StringBuilder sb, ISqlDialect d, IReadOnlyDictionary<AccessorMember, object> hash)
		{
			var i = 0;
			foreach(var (k, v) in hash)
			{
				sb.Append(i == 0 ? " WHERE " : " AND ");
				sb.AppendName(d, k.Name)
					.Append(" = ")
					.AppendParameter(d, v);
				i++;
			}
			return sb;
		}

		private static string TabString(int tabs) => new string(' ', tabs * 4);
	}
}