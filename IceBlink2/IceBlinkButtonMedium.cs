using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace IceBlink2
{
   public  class IceBlinkButtonMedium : System.Windows.Forms.Button
    {
       //private Game game;
       System.Media.SoundPlayer playerButtonEnter = new System.Media.SoundPlayer();
       System.Media.SoundPlayer playerButtonClick = new System.Media.SoundPlayer();
       private Image normalImage;
       private Image hoverImage;
       private Image pressedImage;
       private Image disabledImage;
       private bool pressed = false;
       private bool hover = false;
       private string ibText;

       public Image NormalImage
       {
           get { return this.normalImage; }
           set { this.normalImage = value; }
       }
       public Image HoverImage
       {
           get { return this.hoverImage; }
           set { this.hoverImage = value; }
       }
       public Image PressedImage
       {
           get { return this.pressedImage; }
           set { this.pressedImage = value; }
       }
       public Image DisabledImage
       {
           get { return this.disabledImage; }
           set { this.disabledImage = value; }
       }
       public string TextIB
       {
           get { return this.ibText; }
           set { this.ibText = value; }
       }
     
       public IceBlinkButtonMedium()
       {
           //game = g;           
           // 
           // iceBlinkButton1
           // 
           this.BackColor = System.Drawing.Color.Transparent;
           this.BackgroundImage = global::IceBlink2.Properties.Resources.btn_large;
           this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
           this.FlatAppearance.BorderSize = 0;
           this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
           this.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255); //transparent
           this.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 255, 255, 255); //transparent
           this.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 255, 255, 255); //transparent
           this.HoverImage = global::IceBlink2.Properties.Resources.btn_large;
           this.Name = "iceBlinkButton1";
           this.NormalImage = global::IceBlink2.Properties.Resources.btn_large;
           this.PressedImage = global::IceBlink2.Properties.Resources.btn_large;
           this.Size = new System.Drawing.Size(140, 30);
           this.Text = "";
           this.TextIB = "iceBlinkButton1";
           this.UseVisualStyleBackColor = false;
       }

       public void setupAll(GameView g)
       {
           loadSounds(g);
           loadButtonImages(g);
       }
       private void loadButtonImages(GameView gv)
       {
           try
           {
               if (gv.mod != null)
               {
                   if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\ui\\btn_close_normal.png"))
                   {
                       this.BackgroundImage = (Image)new Bitmap(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\ui\\btn_large.png");
                       this.HoverImage = (Image)new Bitmap(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\ui\\btn_large_on.png");
                       this.NormalImage = (Image)new Bitmap(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\ui\\btn_large.png");
                       this.PressedImage = (Image)new Bitmap(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\ui\\btn_large_on.png");
                   }
                   else
                   {
                       this.BackgroundImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_large.png");
                       this.HoverImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_large_on.png");
                       this.NormalImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_large.png");
                       this.PressedImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_large_on.png");
                   }
               }
               else
               {
                   this.BackgroundImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_large.png");
                   this.HoverImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_large_on.png");
                   this.NormalImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_large.png");
                   this.PressedImage = (Image)new Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\btn_large_on.png");
               }
           }
           catch { }
       }
       private void loadSounds(GameView gv)
       {
           try
           {
               if (gv.mod != null)
               {
                   if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\sounds\\btn_click.wav"))
                   {
                       playerButtonClick.SoundLocation = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\sounds\\btn_click.wav";
                   }
                   else
                   {
                       playerButtonClick.SoundLocation = gv.mainDirectory + "\\default\\NewModule\\sounds\\btn_click.wav";
                   }
               }
               else
               {
                   playerButtonClick.SoundLocation = gv.mainDirectory + "\\default\\NewModule\\sounds\\btn_click.wav";
               }
           }
           catch { }
           try
           {
               if (gv.mod != null)
               {
                   if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\sounds\\btn_hover.wav"))
                   {
                       playerButtonEnter.SoundLocation = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\sounds\\btn_hover.wav";
                   }
                   else
                   {
                       playerButtonEnter.SoundLocation = gv.mainDirectory + "\\default\\NewModule\\sounds\\btn_hover.wav";
                   }
               }
               else
               {
                   playerButtonEnter.SoundLocation = gv.mainDirectory + "\\default\\NewModule\\sounds\\btn_hover.wav";
               }
           }
           catch (Exception ex) { MessageBox.Show(ex.ToString()); }       
       }

       // When the mouse button is pressed, set the "pressed" flag to true 
       // and invalidate the form to cause a repaint.  The .NET Compact Framework 
       // sets the mouse capture automatically.
       protected override void OnMouseDown(MouseEventArgs e)
       {
           this.pressed = true;
           try
           {
               playerButtonClick.Play();
           }
           catch (Exception ex) { MessageBox.Show(ex.ToString()); }
           this.Invalidate();
           base.OnMouseDown(e);
       }

       // When the mouse is released, reset the "pressed" flag 
       // and invalidate to redraw the button in the unpressed state.
       protected override void OnMouseUp(MouseEventArgs e)
       {
           this.pressed = false;
           this.Invalidate();
           base.OnMouseUp(e);
       }

       protected override void OnMouseEnter(EventArgs e)
       {
           this.hover = true;
           try
           {
               playerButtonEnter.Play();
           }
           catch (Exception ex) { MessageBox.Show(ex.ToString()); }
           this.Invalidate();
           base.OnMouseEnter(e);
       }

       protected override void OnMouseLeave(EventArgs e)
       {
           this.hover = false;
           this.Invalidate();
           base.OnMouseLeave(e);
       }

       protected override void OnMouseHover(EventArgs e)
       {
           this.hover = true;
           this.Invalidate();
           base.OnMouseHover(e);
       }

       // Override the OnPaint method to draw the background image and the text.
       protected override void OnPaint(PaintEventArgs e)
       {
           base.OnPaint(e);
           if (this.Enabled)
           {
               if ((this.pressed) && (this.PressedImage != null))
               {
                   this.BackgroundImage = PressedImage;
               }
               else if ((this.hover) && (this.HoverImage != null))
               {
                   this.BackgroundImage = HoverImage;
               }
               else
               {
                   this.BackgroundImage = NormalImage;
               }
           }
           else
           {
               this.BackgroundImage = DisabledImage;
           }
           int x = this.Width / 2;
           int y = this.Height / 2;
           DrawButtonTextShadowOutline(e, x, y, TextIB, 100, 255, Font.FontFamily, Font.Size, Color.White, Color.Black);
       }

       public void DrawButtonTextShadowOutline(PaintEventArgs e, int x, int y, string text, int aShad, int aText, FontFamily font, float fontPointSize, Color textColor, Color shadowColor)
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
               path.AddString(text, font, (int)FontStyle.Regular, emSize, this.DisplayRectangle, strformat);
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
    }
}
