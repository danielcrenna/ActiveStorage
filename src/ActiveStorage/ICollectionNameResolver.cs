// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using TypeKitchen;

namespace ActiveStorage
{
	public interface ICollectionNameResolver
	{
		string ResolveCollectionName(AccessorMembers members)
		{
			return !members.DeclaringType.TryGetAttribute(true, out TableAttribute attribute)
				? members.DeclaringType.GetNonGenericName()
				: attribute.Name;
		}
	}
}