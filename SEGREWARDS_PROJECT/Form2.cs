using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SEGREWARDS_PROJECT.Composition;
using SEGREWARDS_PROJECT.Infrastructure;

namespace SEGREWARDS_PROJECT
{
    public partial class Form2 : Form
    {
        private Image _pendingProfileImage;

        public Form2()
        {
            InitializeComponent();
            buttonsignup.Click += Buttonsignup_Click;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Buttonsignup_Click(object sender, EventArgs e)
        {
            try
            {
                var studentNumber = txtbox_studentnum.Text.Trim();
                var password = txtbox_password.Text;
                var email = txtbox_emaiill.Text.Trim();
                var fullName = nameBox.Text.Trim();

                var result = AppCompositionRoot.Instance.Auth.Register(
                    studentNumber,
                    string.IsNullOrWhiteSpace(email) ? null : email,
                    password,
                    fullName);

                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Persist selected profile image for this student (if any).
                if (_pendingProfileImage != null && !string.IsNullOrWhiteSpace(studentNumber))
                {
                    AppSession.SaveProfileImage(studentNumber, _pendingProfileImage);
                }

                MessageBox.Show(
                    "Account created. You can sign in with your student number and password.",
                    "Sign Up",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pic_logo_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void pic_pass_Click(object sender, EventArgs e)
        {

        }

        private void buttonsignup_Click_1(object sender, EventArgs e)
        {

        }

        private void studentProfile_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Title = "Select profile picture";
                    dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp|All Files|*.*";
                    dialog.Multiselect = false;

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                        return;

                    // Load a copy of the image so the selected file isn't locked.
                    Image newImage;
                    using (var fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var temp = Image.FromStream(fs))
                    {
                        newImage = new Bitmap(temp);
                    }

                    var old = studentProfile.Image;
                    studentProfile.SizeMode = PictureBoxSizeMode.Zoom;
                    studentProfile.Image = newImage;
                    if (old != null && !ReferenceEquals(old, Properties.Resources.user__1_))
                    {
                        old.Dispose();
                    }

                    _pendingProfileImage?.Dispose();
                    _pendingProfileImage = new Bitmap(newImage);

                    // If student number is already filled in, persist immediately so Form4 can load it later.
                    var sn = (txtbox_studentnum.Text ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(sn))
                    {
                        AppSession.SaveProfileImage(sn, _pendingProfileImage);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Profile picture", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
