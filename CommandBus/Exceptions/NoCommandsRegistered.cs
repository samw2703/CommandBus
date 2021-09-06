using System;

namespace CommandBus
{
	internal class NoCommandsRegistered<TCommand> : Exception
	{
		public NoCommandsRegistered() : base($"No commands of type {typeof(TCommand).FullName} registered")
		{
		}
	}
}