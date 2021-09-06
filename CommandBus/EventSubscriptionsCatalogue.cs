using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandBus
{
	internal class EventSubscriptionsCatalogue
	{
		private readonly List<EventSubscriptionsCatalogueItem> _items;
		private readonly IServiceProvider _serviceProvider;

		public EventSubscriptionsCatalogue(List<EventSubscriptionsCatalogueItem> items, IServiceProvider serviceProvider)
		{
			_items = items;
			_serviceProvider = serviceProvider;
		}

		public List<IEventSubscriber<T>> GetSubscribers<T>()
		{
			return _items
				.Where(x => x.EventType == typeof(T))
				.Select(x => _serviceProvider.GetService<IEventSubscriber<T>>(x.SubscriberType))
				.ToList();
		}
	}
}