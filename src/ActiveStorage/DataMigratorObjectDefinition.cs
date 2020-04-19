// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActiveStorage
{
	internal sealed class DataMigratorObjectDefinition : IObjectDefinition
	{
		private readonly IEnumerable<IDataMigrator> _migrators;

		public DataMigratorObjectDefinition(IEnumerable<IDataMigrator> migrators)
		{
			_migrators = migrators;
		}

		public async Task VisitAsync(VisitType visitType)
		{
			if (visitType != VisitType.Eager)
				return;

			foreach (var migrator in _migrators)
				await migrator.UpAsync();
		}
	}
}