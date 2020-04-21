// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActiveStorage.Azure.TableStorage
{
	public class TableAppendStore : IObjectAppendStore
	{
		private readonly ILogger<TableAppendStore> _logger;
		private readonly CloudTable _table;

		public TableAppendStore(CloudTable table, ILogger<TableAppendStore> logger = null)
		{
			_table = table;
			_logger = logger;
		}

		public async Task<Operation<ObjectAppend>> CreateAsync(object @object, CancellationToken cancellationToken = default, params string[] fields)
		{
			var entity = new FastTableEntity(@object, "TODO", "TODO", _logger);
			return await CreateAsync(entity, cancellationToken);
		}

		private async Task<Operation<ObjectAppend>> CreateAsync(ITableEntity entity, CancellationToken cancellationToken = default)
		{
			var insert = TableOperation.Insert(entity);

			var options = new TableRequestOptions();
			var context = new OperationContext();

			var result = await _table.ExecuteAsync(insert, options, context, cancellationToken);

			if (result.HttpStatusCode == (int) HttpStatusCode.NoContent)
				return new Operation<ObjectAppend>(ObjectAppend.Created);

			return new Operation<ObjectAppend>(ObjectAppend.Error);
		}
	}
}