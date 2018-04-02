using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Model.FormParsing;
using Model.Writing;

namespace Model.ComponentParsing
{
    public class ComponentParser
    {
        private readonly DefaultControlPropertyResolver _defaultControlPropertyResolver = new DefaultControlPropertyResolver();

        public ComponentRepresentation GetRepresentation(Component component)
        {
            Dictionary<Component, ComponentRepresentation> cache = new Dictionary<Component, ComponentRepresentation>();

            return GetRepresentation(component, true, cache);
        }

        ComponentRepresentation GetRepresentation(Component component, bool isRoot, Dictionary<Component, ComponentRepresentation> cache)
        {
            if (cache.TryGetValue(component, out ComponentRepresentation result))
                return result;
            result = new ComponentRepresentation(component, isRoot);
            cache.Add(component, result);

            var properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo property in properties)
            {
                if (AllowedProperty(property))
                {
                    var value = property.GetValue(component);
                    if (value is ICollection collectionValue)
                    {
                        CollectionProperty collectionProperty = new CollectionProperty(property.Name);
                        foreach (object child in collectionValue)
                        {
                            if (child is Component childComponent)
                                collectionProperty.Values.Add(GetRepresentation(childComponent, false, cache));
                            else
                                collectionProperty.Values.Add(child);
                        }

                        result.CollectionProperties.Add(collectionProperty);
                    }
                    else
                    {
                        if (!_defaultControlPropertyResolver.IsControlPropertySetToDefaultValue(component, property))
                        {
                            var valueToSet = value;
                            if (valueToSet is Component componentValue)
                                valueToSet = GetRepresentation(componentValue);

                            ValueProperty valueProperty = new ValueProperty(property.Name, valueToSet);
                            result.ValueProperties.Add(valueProperty);
                        }
                    }
                }
            }

            return result;
        }

        private bool AllowedProperty(PropertyInfo property)
        {
            if (property.Name == nameof(Control.Name) || property.Name == nameof(Control.Controls))
                return true;

            var hidden = ((DesignerSerializationVisibilityAttribute) property.GetCustomAttribute(typeof(DesignerSerializationVisibilityAttribute)))?.Visibility == DesignerSerializationVisibility.Hidden;
            if (hidden)
                return false;

            return ((BrowsableAttribute) property.GetCustomAttribute(typeof(BrowsableAttribute)))?.Browsable ?? true;
        }


    }
}

