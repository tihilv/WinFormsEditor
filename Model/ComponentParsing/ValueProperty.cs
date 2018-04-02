namespace Model.FormParsing
{
    public class ValueProperty
    {
        public readonly string Name;
        public readonly object Value;

        public ValueProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}