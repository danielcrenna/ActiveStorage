// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using ActiveLogging;
using ActiveStorage.Internal;
using ActiveStorage.Sql;
using ActiveStorage.Tests.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;

namespace ActiveStorage.Tests
{
	public abstract class SqlFixture : ISqlStoreFixture
	{
		private readonly ISqlDialect _dialect;
		protected readonly IDbConnection Connection;

		protected SqlFixture(IDbConnection connection, ISqlDialect dialect)
		{
			Connection = connection;
			_dialect = dialect;
		}

		public IDbTransaction BeginTransaction()
		{
			return Connection.BeginTransaction();
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return Connection.BeginTransaction(il);
		}

		public void ChangeDatabase(string databaseName)
		{
			Connection.ChangeDatabase(databaseName);
		}

		public void Close()
		{
			Connection.Close();
		}

		public IDbCommand CreateCommand()
		{
			return Connection.CreateCommand();
		}

		public void Open()
		{
			Connection.Open();
		}

		public string ConnectionString
		{
			get => Connection.ConnectionString;
			set => Connection.ConnectionString = value;
		}

		public int ConnectionTimeout => Connection.ConnectionTimeout;

		public string Database => Connection.Database;

		public ConnectionState State => Connection.State;

		public void Dispose()
		{
			Dispose(true);
		}

		public IObjectCountStore GetCountStore()
		{
			return new SqlObjectCountStore(ConnectionString, _dialect, new SafeLogger<SqlObjectCountStore>(new NullLogger<SqlObjectCountStore>()));
		}

		public IObjectAppendStore GetAppendStore()
		{
			return new SqlObjectAppendStore(ConnectionString, _dialect, new AttributeDataInfoProvider(), new SafeLogger<SqlObjectAppendStore>(new NullLogger<SqlObjectAppendStore>()));
		}

		public ISingleObjectQueryByExampleStore GetSingleObjectQueryByExampleStore()
		{
			return new SqlSingleObjectQueryByExampleStore(ConnectionString, _dialect, new SafeLogger<SqlSingleObjectQueryByExampleStore>(new NullLogger<SqlSingleObjectQueryByExampleStore>()));
		}

		public IObjectSaveStore GetSaveStore()
		{
			var logger = new SafeLogger<SqlObjectSaveStore>(new NullLogger<SqlObjectSaveStore>());
			var transform = new CreatedAtTransform(() => DateTimeOffset.UtcNow);
			var store = new SqlObjectSaveStore(ConnectionString, _dialect, new AttributeDataInfoProvider(), logger, transform);
			return store;
		}

		public IObjectAppendStore GetCreateStore()
		{
			var transform = new CreatedAtTransform(() => DateTimeOffset.UtcNow);
			var logger = new SafeLogger<SqlObjectAppendStore>(new NullLogger<SqlObjectAppendStore>());
			var store = new SqlObjectAppendStore(ConnectionString, _dialect, new AttributeDataInfoProvider(), logger, transform);
			return store;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			Connection?.Close();
			Connection?.Dispose();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}