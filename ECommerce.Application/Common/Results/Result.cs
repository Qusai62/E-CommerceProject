using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce.Application.Common.Results
{
    public sealed class Result<T> //sealed: keyword means that another class cannot inherit from
    {
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;//indicates that the property's value is calculated using the expression on the right.

        public T? Value { get; }//This property contains the data returned by a successful operation.

        public IReadOnlyList<Error> Errors { get; }//This property stores the errors explaining why an operation failed.

        private Result(
            bool isSuccess,
            T? value,
            IReadOnlyList<Error> errors)
        {
            IsSuccess = isSuccess;
            Value = value;//Prdouct Resoponse in case the create product 
            Errors = errors;//will be empty if no errors
        }

        public static Result<T> Success(T value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return new Result<T>(
                true,
                value,
                Array.Empty<Error>());
        }

        public static Result<T> Failure(IEnumerable<Error> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);

            var errorList = errors.ToArray();

            if (errorList.Length == 0)
            {
                throw new ArgumentException(
                    "A failure must contain at least one error.",
                    nameof(errors));
            }

            //Why should we reject null entries?
            //Because we want every entry in Errors to represent a valid error object.
            if (errorList.Any(error => error is null))
            {
                throw new ArgumentException(
                    "Errors cannot contain null entries.",
                    nameof(errors));
            }

            return new Result<T>(
                false,
                default,//produces the default value for a particular type.
                Array.AsReadOnly(errorList));

            //Now we wrap that array in a read-only collection.
            //Why?
            //Because once we create a failure result, we don't want consumers changing the number or order of errors.
        }
    }
}