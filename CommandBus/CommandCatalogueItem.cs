using System;

namespace CommandBus
{
	internal abstract class CommandCatalogueItem
	{
		public Type CommandType { get; }
		public Type ValidatorType { get; }

		protected CommandCatalogueItem(Type commandType, Type validatorType)
		{
			CommandType = commandType;
			ValidatorType = validatorType;
		}
	}

	internal class CommandWithResultCatalogueItem<TCommand, TResult> : CommandCatalogueItem
	{
		public CommandHandler<TCommand, TResult> CommandHandler { get; }

		public CommandWithResultCatalogueItem(Type commandType, Type validatorType, CommandHandler<TCommand, TResult> commandHandler) 
			: base(commandType, validatorType)
		{
			CommandHandler = commandHandler;
		}
	}
}