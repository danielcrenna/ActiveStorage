// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;

namespace ActiveStorage.Tests.Fixtures
{
	public abstract class SqlFixture : IDbConnection
	{
		protected readonly IDbConnection Connection;

		protected SqlFixture(IDbConnection connection)
		{
			Connection = connection;
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

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			Connection?.Close();
			Connection?.Dispose();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}