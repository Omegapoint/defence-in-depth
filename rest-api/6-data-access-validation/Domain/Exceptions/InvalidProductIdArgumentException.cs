using System;

namespace Defence.In.Depth.Domain.Exceptions
{
    public class InvalidProductIdArgumentException : ArgumentException
    {
        public InvalidProductIdArgumentException(string message) : base(message)
        {
        }
    }
}
