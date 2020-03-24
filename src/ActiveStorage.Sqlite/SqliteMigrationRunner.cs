// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using FluentMigrator.Runner;
using Microsoft.Data.Sqlite;
using MigrationRunner = ActiveStorage.Sql.MigrationRunner;

namespace ActiveStorage.Sqlite
{
	internal sealed class SqliteMigrationRunner : MigrationRunner
	{
		public SqliteMigrationRunner(string connectionString) : base(connectionString) { }

		public override void CreateDatabaseIfNotExists()
		{
			var builder = new SqliteConnectionStringBuilder(ConnectionString) {Mode = SqliteOpenMode.ReadWriteCreate};
			if (File.Exists(builder.DataSource))
				return;
			var connection = new SqliteConnection(builder.ConnectionString);
			connection.Open();
			connection.Close();
		}

		public override void ConfigureMigrator(IMigrationRunnerBuilder builder)
		{
			builder.AddSQLite();
		}
	}
}