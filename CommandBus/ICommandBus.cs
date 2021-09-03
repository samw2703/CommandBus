using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandBus
{
	public interface ICommandBus
	{
		Task<CommandBusResult<TCommandResult>> Execute<TCommand, TCommandResult>(TCommand command) where TCommandResult : class;
	}

	internal class CommandBus : ICommandBus
	{
		private readonly EventPublisher _eventPublisher;

		public async Task<CommandBusResult<TCommandResult>> Execute<TCommand, TCommandResult>(TCommand command)
			where TCommandResult : class
		{
			var validationResult = await Validate(command);
			if (validationResult.Invalid)
				return CommandBusResult<TCommandResult>.AsValidationResult(validationResult);

			var commandResult = await HandleAndRaiseEvents<TCommand, TCommandResult>(command);

			return CommandBusResult<TCommandResult>.AsCommandResult(commandResult);
		}

		private async Task<TCommandResult> HandleAndRaiseEvents<TCommand, TCommandResult>(TCommand command)
		{
			var commandHandler = GetCommandHandler<TCommand, TCommandResult>();
			var commandResult = await commandHandler.HandleAndGetResult(command);
			RaiseAndFlushEvents(commandHandler);

			return commandResult;
		}

		private async Task<ValidationResult> Validate<TCommand>(TCommand command)
		{
			var validator = GetValidator(command);

			return validator == null
				? ValidationResult.Success()
				: await validator.DoValidation(command);
		}

		private Validator<TCommand> GetValidator<TCommand>(TCommand command)
		{
			throw new System.NotImplementedException();
		}

		private void RaiseAndFlushEvents<TCommand, TCommandResult>(CommandHandler<TCommand, TCommandResult> commandHandler)
		{
			_eventPublisher.Publish(commandHandler.QueuedEvents);
			commandHandler.FlushEvents();
		}

		private CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>()
		{
			throw new System.NotImplementedException();
		}

		public Task<ValidationResult> Execute<TCommand>(TCommand command)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class EventPublisher
	{
		public void Publish(List<object> @events)
		{
		}
	}
}