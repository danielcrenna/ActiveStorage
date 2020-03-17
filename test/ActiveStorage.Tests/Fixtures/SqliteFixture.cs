// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using ActiveStorage.Sqlite;
using ActiveStorage.Tests.Internal;
using Microsoft.Data.Sqlite;

namespace ActiveStorage.Tests.Fixtures
{
	public class SqliteFixture<TMigrationInfo> : SqlFixture
	{
		public SqliteFixture() : base(new SqliteConnection($"Data Source={Guid.NewGuid()}.db"), new SqliteDialect())
		{
			var runner = new SqliteMigrationRunner(Connection.ConnectionString);
			runner.CreateDatabaseIfNotExists();
			runner.MigrateUp(typeof(TMigrationInfo).Assembly, typeof(TMigrationInfo).Namespace);
		}

		protected override void Dispose(bool disposing)
		{
			if (Connection is SqliteConnection connection)
			{
				var dataSource = connection.DataSource;

				base.Dispose(disposing);

				switch (Connection)
				{
					case SqliteConnection _ when dataSource != null:
						try
						{
							File.Delete(dataSource);
						}
						catch (Exception e)
						{
							Trace.TraceError(e.ToString());
						}

						break;
				}
			}
			else
			{
				base.Dispose(disposing);
			}
		}
	}
}