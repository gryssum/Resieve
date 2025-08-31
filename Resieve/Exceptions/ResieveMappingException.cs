using System;

namespace Resieve.Exceptions
{
    public class ResieveMappingException : ArgumentException
    {
        public ResieveMappingException(string message) : base (message)
        {
        }
    }
}