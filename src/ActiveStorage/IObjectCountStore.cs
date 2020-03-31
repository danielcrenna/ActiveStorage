// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;

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


	public interface IObjectCountStore
	{
		Task<Operation<ulong>> CountAsync(Type type, CancellationToken cancellationToken = default);
		Task<Operation<ulong>> CountAsync<T>(CancellationToken cancellationToken = default);
	}
}