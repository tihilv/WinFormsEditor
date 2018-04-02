using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Model.Reading
{
    class DesignerFileProvider
    {
        private readonly string _fileName;

        private SyntaxTree _formTree;
        private SyntaxNode _formRoot;
        
        public DesignerFileProvider(string fileName)
        {
            _fileName = fileName;
        }

        private bool IsInited => _formTree != null;

        void InitIfNeeded()
        {
            if (!IsInited)
            {
                var designerCode = File.ReadAllText(_fileName);
                _formTree = CSharpSyntaxTree.ParseText(designerCode);
                _formRoot = _formTree.GetRoot();
            }
        }

        public ClassDeclarationSyntax GetControlClass()
        {
            // By condition a form should be the first class declared in the file
            InitIfNeeded();
            return _formRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        }

        public void ReplaceBaseClass(ClassDeclarationSyntax classDeclaration, string baseType)
        {
            InitIfNeeded();
            IdentifierNameSyntax identifierName = SyntaxFactory.IdentifierName(baseType);
            BaseTypeSyntax baseTypeSyntax = SyntaxFactory.SimpleBaseType(identifierName);
            SeparatedSyntaxList<BaseTypeSyntax> separatedSyntaxList = new SeparatedSyntaxList<BaseTypeSyntax>();
            separatedSyntaxList = separatedSyntaxList.Add(baseTypeSyntax);
            BaseListSyntax baseSyntaxList = SyntaxFactory.BaseList(separatedSyntaxList);

            _formRoot = _formRoot.ReplaceNode(classDeclaration, classDeclaration.WithBaseList(baseSyntaxList));
            _formTree = _formTree.WithRootAndOptions(_formRoot, _formTree.Options);
        }

        public Control Compile()
        {
            InitIfNeeded();
            // Old style compilation is used cause i'm too lazy to dig into Roslyn compilation errors for Forms assembly.
            CodeDomProvider icc = CodeDomProvider.CreateProvider("CSharp");;

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.AddRange(ReferenceProvider.Assemblies.Select(a=>a.Location).ToArray());
            CompilerResults results = icc.CompileAssemblyFromSource(parameters, _formTree.ToString());

            if (results.Errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in results.Errors)
                    sb.AppendLine(error.ToString());
                throw new Exception(sb.ToString());
            }

            return ControlActivator.CreateControl(results.CompiledAssembly, GetControlClass().Identifier.Text);
        }
    }
}