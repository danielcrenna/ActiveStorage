// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using ActiveStorage.Tests.Fixtures;
using ActiveStorage.Tests.Migrations.SimpleObject;
using ActiveStorage.Tests.Models;

namespace ActiveStorage.Tests.ObjectSaveStore
{
	public class SqlServerSaveStoreTests : ISaveStoreTests
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

		public async Task<bool> Can_save_object_to_store()
		{
			using var db = new SqlServerFixture<AddSimpleObject>();
			db.Open();

			var store = db.GetSaveStore();
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