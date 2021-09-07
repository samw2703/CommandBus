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
				new(typeof(TestCommand), null, typeof(NoResultTestCommandHandler))
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
			Assert.Throws<NoCommandRegistered<TestCommand>>(() => GetValidatorType<TestCommand>());
		}

		[Test]
		public void GetValidator_MultipleItemsExistForCommand_ThrowsMultipleCommandsException()
		{
			var items = new List<CommandCatalogueItem>
			{
				new (typeof(TestCommand), null, typeof(NoResultTestCommandHandler)),
				new (typeof(TestCommand), null, typeof(NoResultTestCommandHandler))
			};
			Assert.Throws<MutlipleCommandsRegistered<TestCommand>>(() => GetValidatorType<TestCommand>(items));
		}

		[Test]
		public void GetValidator_SingleItemExistForCommandButDoesNotHaveValidator_ReturnsNull()
		{
			var items = new List<CommandCatalogueItem>
			{
				new (typeof(TestCommand), null, typeof(NoResultTestCommandHandler))
			};
			var validatorType = GetValidatorType<TestCommand>(items);

			Assert.IsNull(validatorType);
		}

		[Test]
		public void GetValidator_SingleItemExistForCommandAndValidatorExists_ReturnsValidator()
		{
			var items = new List<CommandCatalogueItem>
			{
				new (typeof(TestCommand), typeof(TestCommandValidator), typeof(NoResultTestCommandHandler))
			};
			var validator = GetValidatorType<TestCommand>(items);

			Assert.AreEqual(typeof(TestCommandValidator), validator);
		}

		[Test]
		public void GetCommandHandler_ItemDoesNotExistForCommand_ThrowsNoCommandException()
		{
			Assert.Throws<NoCommandRegistered<TestCommand>>(() => GetCommandHandler<TestCommand>());
		}

		[Test]
		public void GetCommandHandler_MultipleItemsExistForCommand_ThrowsMultipleCommandsException()
		{
			var items = new List<CommandCatalogueItem>
			{
				new (typeof(TestCommand), null, typeof(NoResultTestCommandHandler)),
				new (typeof(TestCommand), null, typeof(NoResultTestCommandHandler))
			};
			Assert.Throws<MutlipleCommandsRegistered<TestCommand>>(() => GetCommandHandler<TestCommand>(items));
		}

		[Test]
		public void GetCommandHandler_SingleItemExistsForCommandAndCommandHandlerReturnsSpecifiedCommandResult_ReturnsCommandHandler()
		{
			var items = new List<CommandCatalogueItem>
			{
				new (typeof(TestCommand), null, typeof(NoResultTestCommandHandler)),
			};
			var commandHandler =  GetCommandHandler<TestCommand>(items);

			Assert.AreEqual(typeof(NoResultTestCommandHandler), commandHandler);
		}

		private bool CommandExists<TCommand>(List<CommandCatalogueItem> items = null)
		{
			return new CommandCatalogue(items ?? new List<CommandCatalogueItem>())
				.CommandExists<TCommand>();
		}

		private Type GetValidatorType<TCommand>(List<CommandCatalogueItem> items = null)
		{
			return new CommandCatalogue(items ?? new List<CommandCatalogueItem>())
				.GetValidatorType<TCommand>();
		}

		private Type GetCommandHandler<TCommand>(List<CommandCatalogueItem> items = null)
		{
			return new CommandCatalogue(items ?? new List<CommandCatalogueItem>())
				.GetCommandHandlerType<TCommand>();
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