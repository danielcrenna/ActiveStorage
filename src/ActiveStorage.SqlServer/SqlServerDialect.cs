// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ActiveStorage.Sql;
using Dapper;

namespace ActiveStorage.SqlServer
{
	public class SqlServerDialect : ISqlDialect
	{
		public char? StartIdentifier => '[';
		public char? EndIdentifier => ']';
		public char? Separator => '.';
		public char? Parameter => '@';
		public char? Quote => '\'';

		public bool TryFetchInsertedKey(FetchInsertedKeyLocation location, out string sql)
		{
			switch (location)
			{
				case FetchInsertedKeyLocation.BeforeValues:
					sql = "OUTPUT Inserted.Id";
					return true;
				case FetchInsertedKeyLocation.AfterStatement:
					sql = null;
					return false;
				default:
					sql = null;
					return false;
			}
		}

		public async Task<T> QuerySingleAsync<T>(string connectionString, string sql,
			Dictionary<string, object> parameters)
		{
			await using var db = new SqlConnection(connectionString);
			var result = await db.QuerySingleAsync<T>(sql, parameters);
			return result;
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string connectionString, string sql,
			Dictionary<string, object> parameters)
		{
			await using var db = new SqlConnection(connectionString);
			var result = await db.QueryAsync<T>(sql, parameters);
			return result;
		}

		public async Task<int> ExecuteAsync(string connectionString, string sql, Dictionary<string, object> parameters)
		{
			await using var db = new SqlConnection(connectionString);
			var result = await db.ExecuteAsync(sql, parameters);
			return result;
		}
	}
}