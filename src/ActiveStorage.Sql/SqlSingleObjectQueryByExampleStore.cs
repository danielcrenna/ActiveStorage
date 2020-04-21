// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveStorage.Sql.Internal;
using Microsoft.Extensions.Logging;

namespace ActiveStorage.Sql
{
	public sealed class SqlSingleObjectQueryByExampleStore : ISingleObjectQueryByExampleStore
	{
		private readonly string _connectionString;
		private readonly ISqlDialect _dialect;
		private readonly ILogger<SqlSingleObjectQueryByExampleStore> _logger;

		public SqlSingleObjectQueryByExampleStore(string connectionString, ISqlDialect dialect, ILogger<SqlSingleObjectQueryByExampleStore> logger)
		{
			_connectionString = connectionString;
			_dialect = dialect;
			_logger = logger;
		}

		public async Task<Operation<TShape>> QuerySingleByExampleAsync<TShape>(object example, CancellationToken cancellationToken = default) =>
			await _dialect.QueryOneByExampleAsync(example,
				async (d, s, p, c) =>
					await _dialect.QuerySingleAsync<TShape>(_connectionString, s, p, c), cancellationToken);

		public async Task<Operation<TShape>> QuerySingleOrDefaultByExampleAsync<TShape>(object example, CancellationToken cancellationToken = default) =>
			await _dialect.QueryOneByExampleAsync(example,
				async (d, s, p, c) =>
					await _dialect.QuerySingleOrDefaultAsync<TShape>(_connectionString, s, p, c), cancellationToken);

		public async Task<Operation<TShape>> QueryFirstByExampleAsync<TShape>(object example, CancellationToken cancellationToken = default) =>
			await _dialect.QueryOneByExampleAsync(example,
				async (d, s, p, c) =>
					await _dialect.QueryFirstAsync<TShape>(_connectionString, s, p, c), cancellationToken);
		
		public async Task<Operation<TShape>> QueryFirstOrDefaultByExampleAsync<TShape>(object example, CancellationToken cancellationToken = default) =>
			await _dialect.QueryOneByExampleAsync(example,
				async (d, s, p, c) =>
					await _dialect.QueryFirstOrDefaultAsync<TShape>(_connectionString, s, p, c), cancellationToken);
	}
}