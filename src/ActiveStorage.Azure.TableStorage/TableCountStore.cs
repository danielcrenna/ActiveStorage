// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActiveStorage.Azure.TableStorage
{
	public class TableCountStore : IObjectCountStore
	{
		private readonly ILogger<TableCountStore> _logger;
		private readonly CloudTable _table;

		public TableCountStore(CloudTable table, ILogger<TableCountStore> logger = null)
		{
			_table = table;
			_logger = logger;
		}

		public async Task<Operation<ulong>> CountAsync(Type type, CancellationToken cancellationToken = default)
		{
			var count = 0L;
			TableContinuationToken token = null;
			var query = new TableQuery();
			do
			{
				var result = await _table.ExecuteQuerySegmentedAsync(query, token);
				foreach (var _ in result.Results)
					Interlocked.Increment(ref count);
				token = result.ContinuationToken;
			} while (token != null);

			return new Operation<ulong>((ulong) count);
		}

		public async Task<Operation<ulong>> CountAsync<T>(CancellationToken cancellationToken = default)
		{
			return await CountAsync(typeof(T), cancellationToken);
		}
	}
}