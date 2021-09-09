using System;
using System.Collections.Generic;

namespace CommandBus
{
	public class CommandHandlerException : Exception
	{
		public List<string> Errors { get; }

		public CommandHandlerException(List<string> errors)
		{
			Errors = errors;
		}
	}
}