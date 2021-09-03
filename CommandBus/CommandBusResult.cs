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

	public class CommandBusResult
	{
		private readonly ValidationResult _validationResult;
		public bool IsInvalid { get; }

		private CommandBusResult(ValidationResult validationResult, bool isInvalid)
		{
			_validationResult = validationResult;
			IsInvalid = isInvalid;
		}

		public ValidationResult AsValidationResult()
			=> _validationResult;

		internal static CommandBusResult AsCommandResult()
			=> new CommandBusResult(null, false);

		internal static CommandBusResult AsValidationResult(ValidationResult validationResult)
			=> new CommandBusResult(validationResult, true);
	}
}