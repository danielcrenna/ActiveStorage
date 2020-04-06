// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using TypeKitchen;

namespace ActiveStorage.Internal
{
	internal static class AccessorMemberExtensions
	{
		public static IReadOnlyDictionary<AccessorMember, object> ToHash(this AccessorMembers members, object @object, string[] fields, IDataInfoProvider provider, IEnumerable<IFieldTransform> transforms)
		{
			var accessor = ReadAccessor.Create(@object, members.Types, members.Scope);

			var usePool = fields.Length > 0 || provider != null;

			// FIXME: remove AsList call
			var include = usePool ? Pooling.ListPool<AccessorMember>.Get() : members.AsList();

			try
			{
				if (fields.Length > 0)
				{
					foreach (var field in fields)
					{
						if (members.TryGetValue(field, out var member) && !IsIgnored(provider, member) && IsSaved(provider, member))
						{
							include.Add(member);
						}
					}
				}
				else
				{
					foreach (var member in members)
					{
						if (IsIgnored(provider, member))
						{
							include.Remove(member);
							continue;
						}

						if (IsSaved(provider, member))
						{
							include.Add(member);
						}
					}
				}

				return members.ToDictionary(k => k, v => ResolveValue(@object, transforms, accessor, v));
			}
			finally
			{
				if (usePool)
				{
					Pooling.ListPool<AccessorMember>.Return((List<AccessorMember>) include);
				}
			}
		}

		private static bool IsIgnored(IDataInfoProvider provider, AccessorMember member) => provider == null || provider.IsIgnored(member);

		private static bool IsSaved(IDataInfoProvider provider, AccessorMember member) => provider == null || provider.IsSaved(member);

		private static object ResolveValue(object @object, IEnumerable<IFieldTransform> transforms, IReadAccessor accessor, AccessorMember v)
		{
			foreach (var field in transforms)
				if (field.TryTransform(accessor, @object, v, out var transformed))
					return transformed;

			accessor.TryGetValue(@object, v.Name, out var untransformed);
			return untransformed;
		}
	}
}