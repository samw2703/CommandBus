using System;
using System.Collections.Generic;

namespace CommandBus
{
	internal class InheritanceTreeProvider
	{
		public List<Type> Get(Type type)
		{
			var inheritanceTree = new List<Type>();
			var baseType = type.BaseType;
			while (true)
			{
				if (baseType == typeof(object))
					break;

				inheritanceTree.Add(baseType);

				baseType = baseType.BaseType;
			}

			return inheritanceTree;
		}
	}
}