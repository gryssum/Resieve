using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ReSieve.Services
{
    public class ReSieveMapper
    {
        private readonly Dictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>> _mappings
            = new Dictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>>();

        public IReadOnlyDictionary<Type, ICollection<KeyValuePair<string, IReSievePropertyMetadata>>> Mappings =>
            _mappings;

        public ReSieveMapperBuilder<TEntity> Property<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            if (!_mappings.ContainsKey(typeof(TEntity)))
            {
                _mappings.Add(typeof(TEntity), new List<KeyValuePair<string, IReSievePropertyMetadata>>());
            }

            return new ReSieveMapperBuilder<TEntity>(this, expression);
        }

        public void AddProperty<TEntity>(KeyValuePair<string, IReSievePropertyMetadata> keyValuePair)
        {
            if (_mappings.ContainsKey(typeof(TEntity)))
            {
                _mappings[typeof(TEntity)].Add(keyValuePair);
                return;
            }
            
            if (_mappings.ContainsKey(typeof(TEntity)) && _mappings[typeof(TEntity)].Any(x => x.Key == keyValuePair.Key))
            {
                throw new ArgumentException($"Property '{keyValuePair.Key}' is already mapped for entity type '{typeof(TEntity).Name}'.");
            }
            
            _mappings.Add(typeof(TEntity), new List<KeyValuePair<string, IReSievePropertyMetadata>>() { keyValuePair });
        }
    }

    public class ReSieveMapperBuilder<TEntity>
    {
        private readonly ReSieveMapper _sievePropertyMapper;
        private readonly string _key;
        
        public ReSieveMapperBuilder(ReSieveMapper sievePropertyMapper, Expression<Func<TEntity, object>> expression)
        {
            _sievePropertyMapper = sievePropertyMapper;

            _key = GetPropertyName(expression);
            _canFilter = false;
            _canSort = false;
        }

        private bool _canFilter;
        private bool _canSort;

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
            var metadata = new ReSievePropertyMetadata()
            {
                CanFilter = _canFilter,
                CanSort = _canSort
            };

            var pair = new KeyValuePair<string, IReSievePropertyMetadata>(_key, metadata);

            _sievePropertyMapper.AddProperty<TEntity>(pair);
        }
        
        private string GetPropertyName(Expression<Func<TEntity, object>> expression)
        {
            if (expression.Body is MemberExpression member)
                return member.Member.Name;

            if (expression.Body is UnaryExpression { Operand: MemberExpression memberOperand })
                return memberOperand.Member.Name;

            throw new ArgumentException("Expression must be a property access.", nameof(expression));
        }
    }
}