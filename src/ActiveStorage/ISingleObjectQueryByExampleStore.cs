// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;

namespace ActiveStorage
{
	public interface ISingleObjectQueryByExampleStore
	{
		Task<Operation<TShape>> QuerySingleByExampleAsync<TShape>(object example,
			CancellationToken cancellationToken = default);

		Task<Operation<TShape>> QuerySingleOrDefaultByExampleAsync<TShape>(object example,
			CancellationToken cancellationToken = default);

		Task<Operation<TShape>> QueryFirstByExampleAsync<TShape>(object example,
			CancellationToken cancellationToken = default);

		Task<Operation<TShape>> QueryFirstOrDefaultByExampleAsync<TShape>(object example,
			CancellationToken cancellationToken = default);
	}
}