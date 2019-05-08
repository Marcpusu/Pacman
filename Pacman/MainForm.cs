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

        const int iDefaultExternalWallWidth = 10;
        const int iDefaultWallWidth = 25;

        const int iDefaultCoinSize = 6;

        bool bGameReady = false;
        bool bGameStarted = false;

        Direction oDirection;
        Direction oPreviousDirection;
        Direction oDirectionPressed;
        Direction oPreviousGif;

        Timer timer = new Timer();
        List<Rectangle> lstWalls = new List<Rectangle>();
        List<Rectangle> lstCoins = new List<Rectangle>();
        List<Rectangle> lstCollectedCoins = new List<Rectangle>();
        List<Rectangle> lstBigCoins = new List<Rectangle>();

        Panel pnlGame = new Panel();
        PictureBox pbPacman = new PictureBox();

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

            InitializeGame();
        }

        private void InitializeGame()
        {
            #region Design

            pnlGame.Size = new Size((iDefaultWallWidth * 17) + (iDefaultExternalWallWidth * 2), (iDefaultWallWidth * 19) + (iDefaultExternalWallWidth * 2));
            pnlGame.Location = new Point((this.ClientSize.Width / 2) - (pnlGame.Size.Width / 2), (this.ClientSize.Height / 2) - (pnlGame.Size.Height / 2));
            pnlGame.Anchor = AnchorStyles.None;
            pnlGame.Name = "pnlGame";
            pnlGame.TabIndex = 1;
            pnlGame.Paint += new PaintEventHandler(pnlGame_Paint);
            pnlGame.BorderStyle = BorderStyle.FixedSingle;

            this.Controls.Add(pnlGame);

            pnlGame.Refresh();

            pbPacman.Name = "pbPacman";
            pbPacman.Size = new Size(25, 25);
            pbPacman.Location = new Point(384, 343);

            pbPacman.SizeMode = PictureBoxSizeMode.StretchImage;
            pbPacman.Image = Properties.Resources.Pacman_Right;
            oPreviousGif = Direction.Right;

            pnlGame.Controls.Add(pbPacman);

            #endregion

            pnlStart.Visible = false;

            Application.DoEvents();

            timer.Interval = iSpeed;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (bGameReady)
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

                bGameStarted = true;
            }
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

                    if (oPreviousGif != Direction.Up)
                    {
                        oPreviousGif = Direction.Up;
                        pbPacman.Image = Properties.Resources.Pacman_Top;
                    }
                        
                    pbPacman.Top -= iDistance;
                    break;
                case Direction.Down:
                    if (oPreviousDirection == Direction.Up)
                        oPreviousDirection = Direction.None;

                    if (oPreviousGif != Direction.Down)
                    {
                        oPreviousGif = Direction.Down;
                        pbPacman.Image = Properties.Resources.Pacman_Bottom;
                    }

                    pbPacman.Top += +iDistance;
                    break;
                case Direction.Left:
                    if (oPreviousDirection == Direction.Right)
                        oPreviousDirection = Direction.None;

                    if (oPreviousGif != Direction.Left)
                    {
                        oPreviousGif = Direction.Left;
                        pbPacman.Image = Properties.Resources.Pacman_Left;
                    }

                    pbPacman.Left -= iDistance;
                    break;
                case Direction.Right:
                    if (oPreviousDirection == Direction.Left)
                        oPreviousDirection = Direction.None;

                    if (oPreviousGif != Direction.Right)
                    {
                        oPreviousGif = Direction.Right;
                        pbPacman.Image = Properties.Resources.Pacman_Right;
                    }

                    pbPacman.Left += iDistance;
                    break;
                default:
                    break;
            }

            foreach (Rectangle coin in lstCoins)
            {
                if (pbPacman.Bounds.IntersectsWith(coin) && !lstCollectedCoins.Exists(x => x.Location == coin.Location && x.Size == coin.Size))
                {
                    lstCollectedCoins.Add(coin);
                }
            }

            if (!pbPacman.Bounds.IntersectsWith(new Rectangle(new Point(0, 0), pnlGame.Size)))
            {
                if (oDirection == Direction.Right)
                    pbPacman.Left = 0;
                else if (oDirection == Direction.Left)
                    pbPacman.Left = pnlGame.Size.Width - pbPacman.Width;
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

            foreach (Rectangle x in lstWalls)
            {
                if (r.IntersectsWith(x))
                {
                    bBlocked = true;
                    break;
                }
            }

            return bBlocked;
        }

        private void pnlGame_Paint(object sender, PaintEventArgs e)
        {
            #region Draw Walls

            Rectangle r = new Rectangle(new Point(0, 0), new Size(pnlGame.Size.Width - 4, iDefaultExternalWallWidth));
            lstWalls.Add(r);
            e.Graphics.DrawRectangle(Pens.MediumBlue, r);

            r = new Rectangle(new Point(0, pnlGame.Size.Height - iDefaultExternalWallWidth - 4), new Size(pnlGame.Size.Width, iDefaultExternalWallWidth));
            lstWalls.Add(r);
            e.Graphics.DrawRectangle(Pens.MediumBlue, r);

            r = new Rectangle(new Point(0, 0), new Size(iDefaultExternalWallWidth, pnlGame.Height / 3));
            lstWalls.Add(r);
            e.Graphics.DrawRectangle(Pens.MediumBlue, r);

            r = new Rectangle(new Point(0, pnlGame.Size.Height - (pnlGame.Height / 3)), new Size(iDefaultExternalWallWidth, pnlGame.Height / 3));
            lstWalls.Add(r);
            e.Graphics.DrawRectangle(Pens.MediumBlue, r);

            r = new Rectangle(new Point(pnlGame.Size.Width - iDefaultExternalWallWidth - 4, 0), new Size(iDefaultExternalWallWidth, pnlGame.Height / 3));
            lstWalls.Add(r);
            e.Graphics.DrawRectangle(Pens.MediumBlue, r);

            r = new Rectangle(new Point(pnlGame.Size.Width - iDefaultExternalWallWidth - 4 , pnlGame.Size.Height - (pnlGame.Height / 3)), new Size(iDefaultExternalWallWidth, pnlGame.Height / 3));
            lstWalls.Add(r);
            e.Graphics.DrawRectangle(Pens.MediumBlue, r);

            #endregion

            if (!bGameStarted)
            {
                #region Draw Coins

                r = new Rectangle(new Point(pnlGame.Size.Width / 2, pnlGame.Height / 2), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(Pens.Yellow, r);
                e.Graphics.FillEllipse(Brushes.Yellow, r);

                #endregion
            }
            else
            {
                #region Undraw Coins

                foreach (Rectangle coin in lstCollectedCoins)
                {
                    e.Graphics.DrawEllipse(Pens.Black, coin);
                    e.Graphics.FillEllipse(Brushes.Black, coin);
                }

                #endregion
            }

            bGameReady = true;
        }
    }
}
