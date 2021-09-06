using System;

namespace CommandBus
{
	internal class NullCommand : Exception
	{
		public NullCommand() : base("Cannot register a null command")
		{
		}
	}
}