// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ActiveStorage.Internal;
using ActiveStorage.Sql;
using ActiveStorage.SqlServer;
using ActiveStorage.Tests.Fixtures;
using ActiveStorage.Tests.Internal;
using ActiveStorage.Tests.Migrations.SimpleObject;
using Dapper;

namespace ActiveStorage.Tests
{
	public class SqlServerSaveStoreTests
	{
		public IDbConnection CreateFixture<TMigrationInfo>() => new SqlServerFixture<TMigrationInfo>(CreateConnectionString());

		private static string CreateConnectionString()
		{
			var database = $"{Guid.NewGuid().ToString("N").ToUpperInvariant()}";
			var builder = new SqlConnectionStringBuilder("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=true;MultipleActiveResultSets=true") {InitialCatalog = "master"};
			using (var connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();

				var fileName = SqlServerMigrationRunner.DatabaseFileName(database);

				connection.Execute($"CREATE DATABASE [{database}] ON (NAME = N'{database}', FILENAME = '{fileName}')");
				connection.Execute($"ALTER DATABASE [{database}] SET READ_COMMITTED_SNAPSHOT ON;");
				connection.Execute($"ALTER DATABASE [{database}] SET ALLOW_SNAPSHOT_ISOLATION ON;");
			}
			builder.InitialCatalog = database;
			return builder.ConnectionString;
		}

		public async Task<bool> Empty_database_has_no_objects()
		{
			using var db = CreateFixture<AddSimpleObject>();
			db.Open();

			var count = await db.QuerySingleAsync<int>("SELECT COUNT(1) FROM [Object]");
			return count == 0;
		}

		public async Task<bool> Can_save_object_to_store()
		{
			using var db = CreateFixture<AddSimpleObject>();
			db.Open();

			var store = new SqlObjectSaveStore(db.ConnectionString, new SqlServerDialect(), new AttributeDataInfoProvider(), new CreatedAtTransform(() => DateTimeOffset.UtcNow));
			var result = await store.SaveAsync(new SimpleObject {Id = 1});
			if (!result.Succeeded)
				return false;

			var count = await db.QuerySingleAsync<int>("SELECT COUNT(1) FROM [Object]");
			return count == 1;
		}

		[Table("Object")]
		public class SimpleObject
		{
			public long Id { get; set; }
			public DateTimeOffset? CreatedAt { get; set; }
		}
	}
}