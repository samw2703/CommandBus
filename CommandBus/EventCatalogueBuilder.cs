using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandBus
{
	internal class EventCatalogueBuilder
	{
		public EventSubscriptionsCatalogue Build(params Assembly[] assemblies)
		{
			var catalogueItems = assemblies
				.SelectMany(x => x.GetTypes())
				.SelectMany(GetCatalogueItems)
				.ToList();
			return new EventSubscriptionsCatalogue(catalogueItems);
		}

		private IEnumerable<EventSubscriptionsCatalogueItem> GetCatalogueItems(Type type)
		{
			return type
					.GetInterfaces()
					.Where(x => x.IsGenericType && !x.IsNested && x.GetGenericTypeDefinition() == typeof(IEventSubscriber<object>).GetGenericTypeDefinition())
					.Select(x => new EventSubscriptionsCatalogueItem(x.GetGenericArguments().First(), type));

		}
	}
}