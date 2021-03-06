﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;

namespace ActiveStorage
{
	public interface IObjectCountStore
	{
		Task<Operation<ulong>> CountAsync(Type type, CancellationToken cancellationToken = default);
		Task<Operation<ulong>> CountAsync<T>(CancellationToken cancellationToken = default);
	}
}