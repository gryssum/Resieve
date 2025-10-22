using System;

namespace Resieve.Exceptions
{
    public class ResieveFilterException(string message) : ArgumentException(message);
}