using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using SharpDX.Windows;

namespace IceBlink2
{
    public partial class IBForm : RenderForm
    {
        private bool moveable;
        public bool dontChange = false;
        private Point currentPosition;
        private const int cGrip = 0; //Grip size
        private string ibTitle = "";
        private Color ibTitleForeColor = Color.White;
        private Color ibTitleShadowColor = Color.Black;
        private Color ibBorderOutsideColor = Color.DarkGray;
        private Color ibBorderMiddleColor = Color.Gray;
        private Color ibBorderInsideColor = Color.DimGray;

        [Category("Appearance")]
        [Description("Gets or sets the title of the title bar")]
        public string IBTitle
        {
            set
            {
                ibTitle = value;
            }
            get
            {
                return ibTitle;
            }
        }
        [Category("Appearance")]
        [Description("Gets or sets the title text color")]
        public Color IBTitleForeColor
        {
            set
            {
                ibTitleForeColor = value;
            }
            get
            {
                return ibTitleForeColor;
            }
        }
        [Category("Appearance")]
        [Description("Gets or sets the title background color")]
        public Color IBTitleShadowColor
        {
            set
            {
                ibTitleShadowColor = value;
            }
            get
            {
                return ibTitleShadowColor;
            }
        }
        [Category("Appearance")]
        [Description("Gets or sets the border outside color")]
        public Color IBBorderOutsideColor
        {
            get { return ibBorderOutsideColor; }
            set { ibBorderOutsideColor = value; }            
        }
        [Category("Appearance")]
        [Description("Gets or sets the border middle color")]
        public Color IBBorderMiddleColor
        {
            get { return ibBorderMiddleColor; }
            set { ibBorderMiddleColor = value; }
        }
        [Category("Appearance")]
        [Description("Gets or sets the border inside color")]
        public Color IBBorderInsideColor
        {
            get { return ibBorderInsideColor; }
            set { ibBorderInsideColor = value; }
        }
        public IceBlinkButtonClose IceBlinkButtonClose
        {
            set
            {
                iceBlinkButtonClose1 = value;
            }
            get
            {
                return iceBlinkButtonClose1;
            }
        }
        public IceBlinkButtonResize IceBlinkButtonResize
        {
            set
            {
                iceBlinkButtonResize1 = value;
            }
            get
            {
                return iceBlinkButtonResize1;
            }
        }

        public IBForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
        public void setupAll(GameView gv)
        {
            loadHeaderImage(gv);
            //this.IBBorderInsideColor = gv.module.ModuleTheme.IBBorderInsideColor;
            //this.IBBorderMiddleColor = gv.module.ModuleTheme.IBBorderMiddleColor;
            //this.IBBorderOutsideColor = gv.module.ModuleTheme.IBBorderOutsideColor;            
        }
        private void loadHeaderImage(GameView gv)
        {
            try
            {
                if (gv.mod != null)
                {
                    if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\ui\\btn_lrg_header.png"))
                    {
                        this.pnlTitle.BackgroundImage = (Image)new Bitmap(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\ui\\btn_lrg_header.png");
                    }
                    else
                    {
                        this.pnlTitle.BackgroundImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_lrg_header.png");
                    }
                }
                else
                {
                    this.pnlTitle.BackgroundImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_lrg_header.png");
                }
            }
            catch { }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);

            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                                  IBBorderInsideColor, 3, ButtonBorderStyle.Solid,
                                  IBBorderInsideColor, 3, ButtonBorderStyle.Solid,
                                  IBBorderInsideColor, 3, ButtonBorderStyle.Solid,
                                  IBBorderInsideColor, 3, ButtonBorderStyle.Solid); //left, top, right, bottom
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                                  IBBorderMiddleColor, 2, ButtonBorderStyle.Solid,
                                  IBBorderMiddleColor, 2, ButtonBorderStyle.Solid,
                                  IBBorderMiddleColor, 2, ButtonBorderStyle.Solid,
                                  IBBorderMiddleColor, 2, ButtonBorderStyle.Solid); //left, top, right, bottom
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                                  IBBorderOutsideColor, 1, ButtonBorderStyle.Solid,
                                  IBBorderOutsideColor, 1, ButtonBorderStyle.Solid,
                                  IBBorderOutsideColor, 1, ButtonBorderStyle.Solid,
                                  IBBorderOutsideColor, 1, ButtonBorderStyle.Solid); //left, top, right, bottom            
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84) // Trap WM_NCHITTEST
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }
        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }*/
        private void pnlTitle_MouseDown(object sender, MouseEventArgs e)
        {
            moveable = true;
            currentPosition.X = e.X;
            currentPosition.Y = e.Y;
        }
        private void pnlTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveable)
            {
                Point newPosition = Control.MousePosition;
                newPosition.X = newPosition.X - currentPosition.X; // .Offset(mouseOffset.X, mouseOffset.Y);                
                newPosition.Y = newPosition.Y - currentPosition.Y;
                this.Location = newPosition;
            }
        }
        private void pnlTitle_MouseUp(object sender, MouseEventArgs e)
        {
            moveable = false;
        }
        private void pnlTitle_Paint(object sender, PaintEventArgs e)
        {
            if (IBTitle.Length > 0)
            {
                DrawTitleTextShadowOutline(e, 100, -10, IBTitle, 100, 255, Font.FontFamily, Font.Size, IBTitleForeColor, IBTitleShadowColor);
            }
        }
        private void iceBlinkButtonClose1_Click(object sender, EventArgs e)
        {
            
            if (!dontChange)
            {
                this.Close();
            }
            dontChange = false;
            
        }
        private void iceBlinkButtonResize1_Click(object sender, EventArgs e)
        {
            
            if (!dontChange)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                else if (this.WindowState == FormWindowState.Normal)
                {
                    //this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
                    this.WindowState = FormWindowState.Maximized;
                }
                this.Invalidate();
                this.Show();
            }
            dontChange = false;
            
        }
        public void DrawTitleTextShadowOutline(PaintEventArgs e, int x, int y, string text, int aShad, int aText, FontFamily font, float fontPointSize, Color textColor, Color shadowColor)
        {
            try
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                StringFormat strformat = new StringFormat();
                strformat.Alignment = StringAlignment.Center;
                strformat.LineAlignment = StringAlignment.Center;

                GraphicsPath path = new GraphicsPath();
                float emSize = e.Graphics.DpiY * fontPointSize / 72;
                Rectangle rect = new Rectangle(this.pnlTitle.DisplayRectangle.X, this.pnlTitle.DisplayRectangle.Y, this.pnlTitle.DisplayRectangle.Width, 22);
                path.AddString(text, font, (int)FontStyle.Regular, emSize, rect, strformat);
                for (int i = 1; i < 6; ++i)
                {
                    Pen pen = new Pen(Color.FromArgb(aShad, shadowColor), i);
                    pen.LineJoin = LineJoin.Round;
                    e.Graphics.DrawPath(pen, path);
                    pen.Dispose();
                }

                SolidBrush brush = new SolidBrush(Color.FromArgb(aText, textColor));
                e.Graphics.FillPath(brush, path);

                path.Dispose();
                brush.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("draw text on button not working: " + ex.ToString());
            }
        }

        private void IBForm_KeyUp(object sender, KeyEventArgs e)
        {
            dontChange = true;
        }
    }

    public class IBPanel : Panel
    {
        public IBPanel()
        {
            this.DoubleBuffered = true;
        }
    }
}
