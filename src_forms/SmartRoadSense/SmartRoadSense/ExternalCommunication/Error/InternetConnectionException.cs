using System;
namespace SmartRoadSense
{
    public class InternetConnectionException : Exception
    {
        protected InternetConnectionException() : base() { } // mandatory

        public InternetConnectionException(string message) : base(message) { }

        public InternetConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
