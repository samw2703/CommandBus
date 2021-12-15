using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CommandBus.TestContext
{
	public class CommandBusTestContext
	{
		private readonly object _serviceProviderFactoryLock = new object();
		private readonly object _commandBusFactoryLock = new object();
		private readonly object _eventPublisherFactoryLock = new object();
		private readonly IServiceCollection _serviceCollection;
		private ICommandBus _commandBus;
		private StubbedEventPublisher _eventPublisher;
		private IServiceProvider _serviceProvider;
		private readonly IAssertionRules _assertionRules;

		public CommandBusTestContext(IServiceCollection serviceCollection, IAssertionRules assertionRules, Assembly assembly, params Assembly[] otherAssemblies)
		{
			_assertionRules = assertionRules;
			_serviceCollection = serviceCollection;
			_serviceCollection.AddCommandBus<StubbedEventPublisher>(assembly, otherAssemblies);
		}

		public async Task<CommandBusResult<TResult>> Execute<TCommand, TResult>(TCommand command) where TResult : class
			=> await GetCommandBus().Execute<TCommand, TResult>(command);

		public async Task<CommandBusResult<NoCommandResult>> Execute<TCommand>(TCommand command)
			=> await Execute<TCommand, NoCommandResult>(command);

		public CommandBusAssertions Assert() => new CommandBusAssertions(GetPublisher(), _assertionRules);

		private StubbedEventPublisher GetPublisher()
		{
			lock (_eventPublisherFactoryLock)
			{
				if (_eventPublisher == null)
					_eventPublisher = (StubbedEventPublisher)GetServiceProvider()
						.GetRequiredService<IEventPublisher>();

				return _eventPublisher;
			}
		}

		private ICommandBus GetCommandBus()
		{
			lock (_commandBusFactoryLock)
			{
				if (_commandBus == null)
					_commandBus = GetServiceProvider().GetRequiredService<ICommandBus>();

				return _commandBus;
			}
		}

		private IServiceProvider GetServiceProvider()
		{
			lock (_serviceProviderFactoryLock)
			{
				if (_serviceProvider == null)
					_serviceProvider = _serviceCollection
						.BuildServiceProvider();

				return _serviceProvider;
			}
		}
	}

	internal class StubbedEventPublisher : IEventPublisher
	{
		public List<object> PublishedEvents { get; } = new List<object>();

		public Task Publish(List<object> events)
		{
			PublishedEvents.AddRange(events);
			return Task.CompletedTask;
		}
	}

	public class CommandBusAssertions
	{
		private readonly StubbedEventPublisher _eventPublisher;
		private readonly IAssertionRules _assertionRules;

		internal CommandBusAssertions(StubbedEventPublisher eventPublisher, IAssertionRules assertionRules)
		{
			_eventPublisher = eventPublisher;
			_assertionRules = assertionRules;
		}

		public void WasRaised<T>()
			=> _assertionRules.True(_eventPublisher.PublishedEvents.Any(x => x.GetType() == typeof(T)), $"No events of type {typeof(T)} were raised");

		public void WasRaised<T>(int expectedTimes)
		{
			var times =  _eventPublisher.PublishedEvents.Count(x => x.GetType() == typeof(T));
			_assertionRules.Equal(expectedTimes, times, $"Event of type {typeof(T)} was expected to be raised {expectedTimes} times but was actually raised {times} times");
		}

		public void WasRaised<T>(Func<T, bool> matches)
			=> _assertionRules.True(_eventPublisher.PublishedEvents.Any(x => x.GetType() == typeof(T) && matches((T)x)), $"No event of type {typeof(T)} could be found for this match");
	}
}