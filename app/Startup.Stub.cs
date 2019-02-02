using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MidnightLizard.Impressions.Processor
{
    public class StartupStub : Startup
    {
        public StartupStub(IConfiguration configuration) : base(configuration)
        {
        }

        public static TResult Resolve<TResult>()
        {
            return new WebHostBuilder().UseStartup<StartupStub>()
                .ConfigureServices(x => x.AddAutofac())
                .Build().Services.GetService<TResult>();
        }

        public static TResult Resolve<TResult, TService>(Func<IServiceProvider, TService> withServiceFactory)
            where TService : class
        {
            return new WebHostBuilder().UseStartup<StartupStub>()
                .ConfigureServices(services =>
                {
                    services.AddAutofac().AddScoped<TService>(withServiceFactory);
                }).Build().Services.GetService<TResult>();
        }
    }
}
