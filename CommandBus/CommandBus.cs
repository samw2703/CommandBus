using System;
using System.Threading.Tasks;

namespace CommandBus
{
	internal class CommandBus : ICommandBus
	{
		private readonly IEventPublisher _eventPublisher;
		private readonly ICommandCatalogue _commandCatalogue;
		private readonly IServiceProvider _serviceProvider;

		public CommandBus(IEventPublisher eventPublisher, ICommandCatalogue commandCatalogue, IServiceProvider serviceProvider)
		{
			_eventPublisher = eventPublisher;
			_commandCatalogue = commandCatalogue;
			_serviceProvider = serviceProvider;
		}

		public async Task<CommandBusResult<TCommandResult>> Execute<TCommand, TCommandResult>(TCommand command)
			where TCommandResult : class
		{
			if (!_commandCatalogue.CommandExists<TCommand>())
				throw new NoCommandRegistered<TCommand>();
			ValidateCommandHandlerProperlyConfigured<TCommand, TCommandResult>();
			var validationResult = await Validate(command);
			if (validationResult.Invalid)
				return CommandBusResult<TCommandResult>.AsValidationResult(validationResult);

			var commandResult = await HandleAndRaiseEvents<TCommand, TCommandResult>(command);

			return CommandBusResult<TCommandResult>.AsCommandResult(commandResult);
		}

		private void ValidateCommandHandlerProperlyConfigured<TCommand, TCommandResult>() where TCommandResult : class
		{
			GetCommandHandler<TCommand, TCommandResult>();
		}

		public async Task<CommandBusResult<NoCommandResult>> Execute<TCommand>(TCommand command)
			=> await Execute<TCommand, NoCommandResult>(command);

		private async Task<TCommandResult> HandleAndRaiseEvents<TCommand, TCommandResult>(TCommand command)
		{
			var commandHandler = GetCommandHandler<TCommand, TCommandResult>();
			var commandResult = await commandHandler.HandleAndGetResult(command);
			RaiseAndFlushEvents(commandHandler);

			return commandResult;
		}

		private async Task<ValidationResult> Validate<TCommand>(TCommand command)
		{
			var validator = GetValidator<TCommand>();

			return validator == null
				? ValidationResult.Success()
				: await validator.DoValidation(command);
		}

		private Validator<TCommand> GetValidator<TCommand>()
		{
			var validatorType = _commandCatalogue.GetValidatorType<TCommand>();
			if (validatorType == null)
				return null;

			return (Validator<TCommand>)_serviceProvider.GetService(validatorType);
		}

		private void RaiseAndFlushEvents<TCommand, TCommandResult>(CommandHandler<TCommand, TCommandResult> commandHandler)
		{
			_eventPublisher.Publish(commandHandler.QueuedEvents);
			commandHandler.FlushEvents();
		}

		private CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>()
		{
			var type = _commandCatalogue.GetCommandHandlerType<TCommand>();
			var service = _serviceProvider.GetService(type) as CommandHandler<TCommand, TCommandResult>;
			if (service == null)
				throw new CommandResultMismatch<TCommand, TCommandResult>();

			return service;
		}
	}
}