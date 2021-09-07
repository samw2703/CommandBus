using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandBus
{
	public static class ServiceCollectionExtensions
	{
		public static void AddCommandBus(this IServiceCollection serviceCollection, params Assembly[] assemblies)
		{
			serviceCollection.AddSingleton(new Config(false));
			serviceCollection.AddSingleton<IEventPublisher, EventPublisher>();
			serviceCollection.AddSingleton<ICommandBus, CommandBus>();
			AddCommandCatalogueServices(serviceCollection, assemblies);
			AddEventCatalogueServices(serviceCollection, assemblies);
		}

		private static void AddEventCatalogueServices(IServiceCollection serviceCollection, Assembly[] assemblies)
		{
			var catalogueItems = new EventCatalogueItemsProvider().Get(assemblies);
			foreach (var catalogueItem in catalogueItems)
				serviceCollection.AddTransient(catalogueItem.SubscriberType);

			serviceCollection.AddSingleton(new EventSubscriptionsCatalogue(catalogueItems));
		}

		private static void AddCommandCatalogueServices(IServiceCollection serviceCollection, Assembly[] assemblies)
		{
			var catalogueItems = new CommandCatalogueItemsProvider().Get(assemblies);
			foreach (var catalogueItem in catalogueItems)
			{
				serviceCollection.AddTransient(catalogueItem.HandlerType);

				if (catalogueItem.ValidatorType != null)
					serviceCollection.AddTransient(catalogueItem.ValidatorType);
			}

			serviceCollection.AddSingleton<ICommandCatalogue>(new CommandCatalogue(catalogueItems));
		}
	}
}