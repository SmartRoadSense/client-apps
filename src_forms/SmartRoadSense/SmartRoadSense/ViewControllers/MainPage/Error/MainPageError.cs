using System;
namespace SmartRoadSense
{
    public class MainPageException : Exception
    {
        protected MainPageException() : base() { } // mandatory

        public MainPageException(string message) : base(message) { }

        public MainPageException(string message, Exception innerException) : base(message, innerException) { }
    }
}
