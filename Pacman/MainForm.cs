using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pacman
{
    public partial class MainForm : Form
    {
        #region Global Vars
        const int iSpeed = 25;
        const int iDistance = 2;

        Direction oDirection;
        Direction oPreviousDirection;
        Direction oDirectionPressed;

        Timer timer = new Timer();
        List<Panel> lstWalls = new List<Panel>();

        private enum Direction
        {
            None = 0,
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4
        }
        #endregion

        public MainForm()
        {
            InitializeComponent();

            timer.Interval = iSpeed;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            oPreviousDirection = oDirection;

            if (e.KeyCode == Keys.Up)
                oDirectionPressed = Direction.Up;
            else if (e.KeyCode == Keys.Down)
                oDirectionPressed = Direction.Down;
            else if (e.KeyCode == Keys.Left)
                oDirectionPressed = Direction.Left;
            else if (e.KeyCode == Keys.Right)
                oDirectionPressed = Direction.Right;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            oDirection = oPreviousDirection;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            oDirection = oDirectionPressed;

            if (Blocked(oDirection))
            {
                oDirection = oPreviousDirection;
                if (Blocked(oPreviousDirection))
                    return;
            }

            switch (oDirection)
            {
                case Direction.Up:
                    if (oPreviousDirection == Direction.Down)
                        oPreviousDirection = Direction.None;
                    pbPacman.Top -= iDistance;
                    break;
                case Direction.Down:
                    if (oPreviousDirection == Direction.Up)
                        oPreviousDirection = Direction.None;
                    pbPacman.Top += +iDistance;
                    break;
                case Direction.Left:
                    if (oPreviousDirection == Direction.Right)
                        oPreviousDirection = Direction.None;
                    pbPacman.Left -= iDistance;
                    break;
                case Direction.Right:
                    if (oPreviousDirection == Direction.Left)
                        oPreviousDirection = Direction.None;
                    pbPacman.Left += iDistance;
                    break;
                default:
                    break;
            }
        }

        private bool Blocked(Direction direction)
        {
            bool bBlocked = false;

            Rectangle r = new Rectangle(new Point(pbPacman.Location.X, pbPacman.Location.Y), pbPacman.Size);

            switch (direction)
            {
                case Direction.Up:
                    r.Location = new Point(pbPacman.Location.X, pbPacman.Location.Y - iDistance);
                    break;
                case Direction.Down:
                    r.Location = new Point(pbPacman.Location.X, pbPacman.Location.Y + iDistance);
                    break;
                case Direction.Left:
                    r.Location = new Point(pbPacman.Location.X - iDistance, pbPacman.Location.Y);
                    break;
                case Direction.Right:
                    r.Location = new Point(pbPacman.Location.X + iDistance, pbPacman.Location.Y);
                    break;
                default:
                    break;
            }

            foreach (Panel x in lstWalls)
            {
                if (r.IntersectsWith(x.Bounds))
                {
                    bBlocked = true;
                    break;
                }
            }

            return bBlocked;
        }
    }
}
