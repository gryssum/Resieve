using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Resieve.Exceptions;
using Resieve.Mappings.Interfaces;

namespace Resieve.Mappings
{
    public class ResieveMapper : IResieveMapper
    {
        private readonly Dictionary<Type, Dictionary<string, ResievePropertyMap>> _propertyMappings
            = new Dictionary<Type, Dictionary<string, ResievePropertyMap>>();

        public IReadOnlyDictionary<Type, Dictionary<string, ResievePropertyMap>> PropertyMappings => _propertyMappings;

        public ResieveMapperBuilder<TEntity> ForProperty<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            if (!_propertyMappings.ContainsKey(typeof(TEntity)))
            {
                _propertyMappings.Add(typeof(TEntity), new Dictionary<string, ResievePropertyMap>());
            }

            return new ResieveMapperBuilder<TEntity>(this, expression);
        }
        
        public ResieveMapperBuilder<TEntity> ForKey<TEntity>(string key)
        {
            if (!_propertyMappings.ContainsKey(typeof(TEntity)))
            {
                _propertyMappings.Add(typeof(TEntity), new Dictionary<string, ResievePropertyMap>());
            }

            return new ResieveMapperBuilder<TEntity>(this, key);
        }

        public void AddDefaultPropertyMap<TEntity>(string key)
        {
            var entityMapping = GetEntityMapping<TEntity>();

            entityMapping.Add(key, new ResievePropertyMap() {CanFilter = false, CanSort = false,});
        }

        public void SetFilterable<TEntity>(string key, Type? customFilter = null)
        {
            var entityMapping = GetEntityMapping<TEntity>();

            if (entityMapping.TryGetValue(key, out var propertyMapping))
            {
                propertyMapping.CanFilter = true;
                propertyMapping.CustomFilter = customFilter;
            }
        }

        public void SetSortable<TEntity>(string key, Type? customSort = null)
        {
            var entityMapping = GetEntityMapping<TEntity>();

            if (entityMapping.TryGetValue(key, out var propertyMapping))
            {
                propertyMapping.CanSort = true;
                propertyMapping.CustomSort = customSort;
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

    public class ResieveMapperBuilder<TEntity>
    {
        private readonly string _key;
        private readonly ResieveMapper _mapper;

        private readonly bool _customFilterNecessary;

        public ResieveMapperBuilder(ResieveMapper mapper, Expression<Func<TEntity, object>> expression)
        {
            _mapper = mapper;
            _key = GetPropertyName(expression);
            _mapper.AddDefaultPropertyMap<TEntity>(_key);
        }

        public ResieveMapperBuilder(ResieveMapper mapper, string key)
        {
            _mapper = mapper;
            _key = key;
            _mapper.AddDefaultPropertyMap<TEntity>(_key);
            _customFilterNecessary = true;
        }
        
        public ResieveMapperBuilder<TEntity> CanFilter()
        {
            GuardAgainstCustomKeyWithoutCustomFilter();
            
            _mapper.SetFilterable<TEntity>(_key);
            return this;
        }

        public ResieveMapperBuilder<TEntity> CanFilter<TCustomFilter>() where TCustomFilter : IResieveCustomFilter<TEntity>
        {
            _mapper.SetFilterable<TEntity>(_key, typeof(TCustomFilter));
            return this;
        }
        
        public ResieveMapperBuilder<TEntity> CanSort()
        {
            _mapper.SetSortable<TEntity>(_key);
            return this;
        }
        
        public ResieveMapperBuilder<TEntity> CanSort<TCustomSort>() where TCustomSort : IResieveCustomSort
        {
            _mapper.SetSortable<TEntity>(_key, typeof(TCustomSort));
            return this;
        }
        
        private void GuardAgainstCustomKeyWithoutCustomFilter()
        {
            if (_customFilterNecessary)
            {
                throw new ResieveMappingException("Custom filter type must be provided when using string key.");
            }
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

            throw new ResieveMappingException($"Expression '{nameof(expression)}' must be a property access.");
        }
    }

}