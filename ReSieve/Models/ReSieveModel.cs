using System.Collections.Generic;

namespace ReSieve.Models
{
    public class ReSieveModel : ReSieveModel<FilterTerm, SortTerm> { }
    
    public class ReSieveModel<TFilterTerm, TSortTerm>
        where TFilterTerm : IFilterTerm, new()
        where TSortTerm : ISortTerm, new()
    {
        public List<TFilterTerm> Filters { get; set; } = new List<TFilterTerm>();
        public List<TSortTerm> Sorts { get; set; } = new List<TSortTerm>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public interface IFilterTerm
    {
        
    }
    
    public class FilterTerm : IFilterTerm
    {
    }

    public interface ISortTerm
    {
        
    }
    
    public class SortTerm : ISortTerm
    {
    }
}