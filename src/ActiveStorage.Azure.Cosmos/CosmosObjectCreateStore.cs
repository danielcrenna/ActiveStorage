// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ActiveErrors;

namespace ActiveStorage.Azure.Cosmos
{
	public sealed class CosmosObjectCreateStore : IObjectCreateStore
	{
		private readonly ICosmosRepository _cosmos;

		public CosmosObjectCreateStore(ICosmosRepository cosmos)
		{
			_cosmos = cosmos;
		}
		
		public async Task<Operation<ObjectCreate>> CreateAsync(object @object, CancellationToken cancellationToken = default, params string[] fields)
		{
			var wrapper = new DocumentWrapper(@object);
			await _cosmos.CreateAsync(wrapper, cancellationToken);
			return new Operation<ObjectCreate>(ObjectCreate.Created);
		}

		/// <summary> Warning: mutable struct! </summary>
		internal struct DocumentWrapper : IDocumentEntity 
		{
			public string Id { get; set;  }
			public string DocumentType { get; }
			public object Entity { get; }

			public DocumentWrapper(object entity)
			{
				Id = default;
				Entity = entity;
				DocumentType = entity.GetType().FullName;
			}
		}
	}
}