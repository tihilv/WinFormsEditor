using System.Drawing;
using System.Windows.Forms;

namespace ClientSide.View.Grips
{
    class LeftGrip: DragGripBase
    {
        public LeftGrip(Control controlToManage) : base(controlToManage)
        {
            Cursor = Cursors.SizeWE;
        }

        protected override void DoModify(Control controlToManage, Point original, Point current)
        {
            var diff = current.X - original.X;

            controlToManage.Width -= diff;
            controlToManage.Left += diff;
        }

        protected override Point GetGripPlacement(Control controlToManage)
        {
            var position = GetPositionInControl(controlToManage);
            var middleTop = position.Y + controlToManage.Height / 2;

            return new Point(position.X, middleTop);
        }
    }
}
