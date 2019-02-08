using Autofac;
using Autofac.Extensions.DependencyInjection;
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
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MidnightLizard.Impressions.Processor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void /*IServiceProvider*/ ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMediatR();
            services.Configure<AggregatesConfig>(Configuration);

            services.AddSingleton<ElasticSearchConfig>(x =>
            {
                var esConfig = new ElasticSearchConfig();
                Configuration.Bind(esConfig);
                return esConfig;
            });
            services.AddSingleton<KafkaConfig>(x => new KafkaConfig
            {
                KAFKA_EVENTS_CONSUMER_CONFIG = JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(
                        Configuration.GetValue<string>(
                            nameof(KafkaConfig.KAFKA_EVENTS_CONSUMER_CONFIG))),

                KAFKA_REQUESTS_CONSUMER_CONFIG = JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(
                        Configuration.GetValue<string>(
                            nameof(KafkaConfig.KAFKA_REQUESTS_CONSUMER_CONFIG))),

                KAFKA_EVENTS_PRODUCER_CONFIG = JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(
                        Configuration.GetValue<string>(
                            nameof(KafkaConfig.KAFKA_EVENTS_PRODUCER_CONFIG))),

                KAFKA_REQUESTS_PRODUCER_CONFIG = JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(
                        Configuration.GetValue<string>(
                            nameof(KafkaConfig.KAFKA_REQUESTS_PRODUCER_CONFIG))),

                EVENT_TOPICS = JsonConvert
                    .DeserializeObject<string[]>(
                        Configuration.GetValue<string>(
                            nameof(KafkaConfig.EVENT_TOPICS))),

                REQUEST_TOPICS = JsonConvert
                    .DeserializeObject<string[]>(
                        Configuration.GetValue<string>(
                            nameof(KafkaConfig.REQUEST_TOPICS))),
            });

            services.AddSingleton<ImpressionsKafkaConfig>(x => new ImpressionsKafkaConfig
            {
                IMPRESSIONS_INTERNAL_EVENTS_TOPIC = this.Configuration.GetValue<string>(
                    nameof(ImpressionsKafkaConfig.IMPRESSIONS_INTERNAL_EVENTS_TOPIC)),

                IMPRESSIONS_FAILED_EVENTS_TOPIC = this.Configuration.GetValue<string>(
                    nameof(ImpressionsKafkaConfig.IMPRESSIONS_FAILED_EVENTS_TOPIC)),

                IMPRESSIONS_INTEGRATION_EVENTS_TOPIC = this.Configuration.GetValue<string>(
                    nameof(ImpressionsKafkaConfig.IMPRESSIONS_INTEGRATION_EVENTS_TOPIC)),
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
