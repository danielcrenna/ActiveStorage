// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.Serialization;

namespace ActiveStorage
{
	[DataContract]
	public enum ObjectCreate
	{
		[EnumMember] NotFound,
		[EnumMember] NoChanges,
		[EnumMember] Created
	}
}