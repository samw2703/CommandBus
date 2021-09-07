using System;

namespace CommandBus
{
	internal interface ICommandCatalogue
	{
		bool CommandExists<TCommand>();
		Type GetValidatorType<TCommand>();
		Type GetCommandHandlerType<TCommand>();
	}
}