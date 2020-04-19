// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace ActiveStorage
{
	public sealed class StorageBuilder
	{
		public IServiceCollection Services { get; }

		public StorageBuilder(IServiceCollection services)
		{
			Services = services;
		}
	}
}