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
	public interface ICosmosRepository<T> where T : IDocumentEntity
	{
		Task<long> CountAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default);

		Task<IEnumerable<T>> RetrieveAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default);

		Task<T> RetrieveAsync(string id, CancellationToken cancellationToken = default);

		Task<IEnumerable<T>> RetrieveAsync(Func<IQueryable<T>, IQueryable<T>> projection,
			CancellationToken cancellationToken = default);

		Task<T> RetrieveSingleAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default);

		Task<T> RetrieveSingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default);

		Task<T> RetrieveFirstAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default);

		Task<T> RetrieveFirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
			CancellationToken cancellationToken = default);

		Task<T> CreateAsync(T item, CancellationToken cancellationToken = default);
		Task<T> UpdateAsync(string id, T item, CancellationToken cancellationToken = default);
		Task<T> UpsertAsync(T item, CancellationToken cancellationToken = default);

		Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
		Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
	}
}