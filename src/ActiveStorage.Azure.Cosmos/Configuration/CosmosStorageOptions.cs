// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Azure.Cosmos;

namespace ActiveStorage.Azure.Cosmos.Configuration
{
	public class CosmosStorageOptions
	{
		public Uri AccountEndpoint { get; set; }
		public string AccountKey { get; set; }
		public string DatabaseId { get; set; }
		public string ContainerId { get; set; }
		public string[] PartitionKeyPaths { get; set; } = {"/id"};
		public int? OfferThroughput { get; set; } = 400;
		public bool SharedCollection { get; set; }

		public CosmosClientOptions ClientOptions { get; set; } = new CosmosClientOptions();

		public string PartitionKeyPath => string.Join("/", PartitionKeyPaths ?? Enumerable.Empty<string>());
	}
}