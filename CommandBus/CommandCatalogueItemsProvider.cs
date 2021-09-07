using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandBus
{
	internal class CommandCatalogueItemsProvider
	{
		private readonly InheritanceTreeProvider _inheritanceTreeProvider;

		public CommandCatalogueItemsProvider()
		{
			_inheritanceTreeProvider = new InheritanceTreeProvider();
		}

		public List<CommandCatalogueItem> Get(params Assembly[] assemblies)
		{
			var handlerDtos = GetHandlerDtos(assemblies);
			var validatorDtos = GetValidatorDtos(assemblies);
			return GetCatalogueItems(handlerDtos, validatorDtos);
		}

		private List<CommandCatalogueItem> GetCatalogueItems(List<HandlerDto> handlerDtos, List<ValidatorDto> validatorDtos)
		{
			ValidateHandlerDtos(handlerDtos);
			ValidateValidatorDtos(validatorDtos);
			return handlerDtos
				.Select(x => x.ToCatalogueItem(validatorDtos))
				.ToList();
		}

		private void ValidateValidatorDtos(List<ValidatorDto> validatorDtos)
		{
			foreach (var validatorDto in validatorDtos)
			{
				var count = validatorDtos.Count(x => x.CommandType == validatorDto.CommandType);
				if (count > 1)
					throw new MultipleValidatorsException(validatorDto.CommandType);
			}
		}

		private void ValidateHandlerDtos(List<HandlerDto> handlerDtos)
		{
			foreach (var handlerDto in handlerDtos)
			{
				var count = handlerDtos.Count(x => x.CommandType == handlerDto.CommandType);
				if (count > 1)
					throw new MutlipleCommandHandlersRegistered(handlerDto.CommandType);
			}
		}

		private List<ValidatorDto> GetValidatorDtos(Assembly[] assemblies)
		{
			return assemblies
				.SelectMany(x => x.GetTypes())
				.Select(GetValidatorDtoOrNull)
				.Where(x => x != null)
				.ToList();
		}

		private ValidatorDto GetValidatorDtoOrNull(Type type)
		{
			if (type.IsNested)
				return null;
			var inheritanceTree = _inheritanceTreeProvider.Get(type);
			var validatorType = inheritanceTree.SingleOrDefault(IsAbstractValidator);
			if (validatorType == null)
				return null;

			return new ValidatorDto(validatorType.GetGenericArguments().First(), type);
		}

		private bool IsAbstractValidator(Type type)
		{
			if (!type.IsGenericType)
				return false;

			return type.GetGenericTypeDefinition() == typeof(Validator<object>).GetGenericTypeDefinition();
		}

		private List<HandlerDto> GetHandlerDtos(Assembly[] assemblies)
		{
			return assemblies
				.SelectMany(x => x.GetTypes())
				.Select(GetCommandHandlerDtoOrNull)
				.Where(x => x != null)
				.ToList();
		}

		private HandlerDto GetCommandHandlerDtoOrNull(Type type)
		{
			if (type.IsNested)
				return null;
			var inheritanceTree = _inheritanceTreeProvider.Get(type);
			var commandHandlerType = inheritanceTree.SingleOrDefault(IsAbstractCommandHandler);
			if (commandHandlerType == null)
				return null;

			return new HandlerDto(commandHandlerType.GetGenericArguments().First(), type);
		}

		private bool IsAbstractCommandHandler(Type type)
		{
			if (!type.IsGenericType)
				return false;

			return type.GetGenericTypeDefinition() == typeof(CommandHandler<object, object>).GetGenericTypeDefinition();
		}

		private class HandlerDto
		{
			public Type CommandType { get; }
			private readonly Type _handlerType;

			public HandlerDto(Type commandType, Type handlerType)
			{
				CommandType = commandType;
				_handlerType = handlerType;
			}

			public CommandCatalogueItem ToCatalogueItem(List<ValidatorDto> validatorDtos)
			{
				var validatorType = validatorDtos.SingleOrDefault(x => x.CommandType == CommandType)?.ValidatorType;
				return new CommandCatalogueItem(CommandType, validatorType, _handlerType);
			}
		}

		private class ValidatorDto
		{
			public Type CommandType { get; }
			public Type ValidatorType { get; }

			public ValidatorDto(Type commandType, Type validatorType)
			{
				CommandType = commandType;
				ValidatorType = validatorType;
			}
		}
	}
}