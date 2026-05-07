using System.Windows.Forms;

namespace SEGREWARDS_PROJECT.UserControls
{
    public sealed class ProfilePage : UserControl
    {
        public ProfilePage()
        {
            Dock = DockStyle.Fill;

            // Profile page should show the Profile UI (Form6).
            var form = new Form6
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };

            Controls.Add(form);
            form.Show();
        }
    }
}

