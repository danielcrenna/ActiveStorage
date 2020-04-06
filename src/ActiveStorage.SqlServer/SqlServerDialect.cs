// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.SqlClient;
using ActiveStorage.Sql;

namespace ActiveStorage.SqlServer
{
	public class SqlServerDialect : SqlCommands<SqlConnection, SqlException>, ISqlDialect
	{
		public char? StartIdentifier => '[';
		public char? EndIdentifier => ']';
		public char? Separator => '.';
		public char? Parameter => '@';
		public char? Quote => '\'';

		public SqlServerDialect() : base(cs => new SqlConnection(cs)) { }

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
	}
}