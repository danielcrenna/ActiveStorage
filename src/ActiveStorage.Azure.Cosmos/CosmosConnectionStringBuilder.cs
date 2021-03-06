﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ActiveStorage.Azure.Cosmos.Configuration;
using ActiveStorage.Azure.Cosmos.Internal;
using Microsoft.Azure.Cosmos;
using TypeKitchen;

namespace ActiveStorage.Azure.Cosmos
{
	public sealed class CosmosConnectionStringBuilder : DbConnectionStringBuilder
	{
		private readonly IDictionary<string, string> _settings;

		public CosmosConnectionStringBuilder() =>
			_settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public CosmosConnectionStringBuilder(CosmosStorageOptions options) : this()
		{
			AccountEndpoint = options.AccountEndpoint;
			AccountKey = options.AccountKey;
			Database = options.DatabaseId;
			DefaultContainer = options.ContainerId;
			SharedCollection = options.SharedCollection;
			PartitionKeyPaths = options.PartitionKeyPaths;
		}

		public CosmosConnectionStringBuilder(string connectionString) : this()
		{
			var entries = connectionString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
			var tokens = entries.Select(part => part.Split(new[] {'='}, 2));
			_settings = tokens.ToDictionary(split => split[0], split => split[1], StringComparer.OrdinalIgnoreCase);

			ConnectionString = ToString();
		}

		public Uri AccountEndpoint
		{
			get => this[Constants.AccountEndpointKey] is string endpoint ? new Uri(endpoint) : null;
			set
			{
				this[Constants.AccountEndpointKey] = value?.OriginalString;
				ConnectionString = ToString();
			}
		}

		public string AccountKey
		{
			get => this[Constants.AccountKeyKey] as string;
			set
			{
				this[Constants.AccountKeyKey] = value;
				ConnectionString = ToString();
			}
		}

		public string DefaultContainer
		{
			get => this[Constants.DefaultCollectionKey] as string;
			set
			{
				this[Constants.DefaultCollectionKey] = value;
				ConnectionString = ToString();
			}
		}

		public string CollectionId
		{
			get => DefaultContainer;
			set => DefaultContainer = value;
		}

		public string ContainerId
		{
			get => DefaultContainer;
			set => DefaultContainer = value;
		}

		public string[] PartitionKeyPaths
		{
			get
			{
				var value = this[Constants.PartitionKeyPathsKey] as string;
				return value?.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
			}
			set
			{
				this[Constants.PartitionKeyPathsKey] = value;
				ConnectionString = ToString();
			}
		}

		public string Database
		{
			get => this[Constants.DatabaseKey] as string;
			set
			{
				this[Constants.DatabaseKey] = value;
				ConnectionString = ToString();
			}
		}

		public string DatabaseId
		{
			get => Database;
			set => Database = value;
		}

		public bool SharedCollection
		{
			get => bool.TryParse(this[Constants.SharedCollectionKey] as string ?? "False", out var b) && b;
			set
			{
				this[Constants.SharedCollectionKey] = value;
				ConnectionString = ToString();
			}
		}

		public void Bind(CosmosStorageOptions options)
		{
			options.AccountEndpoint = AccountEndpoint;
			options.AccountKey = AccountKey;
			options.ContainerId = DefaultContainer;
			options.DatabaseId = Database;
			options.SharedCollection = SharedCollection;
			options.PartitionKeyPaths = PartitionKeyPaths;
		}

		public CosmosClient Build()
		{
			var clientOptions = new CosmosClientOptions();
			return new CosmosClient(AccountEndpoint.OriginalString, AccountKey, clientOptions);
		}

		#region DbConnectionStringBuilder

		public override int Count => _settings.Count;
		public override bool IsFixedSize => false;

		public override object this[string keyword]
		{
			get
			{
				_settings.TryGetValue(keyword, out var value);
				return value;
			}
			set => _settings[keyword] = value?.ToString();
		}

		public override ICollection Keys => (ICollection) _settings.Keys;
		public override ICollection Values => (ICollection) _settings.Values;

		public override bool ShouldSerialize(string keyword)
		{
			return _settings.ContainsKey(keyword);
		}

		public override void Clear()
		{
			_settings.Clear();
		}

		public override bool ContainsKey(string keyword)
		{
			return _settings.ContainsKey(keyword);
		}

		public override bool Remove(string keyword)
		{
			return _settings.Remove(keyword);
		}

		public override bool TryGetValue(string keyword, out object value)
		{
			if (_settings.TryGetValue(keyword, out var valueString))
			{
				value = valueString;
				return true;
			}

			value = default;
			return false;
		}

		public override bool EquivalentTo(DbConnectionStringBuilder connectionStringBuilder)
		{
			if (connectionStringBuilder is CosmosConnectionStringBuilder)
				return ConnectionString.Equals(connectionStringBuilder.ConnectionString,
					StringComparison.OrdinalIgnoreCase);

			throw new InvalidCastException($"The builder passed was not a {nameof(DbConnectionStringBuilder)}.");
		}

		protected override void GetProperties(Hashtable propertyDescriptors)
		{
			foreach (var (k, v) in _settings)
				propertyDescriptors.Add(k, v);
		}

		public override string ToString()
		{
			return Pooling.StringBuilderPool.Scoped(sb =>
			{
				foreach (var (k, v) in _settings)
					sb.Append(k).Append("=").Append(v).Append(";");
			});
		}

		#endregion
	}
}