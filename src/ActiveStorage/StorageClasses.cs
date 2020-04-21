// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Extensions.Primitives;

namespace ActiveStorage
{
	internal static class StorageClasses
	{
		public static readonly HashSet<Type> BooleanTypes = new HashSet<Type>
		{
			typeof(bool),
			typeof(bool?)
		};

		public static readonly HashSet<Type> DateTypes = new HashSet<Type>
		{
			typeof(DateTime),
			typeof(DateTimeOffset),
			typeof(DateTime?),
			typeof(DateTimeOffset?)
		};

		public static readonly HashSet<Type> RealNumberTypes = new HashSet<Type>
		{
			typeof (float),
			typeof (float?),
			typeof (double),
			typeof (double?),
			typeof (decimal),
			typeof (decimal?),
			typeof (Complex),
			typeof (Complex?)
		};

		public static readonly HashSet<Type> TextTypes = new HashSet<Type>
		{
			typeof(char),
			typeof(char?),
			typeof(string),
			typeof(StringValues),
			typeof(StringValues?)
		};

		public static readonly HashSet<Type> IntegerTypes = new HashSet<Type>
		{
			typeof(sbyte),
			typeof(sbyte?),
			typeof(byte),
			typeof(byte?),
			typeof(ushort),
			typeof(ushort?),
			typeof(short),
			typeof(short?),
			typeof(uint),
			typeof(uint?),
			typeof(int),
			typeof(int?),
			typeof(ulong),
			typeof(ulong?),
			typeof(long),
			typeof(long?),
			typeof(BigInteger),
			typeof(BigInteger?)
		};
	}
}