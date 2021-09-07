using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandBus
{
	internal class CommandCatalogue : ICommandCatalogue
	{
		private readonly List<CommandCatalogueItem> _items;

		public CommandCatalogue(List<CommandCatalogueItem> items)
		{
			_items = items ?? new List<CommandCatalogueItem>();
		}

		public bool CommandExists<TCommand>()
			=> _items.Any(x => x.CommandType == typeof(TCommand));

		public Type GetValidatorType<TCommand>()
			=> GetItem<TCommand>().ValidatorType;

		public Type GetCommandHandlerType<TCommand>()
			=> GetItem<TCommand>()
				.HandlerType;

		private CommandCatalogueItem GetItem<TCommand>()
		{
			var items = _items.Where(x => x.CommandType == typeof(TCommand)).ToList();

			if (items.Count > 1)
				throw new MutlipleCommandHandlersRegistered(typeof(TCommand));

			if (items.None())
				throw new NoCommandRegistered<TCommand>();

			return items.Single();
		}
	}
}