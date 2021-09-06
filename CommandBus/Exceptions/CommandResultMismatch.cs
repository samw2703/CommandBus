using System;

namespace CommandBus
{
	internal class CommandResultMismatch<TCommand, TCommandResult> : Exception
	{
		public CommandResultMismatch() 
			: base($"There is no handler that accepts a {typeof(TCommand).FullName} and returns a {typeof(TCommandResult).FullName}")
		{
		}
	}
}