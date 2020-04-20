// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ActiveStorage.Sqlite.Internal
{
	internal sealed class EventIdHandler : SqlMapper.TypeHandler<EventId>
	{
		public static readonly SqlMapper.ITypeHandler Default = new EventIdHandler();

		private EventIdHandler() { }
		
		public override EventId Parse(object value)
		{
			if (value == null || !(value is string valueString) || string.IsNullOrWhiteSpace(valueString))
				return default;

			var tokens = valueString.Split('.', StringSplitOptions.RemoveEmptyEntries);

			var id = int.Parse(tokens[0]);
			var name = tokens[1];

			return new EventId(id, name);
		}

		public override void SetValue(IDbDataParameter parameter, EventId value)
		{
			parameter.DbType = DbType.String;
			parameter.Value = $"{value.Id}.{value.Name}";
		}
	}
}