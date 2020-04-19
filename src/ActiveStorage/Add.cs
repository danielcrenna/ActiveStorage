using System;
using ActiveStorage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveStorage
{
	public static class Add
	{
		public static IServiceCollection AddActiveStorage(this IServiceCollection services, Action<StorageBuilder> builderAction = null)
		{
			services.TryAddSingleton<IStorageProvider, CompositeStorageProvider>();
			var storage = new StorageBuilder(services);
			builderAction?.Invoke(storage);
			return services;
		}
	}
}
