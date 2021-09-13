using System;

namespace LineNotifySDK
{
    public class LineNotifyException : Exception
    {
        public LineNotifyException() : base() { }

        public LineNotifyException(string message) : base(message) { }

        public LineNotifyException(string message, Exception innerException) : base(message, innerException) { }
    }
}