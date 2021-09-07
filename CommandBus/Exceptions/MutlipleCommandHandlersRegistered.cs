using System;

namespace CommandBus
{
	internal class MutlipleCommandHandlersRegistered : Exception
	{
		public MutlipleCommandHandlersRegistered(Type commandType) : base($"Multiple commands of type {commandType.FullName} registered")
		{
		}
	}
}