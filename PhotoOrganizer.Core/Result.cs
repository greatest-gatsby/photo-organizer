using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// Utility class useful for giving descriptive, non-throwing reports on operations.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// The message explaing the error. If the operation succeeded, then this returns String.Empty()
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Returns true if the operation succeeded, else false.
        /// </summary>
        public bool Successful { get; private set; }

        /// <summary>
        /// Creates a FAILING result with the given message
        /// </summary>
        /// <param name="message">Message to explain failure</param>
        /// <param name="args">Optional array of objects to use when formatting the string.</param>
        private Result(string message)
        {
            Message = message;
            Successful = false;
        }

        /// <summary>
        /// Creates a PASSING result 
        /// </summary>
        private Result()
        {
            Message = String.Empty;
            Successful = true;
        }

        /// <summary>
        /// Returns a Successful Result
        /// </summary>
        /// <returns></returns>
        public static Result Success()
        {
            return new Result();
        }

        /// <summary>
        /// Returns a Result indicating FAILURE.
        /// </summary>
        /// <returns></returns>
        public static Result Failure()
        {
            return new Result() { Successful = false };
        }

        /// <summary>
        /// Returns a Result inidicating FAILURE with the given message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Failure(string message)
        {
            return new Result(message);
        }
    }

    /// <summary>
    /// Utility class useful for giving descriptive, non-throwing reports on operations, and returning strongly-typed objects.
    /// </summary>
    public class Result<T>
    {
        /// <summary>
        /// The message explaing the error. If the operation succeeded, then this returns String.Empty()
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Returns true if the operation succeeded, else false.
        /// </summary>
        public bool Successful { get; private set; }

        /// <summary>
        /// Contains data associated with the operation
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Creates a FAILING result with the given message
        /// </summary>
        /// <param name="message">Message to explain failure</param>
        private Result(string message)
        {
            Message = message;
            Successful = false;
        }

        /// <summary>
        /// Creates a PASSING result 
        /// </summary>
        private Result()
        {
            Message = String.Empty;
            Successful = true;
        }

        /// <summary>
        /// Returns a Successful Result with no data
        /// </summary>
        /// <returns></returns>
        public static Result<T> Success()
        {
            return new Result<T>();
        }

        /// <summary>
        /// Returns a passing Result with the given data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Success(T data)
        {
            return new Result<T>()
            {
                Data = data
            };
        }

        /// <summary>
        /// Returns a Result indicating FAILURE with the given message
        /// </summary>
        /// <param name="message">The message to explain the failure</param>
        /// <param name="args">Optional array of objects to use when formatting the string.</param>
        /// <returns>A result indicating FAILURE with the given message</returns>
        public static Result<T> Failure(string message)
        {
            return new Result<T>(message);
        }

        /// <summary>
        /// Returns a Result indicating FAILURE with the given message and data
        /// </summary>
        /// <param name="message">The message to explain the failure</param>
        /// <param name="args">Optional array of objects to use when formatting the string.</param>
        /// <returns>A result indicating FAILURE with the given message</returns>
        public static Result<T> Failure(string message, T data)
        {
            return new Result<T>(message) { Data = data };
        }
    }
}
