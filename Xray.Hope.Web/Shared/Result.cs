namespace Xray.Hope.Web.Shared
{
    public class Result
    {
        public string Message { get; set; }

        public ResultStatus Status { get; set; }

        public string ErrorCode { get; set; }

        public bool IsSuccess => Status == ResultStatus.Success;

        public static Result Failure(string message, string errorCode)
        {
            return new Result
            {
                Message = message,
                ErrorCode = errorCode,
                Status = ResultStatus.Failure
            };
        }

        public static Result<T> Success<T>(T value)
        {
            return new Result<T>
            {
                Value = value,
                Status = ResultStatus.Success
            };
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }

        public static object Create<T>(T value, Result result)
        {
            return new Result<T>
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Value = value,
            };
        }

        public static new Result<T> Failure(string message, string errorCode)
        {
            return new Result<T>
            {
                Message = message,
                ErrorCode = errorCode,
                Status = ResultStatus.Failure
            };
        }
    }

    public enum ResultStatus
    {
        Success, Failure
    }
}
