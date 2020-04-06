// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace ActiveStorage.Sql
{
	public abstract class SqlCommands<TDbConnection, TDbException> : ISqlCommands 
		where TDbConnection : IDbConnection, IAsyncDisposable
		where TDbException : DbException
	{
		private readonly Func<string, TDbConnection> _connectionFactory;

		protected SqlCommands(Func<string, TDbConnection> connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public async Task<T> QuerySingleAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
		{
			try
			{
				await using var db = _connectionFactory(connectionString);
				var result = await db.QuerySingleAsync<T>(sql, parameters);
				return result;
			}
			catch (TDbException e)
			{
				throw OnQueryException(e);
			}
		}

		public async Task<T> QuerySingleOrDefaultAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				await using var db = _connectionFactory(connectionString);
				var result = await db.QuerySingleOrDefaultAsync<T>(sql, parameters);
				return result;
			}
			catch (TDbException e)
			{
				throw OnQueryException(e);
			}
		}

		public async Task<T> QueryFirstAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				await using var db = _connectionFactory(connectionString);
				var result = await db.QueryFirstAsync<T>(sql, parameters);
				return result;
			}
			catch (TDbException e)
			{
				throw OnQueryException(e);
			}
		}

		public async Task<T> QueryFirstOrDefaultAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				await using var db = _connectionFactory(connectionString);
				var result = await db.QueryFirstOrDefaultAsync<T>(sql, parameters);
				return result;
			}
			catch (TDbException e)
			{
				throw OnQueryException(e);
			}
		}
		
		public async Task<IEnumerable<T>> QueryAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
		{
			try
			{
				await using var db = _connectionFactory(connectionString);
				var result = await db.QueryAsync<T>(sql, parameters);
				return result;
			}
			catch (TDbException e)
			{
				throw OnQueryException(e);
			}
		}

		public async Task<int> ExecuteAsync(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
		{
			try
			{
				await using var db = _connectionFactory(connectionString);
				var result = await db.ExecuteAsync(sql, parameters);
				return result;
			}
			catch (TDbException e)
			{
				throw OnExecuteException(e);
			}
		}

		private static Exception OnExecuteException(TDbException e)
		{
			return new StorageException($"Error querying {nameof(TDbException).Replace(nameof(Exception), string.Empty)}", e);
		}

		private static Exception OnQueryException(TDbException e)
		{
			return new StorageException($"Error querying {nameof(TDbException).Replace(nameof(Exception), string.Empty)}", e);
		}

	}
}