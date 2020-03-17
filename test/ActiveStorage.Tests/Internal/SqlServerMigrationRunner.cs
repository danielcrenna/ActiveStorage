// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using FluentMigrator.Runner;

namespace ActiveStorage.Tests.Internal
{
	internal sealed class SqlServerMigrationRunner : MigrationRunner
	{
		public SqlServerMigrationRunner(string connectionString) : base(connectionString) { }

		public override void CreateDatabaseIfNotExists()
		{
			var builder = new SqlConnectionStringBuilder(ConnectionString);
			var fileName = DatabaseFileName(builder.InitialCatalog);
			if (File.Exists(fileName))
				return;

			var connection = new SqlConnection(builder.ConnectionString);
			connection.Open();
			connection.Close();

			Debug.Assert(File.Exists(fileName));
		}

		public static string DatabaseFileName(string databaseName)
		{
			return Path.Combine(EnsureOutputDir(), $"{databaseName}.mdb");
		}

		public static string EnsureOutputDir()
		{
			var outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (outputDir == null)
				throw new InvalidOperationException();
			Directory.CreateDirectory(outputDir);
			return outputDir;
		}

		public override void ConfigureMigrator(IMigrationRunnerBuilder builder)
		{
			builder.AddSqlServer();
		}
	}
}