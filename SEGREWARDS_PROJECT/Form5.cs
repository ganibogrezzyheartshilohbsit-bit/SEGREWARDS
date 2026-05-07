using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SEGREWARDS_PROJECT.Composition;
using SEGREWARDS_PROJECT.Infrastructure;
using SEGREWARDS_PROJECT.Services;
using SEGREWARDS_PROJECT.UserControls;

namespace SEGREWARDS_PROJECT
{
    public partial class Form5 : Form
    {
        private byte[] _proofBytes;
        private string _proofMime;
        private string _proofFileName;

        private UserControl _currentPage;
        private SubmitWastePage _submitWastePage;
        private DashboardPage _dashboardPage;
        private RewardsPage _rewardsPage;
        private LeaderboardPage _leaderboardPage;
        private ProfilePage _profilePage;
        private readonly string _initialPageKey;

        public Form5(string initialPageKey = null)
        {
            _initialPageKey = initialPageKey;
            InitializeComponent();
            buttonsignup.Click += Buttonsignup_Click;
            button1.Click += (sender, e) => pictureBox19_Click(sender, e);

            dashboardBtn.Click += (s, e) => OpenDashboard();
            submitWasteBtn.Click += (s, e) => ShowPage(GetSubmitWastePage());
            rewardsBtn.Click += (s, e) => OpenRewards();
            leaderboardsBtn.Click += (s, e) => ShowPage(GetLeaderboardPage());
            profileBtn.Click += (s, e) => ShowPage(GetProfilePage());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void label14_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            TryPrefillFromSession();
            TryRefreshHeaderPoints();
            RefreshUserHeader();

            // Default/initial page
            if (string.Equals(_initialPageKey, "dashboard", StringComparison.OrdinalIgnoreCase))
            {
                OpenDashboard();
            }
            else if (string.Equals(_initialPageKey, "rewards", StringComparison.OrdinalIgnoreCase))
            {
                OpenRewards();
            }
            else if (string.Equals(_initialPageKey, "leaderboard", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(_initialPageKey, "leaderboards", StringComparison.OrdinalIgnoreCase))
            {
                ShowPage(GetLeaderboardPage());
            }
            else if (string.Equals(_initialPageKey, "profile", StringComparison.OrdinalIgnoreCase))
            {
                ShowPage(GetProfilePage());
            }
            else
            {
                ShowPage(GetSubmitWastePage());
            }
        }

        private void OpenDashboard()
        {
            Hide();
            try
            {
                using (var dashboard = new Form3())
                {
                    dashboard.ShowDialog(this);
                }
            }
            finally
            {
                Show();
            }
        }

        private void OpenRewards()
        {
            Hide();
            try
            {
                using (var rewards = new txtsearchbar())
                {
                    rewards.ShowDialog(this);
                }
            }
            finally
            {
                Show();
            }
        }

        private void ShowPage(UserControl page)
        {
            if (page == null) return;

            panelMainContent.SuspendLayout();
            try
            {
                panelMainContent.Controls.Clear();
                panelMainContent.Controls.Add(page);
                page.Dock = DockStyle.Fill;
                _currentPage = page;
            }
            finally
            {
                panelMainContent.ResumeLayout(true);
            }

            // Keep header/sidebar visible above the page host.
            panelsidebar.BringToFront();
            pictureBox9.BringToFront();
            pictureBox8.BringToFront();
            label8.BringToFront();
            label7.BringToFront();
            pictureBox16.BringToFront();
            pictureBox7.BringToFront();
            label12.BringToFront();
            label9.BringToFront();
            label10.BringToFront();
            pictureBox10.BringToFront();
        }

        private UserControl GetSubmitWastePage()
        {
            if (_submitWastePage != null) return _submitWastePage;

            // Move existing Submit Waste UI into a page (so it becomes navigable).
            if (pictureBox6.Parent != null) pictureBox6.Parent.Controls.Remove(pictureBox6);
            if (panel1.Parent != null) panel1.Parent.Controls.Remove(panel1);
            if (button2.Parent != null) button2.Parent.Controls.Remove(button2);

            _submitWastePage = new SubmitWastePage(pictureBox6, panel1, button2);
            return _submitWastePage;
        }

        private UserControl GetDashboardPage()
        {
            return _dashboardPage ?? (_dashboardPage = new DashboardPage());
        }

        private UserControl GetRewardsPage()
        {
            return _rewardsPage ?? (_rewardsPage = new RewardsPage());
        }

        private UserControl GetLeaderboardPage()
        {
            return _leaderboardPage ?? (_leaderboardPage = new LeaderboardPage());
        }

        private UserControl GetProfilePage()
        {
            return _profilePage ?? (_profilePage = new ProfilePage());
        }

        private void RefreshUserHeader()
        {
            if (AppSession.CurrentUserId == null)
            {
                return;
            }

            // Hydrate session and header from DB (best-effort).
            try
            {
                var user = AppCompositionRoot.Instance.Users.GetById(AppSession.CurrentUserId.Value);
                if (user != null)
                {
                    AppSession.CurrentStudentNumber = user.StudentNumber;
                    AppSession.CurrentFullName = user.FullName;
                    AppSession.CurrentEcoPoints = user.EcoPointsBalance;

                    label7.Text = user.EcoPointsBalance.ToString("N0");
                    label9.Text = user.FullName;
                }
            }
            catch
            {
                // Header is optional if DB is offline.
            }

            // Profile image (persisted from registration)
            try
            {
                pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                var profile = AppSession.TryLoadProfileImage(AppSession.CurrentStudentNumber);
                if (profile != null)
                {
                    var old = pictureBox7.Image;
                    pictureBox7.BackgroundImage = null;
                    pictureBox7.Image = profile;
                    old?.Dispose();
                }
            }
            catch
            {
                // Ignore profile image failures.
            }
        }

        private void TryPrefillFromSession()
        {
            if (AppSession.CurrentUserId == null)
            {
                return;
            }

            try
            {
                var user = AppCompositionRoot.Instance.Users.GetById(AppSession.CurrentUserId.Value);
                if (user == null)
                {
                    return;
                }

                txtboxstudnum.Text = user.StudentNumber;
                txtboxfullname.Text = user.FullName;
                if (!string.IsNullOrWhiteSpace(user.YearLevel))
                {
                    comboBox1.Text = user.YearLevel;
                }

                if (!string.IsNullOrWhiteSpace(user.Course))
                {
                    comboBox2.Text = user.Course;
                }
            }
            catch
            {
                // Prefill is best-effort only.
            }
        }

        private void TryRefreshHeaderPoints()
        {
            if (AppSession.CurrentUserId == null)
            {
                label7.Text = "0";
                return;
            }

            try
            {
                var user = AppCompositionRoot.Instance.Users.GetById(AppSession.CurrentUserId.Value);
                if (user == null)
                {
                    return;
                }

                label7.Text = user.EcoPointsBalance.ToString("N0");
                label9.Text = user.FullName;
                AppSession.CurrentEcoPoints = user.EcoPointsBalance;
                AppSession.CurrentFullName = user.FullName;
                AppSession.CurrentStudentNumber = user.StudentNumber;
            }
            catch
            {
                // Header refresh is optional if DB is offline.
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
        }

        private void label19_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label20_Click(object sender, EventArgs e)
        {
        }

        private void lblstudnum_Click(object sender, EventArgs e)
        {
        }

        private void label21_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label22_Click(object sender, EventArgs e)
        {
        }

        private void lblFullname_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox19_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "JPEG / PNG|*.jpg;*.jpeg;*.png|All files|*.*";
                dlg.Title = "Upload proof";
                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    MessageBox.Show(this, "Please choose a JPEG or PNG file.", "Proof", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var bytes = File.ReadAllBytes(dlg.FileName);
                    if (bytes.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show(this, "Proof must be 5 MB or smaller.", "Proof", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _proofBytes = bytes;
                    _proofMime = ext == ".png" ? "image/png" : "image/jpeg";
                    _proofFileName = Path.GetFileName(dlg.FileName);

                    if (pictureBox20.Image != null)
                    {
                        var old = pictureBox20.Image;
                        pictureBox20.Image = null;
                        old.Dispose();
                    }

                    using (var ms = new MemoryStream(bytes))
                    using (var img = Image.FromStream(ms))
                    {
                        pictureBox20.Image = new Bitmap(img);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not read the image: " + ex.Message, "Proof", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Buttonsignup_Click(object sender, EventArgs e)
        {
            string wasteCode = null;
            if (radiobtnbottle.Checked)
            {
                wasteCode = "BOTTLE";
            }
            else if (radiobtnpaper.Checked)
            {
                wasteCode = "PAPER";
            }
            else if (radioBtnbio.Checked)
            {
                wasteCode = "BIO";
            }

            var request = new WasteSubmitRequest
            {
                StudentNumber = txtboxstudnum.Text,
                FullName = txtboxfullname.Text,
                YearLevel = comboBox1.Text,
                Course = comboBox2.Text,
                WasteTypeCode = wasteCode,
                ProofImageBytes = _proofBytes,
                ProofMimeType = _proofMime,
                ProofFileName = _proofFileName
            };

            try
            {
                var result = AppCompositionRoot.Instance.WasteSubmission.Submit(request);
                MessageBox.Show(
                    this,
                    result.Message,
                    result.Success ? "Submit Waste" : "Validation",
                    MessageBoxButtons.OK,
                    result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                if (result.Success)
                {
                    TryRefreshHeaderPoints();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void leaderboardsBtn_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void dashboardBtn_Click(object sender, EventArgs e)
        {

        }

        private void submitWasteBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
