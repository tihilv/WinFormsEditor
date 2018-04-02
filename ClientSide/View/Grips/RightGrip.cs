using System.Drawing;
using System.Windows.Forms;

namespace ClientSide.View.Grips
{
    class RightGrip: DragGripBase
    {
        public RightGrip(Control controlToManage) : base(controlToManage)
        {
            Cursor = Cursors.SizeWE;
        }

        protected override void DoModify(Control controlToManage, Point original, Point current)
        {
            controlToManage.Width += current.X - original.X;
        }

        protected override Point GetGripPlacement(Control controlToManage)
        {
            var position = GetPositionInControl(controlToManage);
            var middleTop = position.Y + controlToManage.Height / 2;

            return new Point(position.X + controlToManage.Width, middleTop);
        }
    }
}
