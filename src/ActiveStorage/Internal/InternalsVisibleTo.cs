// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ActiveStorage.Sql")]
[assembly: InternalsVisibleTo("ActiveStorage.Sqlite")]
[assembly: InternalsVisibleTo("ActiveStorage.Tests")]

namespace ActiveStorage.Internal
{
	internal sealed class InternalsVisibleTo
	{
	}
}