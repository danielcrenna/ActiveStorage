﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;

namespace ActiveStorage
{
	public interface IObjectQueryByExamplePagedStore
	{
		Task<Operation<IPage<T>>> QueryByExampleAsync<T>(CancellationToken cancellationToken = default);

		Task<Operation<IPage<T>>> QueryByExampleAsync<T>(object example,
			CancellationToken cancellationToken = default);

		Task<Operation<T>> QuerySingleOrDefaultByExampleAsync<T>(object example,
			CancellationToken cancellationToken = default);

		Task<Operation<T>> QueryFirstOrDefaultByExampleAsync<T>(object example,
			CancellationToken cancellationToken = default);
	}
}