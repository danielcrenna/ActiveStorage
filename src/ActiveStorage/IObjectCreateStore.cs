// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;

namespace ActiveStorage
{
	public interface IObjectCreateStore
	{
		Task<Operation<ObjectSave>> CreateAsync(object @object, CancellationToken cancellationToken = default,
			params string[] fields);
	}
}