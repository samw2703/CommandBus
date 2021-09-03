using System;

namespace CommandBus
{
	internal abstract class CommandCatalogueItem
	{
		public Type CommandType { get; }

		protected CommandCatalogueItem(Type commandType)
		{
			CommandType = commandType;
		}
	}

	internal abstract class CommandWithValidatorCatalogueItem<TCommand> : CommandCatalogueItem
	{
		public Validator<TCommand> Validator { get; }

		protected CommandWithValidatorCatalogueItem(Type commandType, Validator<TCommand> validator) 
			: base(commandType)
		{
			Validator = validator;
		}
	}

	internal class CommandWithResultCatalogueItem<TCommand, TResult> : CommandWithValidatorCatalogueItem<TCommand>
	{
		public CommandHandler<TCommand, TResult> CommandHandler { get; }

		public CommandWithResultCatalogueItem(Type commandType, Validator<TCommand> validator, CommandHandler<TCommand, TResult> commandHandler) 
			: base(commandType, validator)
		{
			CommandHandler = commandHandler;
		}
	}
}