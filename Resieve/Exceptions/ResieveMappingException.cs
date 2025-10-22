using System;

namespace Resieve.Exceptions
{
    public class ResieveMappingException(string message) : ArgumentException(message);
}