using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandBus
{
	internal class EventSubscriptionsCatalogue
	{
		private readonly List<EventSubscriptionsCatalogueItem> _items;

		public EventSubscriptionsCatalogue(List<EventSubscriptionsCatalogueItem> items)
		{
			_items = items;
		}

		public List<Type> GetSubscribers(object @event)
		{
			return _items
				.Where(x => x.EventType == @event.GetType())
				.Select(x => x.SubscriberType)
				.ToList();
		}
	}
}