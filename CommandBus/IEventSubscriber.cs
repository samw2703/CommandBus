using System.Threading.Tasks;

namespace CommandBus
{
	public interface IEventSubscriber<T>
	{
		Task Execute(T @event);
	}
}