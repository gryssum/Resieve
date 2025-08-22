using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ReSieve.Mappings
{
    public class ReSieveMapper
    {
        private readonly Dictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>> _filterMappings
            = new Dictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>>();

        private readonly Dictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>> _sortMappings
            = new Dictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>>();

        public IReadOnlyDictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>> FilterMappings => _filterMappings;
        public IReadOnlyDictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>> SortMappings => _sortMappings;

        public ReSieveMapperBuilder<TEntity> Property<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            if (!_filterMappings.ContainsKey(typeof(TEntity)))
            {
                _filterMappings.Add(typeof(TEntity), new List<KeyValuePair<string, IReSievePropertyMetadata>>());
            }
            if (!_sortMappings.ContainsKey(typeof(TEntity)))
            {
                _sortMappings.Add(typeof(TEntity), new List<KeyValuePair<string, IReSievePropertyMetadata>>());
            }
            return new ReSieveMapperBuilder<TEntity>(this, expression);
        }

        public void AddFilterProperty<TEntity>(KeyValuePair<string, IReSievePropertyMetadata> keyValuePair)
        {
            if (_filterMappings.ContainsKey(typeof(TEntity)))
            {
                _filterMappings[typeof(TEntity)].Add(keyValuePair);
                return;
            }
            _filterMappings.Add(typeof(TEntity), new List<KeyValuePair<string, IReSievePropertyMetadata>> {keyValuePair});
        }

        public void AddSortProperty<TEntity>(KeyValuePair<string, IReSievePropertyMetadata> keyValuePair)
        {
            if (_sortMappings.ContainsKey(typeof(TEntity)))
            {
                _sortMappings[typeof(TEntity)].Add(keyValuePair);
                return;
            }
            _sortMappings.Add(typeof(TEntity), new List<KeyValuePair<string, IReSievePropertyMetadata>> {keyValuePair});
        }

        public IReSievePropertyMetadata GetFilterPropertyMetadata<TEntity>(string propertyName)
        {
            if (!_filterMappings.TryGetValue(typeof(TEntity), out var propertyMappings))
            {
                throw new InvalidOperationException($"No filter properties mapped for entity type '{typeof(TEntity).Name}'.");
            }
            var propertyMeta = propertyMappings.FirstOrDefault(x => x.Key == propertyName && x.Value.CanFilter).Value;
            if (propertyMeta == null)
            {
                throw new InvalidOperationException($"Property '{propertyName}' is not mapped or not filterable for entity type '{typeof(TEntity).Name}'.");
            }
            return propertyMeta;
        }

        public IReSievePropertyMetadata GetSortPropertyMetadata<TEntity>(string propertyName)
        {
            if (!_sortMappings.TryGetValue(typeof(TEntity), out var propertyMappings))
            {
                throw new InvalidOperationException($"No sort properties mapped for entity type '{typeof(TEntity).Name}'.");
            }
            var propertyMeta = propertyMappings.FirstOrDefault(x => x.Key == propertyName && x.Value.CanSort).Value;
            if (propertyMeta == null)
            {
                throw new InvalidOperationException($"Property '{propertyName}' is not mapped or not sortable for entity type '{typeof(TEntity).Name}'.");
            }
            return propertyMeta;
        }
    }

    public class ReSieveMapperBuilder<TEntity>
    {
        private readonly string _key;
        private readonly ReSieveMapper _sievePropertyMapper;

        private bool _canFilter;
        private bool _canSort;

        public ReSieveMapperBuilder(ReSieveMapper sievePropertyMapper, Expression<Func<TEntity, object>> expression)
        {
            _sievePropertyMapper = sievePropertyMapper;

            _key = GetPropertyName(expression);
            _canFilter = false;
            _canSort = false;
        }

        public ReSieveMapperBuilder<TEntity> CanFilter()
        {
            _canFilter = true;
            UpdateMap();
            return this;
        }

        public ReSieveMapperBuilder<TEntity> CanSort()
        {
            _canSort = true;
            UpdateMap();
            return this;
        }

        private void UpdateMap()
        {
            var propertyType = typeof(TEntity).GetProperty(_key)?.PropertyType ?? typeof(object);
            var metadata = new ReSievePropertyMetadata {CanFilter = _canFilter, CanSort = _canSort, PropertyType = propertyType};
            var pair = new KeyValuePair<string, IReSievePropertyMetadata>(_key, metadata);
            _sievePropertyMapper.AddFilterProperty<TEntity>(pair);
            _sievePropertyMapper.AddSortProperty<TEntity>(pair);
        }

        private string GetPropertyName(Expression<Func<TEntity, object>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return member.Member.Name;
            }

            if (expression.Body is UnaryExpression {Operand: MemberExpression memberOperand})
            {
                return memberOperand.Member.Name;
            }

            throw new ArgumentException("Expression must be a property access.", nameof(expression));
        }
    }
}