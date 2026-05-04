using System;
using System.Windows.Forms;
using SEGREWARDS_PROJECT.Composition;

namespace SEGREWARDS_PROJECT
{
    public partial class Form2 : Form
    {
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
                var provisionalFullName = string.IsNullOrWhiteSpace(studentNumber)
                    ? "New student"
                    : "Student " + studentNumber;

                var result = AppCompositionRoot.Instance.Auth.Register(
                    studentNumber,
                    string.IsNullOrWhiteSpace(email) ? null : email,
                    password,
                    provisionalFullName);

                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
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
    }
}
