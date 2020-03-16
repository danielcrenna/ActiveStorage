// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentMigrator;

namespace ActiveStorage.Tests.Migrations.SimpleObject
{
	[Migration(1)]
	public class AddSimpleObject : AutoReversingMigration
	{
		public override void Up()
		{
			Create.Table("Object")
				.WithColumn("Id").AsInt64().PrimaryKey()
				.WithColumn("CreatedAt").AsDateTime().NotNullable().Indexed()
				.WithColumn("DeletedAt").AsDateTime().Nullable().Indexed()
				;
		}
	}
}