using System;

namespace CommandBus
{
	internal class NoCommandRegistered<TCommand> : Exception
	{
		public NoCommandRegistered() : base($"No commands of type {typeof(TCommand).FullName} registered")
		{
		}
	}
}