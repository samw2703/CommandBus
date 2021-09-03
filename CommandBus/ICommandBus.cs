using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandBus
{
	public interface ICommandBus
	{
		Task<CommandBusResult<TCommandResult>> Execute<TCommand, TCommandResult>(TCommand command) where TCommandResult : class;
		Task<CommandBusResult<NoCommandResult>> Execute<TCommand>(TCommand command);
	}

	internal class CommandBus : ICommandBus
	{
		private readonly EventPublisher _eventPublisher;
		private readonly CommandCatalogue _commandCatalogue;

		public async Task<CommandBusResult<TCommandResult>> Execute<TCommand, TCommandResult>(TCommand command)
			where TCommandResult : class
		{
			var validationResult = await Validate<TCommand, TCommandResult>(command);
			if (validationResult.Invalid)
				return CommandBusResult<TCommandResult>.AsValidationResult(validationResult);

			var commandResult = await HandleAndRaiseEvents<TCommand, TCommandResult>(command);

			return CommandBusResult<TCommandResult>.AsCommandResult(commandResult);
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

		private async Task<ValidationResult> Validate<TCommand, TCommandResult>(TCommand command)
		{
			var validator = GetValidator<TCommand, TCommandResult>();

			return validator == null
				? ValidationResult.Success()
				: await validator.DoValidation(command);
		}

		private Validator<TCommand> GetValidator<TCommand, TCommandResult>()
			=> _commandCatalogue.GetValidator<TCommand, TCommandResult>();

		private void RaiseAndFlushEvents<TCommand, TCommandResult>(CommandHandler<TCommand, TCommandResult> commandHandler)
		{
			_eventPublisher.Publish(commandHandler.QueuedEvents);
			commandHandler.FlushEvents();
		}

		private CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>()
			=> _commandCatalogue.GetCommandHandler<TCommand, TCommandResult>();
	}

	internal class EventPublisher
	{
		public void Publish(List<object> @events)
		{
		}
	}
}