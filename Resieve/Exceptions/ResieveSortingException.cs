using System;

namespace Resieve.Exceptions
{
    public class ResieveSortingException(string message) : ArgumentException(message);
}