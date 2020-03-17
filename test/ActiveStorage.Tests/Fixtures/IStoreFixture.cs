﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace ActiveStorage.Tests.Fixtures
{
	public interface IStoreFixture : IDisposable
	{
		IObjectCountStore GetCountStore();
		IObjectSaveStore GetSaveStore();
	}
}