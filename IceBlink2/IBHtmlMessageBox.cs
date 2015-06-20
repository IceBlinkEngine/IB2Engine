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
    public partial class IBHtmlMessageBox : IBForm
    {
        public IbbHtmlLogBox hlb;
        GameView gv;

        public IBHtmlMessageBox(GameView g, string htmlstring)
        {
            InitializeComponent();            
            gv = g;
            button1.Text = "RETURN";
            button1.Font = gv.drawFontReg;
            this.IceBlinkButtonClose.Enabled = false;
            this.IceBlinkButtonResize.Enabled = false;
            this.IceBlinkButtonClose.Visible = false;
            this.IceBlinkButtonResize.Visible = false;
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.form_MouseWheel);
            hlb = new IbbHtmlLogBox(this, gv, 10, 30, 398, 400);                       
            hlb.AddHtmlTextToLog(htmlstring);
            hlb.numberOfLinesToShow = 16;
            hlb.AddHtmlTextToLog("");
            hlb.currentTopLineIndex = 0;
            //hlb.scrollToEnd();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Call the OnPaint method of the base class.
            base.OnPaint(e);
            hlb.onDrawLogBox(e.Graphics);
        }
        private void form_MouseWheel(object sender, MouseEventArgs e)
        {
            hlb.onMouseWheel(sender, e);
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            hlb.onMouseUp(sender, e);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        
        public void ResizeForm()
        {
            int totalHeight = hlb.logLinesList.Count * hlb.font.Height + hlb.tbXloc + 50;
            if (totalHeight > 500)
            {
                this.Height = 500;
            }
            else
            {
                this.Height = totalHeight;
            }
            hlb.tbHeight = this.Height - hlb.tbXloc - 100;
            hlb.numberOfLinesToShow = (hlb.tbHeight / hlb.font.Height) - 7;
        }
    }
}
