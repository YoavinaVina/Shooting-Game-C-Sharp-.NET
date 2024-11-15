using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shoot_Out_Game_MOO_ICT
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up";
        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 3;
        Random randNum = new Random();
        int score;

        List<PictureBox> zombiesList = new List<PictureBox>();

        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                player.Image = Properties.Resources.dead;
                GameTimer.Stop();
            }

            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;

            if (goLeft && player.Left > 0)
            {
                player.Left -= speed;
            }
            if (goRight && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }
            if (goUp && player.Top > 40)
            {
                player.Top -= speed;
            }
            if (goDown && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        this.ammo += 5;
                    }
                }

                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 1;
                        if (facing == "up") { player.Image = Properties.Resources.uphit; }
                    }

                    // Call the function to move zombies and apply the separation logic.
                    MoveZombies((PictureBox)x);
                }

                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && (string)x.Tag == "zombie")
                    {
                        if (j.Bounds.IntersectsWith(x.Bounds))
                        {
                            score++;

                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();

                            zombiesList.Remove((PictureBox)x);
                            MakeZombies();
                        }
                    }
                }
            }
        }

        private void MoveZombies(PictureBox zombie)
        {
            if (zombie.Left > player.Left)
            {
                zombie.Left -= zombieSpeed;
                zombie.Image = Properties.Resources.zleft;
            }
            if (zombie.Left < player.Left)
            {
                zombie.Left += zombieSpeed;
                zombie.Image = Properties.Resources.zright;
            }
            if (zombie.Top > player.Top)
            {
                zombie.Top -= zombieSpeed;
                zombie.Image = Properties.Resources.zup;
            }
            if (zombie.Top < player.Top)
            {
                zombie.Top += zombieSpeed;
                zombie.Image = Properties.Resources.zdown;
            }

            // Check for collisions with other zombies
            foreach (PictureBox z in zombiesList)
            {
                if (z != zombie && z.Bounds.IntersectsWith(zombie.Bounds))
                {
                    // Separation force - push zombies apart slightly
                    if (zombie.Left < z.Left)
                    {
                        zombie.Left -= 10;  // Push current zombie to the left
                        z.Left += 10;  // Push the other zombie to the right
                    }
                    else
                    {
                        zombie.Left += 10;  // Push current zombie to the right
                        z.Left -= 10;  // Push the other zombie to the left
                    }

                    if (zombie.Top < z.Top)
                    {
                        zombie.Top -= 10;  // Push current zombie up
                        z.Top += 10;  // Push the other zombie down
                    }
                    else
                    {
                        zombie.Top += 10;  // Push current zombie down
                        z.Top -= 10;  // Push the other zombie up
                    }
                }
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver)
            {
                return;
            }

            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.down;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
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

            if (e.KeyCode == Keys.Space && ammo > 0 && !gameOver)
            {
                ammo--;
                ShootBullet(facing);

                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.KeyCode == Keys.Enter && gameOver == true) { RestartGame(); }
        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);

            shootBullet.MakeBullet(this);
        }

        private void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;
            zombie.Left = randNum.Next(0, 900);
            zombie.Top = randNum.Next(0, 800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            zombiesList.Add(zombie);

            this.Controls.Add(zombie);
            player.BringToFront();
        }

        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Tag = "ammo";
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.Left = randNum.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = randNum.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            this.Controls.Add(ammo);

            ammo.BringToFront();
            player.BringToFront();
        }

        private void RestartGame()
        {
            player.Image = Properties.Resources.up;

            foreach (PictureBox i in zombiesList)
            {
                this.Controls.Remove(i);
            }
            zombiesList.Clear();

            for (int i = 0; i < 3; i++)
            {
                MakeZombies();
            }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            ammo = 10;

            GameTimer.Start();
        }
    }
}
