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

        const int iPacmanWidth = 22;

        const int iDefaultExternalWallWidth = 6;
        const int iDefaultWallWidth = 12;
        const int iDefaultPacmanWallSpace = iPacmanWidth + 2;

        const int iDefaultCoinSize = 4;
        const int iDefaultBigCoinSize = 10;
        const int iDefaultCoinSeparation = 8;

        Pen oDefaultWallColor = Pens.MediumBlue;
        Pen oDefaultCoinColor = Pens.LightYellow;
        Brush oDefaultCoinFillColor = Brushes.LightYellow;
        Pen oDefaultBackColor = Pens.Black;
        Brush oDefaultBackFillColor = Brushes.Black;

        #region Scores
        const int iDefaultCoinScore = 10;
        const int iDefaultBigCoinScore = 50;
        #endregion

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

        Panel pnlStart = new Panel();
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

            StartPanel();

            InitializeGame();
        }

        private void StartPanel()
        {
            Size sz = new Size((iDefaultWallWidth * 11) + (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 8) + 1, (iDefaultWallWidth * 10) + (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 10) + 50 + 1);
            this.ClientSize = sz;

            pnlStart.Size = sz;
            pnlStart.Anchor = AnchorStyles.None;
            pnlStart.Name = "pnlStart";
            pnlStart.TabIndex = 1;

            Label lblHighScoreText = new Label();
            lblHighScoreText.AutoSize = true;
            lblHighScoreText.Name = "lblHighScoreText";
            lblHighScoreText.Text = "HIGH SCORE";
            lblHighScoreText.ForeColor = Color.White;
            lblHighScoreText.Font = new Font("Press Start 2P", 8, FontStyle.Regular);
            lblHighScoreText.TextAlign = ContentAlignment.MiddleCenter;
            
            Label lblHighScoreValue = new Label();
            lblHighScoreValue.AutoSize = true;
            lblHighScoreValue.Name = "lblHighScoreValue";
            lblHighScoreValue.Text = "000000";
            lblHighScoreValue.ForeColor = Color.White;
            lblHighScoreValue.Font = new Font("Press Start 2P", 8, FontStyle.Regular);
            lblHighScoreValue.TextAlign = ContentAlignment.MiddleCenter;

            Label lblCurrentScoreText = new Label();
            lblCurrentScoreText.AutoSize = true;
            lblCurrentScoreText.Name = "lblCurrentScoreText";
            lblCurrentScoreText.Text = "1UP";
            lblCurrentScoreText.ForeColor = Color.White;
            lblCurrentScoreText.Font = new Font("Press Start 2P", 8, FontStyle.Regular);
            lblCurrentScoreText.TextAlign = ContentAlignment.MiddleCenter;

            Label lblCurrentScoreValue = new Label();
            lblCurrentScoreValue.AutoSize = true;
            lblCurrentScoreValue.Name = "lblCurrentScoreValue";
            lblCurrentScoreValue.Text = "000000";
            lblCurrentScoreValue.ForeColor = Color.White;
            lblCurrentScoreValue.Font = new Font("Press Start 2P", 8, FontStyle.Regular);
            lblCurrentScoreValue.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(pnlStart);

            pnlStart.Controls.Add(lblHighScoreText);
            pnlStart.Controls.Add(lblHighScoreValue);
            pnlStart.Controls.Add(lblCurrentScoreText);
            pnlStart.Controls.Add(lblCurrentScoreValue);

            Application.DoEvents();

            pnlStart.Location = new Point((this.ClientSize.Width / 2) - (pnlStart.Size.Width / 2), (this.ClientSize.Height / 2) - (pnlStart.Size.Height / 2));
            lblHighScoreText.Location = new Point((pnlStart.Size.Width / 2) - (lblHighScoreText.Size.Width / 2), 2);
            lblHighScoreValue.Location = new Point(lblHighScoreText.Location.X + (lblHighScoreText.Size.Width / 2) - (lblHighScoreValue.Size.Width / 2), lblHighScoreText.Location.Y + lblHighScoreText.Size.Height);
            lblCurrentScoreValue.Location = new Point((pnlStart.Size.Width / 5) - (lblCurrentScoreValue.Size.Width / 2), 2 + lblCurrentScoreText.Size.Height);
            lblCurrentScoreText.Location = new Point(lblCurrentScoreValue.Location.X + (lblCurrentScoreValue.Size.Width / 2) - (lblCurrentScoreText.Size.Width / 2), 2);
        }

        private void InitializeGame()
        {
            #region Design

            pnlGame.Size = new Size((iDefaultWallWidth * 11) + (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 8) + 1, (iDefaultWallWidth * 10) + (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 10) + 1);
            pnlGame.Location = new Point((pnlStart.Size.Width / 2) - (pnlGame.Size.Width / 2), ((pnlStart.Size.Height + 50) / 2) - (pnlGame.Size.Height / 2));
            pnlGame.Anchor = AnchorStyles.None;
            pnlGame.Name = "pnlGame";
            pnlGame.TabIndex = 2;
            pnlGame.Paint += new PaintEventHandler(pnlGame_Paint);

            pnlStart.Controls.Add(pnlGame);

            pnlGame.Refresh();

            pbPacman.Name = "pbPacman";
            pbPacman.Size = new Size(iPacmanWidth, iPacmanWidth);
            pbPacman.Location = new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 4) - (iDefaultWallWidth / 2) - (iDefaultPacmanWallSpace / 2) + 1 , iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8) + 1);
            pbPacman.SizeMode = PictureBoxSizeMode.StretchImage;
            pbPacman.Image = Properties.Resources.Pacman_Right;

            oPreviousGif = Direction.Right;

            pnlGame.Controls.Add(pbPacman);

            #endregion

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
                if (pbPacman.Bounds.Contains(coin) && !lstCollectedCoins.Exists(x => x.Location == coin.Location && x.Size == coin.Size))
                {
                    lstCollectedCoins.Add(coin);
                    UpdateScore();
                }
            }

            if (!pbPacman.Bounds.IntersectsWith(new Rectangle(new Point(0, 0), pnlGame.Size)))
            {
                if (oDirection == Direction.Right)
                    pbPacman.Left = 0;
                else if (oDirection == Direction.Left)
                    pbPacman.Left = pnlGame.Size.Width - pbPacman.Width;
            }

            if (lstCoins.Count == lstCollectedCoins.Count)
                FinishGame();
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
                Rectangle rec = new Rectangle(new Point(x.Location.X, x.Location.Y), new Size(x.Size.Width + 1, x.Size.Height + 1));
                if (r.IntersectsWith(rec))
                {
                    bBlocked = true;
                    break;
                }
            }

            return bBlocked;
        }

        private void pnlGame_Paint(object sender, PaintEventArgs e)
        {
            if (!bGameStarted)
            {
                #region Draw Walls

                #region External Walls

                Rectangle r = new Rectangle(new Point(0, 0), new Size(pnlGame.Size.Width - 1, iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(0, pnlGame.Size.Height - iDefaultExternalWallWidth - 1), new Size(pnlGame.Size.Width, iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(0, 0), new Size(iDefaultExternalWallWidth, (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 3)));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(pnlGame.Size.Width - iDefaultExternalWallWidth - 1, 0), new Size(iDefaultExternalWallWidth, (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 3)));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(0, iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 3)), new Size(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 3), iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 3)), new Size(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 3), iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultPacmanWallSpace + (iDefaultWallWidth * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 3)), new Size(iDefaultExternalWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 3)), new Size(iDefaultExternalWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(-1, (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 5)), new Size(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 3) + 1, iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 5)), new Size(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 3) + 1, iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(-1, iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 5)), new Size(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 3) + 1, iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 5)), new Size(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 3) + 1, iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultPacmanWallSpace + (iDefaultWallWidth * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 5)), new Size(iDefaultExternalWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 5)), new Size(iDefaultExternalWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(0, (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 7)), new Size(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 3), iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 7)), new Size(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 3), iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(0, (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 7)), new Size(iDefaultExternalWallWidth, (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 3)));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 8) + (iDefaultWallWidth * 11), (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 7)), new Size(iDefaultExternalWallWidth, (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 3)));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(0, iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 8) + (iDefaultWallWidth * 8)), new Size(iDefaultExternalWallWidth + (iDefaultWallWidth * 2), iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 8) + (iDefaultWallWidth * 9), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 8) + (iDefaultWallWidth * 8)), new Size(iDefaultExternalWallWidth + (iDefaultWallWidth * 2), iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                #endregion

                #region Internal Walls

                r = new Rectangle(new Point(iDefaultExternalWallWidth + iDefaultPacmanWallSpace, iDefaultExternalWallWidth + iDefaultPacmanWallSpace), new Size(iDefaultWallWidth * 3, iDefaultWallWidth * 2));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 3), iDefaultExternalWallWidth + iDefaultPacmanWallSpace), new Size((iDefaultWallWidth * 2) + iDefaultPacmanWallSpace, iDefaultWallWidth * 2));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 5), 0), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace + iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 6), iDefaultExternalWallWidth + iDefaultPacmanWallSpace), new Size((iDefaultWallWidth * 2) + iDefaultPacmanWallSpace, iDefaultWallWidth * 2));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), iDefaultExternalWallWidth + iDefaultPacmanWallSpace), new Size(iDefaultWallWidth * 3, iDefaultWallWidth * 2));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + iDefaultPacmanWallSpace, iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + iDefaultWallWidth * 2), new Size(iDefaultWallWidth * 3, iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 2)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 3) + (iDefaultPacmanWallSpace * 2)));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 4), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 2)), new Size((iDefaultWallWidth * 3) + (iDefaultPacmanWallSpace * 2), iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 2)), new Size(iDefaultWallWidth , (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 7), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 2)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 3) + (iDefaultPacmanWallSpace * 2)));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 2)), new Size(iDefaultWallWidth * 3, iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 3)), new Size((iDefaultWallWidth * 2) + iDefaultPacmanWallSpace, iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 6), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 3)), new Size((iDefaultWallWidth * 2) + iDefaultPacmanWallSpace, iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 5)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 4), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 6)), new Size((iDefaultWallWidth * 3) + (iDefaultPacmanWallSpace * 2), iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 6)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 7), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 5)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + iDefaultPacmanWallSpace, iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 7)), new Size((iDefaultWallWidth * 3), iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + iDefaultPacmanWallSpace + (iDefaultWallWidth * 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 7)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 7)), new Size((iDefaultWallWidth * 2) + iDefaultPacmanWallSpace, iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 6), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 7)), new Size((iDefaultWallWidth * 2) + iDefaultPacmanWallSpace, iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 7)), new Size(iDefaultWallWidth * 3, iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 7) + (iDefaultWallWidth * 7)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 2) + (iDefaultWallWidth * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 8) + (iDefaultWallWidth * 8)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 4), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 8) + (iDefaultWallWidth * 8)), new Size((iDefaultWallWidth * 3) + (iDefaultPacmanWallSpace * 2), iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 8) + (iDefaultWallWidth * 8)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 6) + (iDefaultWallWidth * 7), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 8) + (iDefaultWallWidth * 8)), new Size(iDefaultWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + iDefaultPacmanWallSpace, iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 9) + (iDefaultWallWidth * 9)), new Size((iDefaultWallWidth * 5) + (iDefaultPacmanWallSpace * 2), iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 6), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 9) + (iDefaultWallWidth * 9)), new Size((iDefaultWallWidth * 5) + (iDefaultPacmanWallSpace * 2), iDefaultWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                #endregion

                #region Ghost Walls

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 4), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 4)), new Size((iDefaultWallWidth * 3) + (iDefaultPacmanWallSpace * 2), iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 4), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 4)), new Size(iDefaultExternalWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point((iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 6), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 4) + (iDefaultWallWidth * 4)), new Size(iDefaultExternalWallWidth, (iDefaultWallWidth * 2) + iDefaultPacmanWallSpace));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace * 3) + (iDefaultWallWidth * 4), (iDefaultExternalWallWidth * 2) + (iDefaultPacmanWallSpace * 5) + (iDefaultWallWidth * 5)), new Size((iDefaultWallWidth * 3) + (iDefaultPacmanWallSpace * 2), iDefaultExternalWallWidth));
                lstWalls.Add(r);
                e.Graphics.DrawRectangle(oDefaultWallColor, r);

                #endregion

                #endregion
            }

            if (!bGameStarted)
            {
                #region Draw Coins

                Rectangle r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + iDefaultCoinSize + iDefaultCoinSeparation, iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 2) + (iDefaultCoinSeparation * 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 3) + (iDefaultCoinSeparation * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 6) + (iDefaultCoinSeparation * 6), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 7) + (iDefaultCoinSeparation * 7), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 8) + (iDefaultCoinSeparation * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 9) + (iDefaultCoinSeparation * 9), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 10) + (iDefaultCoinSeparation * 10), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 11) + (iDefaultCoinSeparation * 11), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 14) + (iDefaultCoinSeparation * 14), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 15) + (iDefaultCoinSeparation * 15), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 16) + (iDefaultCoinSeparation * 16), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 17) + (iDefaultCoinSeparation * 17), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 18) + (iDefaultCoinSeparation * 18), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 19) + (iDefaultCoinSeparation * 19), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 20) + (iDefaultCoinSeparation * 20), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 21) + (iDefaultCoinSeparation * 21), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 22) + (iDefaultCoinSeparation * 22), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 23) + (iDefaultCoinSeparation * 23), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 24) + (iDefaultCoinSeparation * 24), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + iDefaultCoinSize + iDefaultCoinSeparation), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 3) + (iDefaultCoinSeparation * 3)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 6) + (iDefaultCoinSeparation * 6)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 7) + (iDefaultCoinSeparation * 7)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + iDefaultCoinSize + iDefaultCoinSeparation, iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 2) + (iDefaultCoinSeparation * 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 3) + (iDefaultCoinSeparation * 3), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 6) + (iDefaultCoinSeparation * 6), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 7) + (iDefaultCoinSeparation * 7), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 8) + (iDefaultCoinSeparation * 8), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 9) + (iDefaultCoinSeparation * 9), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 10) + (iDefaultCoinSeparation * 10), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 11) + (iDefaultCoinSeparation * 11), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 12) + (iDefaultCoinSeparation * 12), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 13) + (iDefaultCoinSeparation * 13), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 14) + (iDefaultCoinSeparation * 14), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 15) + (iDefaultCoinSeparation * 15), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 16) + (iDefaultCoinSeparation * 16), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 17) + (iDefaultCoinSeparation * 17), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 18) + (iDefaultCoinSeparation * 18), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 19) + (iDefaultCoinSeparation * 19), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 20) + (iDefaultCoinSeparation * 20), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 21) + (iDefaultCoinSeparation * 21), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 22) + (iDefaultCoinSeparation * 22), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 23) + (iDefaultCoinSeparation * 23), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 24) + (iDefaultCoinSeparation * 24), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 4) + (iDefaultCoinSeparation * 4)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + iDefaultCoinSize + iDefaultCoinSeparation), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 3) + (iDefaultCoinSeparation * 3)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 6) + (iDefaultCoinSeparation * 6)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 7) + (iDefaultCoinSeparation * 7)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + iDefaultCoinSize + iDefaultCoinSeparation), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 2) + (iDefaultCoinSeparation * 2)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 3) + (iDefaultCoinSeparation * 3)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 6) + (iDefaultCoinSeparation * 6)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 7) + (iDefaultCoinSeparation * 7)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 8) + (iDefaultCoinSeparation * 8)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 9) + (iDefaultCoinSeparation * 9)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 10) + (iDefaultCoinSeparation * 10)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 11) + (iDefaultCoinSeparation * 11)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 12) + (iDefaultCoinSeparation * 12)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 13) + (iDefaultCoinSeparation * 13)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 14) + (iDefaultCoinSeparation * 14)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 15) + (iDefaultCoinSeparation * 15)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 16) + (iDefaultCoinSeparation * 16)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 17) + (iDefaultCoinSeparation * 17)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 18) + (iDefaultCoinSeparation * 18)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 19 + (iDefaultCoinSeparation * 19))), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 20) + (iDefaultCoinSeparation * 20)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 21) + (iDefaultCoinSeparation * 21)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 22) + (iDefaultCoinSeparation * 22)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 23) + (iDefaultCoinSeparation * 23)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 24) + (iDefaultCoinSeparation * 24)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 5) + (iDefaultCoinSeparation * 5), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25)), new Size(iDefaultCoinSize, iDefaultCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                #endregion

                #region Draw Big Coins

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultBigCoinSize / 2), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultBigCoinSize / 2) + (iDefaultCoinSize * 2) + (iDefaultCoinSeparation * 2)), new Size(iDefaultBigCoinSize, iDefaultBigCoinSize));
                lstBigCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                r = new Rectangle(new Point(iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultBigCoinSize / 2) + (iDefaultCoinSize * 25) + (iDefaultCoinSeparation * 25), iDefaultExternalWallWidth + (iDefaultPacmanWallSpace / 2) - (iDefaultBigCoinSize / 2) + (iDefaultCoinSize * 2) + (iDefaultCoinSeparation * 2)), new Size(iDefaultBigCoinSize, iDefaultBigCoinSize));
                lstCoins.Add(r);
                e.Graphics.DrawEllipse(oDefaultCoinColor, r);
                e.Graphics.FillEllipse(oDefaultCoinFillColor, r);

                #endregion
            }
            else
            {
                //#region Undraw Collected Coins

                //foreach (Rectangle coin in lstCollectedCoins)
                //{
                //    e.Graphics.DrawEllipse(oDefaultBackColor, coin);
                //    e.Graphics.FillEllipse(oDefaultBackFillColor, coin);
                //}

                //#endregion

                //#region Draw Not Collected Coins

                //lstCoins.Where(x => !lstCollectedCoins.Any(y => Equals(y, x))).ToList().ForEach(z =>
                //{
                //    e.Graphics.DrawEllipse(oDefaultCoinColor, z);
                //    e.Graphics.FillEllipse(oDefaultCoinFillColor, z);
                //});

                //#endregion
            }

            bGameReady = true;
        }

        private void UpdateScore()
        {
            Label lblScoreValue = (Label)this.Controls.Find("lblCurrentScoreValue", true)[0];
            lblScoreValue.Text = (Convert.ToInt32(lblScoreValue.Text) + iDefaultCoinScore).ToString("D6");

            Label lblHighScoreValue = (Label)this.Controls.Find("lblHighScoreValue", true)[0];

            if (Convert.ToInt32(lblScoreValue.Text) > Convert.ToInt32(lblHighScoreValue.Text))
                lblHighScoreValue.Text = lblScoreValue.Text;

        }

        private void FinishGame()
        {
            timer.Stop();
            MessageBox.Show("That was legitness!");
        }
    }
}
