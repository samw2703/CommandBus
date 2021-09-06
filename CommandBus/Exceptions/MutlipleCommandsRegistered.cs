using System;

namespace CommandBus
{
	internal class MutlipleCommandsRegistered<TCommand> : Exception
	{
		public MutlipleCommandsRegistered() : base($"Multiple commands of type {typeof(TCommand).FullName} registered")
		{
		}
	}
}