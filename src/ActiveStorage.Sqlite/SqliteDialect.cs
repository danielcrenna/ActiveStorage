// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using ActiveStorage.Sql;
using ActiveStorage.Sqlite.Internal;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ActiveStorage.Sqlite
{
	public sealed class SqliteDialect : SqlCommands<SqliteConnection, SqliteException>, ISqlDialect
	{
		static SqliteDialect()
		{
			SqlMapper.AddTypeHandler(DateTimeOffsetHandler.Default);
		}

		public char? StartIdentifier => '\"';
		public char? EndIdentifier => '\"';
		public char? Separator => '.';
		public char? Parameter => ':';
		public char? Quote => '\'';
		
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

		public string ResolveTypeName(Type type)
		{
			if (StorageClasses.RealNumberTypes.Contains(type))
				return "REAL";

			if (StorageClasses.TextTypes.Contains(type))
				return "TEXT";

			if (StorageClasses.IntegerTypes.Contains(type))
				return "INTEGER";

			if (StorageClasses.DateTypes.Contains(type))
				return "TEXT"; // ISO-8601: "YYYY-MM-DD HH:MM:SS.SSS"

			if (StorageClasses.BooleanTypes.Contains(type))
				return "INTEGER"; // 0 or 1 
			
			if (type.IsEnum)
				return "TEXT"; // rely on Dapper to map strings to enum members

			return "BLOB";
		}

		public string ResolveTypeDefaultLength(Type type)
		{
			return string.Empty; // Irrelevant, see: https://www.sqlite.org/limits.html
		}

		

		public SqliteDialect() : base(cs => new SqliteConnection(cs)) { }
		public SqliteDialect(Func<string, SqliteConnection> connectionFactory) : base(connectionFactory) { }
	}
}