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
    public partial class TextInputDialog : IBForm
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
            gv = g;
            btnReturn.Text = "RETURN";
            btnReturn.Font = gv.drawFontReg;
            HeaderText = headertxt;
            this.label1.Font = gv.drawFontReg;
            this.label1.Text = headertxt;
            txtInput.Font = gv.drawFontReg;
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
