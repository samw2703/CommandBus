using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class CommandBusTests
	{
		private StubbedEventPublisher _events;
		private TestCommandHandler _handler;
		private TestCommandValidator _validator;

		[SetUp]
		public void Setup()
		{
			_events = new StubbedEventPublisher();
			_handler = new TestCommandHandler();
			_validator = new TestCommandValidator();
		}

		[Test]
		public async Task Execute_NoValidatorIsRegistered_HandlesCommand()
		{
			var commandBus = new CommandBus(_events, MockCommandCatalogue(_handler));
			var result = await commandBus.Execute<TestCommand, TestCommandResult>(new TestCommand());

			Assert.False(result.IsInvalid);
			Assert.True(_handler.Ran);
			Assert.AreEqual(typeof(TestCommandResult), result.AsCommandResult().GetType());
		}

		[Test]
		public async Task Execute_ValidatorIsRegisteredButCommandIsValid_ValidatorIsRunAndHandlesCommand()
		{
			var commandBus = new CommandBus(_events, MockCommandCatalogue(_handler, _validator));
			var result = await commandBus.Execute<TestCommand, TestCommandResult>(new TestCommand());

			Assert.False(result.IsInvalid);
			Assert.True(_handler.Ran);
			Assert.AreEqual(typeof(TestCommandResult), result.AsCommandResult().GetType());
			Assert.True(_validator.Ran);
		}

		[Test]
		public async Task Execute_ValidatorIsRegisteredAndCommandIsInvalid_ValidatorIsRunAndReturnsValidationResult()
		{
			var commandBus = new CommandBus(_events, MockCommandCatalogue(_handler, _validator));
			var result = await commandBus.Execute<TestCommand, TestCommandResult>(new TestCommand(true));

			Assert.True(result.IsInvalid);
			Assert.False(_handler.Ran);
			Assert.True(_validator.Ran);
			Assert.AreEqual(_validator.ValidationMessage, result.AsValidationResult().Errors.Single());
		}

		[Test]
		public async Task Execute_NoValidationErrors_EventsArePublished()
		{
			var commandBus = new CommandBus(_events, MockCommandCatalogue(_handler));
			await commandBus.Execute<TestCommand, TestCommandResult>(new TestCommand());

			_events.CountEquals(3);
			_events.Contains(typeof(TestEvent1), 2);
			_events.Contains(typeof(TestEvent2));
		}

		private ICommandCatalogue MockCommandCatalogue<TCommand, TCommandResult>(CommandHandler<TCommand, TCommandResult> handler, Validator<TCommand> validator = null)
		{
			var mock = new Mock<ICommandCatalogue>();
			mock
				.Setup(x => x.GetValidator<TCommand, TCommandResult>())
				.Returns(validator);
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