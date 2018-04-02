using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ClientSide.View.Grips;

namespace ClientSide.View
{
    class DesignControlsProvider
    {
        private Dictionary<Control, AlterationCapabilities> _alterationCapabilitieses;

        public DesignControlsProvider()
        {
            _alterationCapabilitieses = new Dictionary<Control, AlterationCapabilities>();
        }

        AlterationCapabilities GetAlterationCapabilities(Control control)
        {
            if (!_alterationCapabilitieses.TryGetValue(control, out var result))
            {
                result = new AlterationCapabilities(control);
                _alterationCapabilitieses.Add(control, result);
            }

            return result;
        }

        public IEnumerable<Control> GetControls(Component component)
        {
            if (component is Control control)
            {
                var capabilities = GetAlterationCapabilities(control);

                if (capabilities.Movable)
                    yield return new MoveGrip(control);

                if (capabilities.HeightChangable)
                {
                    yield return new TopGrip(control);
                    yield return new BottomGrip(control);
                }

                if (capabilities.WidthChangable)
                {
                    yield return new LeftGrip(control);
                    yield return new RightGrip(control);
                }
            }
        }
    }

    class AlterationCapabilities
    {
        public readonly bool HeightChangable;
        public readonly bool WidthChangable;
        public readonly bool Movable;

        public AlterationCapabilities(Control control)
        {
            HeightChangable = CanChangeHeight(control);
            WidthChangable = CanChangeWidth(control);
            Movable = CanMove(control);
        }

        protected bool CanChangeWidth(Control control)
        {
            var currentWidth = control.Width;
            try
            {
                control.Width += 100;
                return control.Width != currentWidth;
            }
            finally
            {
                control.Width = currentWidth;
            }
        }

        protected bool CanChangeHeight(Control control)
        {
            var currentHeight = control.Height;
            try
            {
                control.Height += 100;
                return control.Height != currentHeight;
            }
            finally
            {
                control.Height = currentHeight;
            }
        }

        protected bool CanMove(Control control)
        {
            var currentLocation = control.Location;
            try
            {
                control.Left += 100;
                control.Top += 100;
                return control.Left != currentLocation.X || control.Top != currentLocation.Y;
            }
            finally
            {
                control.Location = currentLocation;
            }
        }

    }
}
