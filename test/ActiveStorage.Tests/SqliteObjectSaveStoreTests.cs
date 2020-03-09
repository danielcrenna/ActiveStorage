// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using ActiveStorage.Internal;
using ActiveStorage.Sql;
using ActiveStorage.Sqlite;
using ActiveStorage.Tests.Fixtures;
using ActiveStorage.Tests.Migrations.SimpleObject;
using Dapper;

namespace ActiveStorage.Tests
{
	public class SqliteObjectSaveStoreTests : SqlObjectSaveStoreTests
	{
		public override ISqlDialect Dialect => new SqliteDialect();
		public override IDbConnection CreateFixture<TMigrationInfo>() => new SqliteFixture<TMigrationInfo>($"Data Source={Guid.NewGuid()}.db");

		public bool Empty_database_has_no_objects()
		{
			using var db = CreateFixture<SimpleObject>();
			var count = db.QuerySingle<int>("SELECT COUNT(1) FROM 'Object'");
			return count == 0;
		}

		public bool Can_save_object_to_store()
		{
			using var db = CreateFixture<SimpleObject>();

			var store = new SqlObjectSaveStore(db.ConnectionString, Dialect, new AttributeDataInfoProvider(), new CreatedAtTransform(() => DateTimeOffset.UtcNow));
			var result = store.SaveAsync(new DataRow {Id = 1}).Result;
			if (!result.Succeeded)
				return false;

			var count = db.QuerySingle<int>("SELECT COUNT(1) FROM 'Object'");
			return count == 1;
		}

		[Table("Object")]
		public class DataRow
		{
			public long Id { get; set; }
			public DateTimeOffset? CreatedAt { get; set; }
		}
	}
}