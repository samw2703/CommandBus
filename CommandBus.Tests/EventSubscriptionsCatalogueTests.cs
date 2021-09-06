using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class EventSubscriptionsCatalogueTests
	{
		[Test]
		public void GetSubscribers_NoSubscriberMatchObjectType_ReturnsEmptyList()
		{
			var subscribers = GetSubscribers<object>();

			Assert.True(subscribers.None());
		}

		[Test]
		public void GetSubscribers_MultipleSubscriberMatchesObjectType_ReturnsListOfSubscribers()
		{
			var subscribers = GetSubscribers<Event1>();

			Assert.AreEqual(2, subscribers.Count);
			Assert.IsNotNull(subscribers.SingleOrDefault(x => x.GetType() == typeof(EventSubscriber11)));
			Assert.IsNotNull(subscribers.SingleOrDefault(x => x.GetType() == typeof(EventSubscriber12)));
		}

		private List<IEventSubscriber<T>> GetSubscribers<T>()
		{
			var items = new List<EventSubscriptionsCatalogueItem>
			{
				new EventSubscriptionsCatalogueItem(typeof(Event1), typeof(EventSubscriber11)),
				new EventSubscriptionsCatalogueItem(typeof(Event1), typeof(EventSubscriber12)),
				new EventSubscriptionsCatalogueItem(typeof(Event2), typeof(EventSubscriber21))
			}; 
			return new EventSubscriptionsCatalogue(items, BuildServiceProvider())
				.GetSubscribers<T>();
		}

		private IServiceProvider BuildServiceProvider()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton<EventSubscriber11>();
			serviceCollection.AddSingleton<EventSubscriber12>();
			serviceCollection.AddSingleton<EventSubscriber21>();
			return serviceCollection.BuildServiceProvider();
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