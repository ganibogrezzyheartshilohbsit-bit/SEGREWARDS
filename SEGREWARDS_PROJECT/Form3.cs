using System;
using System.Windows.Forms;

namespace SEGREWARDS_PROJECT
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void lbl_smartwastsystem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void HOME_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            using (var home = new txtsearchbar())
            {
                home.ShowDialog(this);
            }

            Show();
        }
    }
}
