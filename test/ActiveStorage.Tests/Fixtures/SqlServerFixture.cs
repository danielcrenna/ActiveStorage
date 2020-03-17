// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using ActiveStorage.SqlServer;
using ActiveStorage.Tests.Internal;
using Dapper;

namespace ActiveStorage.Tests.Fixtures
{
	public class SqlServerFixture<TMigrationInfo> : SqlFixture
	{
		public SqlServerFixture() : base(new SqlConnection(CreateConnectionString()), new SqlServerDialect())
		{
			var runner = new SqlServerMigrationRunner(Connection.ConnectionString);
			runner.CreateDatabaseIfNotExists();
			runner.MigrateUp(typeof(TMigrationInfo).Assembly, typeof(TMigrationInfo).Namespace);
		}

		private static string CreateConnectionString()
		{
			var database = $"{Guid.NewGuid().ToString("N").ToUpperInvariant()}";
			var builder = new SqlConnectionStringBuilder("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=true;MultipleActiveResultSets=true") {InitialCatalog = "master"};
			using (var connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();

				var fileName = SqlServerMigrationRunner.DatabaseFileName(database);

				connection.Execute($"CREATE DATABASE [{database}] ON (NAME = N'{database}', FILENAME = '{fileName}')");
				connection.Execute($"ALTER DATABASE [{database}] SET READ_COMMITTED_SNAPSHOT ON;");
				connection.Execute($"ALTER DATABASE [{database}] SET ALLOW_SNAPSHOT_ISOLATION ON;");
			}
			builder.InitialCatalog = database;
			return builder.ConnectionString;
		}

		protected override void Dispose(bool disposing)
		{
			var connectionString = ConnectionString;
			var database = Database;

			base.Dispose(disposing);

			var builder = new SqlConnectionStringBuilder(connectionString) {InitialCatalog = "master"};
			using (var db = new SqlConnection(builder.ConnectionString))
			{
				db.Open();
				try
				{
					db.Execute($"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
					db.Execute($"DROP DATABASE [{database}]");
				}
				catch (SqlException e)
				{
					Trace.TraceError(e.ToString());
				}
			}

			switch (Connection)
			{
				case SqlConnection connection when connection.DataSource != null:
					try
					{
						File.Delete(SqlServerMigrationRunner.DatabaseFileName(database));
					}
					catch (Exception e)
					{
						Trace.TraceError(e.ToString());
					}
					break;
			}
		}
	}
}