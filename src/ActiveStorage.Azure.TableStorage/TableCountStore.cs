// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActiveStorage.Azure.TableStorage
{
	public class TableCountStore : IObjectCountStore
	{
		private readonly CloudTable _table;

		public TableCountStore(CloudTable table) => _table = table;

		public async Task<Operation<ulong>> CountAsync(Type type)
		{
			var count = 0L;
			TableContinuationToken token = null;
			var query = new TableQuery();
			do
			{
				var result = await _table.ExecuteQuerySegmentedAsync(query, token);
				foreach (var item in result.Results)
					Interlocked.Increment(ref count);
				token = result.ContinuationToken;
			} while (token != null);

			return new Operation<ulong>((ulong) count);
		}
	}
}