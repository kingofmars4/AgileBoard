using AgileBoard.Domain.Constants;

namespace AgileBoard.Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public string ErrorType { get; private set; } = string.Empty;

        private Result(bool isSuccess, T? data, string errorMessage, string errorType)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        public static Result<T> Success(T data) => new(true, data, string.Empty, string.Empty);
        
        public static Result<T> Failure(string errorMessage, string errorType = "Error") 
            => new(false, default, errorMessage, errorType);
            
        public static Result<T> NotFound(string errorMessage) 
            => new(false, default, errorMessage, "NotFound");
        public static Result<T> NotFound(string entityName, bool isPlural = false) 
            => new(false, default, 
                isPlural ? Messages.Generic.NotFoundPlural(entityName) : Messages.Generic.NotFound(entityName), 
                "NotFound");
            
        public static Result<T> BadRequest(string errorMessage) 
            => new(false, default, errorMessage, "BadRequest");
            
        public static Result<T> Unauthorized(string errorMessage) 
            => new(false, default, errorMessage, "Unauthorized");
            
        public static Result<T> Conflict(string errorMessage) 
            => new(false, default, errorMessage, "Conflict");
    }

    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public string ErrorType { get; private set; } = string.Empty;

        private Result(bool isSuccess, string errorMessage, string errorType)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        public static Result Success() => new(true, string.Empty, string.Empty);
        
        public static Result Failure(string errorMessage, string errorType = "Error") 
            => new(false, errorMessage, errorType);
            
        public static Result NotFound(string errorMessage) 
            => new(false, errorMessage, "NotFound");
            
        public static Result NotFound(string entityName, bool isPlural = false) 
            => new(false, 
                isPlural ? Messages.Generic.NotFoundPlural(entityName) : Messages.Generic.NotFound(entityName), 
                "NotFound");
            
        public static Result BadRequest(string errorMessage) 
            => new(false, errorMessage, "BadRequest");
            
        public static Result Unauthorized(string errorMessage) 
            => new(false, errorMessage, "Unauthorized");
    }
}