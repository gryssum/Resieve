using System;
using System.Collections.Generic;
using System.Linq;

namespace ReSieve.Filtering
{
    public interface IFilterTerm
    {
        string Property { get; }
        FilterOperators Operator { get; }
        string Value { get; }
    }

    public class FilterTerm : IFilterTerm
    {
        public FilterTerm(string property, FilterOperators op, string value)
        {
            Property = property;
            Operator = op;
            Value = value;
        }
     
        public string Property { get; }
        public FilterOperators Operator { get; }
        public string Value { get; }
    }
}