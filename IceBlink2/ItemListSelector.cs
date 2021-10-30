using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public partial class ItemListSelector : IBForm
    {
        GameView gv;
        public string HeaderText = "";
        public List<string> itemList = new List<string>();
        public int selectedIndex = 0;

        public ItemListSelector(GameView g, List<string> itList, string headertxt)
        {
            InitializeComponent();
            this.IceBlinkButtonClose.setupAll(g);
            this.IceBlinkButtonResize.setupAll(g);
            this.IceBlinkButtonClose.Visible = false;
            this.IceBlinkButtonClose.Enabled = false;
            this.IceBlinkButtonResize.Visible = false;
            this.IceBlinkButtonResize.Enabled = false;
            gv = g;
            HeaderText = headertxt;
            this.label1.Font = gv.drawFontReg;
            this.label1.Text = headertxt;
            itemList = itList;
            ResizeWindow();
            CreateButtons();

        }

        public void ResizeWindow()
        {
            this.Height = 120 + itemList.Count * 55;
            this.label1.Height = (int)(gv.drawFontReg.Height * 2f);
            int width = 0;
            width = (int)((float)HeaderText.Length * (float)gv.drawFontReg.Height / 1.3f);
            foreach (string s in itemList)
            {
                int w = (int)((float)s.Length * (float)gv.drawFontReg.Height / 1.3f);
                if (w > width) { width = w; }
            }
            this.Width = width + 30;
        }

        public void CreateButtons()
        {
            int yLoc = 120;
            int cnt = 0;
            foreach (string s in itemList)
            {
                var newButton = new Button();
                // 
                // button1
                // 
                newButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                newButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
                newButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                newButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                //newButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                newButton.Font = gv.drawFontLarge;
                newButton.ForeColor = System.Drawing.Color.White;
                newButton.Location = new System.Drawing.Point(12, yLoc);
                newButton.Name = cnt.ToString();
                newButton.Size = new System.Drawing.Size(this.Width - 24, 45);
                newButton.TabIndex = 0;
                newButton.Text = s;
                newButton.UseVisualStyleBackColor = true;
                newButton.Click += new System.EventHandler(this.btn_Click);
                this.Controls.Add(newButton);
                cnt++;
                yLoc += 55;
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            selectedIndex = Convert.ToInt32(btn.Name);
            this.Close();
        }
    }
}
