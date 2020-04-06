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

namespace ActiveStorage.Tests.ObjectCreateStore
{
	public class SqliteSaveStoreTests : ICreateStoreTests
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
		
		public async Task<bool> Can_create_object_in_store()
		{
			using var db = new SqliteFixture<AddSimpleObject>();
			db.Open();

			var store = db.GetCreateStore();

			var instance = new SimpleObject {Id = 1};

			var result = await store.CreateAsync(instance);
			if (!result.Succeeded)
				return false;

			var countStore = db.GetCountStore();
			var count = await countStore.CountAsync(typeof(SimpleObject));
			if (!count.Succeeded || count.Data != 1)
				return false;

			var queryStore = db.GetSingleObjectQueryByExampleStore();
			var fetched = await queryStore.QueryFirstOrDefaultByExampleAsync<SimpleObject>(new { Id = 1});
			if (fetched == null)
				return false;

			return true;
		}

		public async Task<bool> Cannot_create_same_object_twice()
		{
			using var db = new SqliteFixture<AddSimpleObject>();
			db.Open();

			var instance = new SimpleObject {Id = 1};

			var store = db.GetCreateStore();
			var result = await store.CreateAsync(instance);
			if (!result.Succeeded)
				return false;

			result = await store.CreateAsync(instance);
			if (result.Succeeded)
				return false;

			var counter = db.GetCountStore();
			var count = await counter.CountAsync(typeof(SimpleObject));
			if (!count.Succeeded)
				return false;

			return count.Data == 1;
		}
	}
}