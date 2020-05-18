using ActiveStorage.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace ActiveStorage.Sqlite
{
	public static class Add
	{
		public static IServiceCollection AddSqliteStorage(this StorageBuilder builder, string slot, string connectionString) => builder.Services.AddSqliteStorage(slot, connectionString);
		public static IServiceCollection AddSqliteStorage(this IServiceCollection services, string slot, string connectionString)
		{
			services.AddActiveStorage();

			services.TryAddEnumerable(ServiceDescriptor.Singleton<IAppenderProvider, SqliteAppenderProvider>(r => new SqliteAppenderProvider(slot, connectionString, r.GetService<IDataInfoProvider>())));
			services.TryAddEnumerable(ServiceDescriptor.Singleton<ICounterProvider, SqliteCounterProvider>(r => new SqliteCounterProvider(slot, connectionString, r.GetService<ILogger<SqlObjectCountStore>>())));
			services.TryAddEnumerable(ServiceDescriptor.Singleton<IFetcherProvider, SqliteFetcherProvider>(r => new SqliteFetcherProvider(slot, connectionString, r.GetService<IDataInfoProvider>(), r.GetService<ILogger<SqlObjectFetchStore>>())));
			services.TryAddEnumerable(ServiceDescriptor.Singleton<IDataMigrator, SqliteDataMigrator>(r => new SqliteDataMigrator(connectionString, r.GetRequiredService<IDataInfoProvider>(), r.GetServices<IDataMigratorInfoProvider>())));
			
			return services;
		}
	}
}
