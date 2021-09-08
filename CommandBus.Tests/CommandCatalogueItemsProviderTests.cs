using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace CommandBus.Tests
{
	public class CommandCatalogueItemsProviderTests
	{
		[Test]
		public void Build_ForCommandWithResult_RetrievesCommandHandler()
		{
			var catalogueItem = Get()
				.Single(x => x.CommandType == typeof(TestCommand));

			Assert.AreEqual(typeof(TestCommandHandler), catalogueItem.HandlerType);
		}

		[Test]
		public void Build_ForCommandWithNoResult_RetrievesCommandHandler()
		{
			var catalogueItem = Get()
				.Single(x => x.CommandType == typeof(NoResultTestCommand));

			Assert.AreEqual(typeof(NoResultTestCommandHandler), catalogueItem.HandlerType);
		}

		[Test]
		public void Build_ForCommandWithValidator_RetrievesValidator()
		{
			var catalogueItem = Get()
				.Single(x => x.CommandType == typeof(TestCommand));

			Assert.AreEqual(typeof(TestCommandValidator), catalogueItem.ValidatorType);
		}

		[Test]
		public void Build_ForCommandWithNoValidator_ValidatorIsNull()
		{
			var catalogueItem = Get()
				.Single(x => x.CommandType == typeof(NoValidatorTestCommand));

			Assert.IsNull(catalogueItem.ValidatorType);
		}

		private List<CommandCatalogueItem> Get()
		{
			return new CommandCatalogueItemsProvider()
				.Get(GetType().Assembly);
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

	internal class NoResultTestCommandHandler : CommandHandler<NoResultTestCommand>
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

	internal class NoValidatorTestCommandHandler : CommandHandler<NoValidatorTestCommand>
	{
		public override Task Handle(NoValidatorTestCommand command)
		{
			throw new NotImplementedException();
		}
	}
}