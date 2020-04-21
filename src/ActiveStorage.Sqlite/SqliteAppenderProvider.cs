// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveStorage.Sql;

namespace ActiveStorage.Sqlite
{
	internal sealed class SqliteAppenderProvider : IAppenderProvider
	{
		private readonly string _slot;
		private readonly string _connectionString;
		private readonly IDataInfoProvider _dataInfoProvider;

		public SqliteAppenderProvider(string slot, string connectionString, IDataInfoProvider dataInfoProvider)
		{
			_slot = slot;
			_connectionString = connectionString;
			_dataInfoProvider = dataInfoProvider;
		}

		public IObjectAppendStore GetAppender(string slot)
		{
			if (_slot != slot)
				return default;
			var dialect = new SqliteDialect();
			return new SqlObjectAppendStore(_connectionString, dialect, _dataInfoProvider);
		}
	}
}