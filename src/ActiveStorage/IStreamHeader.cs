// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace ActiveStorage
{
	public interface IStreamHeader
	{
		long Start { get; }
		long End { get; }
		int Count { get; }

		long TotalCount { get; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }

		string BeforePage { get; }
		string AfterPage { get; }
	}
}