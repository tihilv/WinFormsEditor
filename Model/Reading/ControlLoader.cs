using System.Windows.Forms;

namespace Model.Reading
{
    public class ControlLoader
    {
        private const string DesignerFileName = ".Designer.";

        private readonly CodeFileProvider _codeFileProvider;
        private readonly DesignerFileProvider _designerFileProvider;

        public string DesignerFilename { get; private set; }
        
        public ControlLoader(string fileName)
        {
            var extIndex = fileName.LastIndexOf(".");
            
            DesignerFilename = fileName.Substring(0, extIndex) + DesignerFileName + fileName.Substring(extIndex + 1);

            _codeFileProvider = new CodeFileProvider(fileName);
            _designerFileProvider = new DesignerFileProvider(DesignerFilename);
        }

        public Control LoadControl()
        {
            var designerClass = _designerFileProvider.GetControlClass();
            var baseType = _codeFileProvider.GetControlBaseType(designerClass.Identifier.Text);

            _designerFileProvider.ReplaceBaseClass(designerClass, baseType);

            var control = _designerFileProvider.Compile();

            return control;
        }

    }
}
