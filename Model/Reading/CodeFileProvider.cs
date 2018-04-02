using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Model.Reading
{
    class CodeFileProvider
    {
        private readonly string _fileName;

        private SyntaxTree _formTree;
        private SyntaxNode _formRoot;

        private CSharpCompilation _compilation;

        public CodeFileProvider(string fileName)
        {
            _fileName = fileName;
        }

        private bool IsInited => _formTree != null;

        private void InitIfNeeded()
        {
            if (!IsInited)
            {
                string formCode = File.ReadAllText(_fileName);
                _formTree = CSharpSyntaxTree.ParseText(formCode);
                _formRoot = _formTree.GetRoot();
                _compilation = CSharpCompilation.Create("CodeFileProviderCompilation", syntaxTrees: new[] {_formTree}, references: ReferenceProvider.References);
            }
        }

        internal string GetControlBaseType(string className)
        {
            // Class type is not declared in Designer.cs file
            InitIfNeeded();
            var classDeclaration = _formRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(c => c.Identifier.Text == className);
            SemanticModel model = _compilation.GetSemanticModel(_formTree);
            var myClassSymbol = model.GetDeclaredSymbol(classDeclaration);

            var fullName = myClassSymbol.BaseType.ToDisplayString(new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
            return fullName;
        }
    }
}