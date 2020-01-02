namespace MarkdownWebsite.Infrastructure.DependencyResolution
{
    using System;
    using System.IO;
    using System.Reflection;
    using CommandProcessing;
    using LightInject;
    using MediatR;
    using Microsoft.Extensions.Configuration;

    public static class IoC
    {
        private static readonly Lazy<IServiceContainer> Bootstrapper = new Lazy<IServiceContainer>(Initialize, true);

        public static IServiceContainer Container => Bootstrapper.Value;

        private static IServiceContainer Initialize()
        {
            var container = new ServiceContainer();
            container.Register<IMediator, Mediator>();

            container.RegisterAssembly(
                typeof(Program).Assembly,
                (serviceType, implementingType) => serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            container.RegisterOrdered(
                typeof(IPipelineBehavior<,>),
                new[] { typeof(PerformanceLoggingBehavior<,>), typeof(ExceptionLoggingBehavior<,>) },
                type => new PerContainerLifetime());

            container.Register<ServiceFactory>(ctx => ctx.GetInstance);

            var configBuilder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = configBuilder.Build();
            container.RegisterInstance<IConfiguration>(configuration);

            var appState = new AppState();
            container.RegisterInstance(appState);

            return container;
        }
    }
}