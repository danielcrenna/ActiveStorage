// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActiveStorage.Tests.Models
{
	[Table("Object")]
	public class SimpleObject
	{
		public long Id { get; set; }
		public DateTimeOffset? CreatedAt { get; set; }
	}
}