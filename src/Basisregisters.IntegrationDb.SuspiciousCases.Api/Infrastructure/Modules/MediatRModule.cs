namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Infrastructure.Modules
{
    using System.Linq;
    using Autofac;
    using List;
    using MediatR;

    public class MediatRModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder
                .RegisterAssemblyTypes(typeof(SuspiciousCasesListRequestHandler).Assembly)
                .Where(t => t
                    .GetInterfaces()
                    .Any(i => i.IsGenericType
                              && (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                                  || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))))
                .AsImplementedInterfaces();
        }
    }
}
