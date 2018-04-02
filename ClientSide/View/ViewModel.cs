using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClientSide.View.Grips;
using Model.ComponentParsing;
using Model.Writing;

namespace ClientSide.View
{
    class ViewModel: IDisposable
    {
        private readonly string _fileName;
        private readonly Control _control;

        private readonly DesignControlsProvider _designControlsProvider;
        private readonly List<Control> _designControls;
        
        private Control _currentControl;

        private Type _typeToCreate;

        public Control CurrentControl
        {
            get => _currentControl;
            private set
            {
                if (_currentControl != value)
                {
                    DisposeDesignControls();

                    if (value != _control)
                    {
                        _designControls.AddRange(_designControlsProvider.GetControls(value));
                        RegisterDesignControls();
                    }

                    _currentControl = value;
                    CurrentSelectionChanged?.Invoke(value, EventArgs.Empty);
                }
            }
        }

        public Control MainControl => _control;

        public event EventHandler<EventArgs> CurrentSelectionChanged;

        public ViewModel(string fileName, Control control)
        {
            _fileName = fileName;
            _control = control;

            _designControlsProvider = new DesignControlsProvider();
            _designControls = new List<Control>();

            InitInteraction();
        }

        public void Dispose()
        {
            UninitInteraction();

            _control?.Dispose();
        }

        private void InitInteraction()
        {
            var controls = FlatternControls();
            foreach (Control control in controls)
                InitInteraction(control);
        }

        private void UninitInteraction()
        {
            var controls = FlatternControls();
            foreach (Control control in controls)
                UninitInteraction(control);
        }

        private void InitInteraction(Control control)
        {
            control.Click += OnControlClick;
            control.MouseDown += OnControlMouseDown;
        }

        private void UninitInteraction(Control control)
        {
            control.MouseDown -= OnControlMouseDown;
            control.Click -= OnControlClick;
        }

        private void OnControlClick(object sender, EventArgs e)
        {
            if (sender != CurrentControl)
            {
                CurrentControl = sender as Control;
            }
        }

        private void OnControlMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_typeToCreate != null)
            {
                AddControl(_typeToCreate, (Control)sender, mouseEventArgs.Location);
                _typeToCreate = null;
            }
        }

        private void RegisterDesignControls()
        {
            foreach (var designControl in _designControls)
            {
                if (designControl is IGrip grip)
                    grip.PropertyChanged += GripOnPropertyChanged;

                _control.Controls.Add(designControl);
                designControl.BringToFront();
            }
        }

        private void DisposeDesignControls()
        {
            foreach (var designControl in _designControls)
            {
                _control.Controls.Remove(designControl);

                if (designControl is IGrip grip)
                    grip.PropertyChanged -= GripOnPropertyChanged;


                designControl.Dispose();
            }
            
            _designControls.Clear();
        }

        private void GripOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            foreach (var designControl in _designControls.OfType<IGrip>())
                designControl.OnControlChanged();
        }

        private List<Control> FlatternControls()
        {
            List<Control> result = new List<Control>();
            FlatternControls(_control, result);
            return result;
        }

        private void FlatternControls(Control control, List<Control> result)
        {
            result.Add(control);
            foreach (Control child in control.Controls)
                FlatternControls(child, result);
        }

        public void DeleteSelection()
        {
            if (CurrentControl != null && CurrentControl.Parent != null)
            {
                UninitInteraction(CurrentControl);
                CurrentControl.Parent.Controls.Remove(CurrentControl);
                CurrentControl = null;
            }
        }

        public void Save()
        {
            var representation = (new ComponentParser()).GetRepresentation(_control);
            var writer = new ControlCSharpWriter();

            var str = writer.SerializeControl(representation);
            File.WriteAllText(_fileName, str);

        }

        public void AddControlOfType(Type type)
        {
            _typeToCreate = type;
        }

        private void AddControl(Type type, Control parent, Point location)
        {
            var instance = (Control) Activator.CreateInstance(type);

            instance.Left = location.X;
            instance.Top = location.Y;
            instance.Name = GetName(type);
            instance.Text = instance.Name;
            parent.Controls.Add(instance);
            InitInteraction(instance);
        }

        string GetName(Type type)
        {
            string resultBase = type.Name;
            resultBase = resultBase[0].ToString().ToLower() + resultBase.Substring(1);

            HashSet<string> existingNames = new HashSet<string>();
            foreach (var name in FlatternControls().Select(c => c.Name))
                existingNames.Add(name);

            int index = 1;
            string result;
            do
            {
                result = resultBase + index;
                index++;
            } while (existingNames.Contains(result));

            return result;
        }
    }
}