// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveStorage.Sql;
using Microsoft.Extensions.Logging;

namespace ActiveStorage.Sqlite
{
	internal sealed class SqliteFetcherProvider : IFetcherProvider
	{
		private readonly string _slot;
		private readonly string _connectionString;
		private readonly IDataInfoProvider _provider;
		private readonly ILogger<SqlObjectFetchStore> _logger;

		public SqliteFetcherProvider(string slot, string connectionString, IDataInfoProvider provider, ILogger<SqlObjectFetchStore> logger = null)
		{
			_slot = slot;
			_connectionString = connectionString;
			_provider = provider;
			_logger = logger;
		}

		public IObjectFetchStore GetFetcher(string slot)
		{
			if (_slot != slot)
				return default;

			var dialect = new SqliteDialect();
			return new SqlObjectFetchStore(_connectionString, dialect, _provider, _logger);
		}
	}
}