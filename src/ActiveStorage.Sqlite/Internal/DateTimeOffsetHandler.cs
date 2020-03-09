// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using Dapper;

namespace ActiveStorage.Sqlite.Internal
{
	internal sealed class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset?>
	{
		public static readonly DateTimeOffsetHandler Default = new DateTimeOffsetHandler();
		private DateTimeOffsetHandler() { }

		public override void SetValue(IDbDataParameter parameter, DateTimeOffset? value)
		{
			parameter.Value = value.HasValue ? (object) value.Value : DBNull.Value;
		}

		public override DateTimeOffset? Parse(object value)
		{
			return value switch
			{
				null => null,
				DateTimeOffset offset => offset,
				_ => Convert.ToDateTime(value)
			};
		}
	}
}