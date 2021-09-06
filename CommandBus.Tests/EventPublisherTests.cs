using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class EventPublisherTests
	{
		[Test]
		public async Task Publish_EventHasTwoSubscribers_BothAreExecuted()
		{
			var serviceProvider = CreateServiceProvider();
			await Publish(new Event(), serviceProvider);
			Assert.True(serviceProvider.GetService<EventSubscriber1>().Ran);
			Assert.True(serviceProvider.GetService<EventSubscriber2>().Ran);
		}

		[Test]
		public async Task Publish_RunAsynchronously_DoesNotThrow()
		{
			await Publish(new Event(), CreateServiceProvider(), false);
		}

		private async Task Publish(Event @event, IServiceProvider serviceProvider, bool runSynchronously = true)
		{
			await CreateEventPublisher(serviceProvider, runSynchronously)
				.Publish(new List<object> {@event});
		}

		private EventPublisher CreateEventPublisher(IServiceProvider serviceProvider, bool runSynchronously)
		{
			var items = new List<EventSubscriptionsCatalogueItem>
			{
				new EventSubscriptionsCatalogueItem(typeof(Event), typeof(EventSubscriber1)),
				new EventSubscriptionsCatalogueItem(typeof(Event), typeof(EventSubscriber2))
			};
			var catalogue = new EventSubscriptionsCatalogue(items);
			return new EventPublisher(catalogue, new Config(runSynchronously), serviceProvider);
		}

		private IServiceProvider CreateServiceProvider()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton<EventSubscriber1>();
			serviceCollection.AddSingleton<EventSubscriber2>();

			return serviceCollection.BuildServiceProvider();
		}

		private class Event
		{
		}

		private class EventSubscriber1 : IEventSubscriber<Event>
		{
			public bool Ran { get; private set; }

			public Task Execute(Event @event)
			{
				Ran = true;
				return Task.CompletedTask;
			}
		}

		private class EventSubscriber2 : IEventSubscriber<Event>
		{
			public bool Ran { get; private set; }

			public Task Execute(Event @event)
			{
				Ran = true;
				return Task.CompletedTask;
			}
		}
	}
}