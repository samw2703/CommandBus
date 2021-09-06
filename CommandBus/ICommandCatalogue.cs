using System;

namespace CommandBus
{
	internal interface ICommandCatalogue
	{
		bool CommandExists<TCommand>();
		Type GetValidatorType<TCommand, TCommandResult>();
		CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>();
	}
}