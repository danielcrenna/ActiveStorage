// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ActiveStorage.Azure.TableStorage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActiveStorage.Tests.Fixtures
{
	public sealed class AzureCosmosStorageFixture : IStoreFixture
	{
		private readonly CloudTable _table;

		public AzureCosmosStorageFixture()
		{
			var tableName = $"T{Guid.NewGuid().ToString("N").ToUpperInvariant()}";

			_table = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;" +
			                                   "AccountName=devstoreaccount1;" +
			                                   "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
			                                   "TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;")
				.CreateCloudTableClient()
				.GetTableReference(tableName);
		}

		public void Dispose()
		{
			try
			{
				_table?.DeleteIfExistsAsync()?.Wait();
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
			}
		}

		public IObjectCountStore GetCountStore()
		{
			return new TableCountStore(_table);
		}

		public IObjectSaveStore GetSaveStore()
		{
			return new TableSaveStore(_table);
		}

		public async Task OpenAsync()
		{
			try
			{
				await _table.CreateIfNotExistsAsync();
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
				throw;
			}
		}
	}

	public sealed class AzureTableStorageFixture : IStoreFixture
	{
		private readonly CloudTable _table;

		public AzureTableStorageFixture()
		{
			var tableName = $"T{Guid.NewGuid().ToString("N").ToUpperInvariant()}";

			_table = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;" +
			                                   "AccountName=devstoreaccount1;" +
			                                   "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
			                                   "TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;")
				.CreateCloudTableClient()
				.GetTableReference(tableName);
		}

		public void Dispose()
		{
			try
			{
				_table?.DeleteIfExistsAsync()?.Wait();
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
			}
		}

		public IObjectCountStore GetCountStore()
		{
			return new TableCountStore(_table);
		}

		public IObjectSaveStore GetSaveStore()
		{
			return new TableSaveStore(_table);
		}

		public async Task OpenAsync()
		{
			try
			{
				await _table.CreateIfNotExistsAsync();
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
				throw;
			}
		}
	}
}