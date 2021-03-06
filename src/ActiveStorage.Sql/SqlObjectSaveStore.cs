﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveStorage.Internal;
using ActiveStorage.Sql.Internal;
using Microsoft.Extensions.Logging;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	public sealed class SqlObjectFetchStore : IObjectFetchStore
	{
		private readonly string _connectionString;
		private readonly ISqlDialect _dialect;
		private readonly IEnumerable<IFieldTransform> _transforms;
		private readonly IDataInfoProvider _provider;
		private readonly ILogger<SqlObjectFetchStore> _logger;

		public SqlObjectFetchStore(string connectionString, ISqlDialect dialect, IDataInfoProvider provider, ILogger<SqlObjectFetchStore> logger = null, 
			params IFieldTransform[] transforms)
		{
			_connectionString = connectionString;
			_dialect = dialect;
			_transforms = transforms;
			_provider = provider;
			_logger = logger;
		}
		
		public async Task<Operation<IEnumerable<T>>> FetchAsync<T>(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var members = AccessorMembers.Create(typeof(T), AccessorMemberTypes.Properties, AccessorMemberScope.Public);
				var result = await members.SelectAsync<T>(_dialect, _connectionString, cancellationToken: cancellationToken);
				return Operation.FromResult(result);
			}
			catch (StorageException e)
			{
				return Operation.FromResult(Enumerable.Empty<T>(), new List<Error>
				{
					new Error(ErrorEvents.AggregateErrors, e.Message)
				});
			}
		}
	}

	public sealed class SqlObjectSaveStore : IObjectSaveStore
	{
		private readonly string _connectionString;
		private readonly ISqlDialect _dialect;
		private readonly IEnumerable<IFieldTransform> _transforms;
		private readonly IDataInfoProvider _provider;
		private readonly ILogger<SqlObjectSaveStore> _logger;

		public SqlObjectSaveStore(string connectionString, ISqlDialect dialect, IDataInfoProvider provider, ILogger<SqlObjectSaveStore> logger = null, params IFieldTransform[] transforms)
		{
			_connectionString = connectionString;
			_dialect = dialect;
			_transforms = transforms;
			_provider = provider;
			_logger = logger;
		}
		
		public async Task<Operation<ObjectSave>> SaveAsync(object @object, CancellationToken cancellationToken = default, params string[] fields)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var members = AccessorMembers.Create(@object, AccessorMemberTypes.Properties, AccessorMemberScope.Public);
				var hash = members.ToHash(@object, fields, _provider, _transforms);
				await members.InsertAsync(_dialect, _connectionString, hash, cancellationToken);
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