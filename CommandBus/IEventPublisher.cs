using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandBus
{
	internal interface IEventPublisher
	{
		Task Publish(List<object> @events);
	}
}