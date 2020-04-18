// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActiveStorage.Azure.TableStorage
{
	public class TableCreateStore : IObjectCreateStore
	{
		private readonly ILogger<TableCreateStore> _logger;
		private readonly CloudTable _table;

		public TableCreateStore(CloudTable table, ILogger<TableCreateStore> logger = null)
		{
			_table = table;
			_logger = logger;
		}

		public async Task<Operation<ObjectCreate>> CreateAsync(object @object, CancellationToken cancellationToken = default, params string[] fields)
		{
			var entity = new FastTableEntity(@object, "TODO", "TODO", _logger);
			return await CreateAsync(entity, cancellationToken);
		}

		private async Task<Operation<ObjectCreate>> CreateAsync(ITableEntity entity, CancellationToken cancellationToken = default)
		{
			var insert = TableOperation.Insert(entity);
			var result = await _table.ExecuteAsync(insert);

			if (result.HttpStatusCode == (int) HttpStatusCode.NoContent)
				return new Operation<ObjectCreate>(ObjectCreate.Created);

			return new Operation<ObjectCreate>(ObjectCreate.Error);
		}
	}
}