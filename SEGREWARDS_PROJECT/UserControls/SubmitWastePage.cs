using System.Windows.Forms;

namespace SEGREWARDS_PROJECT.UserControls
{
    public sealed class SubmitWastePage : UserControl
    {
        public SubmitWastePage(Control heroImage, Control submitPanel, Control infoBanner)
        {
            Dock = DockStyle.Fill;
            AutoScroll = true;

            // Re-parent existing Form5 controls so we keep the exact designer layout.
            if (heroImage != null) Controls.Add(heroImage);
            if (submitPanel != null) Controls.Add(submitPanel);
            if (infoBanner != null) Controls.Add(infoBanner);

            // Ensure they appear in front within this page.
            heroImage?.BringToFront();
            submitPanel?.BringToFront();
            infoBanner?.BringToFront();
        }
    }
}

