using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ReSieve.Mappings
{
    public class ReSievePropertyMap : IReSievePropertyMetadata
    {
        public bool CanFilter { get; set; }
        public bool CanSort { get; set; }
    }

    public class ReSieveMapper
    {
        private readonly Dictionary<Type, Dictionary<string, ReSievePropertyMap>> _propertyMappings
            = new Dictionary<Type, Dictionary<string, ReSievePropertyMap>>();

        public IReadOnlyDictionary<Type, Dictionary<string, ReSievePropertyMap>> PropertyMappings => _propertyMappings;

        public ReSieveMapperBuilder<TEntity> Property<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            if (!_propertyMappings.ContainsKey(typeof(TEntity)))
            {
                _propertyMappings.Add(typeof(TEntity), new Dictionary<string, ReSievePropertyMap>());
            }

            return new ReSieveMapperBuilder<TEntity>(this, expression);
        }

        public void AddDefaultPropertyMap<TEntity>(string key)
        {
            var entityMapping = GetEntityMapping<TEntity>();

            entityMapping.Add(key, new ReSievePropertyMap() {CanFilter = false, CanSort = false,});
        }

        public void SetFilterable<TEntity>(string key)
        {
            var entityMapping = GetEntityMapping<TEntity>();

            if (entityMapping.TryGetValue(key, out var propertyMapping))
            {
                propertyMapping.CanFilter = true;
            }
        }

        public void SetSortable<TEntity>(string key)
        {
            var entityMapping = GetEntityMapping<TEntity>();

            if (entityMapping.TryGetValue(key, out var propertyMapping))
            {
                propertyMapping.CanSort = true;
            }
        }

        private Dictionary<string, ReSievePropertyMap> GetEntityMapping<TEntity>()
        {
            var type = typeof(TEntity);

            if (!_propertyMappings.TryGetValue(type, out var entityMapping))
            {
                entityMapping = new Dictionary<string, ReSievePropertyMap>();
                _propertyMappings[type] = entityMapping;
            }

            return entityMapping;
        }
    }

    public class ReSieveMapperBuilder<TEntity>
    {
        private readonly string _key;
        private readonly ReSieveMapper _mapper;

        public ReSieveMapperBuilder(ReSieveMapper mapper, Expression<Func<TEntity, object>> expression)
        {
            _mapper = mapper;
            _key = GetPropertyName(expression);
            _mapper.AddDefaultPropertyMap<TEntity>(_key);
        }

        public ReSieveMapperBuilder<TEntity> CanFilter()
        {
            _mapper.SetFilterable<TEntity>(_key);
            return this;
        }

        public ReSieveMapperBuilder<TEntity> CanSort()
        {
            _mapper.SetSortable<TEntity>(_key);
            return this;
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