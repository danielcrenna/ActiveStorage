// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using ActiveLogging;
using ActiveStorage.Internal;
using ActiveStorage.Sql;
using ActiveStorage.Sqlite;
using ActiveStorage.Tests.Fixtures;
using ActiveStorage.Tests.Migrations.SimpleObject;
using ActiveStorage.Tests.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace ActiveStorage.Tests.ObjectSaveStore
{
	public class SqliteSaveStoreTests : ISaveStoreTests
	{
		public async Task<bool> Empty_database_has_no_objects()
		{
			using var db = new SqliteFixture<AddSimpleObject>();
			db.Open();

			var counter = db.GetCountStore();
			var count = await counter.CountAsync(typeof(SimpleObject));
			if (!count.Succeeded)
				return false;

			return count.Data == 0;
		}

		public async Task<bool> Can_save_object_to_store()
		{
			using var db = new SqliteFixture<AddSimpleObject>();
			db.Open();

			var transform = new CreatedAtTransform(() => DateTimeOffset.UtcNow);
			var logger = new SafeLogger<SqlObjectSaveStore>(new NullLogger<SqlObjectSaveStore>());
			var store = new SqlObjectSaveStore(db.ConnectionString, new SqliteDialect(), new AttributeDataInfoProvider(), logger, transform);
			
			var result = await store.SaveAsync(new SimpleObject {Id = 1});
			if (!result.Succeeded)
				return false;

			var counter = db.GetCountStore();
			var count = await counter.CountAsync(typeof(SimpleObject));
			if (!count.Succeeded)
				return false;

			return count.Data == 1;
		}
	}
}