using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class EventCatalogueBuilderTests
	{
		[Test]
		public void Build_WhereMultipleClassesImplementsEventSubscriberForEvent_RetrievesAllSubscribers()
		{
			var subscribers = Get()
				.Where(x => x.EventType == typeof(Event1))
				.Select(x => x.SubscriberType)
				.ToList();

			Assert.AreEqual(2, subscribers.Count);
			Assert.True(subscribers.Contains(typeof(EventSubscriber11)));
			Assert.True(subscribers.Contains(typeof(EventSubscriber12)));
		}

		[Test]
		public void Build_ClassImplementsMultipleEventSubscribers_RetrievesAllSubscribers()
		{
			var catalogueItems = Get();
			var event2Subscribers = catalogueItems
				.Where(x => x.EventType == typeof(Event2))
				.Select(x => x.SubscriberType)
				.ToList();
			var event3Subscribers = catalogueItems
				.Where(x => x.EventType == typeof(Event3))
				.Select(x => x.SubscriberType)
				.ToList();

			Assert.AreEqual(typeof(MultiEventSubscriber), event2Subscribers.Single());
			Assert.AreEqual(typeof(MultiEventSubscriber), event3Subscribers.Single());
		}

		private List<EventSubscriptionsCatalogueItem> Get()
		{
			return new EventCatalogueItemsProvider()
				.Get(GetType().Assembly);
		}
	}

	internal class Event1
	{
	}

	internal class EventSubscriber11 : IEventSubscriber<Event1>
	{
		public Task Execute(Event1 @event)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class EventSubscriber12 : IEventSubscriber<Event1>
	{
		public Task Execute(Event1 @event)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class Event2
	{
	}

	internal class Event3
	{
	}

	internal class MultiEventSubscriber : IEventSubscriber<Event2>, IEventSubscriber<Event3>
	{
		public Task Execute(Event3 @event)
		{
			throw new System.NotImplementedException();
		}

		public Task Execute(Event2 @event)
		{
			throw new System.NotImplementedException();
		}
	}
}