using Autofac;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MidnightLizard.Impressions.Infrastructure.AutofacModules;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Processor.AutofacModules;
using MidnightLizard.Impressions.Processor.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MidnightLizard.Impressions.Processor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void /*IServiceProvider*/ ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMediatR();
            services.Configure<AggregatesConfig>(this.Configuration);

            services.AddSingleton(x =>
                JsonConvert.DeserializeObject<LikesElasticSearchConfig>(
                    this.Configuration.GetValue<string>(
                        LikesElasticSearchConfig.ConfigName)));

            services.AddSingleton(x =>
                JsonConvert.DeserializeObject<FavoritesElasticSearchConfig>(
                    this.Configuration.GetValue<string>(
                        FavoritesElasticSearchConfig.ConfigName)));

            services.AddSingleton(x => new KafkaConfig
            {
                KAFKA_EVENTS_CONSUMER_CONFIG = JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(
                        this.Configuration.GetValue<string>(
                            nameof(KafkaConfig.KAFKA_EVENTS_CONSUMER_CONFIG))),

                KAFKA_REQUESTS_CONSUMER_CONFIG = JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(
                        this.Configuration.GetValue<string>(
                            nameof(KafkaConfig.KAFKA_REQUESTS_CONSUMER_CONFIG))),

                KAFKA_EVENTS_PRODUCER_CONFIG = JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(
                        this.Configuration.GetValue<string>(
                            nameof(KafkaConfig.KAFKA_EVENTS_PRODUCER_CONFIG))),

                KAFKA_REQUESTS_PRODUCER_CONFIG = JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(
                        this.Configuration.GetValue<string>(
                            nameof(KafkaConfig.KAFKA_REQUESTS_PRODUCER_CONFIG))),

                EVENT_TOPICS = JsonConvert
                    .DeserializeObject<string[]>(
                        this.Configuration.GetValue<string>(
                            nameof(KafkaConfig.EVENT_TOPICS))),

                REQUEST_TOPICS = JsonConvert
                    .DeserializeObject<string[]>(
                        this.Configuration.GetValue<string>(
                            nameof(KafkaConfig.REQUEST_TOPICS))),
            });

            services.AddSingleton(x =>
            {
                var config = new LikesKafkaConfig();
                this.Configuration.Bind(config);
                return config;
            });

            services.AddSingleton(x =>
            {
                var config = new FavoritesKafkaConfig();
                this.Configuration.Bind(config);
                return config;
            });

            services.AddMemoryCache();
            services.AddMvc();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<DomainInfrastructureModule>();
            builder.RegisterModule<MessageSerializationModule>();
            builder.RegisterModule<VersionModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
