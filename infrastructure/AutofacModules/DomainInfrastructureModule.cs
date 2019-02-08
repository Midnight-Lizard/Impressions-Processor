using Autofac;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
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
            var ass = this.GetType().Assembly;

            builder.RegisterAssemblyTypes(ass)
                .AsClosedTypesOf(typeof(IDomainEventDispatcher<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(ass)
                .AsClosedTypesOf(typeof(IDomainEventStore<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(ass)
                .AsClosedTypesOf(typeof(IAggregateSnapshotAccessor<,>))
                .SingleInstance();

            builder.RegisterType<MessagingQueue>()
                .As<IMessagingQueue>()
                .SingleInstance();
        }
    }
}
