using Autofac;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Infrastructure.EventStore;
using MidnightLizard.Impressions.Infrastructure.Queue;
using MidnightLizard.Impressions.Infrastructure.Snapshot;
using System.Reflection;

namespace MidnightLizard.Impressions.Infrastructure.AutofacModules
{
    public class DomainInfrastructureModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(DomainEventDispatcher<>).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IDomainEventDispatcher<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(DomainEventStore<>).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IDomainEventStore<>))
                .SingleInstance();

            builder.RegisterType<MessagingQueue>()
                .As<IMessagingQueue>()
                .SingleInstance();

            builder.RegisterType<ImpressionsSnapshotAccessor>()
                .As<IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>>()
                .SingleInstance();

            //builder.RegisterAssemblyTypes(typeof(DomainInfrastructureModule).GetTypeInfo().Assembly)
            //    .AsImplementedInterfaces();
        }
    }
}
