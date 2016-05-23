namespace IceBlink2
{
    partial class IBForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IBForm));
            this.pnlTop = new IceBlink2.IBPanel();
            this.pnlTitle = new IceBlink2.IBPanel();
            this.iceBlinkButtonResize1 = new IceBlink2.IceBlinkButtonResize();
            this.iceBlinkButtonClose1 = new IceBlink2.IceBlinkButtonClose();
            this.pnlTop.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.Transparent;
            this.pnlTop.Controls.Add(this.pnlTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(359, 24);
            this.pnlTop.TabIndex = 2;
            // 
            // pnlTitle
            // 
            this.pnlTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTitle.BackColor = System.Drawing.Color.Transparent;
            this.pnlTitle.BackgroundImage = global::IceBlink2.Properties.Resources.gb_lrg_header;
            this.pnlTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pnlTitle.Controls.Add(this.iceBlinkButtonResize1);
            this.pnlTitle.Controls.Add(this.iceBlinkButtonClose1);
            this.pnlTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlTitle.Location = new System.Drawing.Point(2, 2);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(355, 30);
            this.pnlTitle.TabIndex = 0;
            this.pnlTitle.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlTitle_Paint);
            this.pnlTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTitle_MouseDown);
            this.pnlTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlTitle_MouseMove);
            this.pnlTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlTitle_MouseUp);
            // 
            // iceBlinkButtonResize1
            // 
            this.iceBlinkButtonResize1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iceBlinkButtonResize1.BackColor = System.Drawing.Color.Transparent;
            this.iceBlinkButtonResize1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("iceBlinkButtonResize1.BackgroundImage")));
            this.iceBlinkButtonResize1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iceBlinkButtonResize1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.iceBlinkButtonResize1.FlatAppearance.BorderSize = 0;
            this.iceBlinkButtonResize1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.iceBlinkButtonResize1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.iceBlinkButtonResize1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iceBlinkButtonResize1.HoverImage = ((System.Drawing.Image)(resources.GetObject("iceBlinkButtonResize1.HoverImage")));
            this.iceBlinkButtonResize1.Location = new System.Drawing.Point(309, -1);
            this.iceBlinkButtonResize1.Name = "iceBlinkButtonResize1";
            this.iceBlinkButtonResize1.NormalImage = ((System.Drawing.Image)(resources.GetObject("iceBlinkButtonResize1.NormalImage")));
            this.iceBlinkButtonResize1.PressedImage = ((System.Drawing.Image)(resources.GetObject("iceBlinkButtonResize1.PressedImage")));
            this.iceBlinkButtonResize1.Size = new System.Drawing.Size(23, 23);
            this.iceBlinkButtonResize1.TabIndex = 1;
            this.iceBlinkButtonResize1.UseVisualStyleBackColor = false;
            this.iceBlinkButtonResize1.Click += new System.EventHandler(this.iceBlinkButtonResize1_Click);
            // 
            // iceBlinkButtonClose1
            // 
            this.iceBlinkButtonClose1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iceBlinkButtonClose1.BackColor = System.Drawing.Color.Transparent;
            this.iceBlinkButtonClose1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("iceBlinkButtonClose1.BackgroundImage")));
            this.iceBlinkButtonClose1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iceBlinkButtonClose1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.iceBlinkButtonClose1.FlatAppearance.BorderSize = 0;
            this.iceBlinkButtonClose1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.iceBlinkButtonClose1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.iceBlinkButtonClose1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iceBlinkButtonClose1.HoverImage = ((System.Drawing.Image)(resources.GetObject("iceBlinkButtonClose1.HoverImage")));
            this.iceBlinkButtonClose1.Location = new System.Drawing.Point(331, -1);
            this.iceBlinkButtonClose1.Name = "iceBlinkButtonClose1";
            this.iceBlinkButtonClose1.NormalImage = ((System.Drawing.Image)(resources.GetObject("iceBlinkButtonClose1.NormalImage")));
            this.iceBlinkButtonClose1.PressedImage = ((System.Drawing.Image)(resources.GetObject("iceBlinkButtonClose1.PressedImage")));
            this.iceBlinkButtonClose1.Size = new System.Drawing.Size(23, 23);
            this.iceBlinkButtonClose1.TabIndex = 0;
            this.iceBlinkButtonClose1.UseVisualStyleBackColor = false;
            this.iceBlinkButtonClose1.Click += new System.EventHandler(this.iceBlinkButtonClose1_Click);
            // 
            // IBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 272);
            this.ControlBox = false;
            this.Controls.Add(this.pnlTop);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IBForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.pnlTop.ResumeLayout(false);
            this.pnlTitle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private IBPanel pnlTop;
        private IBPanel pnlTitle;
        private IceBlink2.IceBlinkButtonResize iceBlinkButtonResize1;
        private IceBlink2.IceBlinkButtonClose iceBlinkButtonClose1;
    }
}