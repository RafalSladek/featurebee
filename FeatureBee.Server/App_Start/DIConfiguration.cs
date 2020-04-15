namespace FeatureBee.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.SignalR;
    using Autofac.Integration.WebApi;

    using FeatureBee.Server.Domain.ApplicationServices;
    using FeatureBee.Server.Domain.EventHandlers;
    using FeatureBee.Server.Domain.EventHandlers.DatabaseHandlers;
    using FeatureBee.Server.Domain.EventHandlers.HistoryHandlers;
    using FeatureBee.Server.Domain.EventHandlers.HubHandlers;
    using FeatureBee.Server.Domain.Infrastruture;

    using NEventStore;
    using NEventStore.Dispatcher;
    using NEventStore.Persistence.SqlPersistence.SqlDialects;

    using Module = Autofac.Module;

    public class DIConfiguration : Module
    {
        internal IContainer BuildApplicationContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(this);

            return builder.Build();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Register the SignalR hubs.
            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Repository", StringComparison.Ordinal))
                .AsImplementedInterfaces();


            builder.RegisterType<FeatureApplicationServices>().AsImplementedInterfaces();
            builder.RegisterType<CommandSender>().As<ICommandSender>();

            var innerBuilder = new ContainerBuilder();
            innerBuilder.RegisterTypes(ThisAssembly.GetTypes().Where(_ => typeof(IHubBroadcasterFor).IsAssignableFrom(_)).ToArray())
                .As<IHubBroadcasterFor>();

            innerBuilder.RegisterTypes(ThisAssembly.GetTypes().Where(_ => typeof(IDatabaseBroadcasterFor).IsAssignableFrom(_)).ToArray())
                .As<IDatabaseBroadcasterFor>();

            innerBuilder.RegisterTypes(ThisAssembly.GetTypes().Where(_ => typeof(IHistoryBroadcasterFor).IsAssignableFrom(_)).ToArray())
                .As<IHistoryBroadcasterFor>();

            IContainer innerContainer = innerBuilder.Build();
            var eventHandlers = new IEventHandler[] {
                new DatabaseEventHandler(innerContainer.Resolve<IEnumerable<IDatabaseBroadcasterFor>>()),
                new HubEventHandler(innerContainer.Resolve<IEnumerable<IHubBroadcasterFor>>()),
                new HistoryEventHandler(innerContainer.Resolve<IEnumerable<IHistoryBroadcasterFor>>())
            };
            var dispatcher = new NEventStoreDispatcher(eventHandlers);

            var nEventStore =
                Wireup.Init()
                    .LogToOutputWindow()
                    .UsingSqlPersistence("FeatureBeeContext")
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    // .EnlistInAmbientTransaction()
                    .UsingJsonSerialization()
                    .UsingSynchronousDispatchScheduler()
                    .DispatchTo(new DelegateMessageDispatcher(dispatcher.DispatchCommit))
                    .Build();

            builder.RegisterInstance(nEventStore);
        }
    }
}