using System;
namespace SmartRoadSense
{
    public class RemoteException : Exception
    {
        public int StatusCodeException;

        protected RemoteException() : base() { } // mandatory

        public RemoteException(int code) : base(String.Format("Error code {0}", code))
        {
            StatusCodeException = code;
        }
        public RemoteException(int code, string message) : base(message)
        {
            StatusCodeException = code;
        }
        public RemoteException(int code, string message, Exception innerException) : base(message, innerException)
        {
            StatusCodeException = code;
        }

        public int StatusCode { get { return StatusCodeException; } }
    }
}
