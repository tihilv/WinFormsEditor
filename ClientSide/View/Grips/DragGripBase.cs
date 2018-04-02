using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ClientSide.View.Grips
{
    abstract class DragGripBase: Panel, IGrip
    {
        private const int DefSize = 4;

        private readonly Control _controlToManage;

        private Point _originalLocation;
        private Point _prevLocation;
        private Point _currentLocation;

        private bool _inProgress;

        public event PropertyChangedEventHandler PropertyChanged;

        protected DragGripBase(Control controlToManage)
        {
            _controlToManage = controlToManage;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            Size = new Size(2 * DefSize, 2 * DefSize);
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;

            SetPosition();
        }

        void SetPosition()
        {
            var position = GetGripPlacement(_controlToManage);
            Location = new Point(position.X - DefSize, position.Y - DefSize);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _inProgress = e.Button == MouseButtons.Left;

            if (_inProgress)
            {
                _originalLocation = e.Location;
                _prevLocation = e.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_inProgress)
            {
                _currentLocation = e.Location;

                DoModify(_controlToManage, _originalLocation, _currentLocation);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_controlToManage)));

                _prevLocation = _currentLocation;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _inProgress = false;
        }

        protected abstract void DoModify(Control controlToManage, Point original, Point current);
        protected abstract Point GetGripPlacement(Control controlToManage);

        public void OnControlChanged()
        {
            SetPosition();
        }

        protected Point GetPositionInControl(Control ctrl)
        {
            Point p = ctrl.Location;
            Control parent = ctrl.Parent;
            while (parent != Parent && parent != null)
            {
                p.Offset(parent.Location.X, parent.Location.Y);
                parent = parent.Parent;
            }
            return p;
        }
    }

    internal interface IGrip: INotifyPropertyChanged
    {
        void OnControlChanged();
    }
}
