// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data;
using ActiveStorage.Sql;

namespace ActiveStorage.Tests
{
	public abstract class SqlObjectSaveStoreTests
	{
		public abstract IDbConnection CreateFixture<TMigrationInfo>();
		public abstract ISqlDialect Dialect { get; }
	}
}