using System;
using System.Drawing;
using System.Windows.Forms;

namespace SEGREWARDS_PROJECT
{
    public partial class Profile : Form
    {
        public Profile()
        {
            InitializeComponent();
            Shown += Profile_Shown;
        }

        private void Profile_Shown(object sender, EventArgs e)
        {
            // Use the main shell (Form5) which contains the sidebar navigation.
            Hide();
            using (var shell = new Form5("profile"))
            {
                shell.ShowDialog(this);
            }

            Close();
        }

        private void profilePage1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void studentLabel_Click(object sender, EventArgs e)
        {

        }

        private void sue_Click(object sender, EventArgs e)
        {

        }

        private void Profile_Load(object sender, EventArgs e)
        {

        }

        

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void studentName_Click(object sender, EventArgs e)
        {

        }

        private void editPicture_Click(object sender, EventArgs e)
        {

        }

        private void profilePicture_Click(object sender, EventArgs e)
        {

        }

        private void studentEmailLabel_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
