using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandBus
{
	public interface ICommandBus
	{
		Task<CommandBusResult<TCommandResult>> Execute<TCommand, TCommandResult>(TCommand command) where TCommandResult : class;
		Task<CommandBusResult<NoCommandResult>> Execute<TCommand>(TCommand command);
	}

	internal interface IEventPublisher
	{
		void Publish(List<object> @events);
	}

	internal class EventPublisher : IEventPublisher
	{
		public void Publish(List<object> @events)
		{
		}
	}

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

	public interface IEventSubscriber<T>
	{
		Task Execute(T @event);
	}

	internal static class ServiceProviderExtensions
	{
		public static T GetService<T>(this IServiceProvider serviceProvider, Type type)
			=> (T) serviceProvider.GetService(type);
	}
}