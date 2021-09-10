using System.Collections.Generic;
using System.Linq;

namespace CommandBus
{
	internal static class EnumerableExtensions
	{
		public static bool None<T>(this List<T> list)
			=> !list.Any();

		public static List<T> WithRange<T>(this List<T> list, IEnumerable<T> range)
		{
			list.AddRange(range);
			return list;
		}
	}
}