// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Threading.Tasks;
using ActiveErrors;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActiveStorage.Azure.TableStorage
{
	public class TableSaveStore : IObjectSaveStore
	{
		private readonly ILogger<TableSaveStore> _logger;
		private readonly CloudTable _table;

		public TableSaveStore(CloudTable table, ILogger<TableSaveStore> logger = null)
		{
			_table = table;
			_logger = logger;
		}

		public async Task<Operation<ObjectSave>> SaveAsync(object @object, params string[] fields)
		{
			var entity = new FastTableEntity(@object, "TODO", "TODO", _logger);
			return await InsertAsync(entity);
		}

		private async Task<Operation<ObjectSave>> InsertAsync(ITableEntity entity)
		{
			var insert = TableOperation.Insert(entity);
			var result = await _table.ExecuteAsync(insert);

			if (result.HttpStatusCode == (int) HttpStatusCode.NoContent)
				return new Operation<ObjectSave>(ObjectSave.Created);

			return new Operation<ObjectSave>(ObjectSave.NotFound);
		}
	}
}