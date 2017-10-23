using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Compass.Domain;
using Compass.Domain.DataStore;
using Compass.Domain.DataStore.Couchbase;
using Compass.Domain.Services.KafkaStream;
using Compass.Domain.Services.RegisterNewApplication;
using Compass.Shared;
using Module = Autofac.Module;

namespace Compass.CoreServer
{
    public class CompassDependencyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var postfixes = new List<string> {"Service", "Policy"};
            var exclude = new List<Type> {typeof(CircuitBreakerPolicy), typeof(KafkaStreamService) };
            // Register Domain Services
            var assembly = typeof(IRegisterNewApplicationService).GetTypeInfo().Assembly;
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => postfixes.Any(p => t.Name.EndsWith(p)))
                .Where(t => !exclude.Contains(t))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // Register service to provide environment variables.
            builder.RegisterType<CompassEnvironment>()
                .As<ICompassEnvironment>()
                .InstancePerLifetimeScope();
            
            RegisterDataStore(builder);

            RegisterKafkaService(builder);

            RegisterCircuitBreaker(builder);

            base.Load(builder);
        }

        private void RegisterCircuitBreaker(ContainerBuilder builder)
        {
            builder.RegisterType<CircuitBreakerPolicy>()
                .As<ICircuitBreakerPolicy>()
                .SingleInstance();
        }

        private void RegisterKafkaService(ContainerBuilder builder)
        {
            builder.RegisterType<KafkaStreamService>()
                .As<IKafkaStreamService>()
                .SingleInstance();
        }

        private static void RegisterDataStore(ContainerBuilder builder)
        {
            // To use a datastore other than Couchbase, remove the Couchbase
            // registration and register your own implementation of IDataStore
            // for Compass Core Server to use.

            builder.RegisterType<CouchbaseFactory>()
                .As<ICouchbaseFactory>()
                .InstancePerLifetimeScope();

            // Using this homegrown couchbase client because of a bug 
            // in .NET Core.
            builder.RegisterType<CouchbaseClient>()
                .As<ICouchbaseClient>()
                .InstancePerLifetimeScope();

            // Register DataStore
            builder.RegisterType<DataStore>()
                .As<IDataStore>()
                .InstancePerLifetimeScope();
        }
    }
}
