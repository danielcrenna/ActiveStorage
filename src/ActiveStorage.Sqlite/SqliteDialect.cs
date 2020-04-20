// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Numerics;
using ActiveStorage.Sql;
using ActiveStorage.Sqlite.Internal;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Primitives;

namespace ActiveStorage.Sqlite
{
	public sealed class SqliteDialect : SqlCommands<SqliteConnection, SqliteException>, ISqlDialect
	{
		static SqliteDialect()
		{
			SqlMapper.AddTypeHandler(DateTimeOffsetHandler.Default);
			SqlMapper.AddTypeHandler(typeof(EventIdHandler), EventIdHandler.Default);
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
			if (RealNumberTypes.Contains(type))
				return "REAL";

			if (TextTypes.Contains(type))
				return "TEXT";

			if (IntegerTypes.Contains(type))
				return "INTEGER";

			if (DateTypes.Contains(type))
				return "TEXT"; // ISO-8601: "YYYY-MM-DD HH:MM:SS.SSS"

			if (BooleanTypes.Contains(type))
				return "INTEGER"; // 0 or 1 
			
			if (type.IsEnum)
				return "TEXT"; // rely on Dapper to map strings to enum members

			return "BLOB";
		}

		public string ResolveTypeDefaultLength(Type type)
		{
			return string.Empty; // Irrelevant, see: https://www.sqlite.org/limits.html
		}

		private static readonly HashSet<Type> BooleanTypes = new HashSet<Type>
		{
			typeof(bool),
			typeof(bool?)
		};

		private static readonly HashSet<Type> DateTypes = new HashSet<Type>
		{
			typeof(DateTime),
			typeof(DateTimeOffset),
			typeof(DateTime?),
			typeof(DateTimeOffset?)
		};

		private static readonly HashSet<Type> RealNumberTypes = new HashSet<Type>
		{
			typeof (float),
			typeof (float?),
			typeof (double),
			typeof (double?),
			typeof (decimal),
			typeof (decimal?),
			typeof (Complex),
			typeof (Complex?)
		};

		private static readonly HashSet<Type> TextTypes = new HashSet<Type>
		{
			typeof(char),
			typeof(char?),
			typeof(string),
			typeof(StringValues),
			typeof(StringValues?)
		};

		private static readonly HashSet<Type> IntegerTypes = new HashSet<Type>
		{
			typeof(sbyte),
			typeof(sbyte?),
			typeof(byte),
			typeof(byte?),
			typeof(ushort),
			typeof(ushort?),
			typeof(short),
			typeof(short?),
			typeof(uint),
			typeof(uint?),
			typeof(int),
			typeof(int?),
			typeof(ulong),
			typeof(ulong?),
			typeof(long),
			typeof(long?),
			typeof(BigInteger),
			typeof(BigInteger?)
		};

		public SqliteDialect() : base(cs => new SqliteConnection(cs)) { }
		public SqliteDialect(Func<string, SqliteConnection> connectionFactory) : base(connectionFactory) { }
	}
}