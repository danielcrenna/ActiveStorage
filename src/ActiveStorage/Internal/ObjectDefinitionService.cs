// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ActiveStorage.Internal
{
	internal sealed class ObjectDefinitionHostedService : IHostedService
	{
		private readonly IEnumerable<IObjectDefinition> _definitions;

		public ObjectDefinitionHostedService(IEnumerable<IObjectDefinition> definitions)
		{
			_definitions = definitions;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			foreach (var definition in _definitions)
			{
				await definition.VisitAsync(VisitType.Eager);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}