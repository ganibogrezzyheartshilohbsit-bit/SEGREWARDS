namespace SEGREWARDS_PROJECT
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_wastemngalogo = new System.Windows.Forms.Label();
            this.lbl_smartwastsystem = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pic_lleafogo = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_lleafogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(-3, -91);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1481, 170);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lbl_wastemngalogo
            // 
            this.lbl_wastemngalogo.AutoSize = true;
            this.lbl_wastemngalogo.BackColor = System.Drawing.Color.Black;
            this.lbl_wastemngalogo.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_wastemngalogo.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lbl_wastemngalogo.Location = new System.Drawing.Point(68, 234);
            this.lbl_wastemngalogo.Name = "lbl_wastemngalogo";
            this.lbl_wastemngalogo.Size = new System.Drawing.Size(563, 69);
            this.lbl_wastemngalogo.TabIndex = 8;
            this.lbl_wastemngalogo.Text = "WasteManagement";
            // 
            // lbl_smartwastsystem
            // 
            this.lbl_smartwastsystem.AutoSize = true;
            this.lbl_smartwastsystem.BackColor = System.Drawing.Color.Black;
            this.lbl_smartwastsystem.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_smartwastsystem.Location = new System.Drawing.Point(113, 303);
            this.lbl_smartwastsystem.Name = "lbl_smartwastsystem";
            this.lbl_smartwastsystem.Size = new System.Drawing.Size(380, 29);
            this.lbl_smartwastsystem.TabIndex = 9;
            this.lbl_smartwastsystem.Text = "Smart Waste Management System";
            this.lbl_smartwastsystem.Click += new System.EventHandler(this.lbl_smartwastsystem_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Black;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Lime;
            this.button1.Location = new System.Drawing.Point(197, 440);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(239, 49);
            this.button1.TabIndex = 52;
            this.button1.Text = "GET STARTED";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pic_lleafogo
            // 
            this.pic_lleafogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pic_lleafogo.BackgroundImage")));
            this.pic_lleafogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_lleafogo.Image = ((System.Drawing.Image)(resources.GetObject("pic_lleafogo.Image")));
            this.pic_lleafogo.Location = new System.Drawing.Point(12, 25);
            this.pic_lleafogo.Name = "pic_lleafogo";
            this.pic_lleafogo.Size = new System.Drawing.Size(126, 54);
            this.pic_lleafogo.TabIndex = 54;
            this.pic_lleafogo.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(114, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 22);
            this.label1.TabIndex = 55;
            this.label1.Text = "SEGREWARDS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Black;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(129, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 16);
            this.label2.TabIndex = 56;
            this.label2.Text = "WasteManagement";
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Black;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(104, 354);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(517, 16);
            this.label7.TabIndex = 63;
            this.label7.Text = "Join our school community in making waste management more efficient and rewarding" +
    "!";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1466, 702);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pic_lleafogo);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbl_smartwastsystem);
            this.Controls.Add(this.lbl_wastemngalogo);
            this.Controls.Add(this.pictureBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_lleafogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbl_wastemngalogo;
        private System.Windows.Forms.Label lbl_smartwastsystem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pic_lleafogo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private System.Windows.Forms.Label label7;
    }
}