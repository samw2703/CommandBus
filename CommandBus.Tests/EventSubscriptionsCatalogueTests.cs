using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class EventSubscriptionsCatalogueTests
	{
		[Test]
		public void GetSubscribers_NoSubscriberMatchObjectType_ReturnsEmptyList()
		{
			var subscribers = GetSubscribers(new { });

			Assert.True(subscribers.None());
		}

		[Test]
		public void GetSubscribers_MultipleSubscriberMatchesObjectType_ReturnsListOfSubscribers()
		{
			var subscribers = GetSubscribers(new Event1());

			Assert.AreEqual(2, subscribers.Count);
			Assert.IsNotNull(subscribers.SingleOrDefault(x => x == typeof(EventSubscriber11)));
			Assert.IsNotNull(subscribers.SingleOrDefault(x => x == typeof(EventSubscriber12)));
		}

		private List<Type> GetSubscribers(object @event)
		{
			var items = new List<EventSubscriptionsCatalogueItem>
			{
				new EventSubscriptionsCatalogueItem(typeof(Event1), typeof(EventSubscriber11)),
				new EventSubscriptionsCatalogueItem(typeof(Event1), typeof(EventSubscriber12)),
				new EventSubscriptionsCatalogueItem(typeof(Event2), typeof(EventSubscriber21))
			}; 
			return new EventSubscriptionsCatalogue(items)
				.GetSubscribers(@event);
		}

		private class Event1
		{
		}

		private class Event2
		{
		}

		private class EventSubscriber11 : IEventSubscriber<Event1>
		{
			public Task Execute(Event1 @event)
			{
				throw new NotImplementedException();
			}
		}

		private class EventSubscriber12 : IEventSubscriber<Event1>
		{
			public Task Execute(Event1 @event)
			{
				throw new NotImplementedException();
			}
		}

		private class EventSubscriber21 : IEventSubscriber<Event2>
		{
			public Task Execute(Event2 @event)
			{
				throw new NotImplementedException();
			}
		}
	}
}