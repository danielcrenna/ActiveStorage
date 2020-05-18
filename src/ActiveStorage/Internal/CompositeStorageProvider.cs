// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace ActiveStorage.Internal
{
	internal sealed class CompositeStorageProvider : IStorageProvider
	{
		private readonly IEnumerable<IAppenderProvider> _appenders;
		private readonly IEnumerable<ICounterProvider> _counters;
		private readonly IEnumerable<IFetcherProvider> _fetchers;

		public CompositeStorageProvider(
			IEnumerable<IAppenderProvider> appenders, 
			IEnumerable<ICounterProvider> counters, 
			IEnumerable<IFetcherProvider> fetchers)
		{
			_appenders = appenders;
			_counters = counters;
			_fetchers = fetchers;
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

		public IObjectCountStore GetCounter(string slot)
		{
			foreach (var counter in _counters)
			{
				var store = counter.GetCounter(slot);
				if (store != null)
				{
					return store;
				}
			}
			return default;
		}

		public IObjectFetchStore GetFetcher(string slot)
		{
			foreach (var fetcher in _fetchers)
			{
				var store = fetcher.GetFetcher(slot);
				if (store != null)
				{
					return store;
				}
			}
			return default;
		}
	}
}