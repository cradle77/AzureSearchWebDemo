using System;
using Microsoft.Azure.WebJobs;
using Ninject;
using Common;
using Rebus.Handlers;
using Common.Events;
using SearchSyncJob.Handlers;
using Rebus.Ninject;
using Rebus.Config;
using Rebus.Bus;
using Rebus.Routing.TypeBased;

namespace SearchSyncJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        private static IBus _bus;
        private static IKernel _kernel;

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            _kernel = SetupServices();

            var manager = _kernel.Get<SearchManager>();
            var task = manager.InitializeAsync();

            task.Wait();

            manager.RebuildIndexAsync().Wait();

            _bus.Subscribe<ProductUpdated>();

            //var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();

            Console.ReadLine();
        }

        private static IKernel SetupServices()
        {
            var kernel = new StandardKernel();

            kernel.Bind<ISearchSettings>().To<JobSettings>().InSingletonScope();
            kernel.Bind<IHandleMessages<ProductUpdated>>().To<ProductMessageHandler>();

            var settings = kernel.Get<ISearchSettings>();

            var activator = new NinjectContainerAdapter(kernel);

            _bus = Configure.With(activator)
                .Transport(t => t.UseAzureServiceBus(settings.BusConnectionString, "SearchSyncJob"))
                .Routing(r => r.TypeBased().Map<IProductEvent>("products"))
                .Start();

            return kernel;
        }
    }
}
