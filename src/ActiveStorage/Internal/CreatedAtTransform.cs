// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using TypeKitchen;

namespace ActiveStorage.Internal
{
	internal sealed class CreatedAtTransform : IFieldTransform
	{
		private readonly Func<DateTimeOffset> _getTimestampFunc;

		public CreatedAtTransform(Func<DateTimeOffset> getTimestampFunc) => _getTimestampFunc = getTimestampFunc;

		public bool TryTransform(IReadAccessor accessor, object @object, AccessorMember member, out object transformed)
		{
			if (member.CanWrite && member.Name == "CreatedAt" &&
			    (member.Type == typeof(DateTimeOffset) || member.Type == typeof(DateTimeOffset?)))
			{
				var timestamp = _getTimestampFunc();
				if (WriteAccessor.Create(@object).TrySetValue(@object, member.Name, timestamp))
				{
					transformed = timestamp;
					return true;
				}
			}

			transformed = null;
			return false;
		}
	}
}