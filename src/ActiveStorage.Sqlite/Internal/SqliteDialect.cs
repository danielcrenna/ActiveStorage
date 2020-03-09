// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveStorage.Sql;

namespace ActiveStorage.Sqlite.Internal
{
	internal sealed class SqliteDialect : ISqlDialect
	{
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
	}
}