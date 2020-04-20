// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TypeKitchen;

namespace ActiveStorage.Sql
{
	public interface ISqlDialect : ISqlCommands
	{
		char? StartIdentifier { get; }
		char? EndIdentifier { get; }
		char? Separator { get; }
		char? Parameter { get; }
		char? Quote { get; }

		bool TryFetchInsertedKey(FetchInsertedKeyLocation location, out string sql);

		string ResolveTableName(AccessorMembers members)
		{
			return !members.DeclaringType.TryGetAttribute(true, out TableAttribute attribute)
				? members.DeclaringType.GetNonGenericName()
				: attribute.Name;
		}

		string ResolveSchemaName(AccessorMembers members)
		{
			return !members.DeclaringType.TryGetAttribute(true, out TableAttribute attribute) ? null : attribute.Schema;
		}

		string ResolveColumnName(AccessorMember member)
		{
			return !member.TryGetAttribute(out ColumnAttribute attribute) ? member.Name : attribute.Name;
		}

		string ResolveColumnTypeName(AccessorMember member)
		{
			if (member.TryGetAttribute(out ColumnAttribute attribute) && !string.IsNullOrWhiteSpace(attribute.TypeName))
				return attribute.TypeName;

			return ResolveTypeName(member.Type);
		}

		string ResolveColumnNullability(AccessorMember member)
		{
			if (member.TryGetAttribute(out RequiredAttribute attribute))
				return "NOT NULL";

			return "NULL";
		}

		string ResolveColumnLimit(AccessorMember member)
		{
			return member.TryGetAttribute(out MaxLengthAttribute attribute)
				? attribute.Length.ToString()
				: ResolveTypeDefaultLength(member.Type);
		}

		string ResolveTypeName(Type type);
		string ResolveTypeDefaultLength(Type type);
	}
}