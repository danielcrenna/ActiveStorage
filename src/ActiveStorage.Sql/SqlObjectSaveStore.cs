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
	public sealed class SqlObjectSaveStore : SqlObjectStore, IObjectSaveStore
	{
		public SqlObjectSaveStore(string connectionString, ISqlDialect dialect, IDataInfoProvider provider, ISafeLogger<SqlObjectSaveStore> logger,
			params IFieldTransform[] fieldTransforms) : base(connectionString, dialect, provider, logger, fieldTransforms)
		{
			
		}

		public async Task<Operation<ObjectSave>> SaveAsync(object @object, CancellationToken cancellationToken = default, params string[] fields)
		{
			try
			{
				var members = BeforeCreate(@object, out var hash);
				await InsertAsync(hash, fields, members);
				return Operation.FromResult(ObjectSave.Created);
			}
			catch (StorageException e)
			{
				return Operation.FromResult(ObjectSave.Error, new List<Error>
				{
					new Error(ErrorEvents.AggregateErrors, e.Message)
				});
			}
		}
	}
}