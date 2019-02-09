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
    public partial class TextInputDialog : IBFormGDI
    {
        GameView gv;
        public string HeaderText = "";
        public string textInput = "";

        public TextInputDialog(GameView g, string headertxt)
        {
            InitializeComponent();
            this.IceBlinkButtonClose.setupAll(g);
            this.IceBlinkButtonResize.setupAll(g);
            this.IceBlinkButtonClose.Visible = false;
            this.IceBlinkButtonClose.Enabled = false;
            this.IceBlinkButtonResize.Visible = false;
            this.IceBlinkButtonResize.Enabled = false;
            //this.Activate();
            
            gv = g;
            btnReturn.Text = "RETURN";
            btnReturn.Font = gv.drawFontReg;
            HeaderText = headertxt;
            this.label1.Font = gv.drawFontReg;
            this.label1.Text = headertxt;
            //txtInput.BringToFront();
            //txtInput.Enabled = true;
            //txtInput.Focus();
            txtInput.Font = gv.drawFontReg;
            txtInput.AcceptsReturn = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.Return)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            textInput = txtInput.Text;
        }
    }
}
