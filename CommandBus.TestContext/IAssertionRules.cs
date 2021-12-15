namespace CommandBus.TestContext
{
	public interface IAssertionRules
	{
		void True(bool result, string failMessage);
		void False(bool result, string failMessage);
        void Equal(object? expected, object? actual, string failMessage);
	}
}