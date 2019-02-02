using Autofac;
using Autofac.Core;
using Autofac.Features.Variance;
using MediatR;
using MidnightLizard.Impressions.Processor.Application.DomainRequestHandlers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MidnightLizard.Impressions.Processor.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // enables contravariant Resolve() for interfaces with single contravariant ("in") arg
            builder.RegisterSource(new ContravariantRegistrationSource());

            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();

            // Register all the Command classes (they implement IAsyncRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(typeof(DomainRequestHandler<,,>).GetTypeInfo().Assembly)
                .Where(handler => !handler.IsAbstract)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register the DomainEventHandler classes (they implement IAsyncNotificationHandler<>) in assembly holding the Domain Events
            //builder.RegisterAssemblyTypes(typeof(ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register the Command's Validators (Validators based on FluentValidation library)
            //builder
            //    .RegisterAssemblyTypes(typeof(CreateOrderCommandValidator).GetTypeInfo().Assembly)
            //    .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
            //    .AsImplementedInterfaces();


            //builder.Register<SingleInstanceFactory>(context =>
            //{
            //    var componentContext = context.Resolve<IComponentContext>();
            //    return t => componentContext.TryResolve(t, out var o) ? o : null;
            //});

            //builder.Register<MultiInstanceFactory>(context =>
            //{
            //    var componentContext = context.Resolve<IComponentContext>();
            //    return t => (IEnumerable<object>)componentContext
            //        .Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            //});

            //builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

        }
    }
}
