using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandBus
{
	internal class CommandCatalogue
	{
		private readonly List<CommandCatalogueItem> _items;

		public CommandCatalogue(List<CommandCatalogueItem> items)
		{
			_items = items ?? new List<CommandCatalogueItem>();
		}

		public Validator<TCommand> GetValidator<TCommand>(TCommand command)
		{
			var item = GetItem(command);

			return GetValidator<TCommand>(item);
		}

		public CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>(TCommand command)
		{
			var item = GetItem(command);

			return GetCommandHandler<TCommand, TCommandResult>(item);
		}

		private CommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>(CommandCatalogueItem item)
		{
			try
			{
				return ((CommandWithResultCatalogueItem<TCommand, TCommandResult>) item)
					.CommandHandler;
			}
			catch (InvalidCastException)
			{
				throw new CommandResultMismatch<TCommand, TCommandResult>();
			}
		}

		private Validator<TCommand> GetValidator<TCommand>(CommandCatalogueItem item)
			=> ((CommandWithValidatorCatalogueItem<TCommand>) item).Validator;

		private CommandCatalogueItem GetItem<TCommand>(TCommand command)
		{
			var items = _items.Where(x => x.CommandType == command.GetType()).ToList();

			if (items.Count > 1)
				throw new MutlipleCommandsRegistered<TCommand>(command);

			if (items.None())
				throw new NoCommandsRegistered<TCommand>(command);

			return items.Single();
		}
	}
}