using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandBus
{
	public abstract class CommandHandler<TCommand, TCommandResult>
	{
		internal List<object> QueuedEvents { get; } = new List<object>();
		public abstract Task<TCommandResult> HandleAndGetResult(TCommand command);

		public void EnqueueEvent(object @event)
			=> QueuedEvents.Add(@event);

		internal void FlushEvents()
			=> QueuedEvents.Clear();
	}

	public abstract class NoResultCommandHandler<TCommand> : CommandHandler<TCommand, NoCommandResult>
	{
		public override async Task<NoCommandResult> HandleAndGetResult(TCommand command)
		{
			await Handle(command);

			return new NoCommandResult();
		}

		public abstract Task Handle(TCommand command);
	}

	public class NoCommandResult
	{
	}
}