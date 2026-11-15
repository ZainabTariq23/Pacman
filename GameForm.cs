using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pacman_Game
{
    public partial class GameForm : Form
    {
        SoundPlayer gameMusic = new SoundPlayer();
        Random rnd = new Random();
        int speed = 8;
        int normalSpeed = 8;
        bool mouthOpen = true;
        int redSpeed = 4;
        int yellowSpeed = 3;
        int cyanSpeed = 5;
        int greenSpeed = 4;




        int score = 0;
        int lives = 3;

        bool goLeft;
        bool goRight;
        bool goUp;
        bool goDown;

      
        public GameForm()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.DoubleBuffered = true;
            this.SetStyle(
             ControlStyles.AllPaintingInWmPaint |
             ControlStyles.UserPaint |
             ControlStyles.DoubleBuffer,
             true);
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            lblScore.Text = "Score: " + score;

            lblLives.Text = "Lives: " + lives;

            MovePacman();
            MoveGhosts();
            CheckCollisions();
            CheckWin();
           

        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            gameMusic.SoundLocation = "pacmanmusic.wav";
            gameMusic.Load();
            gameMusic.PlayLooping();

            ghostRed.BringToFront();
            ghostYellow.BringToFront();
            ghostCyan.BringToFront();
            ghostGreen.BringToFront();

            pacman.BringToFront();

        }
        private void MovePacman()
        {
            int moveX = 0;
            int moveY = 0;

            if (goLeft)
                moveX = -speed;

            if (goRight)
                moveX = speed;

            if (goUp)
                moveY = -speed;

            if (goDown)
                moveY = speed;

            // MOVE X
            pacman.Left += moveX;

            if (IsTouchingWall(pacman))
            {
                pacman.Left -= moveX;
            }

            // MOVE Y
            pacman.Top += moveY;

            if (IsTouchingWall(pacman))
            {
                pacman.Top -= moveY;
            }

            // TELEPORT LEFT
            if (pacman.Left < -30)
            {
                pacman.Left = this.ClientSize.Width;
            }

            // TELEPORT RIGHT
            if (pacman.Right > this.ClientSize.Width + 30)
            {
                pacman.Left = 0;
            }
        }
        private void MoveGhosts()
        {
            MoveGhostTowardsPacman(ghostRed, redSpeed);

            MoveGhostTowardsPacman(ghostYellow, yellowSpeed);

            MoveGhostTowardsPacman(ghostCyan, cyanSpeed);

            MoveGhostTowardsPacman(ghostGreen, greenSpeed);
        }
        private void MoveGhostTowardsPacman(PictureBox ghost, int ghostSpeed)
        {
            int oldX = ghost.Left;
            int oldY = ghost.Top;

            bool moved = false;

            // TRY X MOVEMENT FIRST
            if (ghost.Left < pacman.Left)
            {
                ghost.Left += ghostSpeed;

                if (IsTouchingWall(ghost))
                {
                    ghost.Left = oldX;
                }
                else
                {
                    moved = true;
                }
            }
            else if (ghost.Left > pacman.Left)
            {
                ghost.Left -= ghostSpeed;

                if (IsTouchingWall(ghost))
                {
                    ghost.Left = oldX;
                }
                else
                {
                    moved = true;
                }
            }

            // IF X FAILED, TRY Y
            if (!moved)
            {
                if (ghost.Top < pacman.Top)
                {
                    ghost.Top += ghostSpeed;

                    if (IsTouchingWall(ghost))
                    {
                        ghost.Top = oldY;
                    }
                }
                else if (ghost.Top > pacman.Top)
                {
                    ghost.Top -= ghostSpeed;

                    if (IsTouchingWall(ghost))
                    {
                        ghost.Top = oldY;
                    }
                }
            }
        }

        private bool IsTouchingWall(PictureBox obj)
        {
            foreach (Control x in this.Controls)
            {
                if (x is Panel && x.Tag != null && x.Tag.ToString() == "wall")
                {
                    if (obj.Bounds.IntersectsWith(x.Bounds))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void CheckCollisions()
        {
            foreach (Control x in Controls)
            {
               
                if (x is PictureBox && x.Tag != null && x.Tag.ToString() == "coin")
                {
                    if (pacman.Bounds.IntersectsWith(x.Bounds) && x.Visible)
                    {
                        x.Visible = false;
                        score += 10;
                    }
                }

                
                if (x is PictureBox && x.Tag != null && x.Tag.ToString() == "ghost")
                {
                    if (pacman.Bounds.IntersectsWith(x.Bounds))
                    {
                        lives--;

                        lblLives.Text = "Lives: " + lives;

                        pacman.Location = new Point(60, 80);


                        ghostRed.Location = new Point(120, 150);

                        ghostYellow.Location = new Point(700, 150);

                        ghostCyan.Location = new Point(120, 500);

                        ghostGreen.Location = new Point(700, 500);

                        gameTimer.Stop();

                        System.Windows.Forms.Timer hitTimer =
                          new System.Windows.Forms.Timer();

                        hitTimer.Interval = 500;

                        hitTimer.Tick += (s, ev) =>
                        {
                            gameTimer.Start();
                            hitTimer.Stop();
                        };

                        hitTimer.Start();

                        if (lives <= 0)
                        {
                            gameTimer.Stop();

                            gameMusic.Stop();

                            MessageBox.Show(
                                "GAME OVER\n\nFinal Score: " + score,
                                "Pac-Man Deluxe");

                            Application.Exit();
                        }
                    }
                }
            }
        }

        private void CheckWin()
        {
            bool coinsLeft = false;

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag == "coin" && x.Visible)
                {
                    coinsLeft = true;
                }
            }

            if (!coinsLeft)
            {
                gameTimer.Stop();

                MessageBox.Show(
                    "YOU WIN!\n\nFinal Score: " + score,
                    "Victory",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RestartGame();
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            gameMusic.Stop();

            DialogResult result = MessageBox.Show(
                "GAME OVER\n\nScore: " + score + "\n\nPlay Again?",
                "Pac‑Man Deluxe",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                RestartGame();
            }
            else
            {
                Application.Exit();
            }
        }
        private void RestartGame()
        {
            score = 0;
            lives = 3;

            pacman.Location = new Point(60, 80);

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag == "coin")
                {
                    x.Visible = true;
                }
            }
            ghostRed.Location = new Point(300, 200);

            ghostYellow.Location = new Point(600, 200);

            ghostCyan.Location = new Point(300, 450);

            ghostGreen.Location = new Point(600, 450);

            gameTimer.Start();
        }

        private void wall9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
            }

            if (e.KeyCode == Keys.Space)
            {
                speed = 14;
            }

        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            if (e.KeyCode == Keys.Space)
            {
                speed = normalSpeed;
            }
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameTimer.Stop();

            if (gameMusic != null)
            {
                gameMusic.Stop();
                gameMusic.Dispose();
            }

            Application.Exit();
        }

        private void coin20_Click(object sender, EventArgs e)
        {

        }

        private void wall4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void wall56_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ghostYellow_Click(object sender, EventArgs e)
        {

        }

        private void ghostCyan_Click(object sender, EventArgs e)
        {

        }
    }

}
