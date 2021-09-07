using System;

namespace CommandBus
{
	internal class CommandCatalogueItem
	{
		public Type CommandType { get; }
		public Type ValidatorType { get; }
		public Type HandlerType { get; }

		public CommandCatalogueItem(Type commandType, Type validatorType, Type handlerType)
		{
			CommandType = commandType;
			ValidatorType = validatorType;
			HandlerType = handlerType;
		}
	}
}