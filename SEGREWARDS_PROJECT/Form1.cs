using System;
using System.Windows.Forms;
using SEGREWARDS_PROJECT.Composition;
using SEGREWARDS_PROJECT.Infrastructure;

namespace SEGREWARDS_PROJECT
{
    public partial class LOGIN : Form
    {
        public LOGIN()
        {
            InitializeComponent();
            btnSave.Click += BtnSave_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Hide();
            using (var signUp = new Form2())
            {
                signUp.ShowDialog(this);
            }

            Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var result = AppCompositionRoot.Instance.Auth.Login(textBox1.Text, textBox3.Text);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Log In", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AppSession.CurrentUserId = result.User.Id;
                AppSession.CurrentStudentNumber = result.User.StudentNumber;
                AppSession.CurrentFullName = result.User.FullName;
                AppSession.CurrentEcoPoints = result.User.EcoPointsBalance;

                Hide();
                using (var landing = new Form3())
                {
                    landing.ShowDialog(this);
                }

                Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not reach the database. Check App.config connection string and that MySQL is running.\r\n\r\n" + ex.Message,
                    "Connection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.PasswordChar = '*';
            textBox3.UseSystemPasswordChar = true;
        }
    }
}
