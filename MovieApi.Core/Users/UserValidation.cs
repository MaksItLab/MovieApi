using MovieApi.Core.Common;

namespace MovieApi.Core.Users
{
    internal static class UserValidation
    {
        private const int MaxEmailLength = 320;
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 100;

        public static OperationResult ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "Email обязателен.");
            }

            var trimmedEmail = email.Trim();
            if (trimmedEmail.Length > MaxEmailLength)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    $"Email не может быть длиннее {MaxEmailLength} символов.");
            }

            if (!trimmedEmail.Contains('@') || !trimmedEmail.Contains('.'))
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "Email имеет неверный формат.");
            }

            return OperationResult.Success();
        }

        public static OperationResult ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "Password обязателен.");
            }

            if (password.Length < MinPasswordLength)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    $"Password должен быть не короче {MinPasswordLength} символов.");
            }

            if (password.Length > MaxPasswordLength)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    $"Password не может быть длиннее {MaxPasswordLength} символов.");
            }

            return OperationResult.Success();
        }

        public static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}
