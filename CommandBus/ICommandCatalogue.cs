namespace CommandBus
{
	internal interface ICommandCatalogue
	{
		Validator<TCommand> GetValidator<TCommand, TCommandResult>();
		CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>();
	}
}