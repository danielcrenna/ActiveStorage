﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;

namespace ActiveStorage
{
	public interface IObjectUpdateByExampleStore
	{
		Task<Operation<int>> UpdateByExampleAsync<T>(T @object, object example,
			CancellationToken cancellationToken = default);
	}
}