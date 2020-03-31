// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace ActiveStorage
{
	public interface IPage : IPageHeader, IEnumerable
	{
	}

	public interface IPage<out T> : IPage, IEnumerable<T>
	{
	}
}