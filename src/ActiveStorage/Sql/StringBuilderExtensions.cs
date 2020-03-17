// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace ActiveStorage.Sql
{
	internal static class StringBuilderExtensions
	{
		public static StringBuilder AppendTable(this StringBuilder sb, ISqlDialect d, string table, string schema)
		{
			if (!string.IsNullOrWhiteSpace(schema))
				sb.AppendName(d, schema).Append(d.Separator);
			sb.AppendName(d, table);
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
	}
}