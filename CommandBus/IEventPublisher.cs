using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandBus
{
	public interface IEventPublisher
	{
		Task Publish(List<object> @events);
	}
}