// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;

namespace ActiveStorage
{
	public interface IObjectDeleteByExampleStore
	{
		Task<Operation<int>> DeleteByExampleAsync<T>(object example, CancellationToken cancellationToken = default);
	}
}