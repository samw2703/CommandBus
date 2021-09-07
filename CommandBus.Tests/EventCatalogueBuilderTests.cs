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
			var subscribers = Build().GetSubscribers(new Event1());

			Assert.AreEqual(2, subscribers.Count);
			Assert.True(subscribers.Contains(typeof(EventSubscriber11)));
			Assert.True(subscribers.Contains(typeof(EventSubscriber12)));
		}

		[Test]
		public void Build_ClassImplementsMultipleEventSubscribers_RetrievesAllSubscribers()
		{
			var catalogue = Build();
			var event2Subscribers = catalogue.GetSubscribers(new Event2());
			var event3Subscribers = catalogue.GetSubscribers(new Event3());

			Assert.AreEqual(typeof(MultiEventSubscriber), event2Subscribers.Single());
			Assert.AreEqual(typeof(MultiEventSubscriber), event3Subscribers.Single());
		}

		private EventSubscriptionsCatalogue Build()
		{
			return new EventCatalogueBuilder()
				.Build(GetType().Assembly);
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