using System;
using System.Linq;

namespace CommandBus
{
	internal class EventSubscriptionsCatalogueItem
	{
		public Type EventType { get; }
		public Type SubscriberType { get; }

		public EventSubscriptionsCatalogueItem(Type eventType, Type subscriberType)
		{
			Validate(eventType, subscriberType);

			EventType = eventType;
			SubscriberType = subscriberType;
		}

		private void Validate(Type eventType, Type subscriberType)
		{
			var subscriberTypeInterfaces = subscriberType.GetInterfaces();
			if (!subscriberTypeInterfaces.Select(x => x.GetGenericTypeDefinition()).Contains(typeof(IEventSubscriber<>)))
				throw new ArgumentException("The subscriber type must implement IEventSubscriber<T>");

			var implementedSubscriberInterface = subscriberTypeInterfaces.SingleOrDefault(x => IsEventSubscriberOfEvent(x, eventType));
			if (implementedSubscriberInterface == null)
				throw new ArgumentException($"This subscriber type does not subscribe to type {eventType.FullName}");
		}

		private bool IsEventSubscriberOfEvent(Type interfaceType, Type eventType)
		{
			if (interfaceType.GetGenericTypeDefinition() != typeof(IEventSubscriber<>))
				return false;

			return interfaceType.GetGenericArguments().Single() == eventType;
		}
	}
}