// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Threading.Tasks;
using ActiveStorage.Internal;
using ActiveStorage.Sql;
using ActiveStorage.Sqlite;
using ActiveStorage.Tests.Fixtures;
using ActiveStorage.Tests.Migrations.SimpleObject;
using Dapper;

namespace ActiveStorage.Tests
{
	public class SqliteObjectSaveStoreTests
	{
		public IDbConnection CreateFixture<TMigrationInfo>() => new SqliteFixture<TMigrationInfo>(CreateConnectionString());

		private static string CreateConnectionString()
		{
			return $"Data Source={Guid.NewGuid()}.db";
		}

		public async Task<bool> Empty_database_has_no_objects()
		{
			using var db = CreateFixture<AddSimpleObject>();
			db.Open();

			var count = await db.QuerySingleAsync<int>("SELECT COUNT(1) FROM 'Object'");
			return count == 0;
		}

		public async Task<bool> Can_save_object_to_store()
		{
			using var db = CreateFixture<AddSimpleObject>();
			db.Open();

			var store = new SqlObjectSaveStore(db.ConnectionString, new SqliteDialect(), new AttributeDataInfoProvider(), new CreatedAtTransform(() => DateTimeOffset.UtcNow));
			var result = await store.SaveAsync(new DataRow {Id = 1});
			if (!result.Succeeded)
				return false;

			var count = await db.QuerySingleAsync<int>("SELECT COUNT(1) FROM 'Object'");
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