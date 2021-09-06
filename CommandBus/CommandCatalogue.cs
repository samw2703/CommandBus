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

		public Type GetValidatorType<TCommand, TCommandResult>()
			=> GetItem<TCommand, TCommandResult>().ValidatorType;

		public CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>()
			=> GetItem<TCommand, TCommandResult>().CommandHandler;

		private CommandWithResultCatalogueItem<TCommand, TCommandResult> GetItem<TCommand, TCommandResult>()
		{
			var items = _items.Where(x => x.CommandType == typeof(TCommand)).ToList();

			if (items.Count > 1)
				throw new MutlipleCommandsRegistered<TCommand>();

			if (items.None())
				throw new NoCommandRegistered<TCommand>();

			var item = items.Single() as CommandWithResultCatalogueItem<TCommand, TCommandResult>;
			if (item == null)
				throw new CommandResultMismatch<TCommand, TCommandResult>();

			return item;
		}
	}
}