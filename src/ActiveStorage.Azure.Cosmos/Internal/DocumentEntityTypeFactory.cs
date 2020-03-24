// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ActiveStorage.Azure.Cosmos.Internal
{
	internal static class DocumentEntityTypeFactory<T> where T : IDocumentEntity
	{
		// ReSharper disable once StaticMemberInGenericType
		private static readonly Dictionary<Type, string> Types = new Dictionary<Type, string>();

		static DocumentEntityTypeFactory()
		{
			if (FormatterServices.GetSafeUninitializedObject(typeof(T)) is T type)
				Types[typeof(T)] = type.DocumentType;
		}

		public static string Type => Types[typeof(T)];
	}
}