// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using ActiveStorage.Tests.Internal;
using Dapper;

namespace ActiveStorage.Tests.Fixtures
{
	public class SqlServerFixture<TMigrationInfo> : SqlFixture
	{
		private readonly string _connectionString;
		private readonly string _database;

		public SqlServerFixture(string connectionString) : base(new SqlConnection(connectionString))
		{
			var runner = new SqlServerMigrationRunner(connectionString);
			runner.CreateDatabaseIfNotExists();
			runner.MigrateUp(typeof(TMigrationInfo).Assembly, typeof(TMigrationInfo).Namespace);

			_connectionString = connectionString;
			_database = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			var builder = new SqlConnectionStringBuilder(_connectionString) {InitialCatalog = "master"};
			using (var db = new SqlConnection(builder.ConnectionString))
			{
				db.Open();
				try
				{
					db.Execute($"ALTER DATABASE [{_database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
					db.Execute($"DROP DATABASE [{_database}]");
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
						File.Delete(SqlServerMigrationRunner.DatabaseFileName(_database));
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