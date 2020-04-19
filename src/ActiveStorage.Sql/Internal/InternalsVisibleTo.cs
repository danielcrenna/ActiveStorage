// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ActiveStorage.Sqlite")]
[assembly: InternalsVisibleTo("ActiveStorage.SqlServer")]
[assembly: InternalsVisibleTo("ActiveStorage.Tests")]

namespace ActiveStorage.Sql.Internal
{
	internal sealed class InternalsVisibleTo { }
}