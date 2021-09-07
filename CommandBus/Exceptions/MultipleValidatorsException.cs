using System;

namespace CommandBus
{
	internal class MultipleValidatorsException : Exception
	{
		public MultipleValidatorsException(Type commandType) : base($"Multiple validators were registered for {commandType.FullName}")
		{
		}
	}
}