// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace ActiveStorage
{
	public interface IPageHeader
	{
		int Index { get; }
		int Size { get; }
		int Count { get; }

		long TotalCount { get; }
		long TotalPages { get; }

		bool HasPreviousPage { get; }
		bool HasNextPage { get; }

		int Start { get; }
		int End { get; }
	}
}