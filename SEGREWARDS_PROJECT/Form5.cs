using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SEGREWARDS_PROJECT.Composition;
using SEGREWARDS_PROJECT.Infrastructure;
using SEGREWARDS_PROJECT.Services;

namespace SEGREWARDS_PROJECT
{
    public partial class Form5 : Form
    {
        private byte[] _proofBytes;
        private string _proofMime;
        private string _proofFileName;

        public Form5()
        {
            InitializeComponent();
            buttonsignup.Click += Buttonsignup_Click;
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
    }
}
