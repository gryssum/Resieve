using System;

namespace Resieve.Exceptions
{
    public class ResieveSortingException : ArgumentException
    {
        public ResieveSortingException(string message) : base(message)
        {
        }
    }
}