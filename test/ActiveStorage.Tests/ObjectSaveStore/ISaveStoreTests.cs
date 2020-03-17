// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace ActiveStorage.Tests.ObjectSaveStore
{
	public interface ISaveStoreTests
	{
		Task<bool> Empty_database_has_no_objects();
		Task<bool> Can_save_object_to_store();
	}
}