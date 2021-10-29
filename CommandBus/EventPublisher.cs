using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandBus
{
	internal class EventPublisher : IEventPublisher
	{
		private readonly EventSubscriptionsCatalogue _catalogue;
		private readonly Config _config;
		private readonly IServiceProvider _serviceProvider;

		public EventPublisher(EventSubscriptionsCatalogue catalogue, Config config, IServiceProvider serviceProvider)
		{
			_catalogue = catalogue;
			_config = config;
			_serviceProvider = serviceProvider;
		}

		public async Task Publish(List<object> events)
		{
			foreach (var @event in events)
			{
				foreach (var eventSubscriber in _catalogue.GetSubscribers(@event))
					await ExecuteSubscriber(eventSubscriber, @event);
			}
		}

		private async Task ExecuteSubscriber(Type subscriberType, object @event)
		{
			var service = _serviceProvider.GetService(subscriberType);
			var method = subscriberType.GetMethods().Single(x => IsExecuteMethodForEventType(x, @event.GetType()));
			if (_config.PublishSynchronously)
			{
				await (Task) method.Invoke(service, new[] {@event});
				return;
			}

			method.Invoke(service, new[] { @event });
		}

		private bool IsExecuteMethodForEventType(MethodInfo methodInfo, Type eventType)
			=> methodInfo.Name == nameof(IEventSubscriber<object>.Execute) && methodInfo.GetParameters().Single().ParameterType == eventType;
	}
}