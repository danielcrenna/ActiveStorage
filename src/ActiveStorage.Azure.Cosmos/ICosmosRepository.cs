// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveStorage.Azure.Cosmos
{
	public interface ICosmosRepository
	{
		Task<long> CountAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : IDocumentEntity;
		Task<IEnumerable<T>> RetrieveAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : IDocumentEntity;

		Task<T> RetrieveAsync<T>(string id, CancellationToken cancellationToken = default) where T : IDocumentEntity;
		Task<IEnumerable<T>> RetrieveAsync<T>(Func<IQueryable<T>, IQueryable<T>> projection, CancellationToken cancellationToken = default) where T : IDocumentEntity;

		Task<T> RetrieveSingleAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)  where T : IDocumentEntity;

		Task<T> RetrieveSingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : IDocumentEntity;
		Task<T> RetrieveFirstAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : IDocumentEntity;
		Task<T> RetrieveFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : IDocumentEntity;

		Task<T> CreateAsync<T>(T item, CancellationToken cancellationToken = default) where T : IDocumentEntity;
		Task<T> UpdateAsync<T>(string id, T item, CancellationToken cancellationToken = default)  where T : IDocumentEntity;
		Task<T> UpsertAsync<T>(T item, CancellationToken cancellationToken = default)  where T : IDocumentEntity;

		Task<bool> DeleteAsync<T>(string id, CancellationToken cancellationToken = default) where T : IDocumentEntity;
		Task<bool> DeleteAsync<T>(IEnumerable<string> ids, CancellationToken cancellationToken = default) where T : IDocumentEntity;
	}
}