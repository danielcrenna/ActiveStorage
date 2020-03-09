// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.IO;
using ActiveStorage.Sqlite.Internal;
using ActiveStorage.Tests.Internal;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ActiveStorage.Tests.Fixtures
{
	public class SqliteFixture<TMigrationInfo> : IDisposable, IDbConnection
	{
		private readonly SqliteConnection _connection;

		static SqliteFixture() => SqlMapper.AddTypeHandler(DateTimeOffsetHandler.Default);

		public SqliteFixture()
		{
			var connectionString = $"Data Source={Guid.NewGuid()}.db";

			var runner = new SqliteMigrationRunner(connectionString);
			runner.CreateDatabaseIfNotExists();
			runner.MigrateUp(typeof(TMigrationInfo).Assembly, typeof(TMigrationInfo).Namespace);

			_connection = new SqliteConnection(connectionString);
		}

		public IDbTransaction BeginTransaction()
		{
			return ((IDbConnection) _connection).BeginTransaction();
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return ((IDbConnection) _connection).BeginTransaction(il);
		}

		public void ChangeDatabase(string databaseName)
		{
			_connection.ChangeDatabase(databaseName);
		}

		public void Close()
		{
			_connection.Close();
		}

		public IDbCommand CreateCommand()
		{
			return ((IDbConnection) _connection).CreateCommand();
		}

		public void Open()
		{
			_connection.Open();
		}

		public string ConnectionString
		{
			get => _connection.ConnectionString;
			set => _connection.ConnectionString = value;
		}

		public int ConnectionTimeout => _connection.ConnectionTimeout;

		public string Database => _connection.Database;

		public ConnectionState State => _connection.State;

		public void Dispose()
		{
			_connection?.Close();
			_connection?.Dispose();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			if (_connection?.DataSource != null)
				File.Delete(_connection.DataSource);
		}
	}
}