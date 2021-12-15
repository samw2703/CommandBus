namespace CommandBus.TestContext
{
	public static class CommandBusResultExtensions
	{
		public static CommandBusResultAssertions<T> Assert<T>(this CommandBusResult<T> result, IAssertionRules assertionRules) where T : class
			=> new CommandBusResultAssertions<T>(result, assertionRules);
	}
}