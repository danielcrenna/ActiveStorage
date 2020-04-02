// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveLogging;

namespace ActiveStorage.Sql
{
	public sealed class SqlObjectCreateStore : SqlObjectStore, IObjectCreateStore
	{
		public SqlObjectCreateStore(string connectionString, ISqlDialect dialect, IDataInfoProvider provider, ISafeLogger<SqlObjectCreateStore> logger = null, params IFieldTransform[] fieldTransforms) : base(connectionString, dialect, provider, logger, fieldTransforms)
		{ }

		public async Task<Operation<ObjectCreate>> CreateAsync(object @object, CancellationToken cancellationToken = default, params string[] fields)
		{
			try
			{
				var members = BeforeCreate(@object, out var hash);
				await InsertAsync(hash, fields, members);
				return Operation.FromResult(ObjectCreate.Created);
			}
			catch (StorageException e)
			{
				return Operation.FromResult(ObjectCreate.Error, new List<Error>
				{
					new Error(ErrorEvents.AggregateErrors, e.Message)
				});
			}
		}
	}
}