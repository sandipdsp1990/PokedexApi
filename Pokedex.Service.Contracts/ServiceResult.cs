using System.Net;

namespace Pokedex.Service.Contracts
{
    public abstract class ServiceResult
    {
        public bool Success { get; protected set; }
        public HttpStatusCode HttpStatusCode { get; protected set; }
        public string Message { get; protected set; }

        protected ServiceResult(bool success)
        {
            Success = success;
        }

        protected ServiceResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        protected ServiceResult(bool success, HttpStatusCode httpStatusCode)
        {
            Success = success;
            HttpStatusCode = httpStatusCode;
        }

        protected ServiceResult(bool success, HttpStatusCode httpStatusCode, string message)
        {
            Success = success;
            Message = message;
            HttpStatusCode = httpStatusCode;
        }
    }
    public class ServiceResult<T> : ServiceResult
    {
        public T ResultObject { get; set; }

        public ServiceResult(bool success, T resultObject)
           : base(success)
        {
            ResultObject = resultObject;
        }

        public ServiceResult(bool success, string message, T resultObject)
            : base(success, message)
        {
            ResultObject = resultObject;
        }

        public ServiceResult(bool success, HttpStatusCode httpStatusCode, string message, T resultObject)
           : base(success, httpStatusCode, message)
        {
            ResultObject = resultObject;
        }
    }
    public class SuccessServiceResult<T> : ServiceResult<T>
    {
        public SuccessServiceResult(T resultObject)
            : base(true, resultObject)
        {
            ResultObject = resultObject;
        }

        public SuccessServiceResult(string message, T resultObject)
            : base(true, message, resultObject)
        {
            ResultObject = resultObject;
        }
    }

    public class FalseServiceResult<T> : ServiceResult<T>
    {
        public FalseServiceResult(HttpStatusCode httpStatusCode, string message, T resultObject)
            : base(false, httpStatusCode, message, resultObject)
        {
            ResultObject = resultObject;
        }
    }
}
