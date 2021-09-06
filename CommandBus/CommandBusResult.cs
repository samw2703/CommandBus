namespace CommandBus
{
	public class CommandBusResult<TCommandResult>
		where TCommandResult : class
	{
		private readonly ValidationResult _validationResult;
		private readonly TCommandResult _commandResult;

		public bool IsInvalid { get; set; }

		private CommandBusResult(ValidationResult validationResult, TCommandResult commandResult, bool isInvalid)
		{
			_validationResult = validationResult;
			_commandResult = commandResult;
			IsInvalid = isInvalid;
		}

		public ValidationResult AsValidationResult()
			=> _validationResult;

		public TCommandResult AsCommandResult()
			=> _commandResult;

		internal static CommandBusResult<TCommandResult> AsCommandResult(TCommandResult commandResult)
			=> new CommandBusResult<TCommandResult>(null, commandResult, false);

		internal static CommandBusResult<TCommandResult> AsValidationResult(ValidationResult validationResult)
			=> new CommandBusResult<TCommandResult>(validationResult, null, true);
	}
}