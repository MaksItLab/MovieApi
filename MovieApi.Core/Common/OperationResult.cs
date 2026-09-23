using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Common
{
    public class OperationResult
    {
        private protected OperationResult(bool isSuccess, OperationErrorType? errorType, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }

        public OperationErrorType? ErrorType { get; }

        public string? ErrorMessage { get; }

        public static OperationResult Success()
        {
            return new OperationResult(true, null, null);
        }

        public static OperationResult Failure(OperationErrorType errorType, string errorMessage)
        {
            return new OperationResult(false, errorType, errorMessage);
        }
    }

    public sealed class OperationResult<T> : OperationResult
    {
        private OperationResult(bool isSuccess, T? value, OperationErrorType? errorType, string? errorMessage)
            : base(isSuccess, errorType, errorMessage)
        {
            Value = value;
        }

        public T? Value { get; }

        public static OperationResult<T> Success(T value)
        {
            return new OperationResult<T>(true, value, null, null);
        }

        public static new OperationResult<T> Failure(OperationErrorType errorType, string errorMessage)
        {
            return new OperationResult<T>(false, default, errorType, errorMessage);
        }
    }

    public sealed record OperationError(
        OperationErrorType? Type,
        string Message);
}
