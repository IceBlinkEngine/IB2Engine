namespace IceBlink2
{
    partial class Config
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFull = new System.Windows.Forms.Button();
            this.btn1008 = new System.Windows.Forms.Button();
            this.btn672 = new System.Windows.Forms.Button();
            this.btn336 = new System.Windows.Forms.Button();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.btnCustom = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnFull);
            this.groupBox1.Controls.Add(this.btn1008);
            this.groupBox1.Controls.Add(this.btn672);
            this.groupBox1.Controls.Add(this.btn336);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(220, 217);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Game Window Size";
            // 
            // btnFull
            // 
            this.btnFull.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFull.Location = new System.Drawing.Point(16, 164);
            this.btnFull.Margin = new System.Windows.Forms.Padding(4);
            this.btnFull.Name = "btnFull";
            this.btnFull.Size = new System.Drawing.Size(188, 39);
            this.btnFull.TabIndex = 3;
            this.btnFull.Text = "Full Screen";
            this.btnFull.UseVisualStyleBackColor = true;
            this.btnFull.Click += new System.EventHandler(this.btnFull_Click);
            // 
            // btn1008
            // 
            this.btn1008.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn1008.Location = new System.Drawing.Point(16, 117);
            this.btn1008.Margin = new System.Windows.Forms.Padding(4);
            this.btn1008.Name = "btn1008";
            this.btn1008.Size = new System.Drawing.Size(188, 39);
            this.btn1008.TabIndex = 2;
            this.btn1008.Text = "1920 x 1080";
            this.btn1008.UseVisualStyleBackColor = true;
            this.btn1008.Click += new System.EventHandler(this.btn1008_Click);
            // 
            // btn672
            // 
            this.btn672.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn672.Location = new System.Drawing.Point(16, 70);
            this.btn672.Margin = new System.Windows.Forms.Padding(4);
            this.btn672.Name = "btn672";
            this.btn672.Size = new System.Drawing.Size(188, 39);
            this.btn672.TabIndex = 1;
            this.btn672.Text = "1280 x 720";
            this.btn672.UseVisualStyleBackColor = true;
            this.btn672.Click += new System.EventHandler(this.btn672_Click);
            // 
            // btn336
            // 
            this.btn336.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn336.Location = new System.Drawing.Point(16, 23);
            this.btn336.Margin = new System.Windows.Forms.Padding(4);
            this.btn336.Name = "btn336";
            this.btn336.Size = new System.Drawing.Size(188, 39);
            this.btn336.TabIndex = 0;
            this.btn336.Text = "768 x 480";
            this.btn336.UseVisualStyleBackColor = true;
            this.btn336.Click += new System.EventHandler(this.btn336_Click);
            // 
            // numWidth
            // 
            this.numWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numWidth.Location = new System.Drawing.Point(8, 23);
            this.numWidth.Margin = new System.Windows.Forms.Padding(4);
            this.numWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(96, 26);
            this.numWidth.TabIndex = 2;
            this.numWidth.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numWidth);
            this.groupBox2.Location = new System.Drawing.Point(244, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(115, 63);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Width";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numHeight);
            this.groupBox3.Location = new System.Drawing.Point(367, 15);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(115, 63);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Height";
            // 
            // numHeight
            // 
            this.numHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numHeight.Location = new System.Drawing.Point(8, 23);
            this.numHeight.Margin = new System.Windows.Forms.Padding(4);
            this.numHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(96, 26);
            this.numHeight.TabIndex = 2;
            this.numHeight.Value = new decimal(new int[] {
            768,
            0,
            0,
            0});
            // 
            // btnCustom
            // 
            this.btnCustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustom.Location = new System.Drawing.Point(244, 85);
            this.btnCustom.Margin = new System.Windows.Forms.Padding(4);
            this.btnCustom.Name = "btnCustom";
            this.btnCustom.Size = new System.Drawing.Size(237, 63);
            this.btnCustom.TabIndex = 5;
            this.btnCustom.Text = "Use Custom Size Entered Above";
            this.btnCustom.UseVisualStyleBackColor = true;
            this.btnCustom.Click += new System.EventHandler(this.btnCustom_Click);
            // 
            // Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 246);
            this.ControlBox = false;
            this.Controls.Add(this.btnCustom);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(513, 285);
            this.MinimumSize = new System.Drawing.Size(513, 285);
            this.Name = "Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select the Window Size to Use";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnFull;
        private System.Windows.Forms.Button btn1008;
        private System.Windows.Forms.Button btn672;
        private System.Windows.Forms.Button btn336;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.Button btnCustom;
    }
}