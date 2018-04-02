using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Model.FormParsing;

namespace Model.ComponentParsing
{
    public class ComponentRepresentation
    {
        private const string NameProperty = "Name";

        public readonly Type ComponentType;
        public readonly bool IsRoot;

        public readonly List<ValueProperty> ValueProperties;
        public readonly List<CollectionProperty> CollectionProperties;
        public readonly string Name;

        public ComponentRepresentation(Component component, bool isRoot)
        {
            ComponentType = component.GetType();
            IsRoot = isRoot;

            ValueProperties = new List<ValueProperty>();
            CollectionProperties = new List<CollectionProperty>();

            var property = component.GetType().GetProperty(NameProperty);
            if (property != null)
                Name = (string) property.GetValue(component);
            else
                Name = null;
        }

        public IEnumerable<ComponentRepresentation> Flattern()
        {
            foreach (var child in CollectionProperties.SelectMany(c => c.Values).OfType<ComponentRepresentation>().SelectMany(o => o.Flattern()))
                yield return child;

            yield return this;
        }
    }
}