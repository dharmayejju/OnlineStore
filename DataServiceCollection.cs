using OnlineStore;
using OnlineStore.Infrastructure;
using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataServiceCollection
    {
        public static IServiceCollection AddData(this IServiceCollection services, Action<DataServiceOptions> configure)
        {
            services
                .Configure(configure)
                .AddOptions()
                .AddSingleton<IConnectionSettings, DataServiceConnectionSettings>()
                .AddScoped<IConnection, MySqlDbConnection>();

            return services;
        }
    }
}
