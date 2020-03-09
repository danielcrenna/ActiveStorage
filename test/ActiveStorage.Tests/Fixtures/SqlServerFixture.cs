// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using ActiveStorage.Tests.Internal;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ActiveStorage.Tests.Fixtures
{
	public class SqlServerFixture<TMigrationInfo> : SqlFixture, IDatabaseFixture
	{
		public SqlServerFixture(string connectionString) : base(new SqliteConnection(connectionString))
		{
			var runner = new SqlServerMigrationRunner(connectionString);
			runner.CreateDatabaseIfNotExists();
			runner.MigrateUp(typeof(TMigrationInfo).Assembly, typeof(TMigrationInfo).Namespace);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			var builder = new SqlConnectionStringBuilder(ConnectionString) {InitialCatalog = "master"};
			using (var db = new SqlConnection(builder.ConnectionString))
			{
				db.Open();
				try
				{
					db.Execute($"ALTER DATABASE [{Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
					db.Execute($"DROP DATABASE [{Database}]");
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
						File.Delete($"{Database}.mdf");
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