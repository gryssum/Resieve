using System;

namespace Resieve.Exceptions
{
    public class ResieveFilterException : ArgumentException
    {
        public ResieveFilterException(string message) : base(message)
        {
        }
    }
}