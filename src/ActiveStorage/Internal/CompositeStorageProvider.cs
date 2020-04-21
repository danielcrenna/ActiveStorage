// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace ActiveStorage.Internal
{
	internal sealed class CompositeStorageProvider : IStorageProvider
	{
		private readonly IEnumerable<IAppenderProvider> _appenders;

		public CompositeStorageProvider(IEnumerable<IAppenderProvider> appenders)
		{
			_appenders = appenders;
		}

		public IObjectAppendStore GetAppender(string slot)
		{
			foreach (var appender in _appenders)
			{
				var store = appender.GetAppender(slot);
				if (store != null)
				{
					return store;
				}
			}
			return default;
		}
	}
}