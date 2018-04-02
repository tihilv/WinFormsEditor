using System.Drawing;
using System.Windows.Forms;

namespace ClientSide.View.Grips
{
    class TopGrip: DragGripBase
    {
        public TopGrip(Control controlToManage) : base(controlToManage)
        {
            Cursor = Cursors.SizeNS;
        }

        protected override void DoModify(Control controlToManage, Point original, Point current)
        {
            var diff = current.Y - original.Y;
            controlToManage.Top += diff;
            controlToManage.Height -= diff;
        }

        protected override Point GetGripPlacement(Control controlToManage)
        {
            var position = GetPositionInControl(controlToManage);
            var middleLeft = position.X + controlToManage.Width / 2;

            return new Point(middleLeft, position.Y);
        }
    }
}
