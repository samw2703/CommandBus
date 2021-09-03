using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class CommandCatalogueTests
	{

		[Test]
		public void GetValidator_ItemDoesNotExistForCommand_ThrowsNoCommandException()
		{
			Assert.Throws<NoCommandsRegistered<TestCommand>>(() => GetValidator<TestCommand, NoCommandResult>());
		}

		[Test]
		public void GetValidator_MultipleItemsExistForCommand_ThrowsMultipleCommandsException()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler()),
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler())
			};
			Assert.Throws<MutlipleCommandsRegistered<TestCommand>>(() => GetValidator<TestCommand, NoCommandResult>(items));
		}

		[Test]
		public void GetValidator_SingleItemExistForCommandButDoesNotHaveValidator_ReturnsNull()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler())
			};
			var validator = GetValidator<TestCommand, NoCommandResult>(items);

			Assert.IsNull(validator);
		}

		[Test]
		public void GetValidator_SingleItemExistForCommandAndValidatorExists_ReturnsValidator()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), new TestCommandValidator(), new NoResultTestCommandHandler())
			};
			var validator = GetValidator<TestCommand, NoCommandResult>(items);

			Assert.AreEqual(typeof(TestCommandValidator), validator.GetType());
		}

		[Test]
		public void GetValidator_SingleItemExistsForCommandButCommandHandlerDoesNotReturnSpecifiedCommandResult_ThrowsCommandResultMismatchException()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler()),
			};
			Assert.Throws<CommandResultMismatch<TestCommand, TestCommandResult>>(() => GetValidator<TestCommand, TestCommandResult>(items));
		}

		[Test]
		public void GetCommandHandler_ItemDoesNotExistForCommand_ThrowsNoCommandException()
		{
			Assert.Throws<NoCommandsRegistered<TestCommand>>(() => GetCommandHandler<TestCommand, NoCommandResult>());
		}

		[Test]
		public void GetCommandHandler_MultipleItemsExistForCommand_ThrowsMultipleCommandsException()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler()),
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler())
			};
			Assert.Throws<MutlipleCommandsRegistered<TestCommand>>(() => GetCommandHandler<TestCommand, NoCommandResult>(items));
		}

		[Test]
		public void GetCommandHandler_SingleItemExistsForCommandButCommandHandlerDoesNotReturnSpecifiedCommandResult_ThrowsCommandResultMismatchException()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler()),
			};
			Assert.Throws<CommandResultMismatch<TestCommand, TestCommandResult>>(() => GetCommandHandler<TestCommand, TestCommandResult>(items));
		}

		[Test]
		public void GetCommandHandler_SingleItemExistsForCommandAndCommandHandlerReturnsSpecifiedCommandResult_ReturnsCommandHandler()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler()),
			};
			var commandHandler =  GetCommandHandler<TestCommand, NoCommandResult>(items);

			Assert.AreEqual(typeof(NoResultTestCommandHandler), commandHandler.GetType());
		}

		private Validator<TCommand> GetValidator<TCommand, TCommandResult>(List<CommandCatalogueItem> items = null)
		{
			return new CommandCatalogue(items ?? new List<CommandCatalogueItem>())
				.GetValidator<TCommand, TCommandResult>();
		}

		private CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>(List<CommandCatalogueItem> items = null)
		{
			return new CommandCatalogue(items ?? new List<CommandCatalogueItem>())
				.GetCommandHandler<TCommand, TCommandResult>();
		}

		private class TestCommand
		{
		}

		private class TestCommandValidator : Validator<TestCommand>
		{
			public override Task Validate(TestCommand command)
			{
				return Task.CompletedTask;
			}
		}

		private class NoResultTestCommandHandler : NoResultCommandHandler<TestCommand>
		{
			public override Task Handle(TestCommand command)
			{
				return Task.CompletedTask;
			}
		}

		private class TestCommandResult
		{
		}
	}
}