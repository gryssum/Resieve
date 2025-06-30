using ReSieve.Filtering;
using ReSieve.Sorting;

namespace ReSieve
{
    public class ReSieveModel : ReSieveModel<FilterTerm, SortTerm>
    {
    }

    public abstract class ReSieveModel<TFilterTerm, TSortTerm>
        where TFilterTerm : IFilterTerm
        where TSortTerm : ISortTerm
    {
        public string? Filters { get; set; }

        public string? Sorts { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}