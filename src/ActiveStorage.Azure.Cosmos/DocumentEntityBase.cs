// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace ActiveStorage.Azure.Cosmos
{
	public abstract class DocumentEntityBase<T> : IDocumentEntity
	{
		public string Id { get; set; }

		public string DocumentType
		{
			get => typeof(T).Name;
			set
			{
				if (value != typeof(T).Name)
					throw new InvalidOperationException();
			}
		}
	}
}