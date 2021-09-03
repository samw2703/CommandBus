using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandBus
{
	public abstract class Validator<TCommand>
	{
		private readonly List<string> _errors = new List<string>();
		internal async Task<ValidationResult> DoValidation(TCommand command)
		{
			await Validate(command);

			return _errors.Any()
				? ValidationResult.Fail(_errors)
				: ValidationResult.Success();
		}

		public abstract Task Validate(TCommand command);

		public void AddErrorMessage(string message) => _errors.Add(message);
	}
}