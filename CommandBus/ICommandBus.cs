using System.Threading.Tasks;

namespace CommandBus
{
	public interface ICommandBus
	{
		Task<CommandBusResult<TCommandResult>> Execute<TCommand, TCommandResult>(TCommand command) where TCommandResult : class;
		Task<CommandBusResult<NoCommandResult>> Execute<TCommand>(TCommand command);
	}
}