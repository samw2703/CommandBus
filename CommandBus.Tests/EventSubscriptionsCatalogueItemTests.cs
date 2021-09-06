using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class EventSubscriptionsCatalogueItemTests
	{
		[Test]
		public void Ctor_SubscriberTypeIsNotEventSubscriber_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new EventSubscriptionsCatalogueItem(typeof(object), typeof(object)), 
				"The subscriber type must implement IEventSubscriber<T>");
		}

		[Test]
		public void Ctor_SubscriberTypeIsEventSubscriber_DoesNotThrow()
		{
			Assert.DoesNotThrow(() => new EventSubscriptionsCatalogueItem(typeof(object), typeof(TestSubscriber)));
		}

		[Test]
		public void Ctor_SubscriberTypeIsEventSubscriberForDifferentType_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new EventSubscriptionsCatalogueItem(typeof(string), typeof(TestSubscriber)),
				$"This subscriber type does not subscribe to type {typeof(string).FullName}");
		}

		private class TestSubscriber : IEventSubscriber<object>
		{
			public Task Execute(object @event)
			{
				throw new NotImplementedException();
			}
		}
	}
}