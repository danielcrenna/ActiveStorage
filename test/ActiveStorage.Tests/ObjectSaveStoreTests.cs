// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using ActiveStorage.Internal;
using ActiveStorage.Sql;
using ActiveStorage.Sqlite;
using ActiveStorage.Tests.Fixtures;
using ActiveStorage.Tests.Migrations.SimpleObject;
using Dapper;

namespace ActiveStorage.Tests
{
	public class ObjectSaveStoreTests
	{
		public bool Empty_database_has_no_objects()
		{
			using var db = new SqliteFixture<SimpleObject>();

			var count = db.QuerySingle<int>("SELECT COUNT(1) FROM 'Object'");
			return count == 0;
		}

		public bool Can_save_object_to_store()
		{
			using var db = new SqliteFixture<SimpleObject>();

			var store = new SqlObjectSaveStore(db.ConnectionString, new SqliteDialect(), new AttributeDataInfoProvider(), new CreatedAtTransform(() => DateTimeOffset.UtcNow));
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