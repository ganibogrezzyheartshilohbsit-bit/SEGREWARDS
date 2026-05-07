using System.Windows.Forms;

namespace SEGREWARDS_PROJECT.UserControls
{
    public sealed class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            Dock = DockStyle.Fill;

            // Bind Dashboard to Form3 by hosting it inside this page.
            var form = new Form3
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

