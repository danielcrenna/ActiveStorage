// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TypeKitchen;

namespace ActiveStorage
{
	public sealed class AttributeDataInfoProvider : IDataInfoProvider
	{
		public bool IsIgnored(AccessorMember member)
		{
			return member.HasAttribute<NotMappedAttribute>();
		}

		public bool IsSaved(AccessorMember member)
		{
			if (!member.TryGetAttribute(out DatabaseGeneratedAttribute attribute))
				return true;

			return attribute.DatabaseGeneratedOption switch
			{
				DatabaseGeneratedOption.None => true,
				DatabaseGeneratedOption.Identity => false,
				DatabaseGeneratedOption.Computed => false,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}