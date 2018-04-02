using System;
using System.Windows.Forms;
using ClientSide.View;
using Model;
using Model.Reading;

namespace ClientSide
{
    public partial class MainForm : Form
    {
        private ViewModel _currentViewModel;

        public MainForm()
        {
            InitializeComponent();

            FillAddMenu();
        }

        void FillAddMenu()
        {
            foreach (Type type in ControlActivator.ControlTypes)
            {
                var item = new ToolStripMenuItem(type.Name) {Tag = type};
                item.Click += OnAddControlClick;
                addToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void OnAddControlClick(object sender, EventArgs eventArgs)
        {
            var type = (sender as ToolStripMenuItem)?.Tag as Type;
            if (type != null)
            {
                _currentViewModel.AddControlOfType(type);
            }
        }

        public MainForm(string fileName):this()
        {
            if (fileName != null)
            LoadModel(fileName);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseCurrentModel())
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "CSharp files|*.cs";
                    if (dialog.ShowDialog() == DialogResult.OK)
                        LoadModel(dialog.FileName);
                }
            }
        }

        void LoadModel(string fileName)
        {
            ControlLoader loader = new ControlLoader(fileName);
            Control control = loader.LoadControl();

            _currentViewModel = new ViewModel(loader.DesignerFilename, control);

            if (control is Form form)
            {
                form.TopLevel = false;
                form.Left = 0;
                form.Top = 0;
                form.Show();
            }

            formPanel.Controls.Add(control);

            _currentViewModel.CurrentSelectionChanged += OnCurrentSelectionChanged;
        }

        private void OnCurrentSelectionChanged(object sender, EventArgs eventArgs)
        {
            propertyGrid.SelectedObject = sender;
        }
        
        bool CloseCurrentModel()
        {
            if (_currentViewModel != null)
            {
                formPanel.Controls.Remove(_currentViewModel.MainControl);
                _currentViewModel.CurrentSelectionChanged -= OnCurrentSelectionChanged;
                _currentViewModel.Dispose();
            }

            return true;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                _currentViewModel.DeleteSelection();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentViewModel?.Save();
        }

        
    }
}
