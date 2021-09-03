using System;

namespace CommandBus
{
	internal class MutlipleCommandsRegistered<TCommand> : Exception
	{
		public MutlipleCommandsRegistered(TCommand command) : base($"Multiple commands of type {command.GetType().FullName} registered")
		{
		}
	}
}