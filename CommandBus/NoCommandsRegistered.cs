using System;

namespace CommandBus
{
	internal class NoCommandsRegistered<TCommand> : Exception
	{
		public NoCommandsRegistered(TCommand command) : base($"No commands of type {command.GetType().FullName} registered")
		{
		}
	}
}