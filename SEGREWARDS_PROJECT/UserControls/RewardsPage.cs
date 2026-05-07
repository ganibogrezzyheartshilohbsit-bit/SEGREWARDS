using System.Windows.Forms;

namespace SEGREWARDS_PROJECT.UserControls
{
    public sealed class RewardsPage : UserControl
    {
        public RewardsPage()
        {
            Dock = DockStyle.Fill;

            // Reuse the existing Rewards form UI by hosting it inside this page.
            var form = new txtsearchbar
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

