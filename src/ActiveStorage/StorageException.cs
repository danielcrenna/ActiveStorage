// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace ActiveStorage
{
	public sealed class StorageException : Exception
	{
		public StorageException(string message, Exception innerException = null) : base(message, innerException) { }
	}
}