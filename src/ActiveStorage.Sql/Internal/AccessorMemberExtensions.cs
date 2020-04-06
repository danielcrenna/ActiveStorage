// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveStorage.Sql.Builders;
using TypeKitchen;

namespace ActiveStorage.Sql.Internal
{
	internal static class AccessorMemberExtensions
	{
		public static async Task<Operation<ulong>> CountAsync(this AccessorMembers members, ISqlDialect dialect, string connectionString, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var sql = dialect.Count(members);
			var count = await dialect.QuerySingleAsync<ulong>(connectionString, sql, cancellationToken: cancellationToken);
			return new Operation<ulong> {Data = count};
		}

		public static async Task InsertAsync(this AccessorMembers members, ISqlDialect dialect, string connectionString, IReadOnlyDictionary<AccessorMember, object> hash, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var sql = dialect.InsertInto(members, hash.Keys, hash.Count, false);
			var parameters = hash.Keys.ToDictionary(k => $"{dialect.Parameter}{dialect.ResolveColumnName(k)}", v => hash[v]);
			
			var inserted = await dialect.ExecuteAsync(connectionString, sql, parameters, cancellationToken);
			Debug.Assert(inserted == 1);
		}

		public static IReadOnlyDictionary<AccessorMember, object> ToHash(this AccessorMembers members, object @object, params IFieldTransform[] transforms)
		{
			var accessor = ReadAccessor.Create(@object, members.Types, members.Scope);
			
			return members.ToDictionary(k => k, v =>
			{
				foreach (var field in transforms)
					if (field.TryTransform(accessor, @object, v, out var transformed))
						return transformed;

				accessor.TryGetValue(@object, v.Name, out var untransformed);
				return untransformed;
			});
		}
	}
}