// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using TypeKitchen;

namespace ActiveStorage
{
	public interface IDataInfoProvider
	{
		bool IsIgnored(AccessorMember member);
		bool IsSaved(AccessorMember member);
	}
}