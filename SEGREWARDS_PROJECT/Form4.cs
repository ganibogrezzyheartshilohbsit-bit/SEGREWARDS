using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SEGREWARDS_PROJECT.Composition;
using SEGREWARDS_PROJECT.Infrastructure;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT
{
    public partial class txtsearchbar : Form
    {
        private readonly ListBox _rewardSearchResults = new ListBox();
        private RewardCatalogRecord _featuredReward;
        private readonly FlowLayoutPanel _popularRewardsFlow = new FlowLayoutPanel();
        private System.Collections.Generic.List<RewardCatalogRecord> _lastSearchResults = new System.Collections.Generic.List<RewardCatalogRecord>();
        private System.Collections.Generic.List<RewardCatalogRecord> _popularRewardsCache = new System.Collections.Generic.List<RewardCatalogRecord>();

        public txtsearchbar()
        {
            InitializeComponent();
            _rewardSearchResults.Height = 140;
            _rewardSearchResults.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _rewardSearchResults.Visible = false;
            _rewardSearchResults.IntegralHeight = false;
            _rewardSearchResults.BorderStyle = BorderStyle.FixedSingle;
            _rewardSearchResults.BackColor = Color.White;
            _rewardSearchResults.ForeColor = Color.Black;
            _rewardSearchResults.DrawMode = DrawMode.Normal;
            _rewardSearchResults.ItemHeight = 20;
            _rewardSearchResults.FormattingEnabled = true;
            _rewardSearchResults.DisplayMember = "DisplayText";
            Controls.Add(_rewardSearchResults);

            searchBox.GotFocus += TextBox1_GotFocus;
            searchBox.TextChanged += TextBox1_TextChanged;
            searchBox.LostFocus += SearchBox_LostFocus;
            _rewardSearchResults.Click += RewardSearchResults_Click;
            _rewardSearchResults.DoubleClick += RewardSearchResults_Click;
            buttonsignup.Click += Buttonsignup_Click;
            redeemButton.Click += RedeemButton_Click;

            // Dynamic popular rewards container (prevents cards/buttons being clipped at runtime).
            _popularRewardsFlow.AutoScroll = true;
            _popularRewardsFlow.WrapContents = false;
            _popularRewardsFlow.FlowDirection = FlowDirection.LeftToRight;
            _popularRewardsFlow.Visible = true;
            _popularRewardsFlow.BackColor = Color.Transparent;
            Controls.Add(_popularRewardsFlow);
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
            PositionRewardSearchResults();

            RefreshEcoPointsHeader();
            RefreshUserHeader();
            EnsurePopularRewardsLayout();
            LoadPopularRewardsFromDatabase();
            RunRewardSearch();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionRewardSearchResults();
        }

        private void PositionRewardSearchResults()
        {
            if (searchBox == null) return;

            // Dropdown-style list under the search box (so it never covers Claim Rewards area).
            var left = searchBox.Left;
            var top = searchBox.Bottom + 4;
            var width = searchBox.Width;
            var maxHeight = 220;

            _rewardSearchResults.Left = left;
            _rewardSearchResults.Top = top;
            _rewardSearchResults.Width = width;
            _rewardSearchResults.Height = Math.Min(maxHeight, _rewardSearchResults.Height);
            _rewardSearchResults.BringToFront();
        }

        private void RefreshUserHeader()
        {
            // If session data is incomplete, try to hydrate it from the DB.
            if (AppSession.CurrentUserId != null &&
                (string.IsNullOrWhiteSpace(AppSession.CurrentStudentNumber) || string.IsNullOrWhiteSpace(AppSession.CurrentFullName)))
            {
                try
                {
                    var u = AppCompositionRoot.Instance.Users.GetById(AppSession.CurrentUserId.Value);
                    if (u != null)
                    {
                        AppSession.CurrentStudentNumber = u.StudentNumber;
                        AppSession.CurrentFullName = u.FullName;
                        AppSession.CurrentEcoPoints = u.EcoPointsBalance;
                    }
                }
                catch
                {
                    // Optional if DB is offline.
                }
            }

            // Full name
            userName.Text = string.IsNullOrWhiteSpace(AppSession.CurrentFullName) ? "Student" : AppSession.CurrentFullName;

            // Eco points (fallback to session even if DB refresh failed)
            if (AppSession.CurrentEcoPoints > 0)
            {
                ecoPoints.Text = AppSession.CurrentEcoPoints.ToString("N0");
            }

            // Profile image (persisted from registration)
            landingPageProfile.SizeMode = PictureBoxSizeMode.Zoom;
            var profile = AppSession.TryLoadProfileImage(AppSession.CurrentStudentNumber);
            if (profile != null)
            {
                var old = landingPageProfile.Image;
                landingPageProfile.BackgroundImage = null;
                landingPageProfile.Image = profile;
                old?.Dispose();
            }
        }

        private void TextBox1_GotFocus(object sender, EventArgs e)
        {
            if (searchBox.Text != null && searchBox.Text.Contains("Search habbits"))
            {
                searchBox.Text = string.Empty;
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            RunRewardSearch();
        }

        // Kept for WinForms designer event wiring compatibility.
        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            RunRewardSearch();
        }

        private void SearchBox_LostFocus(object sender, EventArgs e)
        {
            // If user clicks on the listbox, don't hide it before click is processed.
            if (_rewardSearchResults.Focused) return;
            _rewardSearchResults.Visible = false;
        }

        private void RewardSearchResults_Click(object sender, EventArgs e)
        {
            var selected = _rewardSearchResults.SelectedItem as RewardCatalogRecord;
            if (selected == null) return;

            ApplyRewardToFeaturedCard(selected);
            _rewardSearchResults.Visible = false;
        }

        // (Owner-draw removed; standard drawing + DisplayMember is more reliable here.)

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
                    ecoPoints.Text = user.EcoPointsBalance.ToString("N0");
                    AppSession.CurrentEcoPoints = user.EcoPointsBalance;
                    AppSession.CurrentFullName = user.FullName;
                    AppSession.CurrentStudentNumber = user.StudentNumber;
                    if (!string.IsNullOrWhiteSpace(user.FullName))
                    {
                        userName.Text = user.FullName;
                    }
                }
            }
            catch
            {
                // Header is optional if DB is offline.
            }
        }

        private void EnsurePopularRewardsLayout()
        {
            // Place the dynamic card row where the old static cards used to be.
            _popularRewardsFlow.Left = 106;
            _popularRewardsFlow.Top = 544;
            _popularRewardsFlow.Width = Math.Max(600, ClientSize.Width - 200);
            _popularRewardsFlow.Height = 410; // enough for 400px cards + scroll bar
            _popularRewardsFlow.BringToFront();

            // Move the existing designer card (panel1) into the flow as the first card.
            if (panel1 != null && panel1.Parent != _popularRewardsFlow)
            {
                Controls.Remove(panel1);
                panel1.Margin = new Padding(0, 0, 20, 0);
                _popularRewardsFlow.Controls.Add(panel1);
            }
        }

        private void LoadPopularRewardsFromDatabase()
        {
            try
            {
                var list = AppCompositionRoot.Instance.RewardCatalog.ListActive(20);
                _popularRewardsCache = list == null
                    ? new System.Collections.Generic.List<RewardCatalogRecord>()
                    : new System.Collections.Generic.List<RewardCatalogRecord>(list);
                _featuredReward = (list != null && list.Count > 0) ? list[0] : null;

                if (_featuredReward == null)
                {
                    rewardLabel.Text = "No rewards yet";
                    rewardTextLabel.Text = "Rewards will appear here once added to the database.";
                    rewardEcoCost.Text = string.Empty;
                    redeemButton.Enabled = false;
                    return;
                }

                rewardLabel.Text = _featuredReward.Name ?? "Reward";
                rewardTextLabel.Text = string.IsNullOrWhiteSpace(_featuredReward.Description)
                    ? "—"
                    : _featuredReward.Description;
                rewardEcoCost.Text = _featuredReward.PointsCost.ToString("N0") + " EcoPoints";

                ApplyRewardImage(rewardImage, _featuredReward.ImagePath);

                redeemButton.Enabled =
                    AppSession.CurrentUserId != null &&
                    AppSession.CurrentEcoPoints >= _featuredReward.PointsCost;

                // Render the rest of the popular rewards as additional cards.
                // Keep it lightweight: show up to 4 total cards (including panel1).
                RenderAdditionalPopularRewardCards(_popularRewardsCache);
            }
            catch (Exception ex)
            {
                // Make the failure obvious (otherwise the designer defaults look like "wrong DB data").
                rewardLabel.Text = "Rewards unavailable";
                rewardTextLabel.Text = ex.Message;
                rewardEcoCost.Text = string.Empty;
                redeemButton.Enabled = false;
            }
        }

        private void ApplyRewardToFeaturedCard(RewardCatalogRecord reward)
        {
            if (reward == null) return;

            _featuredReward = reward;
            rewardLabel.Text = reward.Name ?? "Reward";
            rewardTextLabel.Text = string.IsNullOrWhiteSpace(reward.Description) ? "—" : reward.Description;
            rewardEcoCost.Text = reward.PointsCost.ToString("N0") + " EcoPoints";
            ApplyRewardImage(rewardImage, reward.ImagePath);

            redeemButton.Enabled =
                AppSession.CurrentUserId != null &&
                AppSession.CurrentEcoPoints >= reward.PointsCost;

            // Prevent duplicates: re-render the other cards excluding the featured reward.
            RenderAdditionalPopularRewardCards(_popularRewardsCache);
        }

        private void RenderAdditionalPopularRewardCards(System.Collections.Generic.IReadOnlyList<RewardCatalogRecord> rewards)
        {
            // Remove previously generated cards (keep panel1 as first card).
            for (var i = _popularRewardsFlow.Controls.Count - 1; i >= 0; i--)
            {
                var c = _popularRewardsFlow.Controls[i];
                if (!ReferenceEquals(c, panel1))
                {
                    _popularRewardsFlow.Controls.RemoveAt(i);
                    c.Dispose();
                }
            }

            if (rewards == null || rewards.Count <= 1) return;

            var added = 1; // panel1 is already present
            for (var i = 1; i < rewards.Count && added < 4; i++)
            {
                var r = rewards[i];
                if (_featuredReward != null && r != null && r.Id == _featuredReward.Id)
                {
                    continue;
                }
                var card = CreateRewardCardPanel(r);
                card.Margin = new Padding(0, 0, 20, 0);
                _popularRewardsFlow.Controls.Add(card);
                added++;
            }
        }

        private Panel CreateRewardCardPanel(RewardCatalogRecord reward)
        {
            var card = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = 275,
                Height = 400,
                BackColor = Color.White
            };

            var pic = new PictureBox
            {
                Left = 18,
                Top = 14,
                Width = 241,
                Height = 159,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Use designer resource as fallback when no DB path exists.
            pic.BackgroundImage = rewardImage.BackgroundImage;
            pic.BackgroundImageLayout = ImageLayout.Stretch;

            var name = new Label
            {
                AutoSize = true,
                Left = 20,
                Top = 198,
                Font = rewardLabel.Font,
                ForeColor = rewardLabel.ForeColor,
                BackColor = Color.White,
                Text = reward?.Name ?? "Reward"
            };

            var desc = new Label
            {
                AutoSize = true,
                Left = 20,
                Top = 227,
                Font = rewardTextLabel.Font,
                ForeColor = rewardTextLabel.ForeColor,
                BackColor = Color.White,
                MaximumSize = new Size(235, 0),
                Text = string.IsNullOrWhiteSpace(reward?.Description) ? "—" : reward.Description
            };

            var cost = new Label
            {
                AutoSize = true,
                Left = 20,
                Top = 263,
                Font = rewardEcoCost.Font,
                ForeColor = rewardEcoCost.ForeColor,
                BackColor = Color.White,
                Text = (reward?.PointsCost ?? 0).ToString("N0") + " EcoPoints"
            };

            var btn = new Button
            {
                Left = 39,
                Top = 312,
                Width = 202,
                Height = 50,
                Font = redeemButton.Font,
                ForeColor = redeemButton.ForeColor,
                BackColor = redeemButton.BackColor,
                FlatStyle = redeemButton.FlatStyle,
                Text = redeemButton.Text,
                TextImageRelation = redeemButton.TextImageRelation,
                Enabled = AppSession.CurrentUserId != null && reward != null && AppSession.CurrentEcoPoints >= reward.PointsCost,
                Tag = reward
            };
            btn.Click += RedeemPopularReward_Click;

            card.Controls.Add(pic);
            card.Controls.Add(name);
            card.Controls.Add(desc);
            card.Controls.Add(cost);
            card.Controls.Add(btn);

            ApplyRewardImage(pic, reward?.ImagePath);
            return card;
        }

        private void RedeemPopularReward_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var reward = btn?.Tag as RewardCatalogRecord;
            RedeemReward(reward);
        }

        private static string ResolveRewardImagePath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return null;

            var p = imagePath.Trim();
            if (Path.IsPathRooted(p)) return p;

            // Relative to the app folder (bin\Debug) at runtime.
            return Path.Combine(Application.StartupPath, p);
        }

        private void ApplyRewardImage(PictureBox pictureBox, string imagePath)
        {
            try
            {
                if (pictureBox == null) return;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                var resolved = ResolveRewardImagePath(imagePath);
                if (string.IsNullOrWhiteSpace(resolved) || !File.Exists(resolved))
                {
                    return;
                }

                Image img;
                using (var fs = new FileStream(resolved, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var temp = Image.FromStream(fs))
                {
                    img = new Bitmap(temp);
                }

                var old = pictureBox.Image;
                pictureBox.BackgroundImage = null;
                pictureBox.Image = img;
                old?.Dispose();
            }
            catch
            {
                // Keep default image if the file can't be read.
            }
        }

        private void RedeemButton_Click(object sender, EventArgs e)
        {
            RedeemReward(_featuredReward);
        }

        private void RedeemReward(RewardCatalogRecord reward)
        {
            if (reward == null)
            {
                MessageBox.Show("No reward selected.", "Redeem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (AppSession.CurrentUserId == null)
            {
                MessageBox.Show("Please sign in to redeem rewards.", "Redeem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = AppCompositionRoot.Instance.RewardRedemption.Redeem(
                    AppSession.CurrentUserId.Value,
                    reward.Id);

                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Redeem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AppSession.CurrentEcoPoints = result.NewBalance;
                RefreshEcoPointsHeader();

                // Update enabled state for all redeem buttons (featured + dynamic cards).
                redeemButton.Enabled = AppSession.CurrentEcoPoints >= reward.PointsCost;
                foreach (Control c in _popularRewardsFlow.Controls)
                {
                    foreach (Control child in c.Controls)
                    {
                        var btn = child as Button;
                        if (btn?.Tag is RewardCatalogRecord r)
                        {
                            btn.Enabled = AppSession.CurrentEcoPoints >= r.PointsCost;
                        }
                    }
                }

                MessageBox.Show(
                    "Redeemed: " + reward.Name + "\r\nNew balance: " + AppSession.CurrentEcoPoints.ToString("N0") + " EcoPoints",
                    "Redeem",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Redeem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunRewardSearch()
        {
            try
            {
                var q = (searchBox.Text ?? string.Empty).Trim();
                if (q.IndexOf("Search habbits", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    q = string.Empty;
                }

                if (string.IsNullOrWhiteSpace(q))
                {
                    _rewardSearchResults.Visible = false;
                    return;
                }

                var items = AppCompositionRoot.Instance.RewardSearch.Search(
                    q,
                    50);

                _lastSearchResults = items == null
                    ? new System.Collections.Generic.List<RewardCatalogRecord>()
                    : new System.Collections.Generic.List<RewardCatalogRecord>(items);

                _rewardSearchResults.DataSource = null;
                _rewardSearchResults.Items.Clear();
                foreach (var r in _lastSearchResults)
                {
                    _rewardSearchResults.Items.Add(r);
                }

                if (_rewardSearchResults.Items.Count == 0)
                {
                    _rewardSearchResults.DataSource = null;
                    _rewardSearchResults.Items.Clear();
                    _rewardSearchResults.Items.Add("No rewards match that search.");
                }

                // Size like a dropdown (don’t block the rest of the page).
                var desired = (_rewardSearchResults.ItemHeight * Math.Min(_rewardSearchResults.Items.Count, 8)) + 6;
                _rewardSearchResults.Height = Math.Max(60, Math.Min(220, desired));
                PositionRewardSearchResults();
                _rewardSearchResults.Visible = true;
            }
            catch (Exception ex)
            {
                _rewardSearchResults.DataSource = null;
                _rewardSearchResults.Items.Clear();
                _rewardSearchResults.Items.Add("Search unavailable: " + ex.Message);
                _rewardSearchResults.Height = 60;
                PositionRewardSearchResults();
                _rewardSearchResults.Visible = true;
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

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void landingPageProfile_Click(object sender, EventArgs e)
        {

        }

        private void ecoPoints_Click(object sender, EventArgs e)
        {

        }

        private void rewardEcoCost_Click(object sender, EventArgs e)
        {

        }
    }
}
