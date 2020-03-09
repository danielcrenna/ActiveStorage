// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using ActiveStorage.Tests.Internal;
using Microsoft.Data.Sqlite;

namespace ActiveStorage.Tests.Fixtures
{
	public class SqliteFixture<TMigrationInfo> : SqlFixture, IDatabaseFixture
	{
		public SqliteFixture(string connectionString) : base(new SqliteConnection(connectionString))
		{
			var runner = new SqliteMigrationRunner(connectionString);
			runner.CreateDatabaseIfNotExists();
			runner.MigrateUp(typeof(TMigrationInfo).Assembly, typeof(TMigrationInfo).Namespace);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			switch (Connection)
			{
				case SqliteConnection connection when connection.DataSource != null:
					try
					{
						File.Delete(connection.DataSource);
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