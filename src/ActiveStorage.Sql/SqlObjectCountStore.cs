// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveStorage.Sql.Builders;
using Microsoft.Extensions.DependencyInjection;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	

	public sealed class SqlObjectCountStore : IObjectCountStore
	{
		private readonly string _connectionString;
		private readonly ISqlDialect _dialect;

		public SqlObjectCountStore(string connectionString, ISqlDialect dialect)
		{
			_connectionString = connectionString;
			_dialect = dialect;
		}

		public async Task<Operation<ulong>> CountAsync(Type type, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var members = AccessorMembers.Create(type, AccessorMemberTypes.Properties, AccessorMemberScope.Public);
			var sql = _dialect.Count(members);
			var count = await _dialect.QuerySingleAsync<ulong>(_connectionString, sql);
			return new Operation<ulong> {Data = count};
		}

		public Task<Operation<ulong>> CountAsync<T>(CancellationToken cancellationToken = default)
		{
			return CountAsync(typeof(T), cancellationToken);
		}
	}
}