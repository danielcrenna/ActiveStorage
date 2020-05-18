// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveStorage.Sql;
using Microsoft.Extensions.Logging;

namespace ActiveStorage.Sqlite
{
	internal sealed class SqliteCounterProvider : ICounterProvider
	{
		private readonly string _slot;
		private readonly string _connectionString;
		private readonly ILogger<SqlObjectCountStore> _logger;

		public SqliteCounterProvider(string slot, string connectionString, ILogger<SqlObjectCountStore> logger = null)
		{
			_slot = slot;
			_connectionString = connectionString;
			_logger = logger;
		}

		public IObjectCountStore GetCounter(string slot)
		{
			if (_slot != slot)
				return default;

			var dialect = new SqliteDialect();
			return new SqlObjectCountStore(_connectionString, dialect, _logger);
		}
	}
}