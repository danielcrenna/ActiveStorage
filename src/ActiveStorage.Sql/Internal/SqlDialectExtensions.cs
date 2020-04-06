// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveStorage.Sql.Builders;
using TypeKitchen;

namespace ActiveStorage.Sql.Internal
{
	internal static class SqlDialectExtensions
	{
		public static async Task<Operation<TShape>> QueryOneByExampleAsync<TShape>(this ISqlDialect dialect, object example, Func<ISqlDialect, string, IReadOnlyDictionary<string, object>, CancellationToken, Task<TShape>> queryFunc, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var members = AccessorMembers.Create(example, AccessorMemberTypes.Properties, AccessorMemberScope.Public);
			var hash = members.ToHash(example);

			var sql = dialect.Select(members, hash);
			var parameters = hash.ToDictionary(k => $"{dialect.Parameter}{dialect.ResolveColumnName(k.Key)}", v => hash[v.Key]);

			var result = await queryFunc(dialect, sql, parameters, cancellationToken);
			return new Operation<TShape> {Data = result};
		}
	}
}