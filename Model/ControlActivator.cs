using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Model
{
    public static class ControlActivator
    {
        private static readonly Lazy<Type[]> _controlTypes;
        static ControlActivator()
        {
            _controlTypes = new Lazy<Type[]>(()=>GetAllControlTypes().OrderBy(t=>t.Name).ToArray());
        }

        const string InitializeComponentMethod = "InitializeComponent";

        internal static Control CreateControl(Assembly assembly, string typeName)
        {
            var type = assembly.GetTypes().FirstOrDefault((t => t.Name == typeName));
            if (type == null)
                throw new TypeLoadException($"Unable to find type {typeName}");

            var obj = Activator.CreateInstance(type);

            var initMethod = type.GetMethod(InitializeComponentMethod, BindingFlags.Instance | BindingFlags.NonPublic);

            if (initMethod != null)
                initMethod.Invoke(obj, new object[0]);

            return (Control) obj;
        }

        public static Type[] ControlTypes => _controlTypes.Value;

        private static IEnumerable<Type> GetAllControlTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetExportedTypes())
                if (type.IsSubclassOf(typeof(Control)) && !type.IsAbstract)
                    yield return type;
        }
    }
}