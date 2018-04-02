using System.Drawing;
using System.Windows.Forms;

namespace ClientSide.View.Grips
{
    class MoveGrip: DragGripBase
    {
        public MoveGrip(Control controlToManage) : base(controlToManage)
        {
            Cursor = Cursors.SizeAll;
        }

        protected override void DoModify(Control controlToManage, Point original, Point current)
        {
            controlToManage.Left += current.X - original.X;
            controlToManage.Top += current.Y - original.Y;
        }

        protected override Point GetGripPlacement(Control controlToManage)
        {
            var position = GetPositionInControl(controlToManage);
            var middleLeft = position.X + controlToManage.Width / 2;
            var middleTop = position.Y + controlToManage.Height / 2;

            return new Point(middleLeft, middleTop);
        }
    }
}
