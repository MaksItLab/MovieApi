using MovieApi.Contracts.Actors;
using MovieApi.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Actors
{
    internal static class ActorValidation
    {
        private const int MaxNameLength = 100;

        public static OperationResult Validate(CreateActorRequest request)
        {
            return ValidateFields(request.FirstName, request.LastName, request.BirthDate);
        }

        public static OperationResult Validate(UpdateActorRequest request)
        {
            return ValidateFields(request.FirstName, request.LastName, request.BirthDate);
        }

        private static OperationResult ValidateFields(
        string firstName,
        string lastName,
        DateTime? birthDate)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "FirstName обязателен.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "LastName обязателен.");
            }

            var normalizedFirstName = firstName.Trim();
            var normalizedLastName = lastName.Trim();

            if (normalizedFirstName.Length > MaxNameLength)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    $"FirstName не может быть длиннее {MaxNameLength} символов.");
            }

            if (normalizedLastName.Length > MaxNameLength)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    $"LastName не может быть длиннее {MaxNameLength} символов.");
            }

            if (birthDate.HasValue && birthDate.Value.Date > DateTime.UtcNow.Date)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "BirthDate не может быть в будущем.");
            }

            return OperationResult.Success();
        }
    }
}
