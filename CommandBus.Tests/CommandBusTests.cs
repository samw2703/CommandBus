using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class CommandBusTests
	{
		private StubbedEventPublisher _events;
		private TestCommandHandler _handler;
		private IServiceProvider _serviceProvider;

		[SetUp]
		public void Setup()
		{
			_events = new StubbedEventPublisher();
			_handler = new TestCommandHandler();
			_serviceProvider = CreateServiceProvider();
		}

		[Test]
		public async Task Execute_GivenCommandIsNotRegistered_ThrowsNoCommandRegistered()
		{
			Assert.ThrowsAsync<NoCommandRegistered<object>>(async () => await Execute<object, object>(new { }, null, commandExists: false));
		}


		[Test]
		public async Task Execute_NoValidatorIsRegistered_HandlesCommand()
		{
			var result = await Execute(new TestCommand(), _handler);

			Assert.False(result.IsInvalid);
			Assert.True(_handler.Ran);
			Assert.AreEqual(typeof(TestCommandResult), result.AsCommandResult().GetType());
		}

		[Test]
		public async Task Execute_ValidatorIsRegisteredButCommandIsValid_ValidatorIsRunAndHandlesCommand()
		{
			var result = await Execute(new TestCommand(), _handler, typeof(TestCommandValidator));

			Assert.False(result.IsInvalid);
			Assert.True(_handler.Ran);
			Assert.AreEqual(typeof(TestCommandResult), result.AsCommandResult().GetType());
			Assert.True(_serviceProvider.GetService<TestCommandValidator>().Ran);
		}

		[Test]
		public async Task Execute_ValidatorIsRegisteredAndCommandIsInvalid_ValidatorIsRunAndReturnsValidationResult()
		{
			var result = await Execute(new TestCommand(true), _handler, typeof(TestCommandValidator));

			Assert.True(result.IsInvalid);
			Assert.False(_handler.Ran);
			var validator = _serviceProvider.GetService<TestCommandValidator>();
			Assert.True(validator.Ran);
			Assert.AreEqual(validator.ValidationMessage, result.AsValidationResult().Errors.Single());
		}

		[Test]
		public async Task Execute_NoValidationErrors_EventsArePublished()
		{
			await Execute(new TestCommand(), _handler);

			_events.CountEquals(3);
			_events.Contains(typeof(TestEvent1), 2);
			_events.Contains(typeof(TestEvent2));
		}

		private async Task<CommandBusResult<TCommandResult>> Execute<TCommand, TCommandResult>(TCommand command,
			CommandHandler<TCommand, TCommandResult> handler,
			Type validatorType = null,
			bool commandExists = true )
			where TCommandResult : class
		{
			var commandBus = new CommandBus(_events, MockCommandCatalogue(handler, validatorType, commandExists), _serviceProvider);
			return await commandBus.Execute<TCommand, TCommandResult>(command);
		}

		private IServiceProvider CreateServiceProvider()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton<TestCommandValidator>();
			serviceCollection.AddSingleton<TestCommandHandler>();
			return serviceCollection.BuildServiceProvider();
		}

		private ICommandCatalogue MockCommandCatalogue<TCommand, TCommandResult>(CommandHandler<TCommand, TCommandResult> handler, Type validatorType, bool commandExists)
		{
			var mock = new Mock<ICommandCatalogue>();
			mock.Setup(x => x.CommandExists<TCommand>()).Returns(commandExists);
			mock
				.Setup(x => x.GetValidatorType<TCommand, TCommandResult>())
				.Returns(validatorType);
			mock
				.Setup(x => x.GetCommandHandler<TCommand, TCommandResult>())
				.Returns(handler);

			return mock.Object;
		}

		private class StubbedEventPublisher : IEventPublisher
		{
			private readonly List<object> _events = new List<object>();

			public async Task Publish(List<object> events)
				=> _events.AddRange(events);

			public bool CountEquals(int expectedCount)
				=> _events.Count == expectedCount;

			public bool Contains(Type expectedObjectType, int times = 1)
				=> _events.Count(x => x.GetType() == expectedObjectType) == times;
		}

		private class TestCommand
		{
			public bool Invalid { get; }

			public TestCommand(bool invalid = false)
			{
				Invalid = invalid;
			}
		}

		private class TestCommandResult
		{
		}

		private class TestCommandHandler : CommandHandler<TestCommand, TestCommandResult>
		{
			public bool Ran { get; private set; }

			public override Task<TestCommandResult> HandleAndGetResult(TestCommand command)
			{
				Ran = true;

				EnqueueEvent(new TestEvent1());
				EnqueueEvent(new TestEvent1());
				EnqueueEvent(new TestEvent2());

				return Task.FromResult(new TestCommandResult());
			}
		}

		private class TestCommandValidator : Validator<TestCommand>
		{
			public string ValidationMessage = "There was an error";
			public bool Ran { get; private set; }

			public override Task Validate(TestCommand command)
			{
				Ran = true;

				if (command.Invalid)
					AddErrorMessage(ValidationMessage);

				return Task.CompletedTask;
			}
		}

		private class TestEvent1
		{
		}

		private class TestEvent2
		{
		}
	}
}