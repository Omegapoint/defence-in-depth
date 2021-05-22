using System;

namespace Defence.In.Depth.Domain.Exceptions
{
    public class InvalidProductNameArgumentException : ArgumentException
    {
        public InvalidProductNameArgumentException(string message) : base(message)
        {
        }
    }
}
