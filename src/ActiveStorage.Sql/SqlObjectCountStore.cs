// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveStorage.Sql.Internal;
using Microsoft.Extensions.Logging;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	public sealed class SqlObjectCountStore : IObjectCountStore
	{
		private readonly string _connectionString;
		private readonly ISqlDialect _dialect;
		private readonly ILogger<SqlObjectCountStore> _logger;

		public SqlObjectCountStore(string connectionString, ISqlDialect dialect, ILogger<SqlObjectCountStore> logger)
		{
			_connectionString = connectionString;
			_dialect = dialect;
			_logger = logger;
		}

		public async Task<Operation<ulong>> CountAsync(Type type, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var members = AccessorMembers.Create(type, AccessorMemberTypes.Properties, AccessorMemberScope.Public);
			return await members.CountAsync(_dialect, _connectionString, cancellationToken);
		}

		public Task<Operation<ulong>> CountAsync<T>(CancellationToken cancellationToken = default) => CountAsync(typeof(T), cancellationToken);
	}
}