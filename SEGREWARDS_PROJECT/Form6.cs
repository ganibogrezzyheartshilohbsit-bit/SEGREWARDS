using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SEGREWARDS_PROJECT.Composition;
using SEGREWARDS_PROJECT.Infrastructure;

namespace SEGREWARDS_PROJECT
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
            pictureBox7.Cursor = Cursors.Hand;
            pictureBox7.Click += PictureBox7_Click;
            editPicture.Click += PictureBox7_Click;
            profilePicture.Cursor = Cursors.Hand;
            profilePicture.Click += PictureBox7_Click;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            RefreshHeader();
            RenderMockSummary();
        }

        private void RefreshHeader()
        {
            if (AppSession.CurrentUserId == null)
            {
                return;
            }

            try
            {
                var user = AppCompositionRoot.Instance.Users.GetById(AppSession.CurrentUserId.Value);
                if (user != null)
                {
                    AppSession.CurrentStudentNumber = user.StudentNumber;
                    AppSession.CurrentFullName = user.FullName;
                    AppSession.CurrentEcoPoints = user.EcoPointsBalance;

                    studentName.Text = string.IsNullOrWhiteSpace(user.FullName) ? "Student" : user.FullName;
                    label9.Text = user.EcoPointsBalance.ToString("N0");
                    studentId.Text = user.StudentNumber;
                    studentEmail.Text = string.IsNullOrWhiteSpace(user.Email) ? "—" : user.Email;
                }
            }
            catch
            {
                // Optional if DB is offline.
            }

            try
            {
                pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                var profile = AppSession.TryLoadProfileImage(AppSession.CurrentStudentNumber);
                if (profile != null)
                {
                    var old = pictureBox7.Image;
                    pictureBox7.BackgroundImage = null;
                    pictureBox7.Image = profile;
                    old?.Dispose();
                }
            }
            catch
            {
                // Ignore profile image failures.
            }

            try
            {
                profilePicture.SizeMode = PictureBoxSizeMode.Zoom;
                var profile = AppSession.TryLoadProfileImage(AppSession.CurrentStudentNumber);
                if (profile != null)
                {
                    var old = profilePicture.Image;
                    profilePicture.BackgroundImage = null;
                    profilePicture.Image = profile;
                    old?.Dispose();
                }
            }
            catch
            {
                // Ignore profile image failures.
            }
        }

        private void PictureBox7_Click(object sender, EventArgs e)
        {
            // Click profile icon to change profile picture (and persist it).
            try
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Title = "Select profile picture";
                    dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp|All Files|*.*";
                    dialog.Multiselect = false;

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                        return;

                    Image newImage;
                    using (var fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var temp = Image.FromStream(fs))
                    {
                        newImage = new Bitmap(temp);
                    }

                    var old = pictureBox7.Image;
                    pictureBox7.BackgroundImage = null;
                    pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox7.Image = newImage;
                    old?.Dispose();

                    if (!string.IsNullOrWhiteSpace(AppSession.CurrentStudentNumber))
                    {
                        AppSession.SaveProfileImage(AppSession.CurrentStudentNumber, newImage);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Profile picture", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RenderMockSummary()
        {
            // Simple mock summary data for now (replace later with real DB queries).
            // Keep the title label2, clear the rest.
            for (var i = panel3.Controls.Count - 1; i >= 0; i--)
            {
                var c = panel3.Controls[i];
                if (!ReferenceEquals(c, label2))
                {
                    panel3.Controls.RemoveAt(i);
                    c.Dispose();
                }
            }

            var subtitle = new Label
            {
                AutoSize = true,
                Text = "Mock data (replace with live stats later)",
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular),
                ForeColor = Color.DimGray,
                Location = new Point(label2.Left, label2.Bottom + 10)
            };

            var a = new Label
            {
                AutoSize = true,
                Text = "Total submissions: 12",
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Location = new Point(label2.Left, subtitle.Bottom + 22)
            };

            var b = new Label
            {
                AutoSize = true,
                Text = "Approved submissions: 9",
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Location = new Point(label2.Left, a.Bottom + 14)
            };

            var c1 = new Label
            {
                AutoSize = true,
                Text = "Rewards redeemed: 3",
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Location = new Point(label2.Left, b.Bottom + 14)
            };

            var d = new Label
            {
                AutoSize = true,
                Text = "Current streak: 5 days",
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Location = new Point(label2.Left, c1.Bottom + 14)
            };

            panel3.Controls.Add(subtitle);
            panel3.Controls.Add(a);
            panel3.Controls.Add(b);
            panel3.Controls.Add(c1);
            panel3.Controls.Add(d);
        }
    }
}
