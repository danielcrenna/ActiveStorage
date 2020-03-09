// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using ActiveStorage.Sql;
using ActiveStorage.Sqlite.Internal;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ActiveStorage.Sqlite
{
	public sealed class SqliteDialect : ISqlDialect
	{
		static SqliteDialect() => SqlMapper.AddTypeHandler(DateTimeOffsetHandler.Default);

		public char? StartIdentifier => '\"';
		public char? EndIdentifier => '\"';
		public char? Separator => '.';
		public char? Parameter => ':';
		public char? Quote => '\'';

		public async Task<int> ExecuteAsync(string connectionString, string sql, Dictionary<string, object> parameters)
		{
			await using var db = new SqliteConnection(connectionString);
			var result = await db.ExecuteAsync(sql, parameters);
			return result;
		}

		public bool TryFetchInsertedKey(FetchInsertedKeyLocation location, out string sql)
		{
			switch (location)
			{
				case FetchInsertedKeyLocation.AfterStatement:
					sql = $"SELECT LAST_INSERT_ROWID() AS {Quote}Id{Quote}";
					return true;
				case FetchInsertedKeyLocation.BeforeValues:
					sql = null;
					return false;
				default:
					sql = null;
					return false;
			}
		}
	}
}