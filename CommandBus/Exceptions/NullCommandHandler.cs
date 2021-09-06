using System;

namespace CommandBus
{
	internal class NullCommandHandler : Exception
	{
		public NullCommandHandler() : base("Cannot register a null command handler")
		{
		}
	}
}