using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;

namespace Model.Reading
{
    // Stub to represent referenced assemblies
    class ReferenceProvider
    {
        private static readonly Lazy<Assembly[]> _assemblies;
        private static readonly Lazy<MetadataReference[]> _references;

        static ReferenceProvider()
        {
            _assemblies = new Lazy<Assembly[]>(()=> new Assembly[]
            {
                typeof(object).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Form).Assembly,
                typeof(IContainer).Assembly,
                typeof(Point).Assembly

            });
            _references = new Lazy<MetadataReference[]>(() => Assemblies.Select(a=>MetadataReference.CreateFromFile(a.Location)).ToArray());
        }

        public static Assembly[] Assemblies => _assemblies.Value;
        public static MetadataReference[] References => _references.Value;
    }
}