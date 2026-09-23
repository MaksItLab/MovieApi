using MovieApi.Contracts.Reviews;
using MovieApi.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Reviews
{
    internal static class ReviewValidation
    {
        private const int MaxTextLength = 2000;
        private const int MinRating = 1;
        private const int MaxRating = 10;

        public static OperationResult Validate(CreateReviewRequest request)
        {
            return ValidateFields(request.Text, request.Rating);
        }

        public static OperationResult Validate(UpdateReviewRequest request)
        {
            return ValidateFields(request.Text, request.Rating);
        }

        private static OperationResult ValidateFields(
            string text,
            int rating)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "Text обязателен.");
            }

            var normalizedText = text.Trim();

            if (normalizedText.Length > MaxTextLength)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    $"Text не может быть длиннее {MaxTextLength} символов.");
            }

            if (rating is < MinRating or > MaxRating)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    $"Rating должен быть от {MinRating} до {MaxRating}.");
            }

            return OperationResult.Success();
        }
    }
}
