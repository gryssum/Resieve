using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Resieve.Mappings
{
    public class ResieveMapper : IResieveMapper
    {
        private readonly Dictionary<Type, Dictionary<string, ResievePropertyMap>> _propertyMappings
            = new Dictionary<Type, Dictionary<string, ResievePropertyMap>>();

        public IReadOnlyDictionary<Type, Dictionary<string, ResievePropertyMap>> PropertyMappings => _propertyMappings;

        public ResieveMapperBuilder<TEntity> Property<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            if (!_propertyMappings.ContainsKey(typeof(TEntity)))
            {
                _propertyMappings.Add(typeof(TEntity), new Dictionary<string, ResievePropertyMap>());
            }

            return new ResieveMapperBuilder<TEntity>(this, expression);
        }

        public void AddDefaultPropertyMap<TEntity>(string key)
        {
            var entityMapping = GetEntityMapping<TEntity>();

            entityMapping.Add(key, new ResievePropertyMap() {CanFilter = false, CanSort = false,});
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

        private Dictionary<string, ResievePropertyMap> GetEntityMapping<TEntity>()
        {
            var type = typeof(TEntity);

            if (!_propertyMappings.TryGetValue(type, out var entityMapping))
            {
                entityMapping = new Dictionary<string, ResievePropertyMap>();
                _propertyMappings[type] = entityMapping;
            }

            return entityMapping;
        }
    }

    public interface IResieveMapper
    {
        IReadOnlyDictionary<Type, Dictionary<string, ResievePropertyMap>> PropertyMappings { get; }
    }

    public class ResieveMapperBuilder<TEntity>
    {
        private readonly string _key;
        private readonly ResieveMapper _mapper;

        public ResieveMapperBuilder(ResieveMapper mapper, Expression<Func<TEntity, object>> expression)
        {
            _mapper = mapper;
            _key = GetPropertyName(expression);
            _mapper.AddDefaultPropertyMap<TEntity>(_key);
        }

        public ResieveMapperBuilder<TEntity> CanFilter()
        {
            _mapper.SetFilterable<TEntity>(_key);
            return this;
        }

        public ResieveMapperBuilder<TEntity> CanSort()
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