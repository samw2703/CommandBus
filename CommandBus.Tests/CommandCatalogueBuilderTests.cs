using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CommandBus.Tests
{
	public class CommandCatalogueBuilderTests
	{
		[Test]
		public void Build_ForCommandWithResult_RetrievesCommandHandler()
		{
			var commandHandlerType = Build()
				.GetCommandHandlerType<TestCommand>();

			Assert.AreEqual(typeof(TestCommandHandler), commandHandlerType);
		}

		[Test]
		public void Build_ForCommandWithNoResult_RetrievesCommandHandler()
		{
			var commandHandlerType = Build()
				.GetCommandHandlerType<NoResultTestCommand>();

			Assert.AreEqual(typeof(NoResultTestCommandHandler), commandHandlerType);
		}

		[Test]
		public void Build_ForCommandWithValidator_RetrievesValidator()
		{
			var validatorType = Build()
				.GetValidatorType<TestCommand>();

			Assert.AreEqual(typeof(TestCommandValidator), validatorType);
		}

		[Test]
		public void Build_ForCommandWithNoValidator_ValidatorIsNull()
		{
			var validatorType = Build()
				.GetValidatorType<NoValidatorTestCommand>();

			Assert.IsNull(validatorType);
		}



		private CommandCatalogue Build()
		{
			return new CommandCatalogueBuilder().Build(GetType().Assembly);
		}
	}

	internal class TestCommand
	{
	}

	internal class TestCommandResult
	{
	}

	internal class TestCommandValidator : Validator<TestCommand>
	{
		public override Task Validate(TestCommand command)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class TestCommandHandler : CommandHandler<TestCommand, TestCommandResult>
	{
		public override Task<TestCommandResult> HandleAndGetResult(TestCommand command)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class NoResultTestCommand
	{
	}

	internal class NoResultTestCommandHandler : NoResultCommandHandler<NoResultTestCommand>
	{
		public override Task Handle(NoResultTestCommand command)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class NoResultTestCommandValidator : Validator<NoResultTestCommand>
	{
		public override Task Validate(NoResultTestCommand command)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class NoValidatorTestCommand
	{
	}

	internal class NoValidatorTestCommandHandler : NoResultCommandHandler<NoValidatorTestCommand>
	{
		public override Task Handle(NoValidatorTestCommand command)
		{
			throw new NotImplementedException();
		}
	}
}