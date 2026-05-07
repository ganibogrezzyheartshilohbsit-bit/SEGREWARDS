using System.Drawing;
using System.Windows.Forms;

namespace SEGREWARDS_PROJECT.UserControls
{
    public sealed class LeaderboardPage : UserControl
    {
        public LeaderboardPage()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            var lbl = new Label
            {
                AutoSize = true,
                Text = "Leaderboards (coming soon)",
                Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold),
                Location = new Point(20, 20)
            };

            Controls.Add(lbl);
        }
    }
}

