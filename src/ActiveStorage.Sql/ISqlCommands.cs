// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveStorage.Sql
{
	public interface ISqlCommands
	{
		Task<IEnumerable<T>> QueryAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null, CancellationToken cancellationToken = default);

		Task<T> QuerySingleAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
		Task<T> QuerySingleOrDefaultAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
		Task<T> QueryFirstAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
		Task<T> QueryFirstOrDefaultAsync<T>(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
		
		Task<int> ExecuteAsync(string connectionString, string sql, IReadOnlyDictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
	}
}