using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class CommandCatalogueTests
	{
		[Test]
		public void CommandExists_CommandOfThatTypeExistsInCatalogue_ReturnsTrue()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler())
			};

			Assert.True(CommandExists<TestCommand>(items));
		}

		[Test]
		public void CommandExists_NoCommandOfThatTypeExistsInCatalogue_ReturnsFalse()
		{
			Assert.False(CommandExists<object>());
		}

		[Test]
		public void GetValidator_ItemDoesNotExistForCommand_ThrowsNoCommandException()
		{
			Assert.Throws<NoCommandRegistered<TestCommand>>(() => GetValidatorType<TestCommand, NoCommandResult>());
		}

		[Test]
		public void GetValidator_MultipleItemsExistForCommand_ThrowsMultipleCommandsException()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler()),
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler())
			};
			Assert.Throws<MutlipleCommandsRegistered<TestCommand>>(() => GetValidatorType<TestCommand, NoCommandResult>(items));
		}

		[Test]
		public void GetValidator_SingleItemExistForCommandButDoesNotHaveValidator_ReturnsNull()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler())
			};
			var validatorType = GetValidatorType<TestCommand, NoCommandResult>(items);

			Assert.IsNull(validatorType);
		}

		[Test]
		public void GetValidator_SingleItemExistForCommandAndValidatorExists_ReturnsValidator()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), typeof(TestCommandValidator), new NoResultTestCommandHandler())
			};
			var validator = GetValidatorType<TestCommand, NoCommandResult>(items);

			Assert.AreEqual(typeof(TestCommandValidator), validator);
		}

		[Test]
		public void GetValidator_SingleItemExistsForCommandButCommandHandlerDoesNotReturnSpecifiedCommandResult_ThrowsCommandResultMismatchException()
		{
			var items = new List<CommandCatalogueItem>
			{
				new CommandWithResultCatalogueItem<TestCommand, NoCommandResult>(typeof(TestCommand), null, new NoResultTestCommandHandler()),
			};
			Assert.Throws<CommandResultMismatch<TestCommand, TestCommandResult>>(() => GetValidatorType<TestCommand, TestCommandResult>(items));
		}

		[Test]
		public void GetCommandHandler_ItemDoesNotExistForCommand_ThrowsNoCommandException()
		{
			Assert.Throws<NoCommandRegistered<TestCommand>>(() => GetCommandHandler<TestCommand, NoCommandResult>());
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

		private bool CommandExists<TCommand>(List<CommandCatalogueItem> items = null)
		{
			return new CommandCatalogue(items ?? new List<CommandCatalogueItem>())
				.CommandExists<TCommand>();
		}

		private Type GetValidatorType<TCommand, TCommandResult>(List<CommandCatalogueItem> items = null)
		{
			return new CommandCatalogue(items ?? new List<CommandCatalogueItem>())
				.GetValidatorType<TCommand, TCommandResult>();
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