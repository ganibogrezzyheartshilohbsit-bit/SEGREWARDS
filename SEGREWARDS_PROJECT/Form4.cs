using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SEGREWARDS_PROJECT.Composition;
using SEGREWARDS_PROJECT.Infrastructure;

namespace SEGREWARDS_PROJECT
{
    public partial class txtsearchbar : Form
    {
        private readonly ListBox _rewardSearchResults = new ListBox();

        public txtsearchbar()
        {
            InitializeComponent();
            _rewardSearchResults.Height = 140;
            _rewardSearchResults.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Controls.Add(_rewardSearchResults);

            textBox1.GotFocus += TextBox1_GotFocus;
            textBox1.TextChanged += TextBox1_TextChanged;
            buttonsignup.Click += Buttonsignup_Click;
        }

        private void Buttonsignup_Click(object sender, EventArgs e)
        {
            using (var waste = new Form5())
            {
                waste.ShowDialog(this);
            }

            RefreshEcoPointsHeader();
            RunRewardSearch();
        }

        private void txtsearchbar_Load(object sender, EventArgs e)
        {
            _rewardSearchResults.Left = 100;
            _rewardSearchResults.Width = Math.Max(400, ClientSize.Width - 200);
            _rewardSearchResults.Top = Math.Max(100, ClientSize.Height - _rewardSearchResults.Height - 40);

            RefreshEcoPointsHeader();
            RunRewardSearch();
        }

        private void TextBox1_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.Contains("Search habbits"))
            {
                textBox1.Text = string.Empty;
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            RunRewardSearch();
        }

        private void RefreshEcoPointsHeader()
        {
            if (AppSession.CurrentUserId == null)
            {
                return;
            }

            try
            {
                var user = AppCompositionRoot.Instance.Users.GetById(AppSession.CurrentUserId.Value);
                if (user != null)
                {
                    label4.Text = user.EcoPointsBalance.ToString("N0");
                    AppSession.CurrentEcoPoints = user.EcoPointsBalance;
                }
            }
            catch
            {
                // Header is optional if DB is offline.
            }
        }

        private void RunRewardSearch()
        {
            try
            {
                var q = (textBox1.Text ?? string.Empty).Trim();
                if (q.IndexOf("Search habbits", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    q = string.Empty;
                }

                var items = AppCompositionRoot.Instance.RewardSearch.Search(
                    string.IsNullOrWhiteSpace(q) ? null : q,
                    50);

                _rewardSearchResults.Items.Clear();
                var sb = new StringBuilder();
                foreach (var it in items)
                {
                    sb.Clear();
                    sb.Append(it.Name);
                    sb.Append(" — ");
                    sb.Append(it.PointsCost);
                    sb.Append(" EcoPoints");
                    if (!string.IsNullOrWhiteSpace(it.Description))
                    {
                        sb.Append(" — ");
                        sb.Append(it.Description);
                    }

                    _rewardSearchResults.Items.Add(sb.ToString());
                }

                if (_rewardSearchResults.Items.Count == 0)
                {
                    _rewardSearchResults.Items.Add("No rewards match that search.");
                }
            }
            catch (Exception ex)
            {
                _rewardSearchResults.Items.Clear();
                _rewardSearchResults.Items.Add("Search unavailable: " + ex.Message);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
        }

        private void label13_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
        }
    }
}
