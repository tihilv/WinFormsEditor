using System.Collections.Generic;

namespace Model.ComponentParsing
{
    public class CollectionProperty
    {
        public readonly string Name;

        public readonly List<object> Values;

        public CollectionProperty(string name)
        {
            Name = name;

            Values = new List<object>();
        }
    }
}