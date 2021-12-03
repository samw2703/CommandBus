using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommandBus
{
	public static class ServiceCollectionExtensions
	{
		public static void AddCommandBus(this IServiceCollection serviceCollection, Assembly assembly, params Assembly[] otherAssemblies)
		{
			serviceCollection.AddSingleton(new Config(false));
			serviceCollection.TryAddScoped<IEventPublisher, EventPublisher>();
			serviceCollection.AddScoped<ICommandBus, CommandBus>();
			var assemblies = new List<Assembly> {assembly}
				.WithRange(otherAssemblies)
				.ToArray();
			AddCommandCatalogueServices(serviceCollection, assemblies);
			AddEventCatalogueServices(serviceCollection, assemblies);
		}

		public static void AddCommandBus<TEventPublisher>(this IServiceCollection serviceCollection, Assembly assembly, params Assembly[] otherAssemblies)
			where TEventPublisher : class, IEventPublisher
		{
			serviceCollection.AddScoped<IEventPublisher, TEventPublisher>();
			serviceCollection.AddCommandBus(assembly, otherAssemblies);
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