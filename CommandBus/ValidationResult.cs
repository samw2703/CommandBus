using System.Collections.Generic;

namespace CommandBus
{
	public class ValidationResult
	{
		public bool Invalid { get; }
		public List<string> Errors { get; }

		private ValidationResult(bool invalid, List<string> errors)
		{
			Invalid = invalid;
			Errors = errors;
		}

		public static ValidationResult Success()
			=> new ValidationResult(false, new List<string>());

		public static ValidationResult Fail(List<string> errors)
			=> new ValidationResult(true, errors);
	}
}