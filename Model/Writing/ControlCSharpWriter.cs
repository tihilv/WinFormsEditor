using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.ComponentParsing;

namespace Model.Writing
{
    public class ControlCSharpWriter
    {
        public string SerializeControl(ComponentRepresentation root)
        {
            var allControls = root.Flattern().ToArray();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {root.ComponentType.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    partial class {root.ComponentType.Name}");
            sb.AppendLine("    {");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Required designer variable.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        private System.ComponentModel.IContainer components = null;");
            sb.AppendLine("");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Clean up any resources being used.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <param name=\"disposing\">true if managed resources should be disposed; otherwise, false.</param>");
            sb.AppendLine("        protected override void Dispose(bool disposing)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (disposing && (components != null))");
            sb.AppendLine("            {");
            sb.AppendLine("                components.Dispose();");
            sb.AppendLine("            }");
            sb.AppendLine("            base.Dispose(disposing);");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        #region Windows Form Designer generated code");
            sb.AppendLine("");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Required method for Designer support - do not modify");
            sb.AppendLine("        /// the contents of this method with the code editor.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        private void InitializeComponent()");
            sb.AppendLine("        {");

            InitializeVariables(sb, allControls);
            InitializeControlsProperties(sb, allControls);
            
            sb.AppendLine("            this.PerformLayout();");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        #endregion");

            DeclareVariables(sb, allControls);
            
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void InitializeVariables(StringBuilder sb, IEnumerable<ComponentRepresentation> controls)
        {
            foreach (var data in controls)
                if (!data.IsRoot)
                    sb.AppendLine($"            this.{data.Name} = new {data.ComponentType.FullName}();");
        }

        private void InitializeControlsProperties(StringBuilder sb, IEnumerable<ComponentRepresentation> controls)
        {
            foreach (var data in controls)
                InitializeControlProperties(sb, data);
        }

        private void InitializeControlProperties(StringBuilder sb, ComponentRepresentation data)
        {
            sb.AppendLine("            //");
            sb.AppendLine($"            // {data.Name}");
            sb.AppendLine("            //");

            string controlName;
            if (data.IsRoot)
                controlName = "this";
            else
                controlName = $"this.{data.Name}";

            foreach (var property in data.ValueProperties)
            {
                var valueCode = ProvideValueCode(property.Value);
                if (valueCode != null)
                    sb.AppendLine($"            {controlName}.{property.Name} = {valueCode};");
            }

            foreach (CollectionProperty property in data.CollectionProperties)
            {
                foreach (var value in property.Values)
                {
                    var valueCode = ProvideValueCode(value);
                    sb.AppendLine($"            {controlName}.{property.Name}.Add({valueCode});");
                }
            }
        }

        private string ProvideValueCode(object existingValue)
        {
            if (existingValue == null)
                return "null";

            if (existingValue is string s)
                return $"\"{s}\"";

            if (existingValue is bool b)
                return b ? "true" : "false";

            if (existingValue is System.Drawing.Point p)
                return $"new {p.GetType().FullName}({p.X}, {p.Y})";

            if (existingValue is System.Drawing.Size sz)
                return $"new {sz.GetType().FullName}({sz.Width}, {sz.Height})";

            if (existingValue is Enum e)
                return $"{e.GetType().FullName}.{e.ToString()}";


            if (existingValue is System.Windows.Forms.Padding pd)
            {
                if (pd.Left == pd.Right && pd.Right == pd.Top && pd.Top == pd.Bottom)
                    return $"new {pd.GetType().FullName}({pd.Left})";
                else
                    return $"new {pd.GetType().FullName}({pd.Left}, {pd.Top}, {pd.Right}, {pd.Bottom})";
            }


            if (existingValue is System.Drawing.Color c)
            {
                if (c.IsSystemColor)
                    return $"{typeof(System.Drawing.SystemColors).FullName}.{c.Name}";
                    
                if (c.IsNamedColor)
                    return $"{typeof(System.Drawing.Color).FullName}.{c.Name}";

                if (c.IsKnownColor)
                    return $"{typeof(System.Drawing.KnownColor).FullName}.{c.ToKnownColor()}";
                else 
                    return $"new {c.GetType().FullName}({c.R}, {c.G}, {c.B}, {c.A})";
            }

            if (existingValue is ComponentRepresentation r)
            {
                if (!string.IsNullOrEmpty(r.Name))
                    return $"this.{r.Name}";
            }

            if (existingValue.GetType().IsValueType)
                return existingValue.ToString();

            return null;
        }
        
        private void DeclareVariables(StringBuilder sb, IEnumerable<ComponentRepresentation> controls)
        {
            foreach (var data in controls)
                if (!data.IsRoot)
                    sb.AppendLine($"        private {data.ComponentType.FullName} {data.Name};");
        }
    }
}
