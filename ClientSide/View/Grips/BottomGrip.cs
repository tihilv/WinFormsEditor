using System.Drawing;
using System.Windows.Forms;

namespace ClientSide.View.Grips
{
    class BottomGrip: DragGripBase
    {
        public BottomGrip(Control controlToManage) : base(controlToManage)
        {
            Cursor = Cursors.SizeNS;
        }

        protected override void DoModify(Control controlToManage, Point original, Point current)
        {
            controlToManage.Height += current.Y - original.Y;
        }

        protected override Point GetGripPlacement(Control controlToManage)
        {
            var position = GetPositionInControl(controlToManage);
            var middleLeft = position.X + controlToManage.Width / 2;

            return new Point(middleLeft, position.Y + controlToManage.Height);
        }
    }
}
