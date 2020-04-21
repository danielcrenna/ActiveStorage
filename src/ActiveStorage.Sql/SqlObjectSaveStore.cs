// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;
using ActiveLogging;
using ActiveStorage.Internal;
using ActiveStorage.Sql.Internal;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	public sealed class SqlObjectSaveStore : IObjectSaveStore
	{
		private readonly string _connectionString;
		private readonly ISqlDialect _dialect;
		private readonly IEnumerable<IFieldTransform> _transforms;
		private readonly IDataInfoProvider _provider;
		private readonly ISafeLogger<SqlObjectSaveStore> _logger;

		public SqlObjectSaveStore(string connectionString, ISqlDialect dialect, IDataInfoProvider provider, ISafeLogger<SqlObjectSaveStore> logger = null, params IFieldTransform[] transforms)
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