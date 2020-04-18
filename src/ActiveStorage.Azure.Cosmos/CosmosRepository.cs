// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ActiveLogging;
using ActiveStorage.Azure.Cosmos.Configuration;
using ActiveStorage.Azure.Cosmos.Internal;
using ActiveStorage.DataAnnotations;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using TypeKitchen;

namespace ActiveStorage.Azure.Cosmos
{
	public class CosmosRepository : ICosmosRepository
	{
		private readonly Container _container;

		private readonly ISafeLogger<CosmosRepository> _logger;
		private readonly IOptionsMonitor<CosmosStorageOptions> _options;
		private readonly string _slot;

		public CosmosRepository(string slot, Container container, IOptionsMonitor<CosmosStorageOptions> options,
			ISafeLogger<CosmosRepository> logger)
		{
			_slot = slot;
			_container = container;
			_options = options;
			_logger = logger;
		}

		public async Task<T> CreateAsync<T>(T item, CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			await BeforeSaveAsync(item, cancellationToken);

			var partitionKey = new PartitionKey(item.Id);
			var options = new ItemRequestOptions();

			var document = await _container.CreateItemAsync(item, partitionKey, options, cancellationToken);
			return document.Resource;
		}

		public async Task<T> RetrieveAsync<T>(string id, CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var partitionKey = new PartitionKey(id);
				var options = new ItemRequestOptions();
				return await _container.ReadItemAsync<T>(id, partitionKey, options, cancellationToken);
			}
			catch (CosmosException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
				{
					return default;
				}

				throw;
			}
		}

		public Task<long> CountAsync<T>(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var queryable = CreateDocumentQuery<T>();
			var query = predicate != null ? queryable.Where(predicate).Count() : queryable.Count();
			return Task.FromResult((long) query);
		}

		public async Task<IEnumerable<T>> RetrieveAsync<T>(Func<IQueryable<T>, IQueryable<T>> projection,
			CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var queryable = projection(CreateDocumentQuery<T>());
			var result = await GetResultsAsync(queryable, cancellationToken);
			return result;
		}

		public async Task<IEnumerable<T>> RetrieveAsync<T>(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var queryable = CreateDocumentQuery<T>();
			var query = predicate != null ? queryable.Where(predicate) : queryable;
			return await GetResultsAsync(query, cancellationToken);
		}

		public async Task<T> RetrieveSingleAsync<T>(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var results = await RetrieveAsync(predicate, cancellationToken);
			return results.Single();
		}

		public async Task<T> RetrieveSingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var results = await RetrieveAsync(predicate, cancellationToken);

			return results.SingleOrDefault();
		}

		public async Task<T> RetrieveFirstAsync<T>(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var results = await RetrieveAsync(predicate, cancellationToken);

			return results.First();
		}

		public async Task<T> RetrieveFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			var results = await RetrieveAsync(predicate, cancellationToken);

			return results.FirstOrDefault();
		}

		public async Task<T> UpdateAsync<T>(string id, T item, CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var partitionKey = new PartitionKey(id);
			var options = new ItemRequestOptions();

			return await _container.ReplaceItemAsync(item, id, partitionKey, options, cancellationToken);
		}

		public async Task<T> UpsertAsync<T>(T item, CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var partitionKey = new PartitionKey(item.Id);

			try
			{
				var options = new ItemRequestOptions();

				var response = await _container.ReadItemAsync<T>(item.Id, partitionKey, cancellationToken: cancellationToken);
         
				if ((int) response.StatusCode > 199 && (int) response.StatusCode < 300)
				{
					response = await _container.ReplaceItemAsync(item, item.Id, partitionKey, cancellationToken: cancellationToken);
				}

				return response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				var response = await _container.CreateItemAsync(item, partitionKey,
					cancellationToken: cancellationToken);

				// FIXME: add metrics
				return response.Resource;
			}
		}

		public async Task<bool> DeleteAsync<T>(string id, CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var partitionKey = new PartitionKey(id);
			var options = new ItemRequestOptions();
			var response = await _container.DeleteItemAsync<T>(id, partitionKey, options, cancellationToken);

			return response.StatusCode == HttpStatusCode.NoContent;
		}

		public async Task<bool> DeleteAsync<T>(IEnumerable<string> ids, CancellationToken cancellationToken = default) where T : IDocumentEntity
		{
			cancellationToken.ThrowIfCancellationRequested();

			var deleted = 0;

			// ReSharper disable once PossibleMultipleEnumeration
			var total = ids.Count();

			// ReSharper disable once PossibleMultipleEnumeration
			foreach (var id in ids)
			{
				try
				{
					var options = new ItemRequestOptions();
					await _container.DeleteItemAsync<T>(id, new PartitionKey(id), options, cancellationToken);
					deleted++;
				}
				catch (Exception e)
				{
					Trace.TraceError(e.ToString());
					throw;
				}
			}

			return deleted == total;
		}

		private async Task BeforeSaveAsync<T>(T item, CancellationToken cancellationToken) where T : IDocumentEntity
		{
			await ValidateUniqueFields(item, cancellationToken);
		}

		private async Task ValidateUniqueFields<T>(T item, CancellationToken cancellationToken) where T : IDocumentEntity
		{
			IQueryable<T> queryable = null;
			foreach (var member in AccessorMembers.Create(typeof(T)))
			{
				if (!member.HasAttribute<UniqueAttribute>())
				{
					continue;
				}

				queryable ??= CreateDocumentQuery<T>();
				queryable = queryable.Where(ComputedPredicate<T>.AsExpression(member.Name, ExpressionOperator.Equal,
					ReadAccessor.Create(item)[item, member.Name]));
			}

			if (queryable == null)
			{
				return;
			}

			var results = await GetResultsAsync(queryable, cancellationToken);
			if (results.Count > 0)
			{
				throw new DataException("Creating document would violate unique constraints for the document type.");
			}
		}

		private IQueryable<T> CreateDocumentQuery<T>() where T : IDocumentEntity
		{
			var allowSynchronousExecution = false;
			var options = new QueryRequestOptions();
			string continuationToken = null;

			IQueryable<T> queryable = _container.GetItemLinqQueryable<T>(
				allowSynchronousExecution,
				continuationToken,
				options);

			if (_options.Get(_slot).SharedCollection)
			{
				queryable = queryable.Where(x => x.DocumentType == DocumentEntityTypeFactory<T>.Type);
			}

			return queryable;
		}

		private static async Task<IList<T>> GetResultsAsync<T>(IQueryable<T> query, CancellationToken cancellationToken)
		{
			try
			{
				var iterator = query.ToFeedIterator();

				var items = new List<T>();
				items.AddRange(await iterator.ReadNextAsync(cancellationToken));
				while (iterator.HasMoreResults)
					items.AddRange(await iterator.ReadNextAsync(cancellationToken));

				return items;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}