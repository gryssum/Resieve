using System.Collections.Generic;

namespace ReSieve.Filtering.TreeBuilder
{
    public abstract class FilterNode
    {
    }

    public class ComparisonNode : FilterNode
    {
        public ComparisonNode(string propertyName, FilterOperators operatorType, string value)
        {
            PropertyName = propertyName;
            Operator = operatorType;
            Value = value;
        }
        
        public string PropertyName { get; set; }
        public FilterOperators Operator { get; set; }
        public string Value { get; set; }
    }
    
    public class GroupComparisonNode : FilterNode
    {
        public GroupComparisonNode(string propertyName, FilterOperators operatorType,  List<string>  values)
        {
            PropertyName = propertyName;
            Operator = operatorType;
            Values = values;
        }
        
        public string PropertyName { get; set; }
        public FilterOperators Operator { get; set; }
        public List<string> Values { get; set; }
    }

    public class LogicalNode : FilterNode
    {
        public LogicalNode(FilterLogicalOperator operatorType, List<FilterNode> children)
        {
            Operator = operatorType;
            Children = children;
        }
        public FilterLogicalOperator Operator { get; }
        public List<FilterNode> Children { get; }
    }
}