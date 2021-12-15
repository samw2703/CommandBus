using System;

namespace CommandBus.TestContext
{
	public class CommandBusResultAssertions<T> where T : class
	{
		private readonly CommandBusResult<T> _result;
		private readonly IAssertionRules _assertionRules;

		internal CommandBusResultAssertions(CommandBusResult<T> result, IAssertionRules assertionRules)
		{
			_result = result;
			_assertionRules = assertionRules;
		}

		public void Valid()
			=> _assertionRules.False(_result.IsInvalid, "CommandBusResult was expected to be valid but was in fact invalid");

		public void Invalid()
			=> _assertionRules.True(_result.IsInvalid, "CommandBusResult was expected to be invalid but was in fact valid");

		public void HasError(string error)
		{
			Invalid();

			_assertionRules.True(_result.AsValidationResult().Errors.Contains(error), $"CommandBusResult did not contain error:{Environment.NewLine}\"{error}\"");
		}

		public void DoesNotHaveError(string error)
		{
			Invalid();

			_assertionRules.False(_result.AsValidationResult().Errors.Contains(error), $"CommandBusResult contains error:{Environment.NewLine}\"{error}\"");
		}
	}
}