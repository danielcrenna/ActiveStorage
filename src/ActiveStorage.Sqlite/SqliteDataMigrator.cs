// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ActiveStorage.Sql.Builders;

namespace ActiveStorage.Sqlite
{
	internal sealed class SqliteDataMigrator : IDataMigrator
	{
		private readonly string _connectionString;
		private readonly IDataInfoProvider _infoProvider;
		private readonly IEnumerable<IDataMigratorInfoProvider> _providers;

		public SqliteDataMigrator(string connectionString, 
			IDataInfoProvider infoProvider,
			IEnumerable<IDataMigratorInfoProvider> providers)
		{
			_connectionString = connectionString;
			_infoProvider = infoProvider;
			_providers = providers;
		}

		public async Task UpAsync(CancellationToken cancellationToken = default)
		{
			var runner = new SqliteMigrationRunner(_connectionString);
			runner.CreateDatabaseIfNotExists();

			var dialect = new SqliteDialect();

			var sequence = 0L;
			foreach(var provider in _providers)
			{
				foreach(var subject in provider.GetMigrationSubjects())
				{
					var sql = dialect.Up(subject, ++sequence, _infoProvider);
					var parameters = new Dictionary<string, object>();
					await dialect.ExecuteAsync(_connectionString, sql, parameters, cancellationToken);
				}
			}
		}
	}
}