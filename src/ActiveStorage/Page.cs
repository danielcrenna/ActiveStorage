// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ActiveStorage
{
	[DataContract]
	public class Page<T> : Page, IPage<T>
	{
		private readonly IEnumerable<T> _source;

		public Page(IEnumerable<T> source, int count, int index, int size, long totalCount) : base(source, count, index,
			size, totalCount) => _source = source;

		public new IEnumerator<T> GetEnumerator()
		{
			return _source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	[DataContract]
	public class Page : IPage
	{
		private readonly IEnumerable _source;

		public Page(IEnumerable source, int count, int index, int size, long totalCount)
		{
			_source = source;

			Index = index;
			Size = size;
			Count = count;
			TotalCount = totalCount;
			TotalPages = (int) Math.Ceiling(TotalCount / (double) Count);
			HasPreviousPage = Index > 1;
			HasNextPage = Index < TotalPages - 1;
			Start = Count * Index - Count + 1;
			End = Start + Count - 1;
		}

		[DataMember] public int Index { get; set; }

		[DataMember] public int Size { get; set; }

		[DataMember] public int Count { get; set; }

		[DataMember] public long TotalCount { get; set; }

		[DataMember] public long TotalPages { get; set; }

		[DataMember] public bool HasPreviousPage { get; set; }

		[DataMember] public bool HasNextPage { get; set; }

		[DataMember] public int Start { get; set; }

		[DataMember] public int End { get; set; }

		public IEnumerator GetEnumerator()
		{
			return _source.GetEnumerator();
		}
	}
}