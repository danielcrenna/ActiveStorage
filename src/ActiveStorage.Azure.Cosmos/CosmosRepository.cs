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
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using TypeKitchen;

namespace ActiveStorage.Azure.Cosmos
{
	public class CosmosRepository<T> : ICosmosRepository<T> where T : IDocumentEntity
	{
		private readonly Container _container;

		private readonly ISafeLogger<CosmosRepository<T>> _logger;
		private readonly IOptionsMonitor<CosmosStorageOptions> _options;
		private readonly ITypeReadAccessor _reads;
		private readonly string _slot;

		public CosmosRepository(string slot, Container container, IOptionsMonitor<CosmosStorageOptions> options,
			ISafeLogger<CosmosRepository<T>> logger)
		{
			_slot = slot;
			_container = container;
			_options = options;
			_logger = logger;

			_reads = ReadAccessor.Create(typeof(T));
		}

		public async Task<T> CreateAsync(T item, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await BeforeSaveAsync(item, cancellationToken);

			var partitionKey = new PartitionKey(item.Id);
			var options = new ItemRequestOptions();

			var document = await _container.CreateItemAsync(item, partitionKey, options, cancellationToken);
			return document.Resource;
		}

		public async Task<T> RetrieveAsync(string id, CancellationToken cancellationToken = default)
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

		public Task<long> CountAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var queryable = CreateDocumentQuery();
			var query = predicate != null ? queryable.Where(predicate).Count() : queryable.Count();
			return Task.FromResult((long) query);
		}

		public async Task<IEnumerable<T>> RetrieveAsync(Func<IQueryable<T>, IQueryable<T>> projection,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var queryable = projection(CreateDocumentQuery());
			var result = await GetResultsAsync(queryable, cancellationToken);
			return result;
		}

		public async Task<IEnumerable<T>> RetrieveAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var queryable = CreateDocumentQuery();
			var query = predicate != null ? queryable.Where(predicate) : queryable;
			return await GetResultsAsync(query, cancellationToken);
		}

		public async Task<T> RetrieveSingleAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var results = await RetrieveAsync(predicate, cancellationToken);
			return results.Single();
		}

		public async Task<T> RetrieveSingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var results = await RetrieveAsync(predicate, cancellationToken);

			return results.SingleOrDefault();
		}

		public async Task<T> RetrieveFirstAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var results = await RetrieveAsync(predicate, cancellationToken);

			return results.First();
		}

		public async Task<T> RetrieveFirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default)
		{
			var results = await RetrieveAsync(predicate, cancellationToken);

			return results.FirstOrDefault();
		}

		public async Task<T> UpdateAsync(string id, T item, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var partitionKey = new PartitionKey(id);
			var options = new ItemRequestOptions();

			return await _container.ReplaceItemAsync(item, id, partitionKey, options, cancellationToken);
		}

		public async Task<T> UpsertAsync(T item, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var response = await _container.ReadItemAsync<T>(item.Id, new PartitionKey(item.Id),
					cancellationToken: cancellationToken);

				// FIXME: add metrics
				return response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				var response = await _container.CreateItemAsync(item, new PartitionKey(item.Id),
					cancellationToken: cancellationToken);

				// FIXME: add metrics
				return response.Resource;
			}
		}

		public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var partitionKey = new PartitionKey(id);
			var options = new ItemRequestOptions();
			var response = await _container.DeleteItemAsync<T>(id, partitionKey, options, cancellationToken);

			return response.StatusCode == HttpStatusCode.NoContent;
		}

		public async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
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

		private async Task BeforeSaveAsync(T item, CancellationToken cancellationToken)
		{
			await ValidateUniqueFields(item, cancellationToken);
		}

		private async Task ValidateUniqueFields(T item, CancellationToken cancellationToken)
		{
			IQueryable<T> queryable = null;
			foreach (var member in AccessorMembers.Create(typeof(T)))
			{
				if (!member.HasAttribute<UniqueAttribute>())
				{
					continue;
				}

				queryable ??= CreateDocumentQuery();
				queryable = queryable.Where(ComputedPredicate<T>.AsExpression(member.Name, ExpressionOperator.Equal,
					_reads[item, member.Name]));
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

		private IQueryable<T> CreateDocumentQuery()
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

		private static async Task<IList<T>> GetResultsAsync(IQueryable<T> query, CancellationToken cancellationToken)
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