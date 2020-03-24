﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using ActiveStorage.Internal;
using ActiveStorage.Sql;

namespace ActiveStorage.Tests.Fixtures
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
			return new SqlObjectCountStore(ConnectionString, _dialect);
		}

		public IObjectSaveStore GetSaveStore()
		{
			return new SqlObjectSaveStore(ConnectionString, _dialect,
				new AttributeDataInfoProvider(),
				new CreatedAtTransform(() => DateTimeOffset.UtcNow));
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