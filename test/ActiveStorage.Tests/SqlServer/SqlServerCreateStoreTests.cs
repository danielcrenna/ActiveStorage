// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using ActiveStorage.Tests.Fixtures;
using ActiveStorage.Tests.Migrations.SimpleObject;
using ActiveStorage.Tests.Models;

namespace ActiveStorage.Tests.SqlServer
{
	public class SqlServerCreateStoreTests : ICreateStoreTests
	{
		public async Task<bool> Empty_database_has_no_objects()
		{
			using var db = new SqlServerFixture<AddSimpleObject>();
			db.Open();

			var counter = db.GetCountStore();
			var count = await counter.CountAsync(typeof(SimpleObject));
			if (!count.Succeeded)
				return false;

			return count.Data == 0;
		}

		public async Task<bool> Can_create_object_in_store()
		{
			using var db = new SqlServerFixture<AddSimpleObject>();
			db.Open();

			var store = db.GetCreateStore();
			var result = await store.CreateAsync(new SimpleObject {Id = 1});
			if (!result.Succeeded)
				return false;

			var counter = db.GetCountStore();
			var count = await counter.CountAsync(typeof(SimpleObject));
			if (!count.Succeeded)
				return false;

			return count.Data == 1;
		}

		public async Task<bool> Cannot_create_same_object_twice()
		{
			using var db = new SqlServerFixture<AddSimpleObject>();
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