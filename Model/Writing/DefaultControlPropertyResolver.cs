using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Model.Writing
{
    internal class DefaultControlPropertyResolver
    {
        private readonly Dictionary<Type, object> _defaultObjectCache;

        public DefaultControlPropertyResolver()
        {
            _defaultObjectCache = new Dictionary<Type, object>();
        }

        public bool IsControlPropertySetToDefaultValue(Component component, PropertyInfo property)
        {
            if (property.Name == nameof(Control.Name)) // Name is always non-default
                return false;

            var existingValue = property.GetValue(component);
            var defaultValue = GetDefaultValue(component, property);

            if (Equals(existingValue, defaultValue))
                return true;

            defaultValue = GetDefaultByTypeInstance(component.GetType(), property);

            return Equals(existingValue, defaultValue);
        }

        private object GetDefaultValue(Component component, PropertyInfo property)
        {
            var type = component.GetType();
            var defaultProperty = type.GetProperty($"Default{property.Name}", BindingFlags.Instance | BindingFlags.NonPublic);
            if (defaultProperty != null)
                return defaultProperty.GetValue(component);

            return false;
        }


        private object GetDefaultByTypeInstance(Type type, PropertyInfo property)
        {
            object obj;
            if (!_defaultObjectCache.TryGetValue(type, out obj))
            {
                obj = Activator.CreateInstance(type);
                _defaultObjectCache.Add(type, obj);
            }

            return property.GetValue(obj);
        }
    }
}