// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ActiveStorage.Azure.Cosmos.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace ActiveStorage.Azure.Cosmos
{
	public class CosmosMigrationRunner
	{
		private readonly CosmosClient _client;
		private readonly IOptionsMonitor<CosmosStorageOptions> _options;

		public CosmosMigrationRunner(string slot, IOptionsMonitor<CosmosStorageOptions> options)
		{
			_options = options;

			var storageOptions = _options.Get(slot);
			_client = new CosmosClient(storageOptions.AccountEndpoint?.OriginalString, storageOptions.AccountKey,
				new CosmosClientOptions
				{
					AllowBulkExecution = true,
					ConnectionMode = ConnectionMode.Gateway,
					MaxRetryAttemptsOnRateLimitedRequests = 100
				});
		}

		public async Task<Container> CreateContainerIfNotExistsAsync(CancellationToken cancellationToken = default)
		{
			var databaseName = _options.CurrentValue.DatabaseId;
			var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(databaseName, cancellationToken: cancellationToken);

			var containerName = _options.CurrentValue.ContainerId;
			var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName,
				_options.CurrentValue.PartitionKeyPath,
				_options.CurrentValue.OfferThroughput, cancellationToken: cancellationToken);

			return containerResponse.Container;
		}
	}
}