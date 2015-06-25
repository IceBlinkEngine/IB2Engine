using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class IbbHtmlTextBox
    {
        public GameView gv;

        public SolidBrush brush = new SolidBrush(Color.Black);
        public FontFamily fontfamily;
        public Font font;
        
        public List<string> tagStack = new List<string>();
        public List<FormattedLine> logLinesList = new List<FormattedLine>();
        int xLoc = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;
        public bool showBoxBorder = false;

        public IbbHtmlTextBox(GameView g, int locX, int locY, int width, int height)
        {
            gv = g;
            fontfamily = gv.family;
            font = new Font(fontfamily, 20.0f * (float)gv.squareSize / 100.0f);
            //font = gv.drawFontReg;
            tbXloc = locX;
            tbYloc = locY;
            tbWidth = width;
            tbHeight = height;
            brush.Color = Color.Red;
        }
        public IbbHtmlTextBox(GameView g)
        {
            gv = g;
            fontfamily = gv.family;
            font = new Font(fontfamily, 20.0f * (float)gv.squareSize / 100.0f);
            //font = gv.drawFontReg;
            brush.Color = Color.Red;
        }

        public void DrawBitmap(Graphics g, Bitmap bmp, int x, int y)
        {
            Rectangle src = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle dst = new Rectangle(x + tbXloc, y + tbYloc + gv.oYshift, bmp.Width, bmp.Height);
            g.DrawImage(bmp, dst, src, GraphicsUnit.Pixel);
        }
        public void DrawString(Graphics g, string text, Font f, SolidBrush sb, int x, int y)
        {
            if ((y > -2) && (y <= tbHeight - f.Height))
            {
                g.DrawString(text, f, sb, x + tbXloc, y + tbYloc + gv.oYshift);
            }
        }

        public void AddHtmlTextToLog(string htmlText)
        {
            htmlText = htmlText.Replace("\r\n", " ");
            htmlText = htmlText.Replace("\"", "'");

            if ((htmlText.EndsWith("<br>")) || (htmlText.EndsWith("<BR>")))
            {
                ProcessHtmlString(htmlText, tbWidth);
            }
            else
            {
                ProcessHtmlString(htmlText + "<br>", tbWidth);
            }
        }
        public void ProcessHtmlString(string text, int width)
        {
            bool tagMode = false;
            string tag = "";
            FormattedWord newWord = new FormattedWord();
            FormattedLine newLine = new FormattedLine();
            int lineHeight = 0;

            foreach (char c in text)
            {
                #region Start/Stop Tags
                //start a tag and check for end of word
                if (c == '<')
                {
                    tagMode = true;

                    if (newWord.text != "")
                    {
                        newWord.fontStyle = GetFontStyle();
                        newWord.fontSize = GetFontSizeInPixels();
                        newWord.color = GetColor();
                        font = new Font(fontfamily, newWord.fontSize, newWord.fontStyle);
                        int wordWidth = (int)((font.Size / fontHeightToWidthRatio) * (float)newWord.text.Length);
                        if (font.Height > lineHeight) { lineHeight = font.Height; }
                        //int wordWidth = (int)(frm.gCanvas.MeasureString(newWord.word, font)).Width;
                        if (xLoc + wordWidth > width) //word wrap
                        {
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        else //no word wrap, just add word
                        {
                            newLine.wordsList.Add(newWord);
                        }
                        //instead of drawing, just add to line list 
                        //DrawString(g, word, font, brush, xLoc, yLoc);
                        xLoc += wordWidth;
                        newWord = new FormattedWord();
                    }
                    continue;
                }
                //end a tag
                else if (c == '>')
                {
                    //check for ending type tag
                    if (tag.StartsWith("/"))
                    {
                        //if </>, remove corresponding tag from stack
                        string tagMinusSlash = tag.Substring(1);
                        if (tag.StartsWith("/font"))
                        {
                            for (int i = tagStack.Count - 1; i > 0; i--)
                            {
                                if (tagStack[i].StartsWith("font"))
                                {
                                    tagStack.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            tagStack.Remove(tagMinusSlash);
                        }
                    }
                    else
                    {
                        //check for line break
                        if ((tag.ToLower() == "br") || (tag == "BR"))
                        {
                            newWord.fontStyle = GetFontStyle();
                            newWord.fontSize = GetFontSizeInPixels();
                            newWord.color = GetColor();
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            //newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        //else if <>, add this tag to the stack
                        tagStack.Add(tag);
                    }
                    tagMode = false;
                    tag = "";
                    continue;
                }
                #endregion

                #region Words
                if (!tagMode)
                {
                    if (c != ' ') //keep adding to word until hit a space
                    {
                        newWord.text += c;
                    }
                    else //hit a space so end word
                    {
                        newWord.fontStyle = GetFontStyle();
                        newWord.fontSize = GetFontSizeInPixels();
                        newWord.color = GetColor();
                        font = new Font(fontfamily, newWord.fontSize, newWord.fontStyle);
                        int wordWidth = (int)((font.Size / fontHeightToWidthRatio) * (float)newWord.text.Length);
                        if (font.Height > lineHeight) { lineHeight = font.Height; }
                        //int wordWidth = (int)(g.MeasureString(newWord.word, font)).Width;
                        if (xLoc + wordWidth > width) //word wrap
                        {
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        else //no word wrap, just add word
                        {
                            newLine.wordsList.Add(newWord);
                        }
                        //instead of drawing, just add to line list 
                        //DrawString(g, word, font, brush, xLoc, yLoc);
                        xLoc += wordWidth;
                        newWord = new FormattedWord();
                    }
                }
                else if (tagMode)
                {
                    tag += c;
                }
                #endregion
            }
        }

        public void onDrawLogBox(Graphics g)
        {
            //only draw lines needed to fill textbox
            int xLoc = 0;
            int yLoc = 0;
            //loop through 5 lines from current index point
            for (int i = 0; i < logLinesList.Count; i++)
            {
                //loop through each line and print each word
                foreach (FormattedWord word in logLinesList[i].wordsList)
                {
                    //print each word and move xLoc
                    font = new Font(fontfamily, word.fontSize, word.fontStyle);
                    int wordWidth = (int)(g.MeasureString(word.text, font)).Width;
                    brush.Color = word.color;
                    int difYheight = logLinesList[i].lineHeight - font.Height;
                    DrawString(g, word.text, font, brush, xLoc, yLoc + difYheight);
                    xLoc += wordWidth;
                }
                xLoc = 0;
                yLoc += logLinesList[i].lineHeight;
            }

            //draw border for debug info
            if (showBoxBorder)
            {
                g.DrawRectangle(new Pen(Color.DimGray), new Rectangle(tbXloc, tbYloc + gv.oYshift, tbWidth, tbHeight));
            }
        }

        private Color GetColor()
        {
            //will end up using the last color on the stack
            Color clr = Color.White;
            foreach (string s in tagStack)
            {
                if ((s == "font color='red'") || (s == "font color = 'red'"))
                {
                    clr = Color.Red;
                }
                else if ((s == "font color='lime'") || (s == "font color = 'lime'"))
                {
                    clr = Color.Lime;
                }
                else if ((s == "font color='black'") || (s == "font color = 'black'"))
                {
                    clr = Color.Black;
                }
                else if ((s == "font color='white'") || (s == "font color = 'white'"))
                {
                    clr = Color.White;
                }
                else if ((s == "font color='silver'") || (s == "font color = 'silver'"))
                {
                    clr = Color.Gray;
                }
                else if ((s == "font color='grey'") || (s == "font color = 'grey'"))
                {
                    clr = Color.DimGray;
                }
                else if ((s == "font color='aqua'") || (s == "font color = 'aqua'"))
                {
                    clr = Color.Aqua;
                }
                else if ((s == "font color='fuchsia'") || (s == "font color = 'fuchsia'"))
                {
                    clr = Color.Fuchsia;
                }
                else if ((s == "font color='yellow'") || (s == "font color = 'yellow'"))
                {
                    clr = Color.Yellow;
                }
            }
            return clr;
        }
        private FontStyle GetFontStyle()
        {
            FontStyle style = FontStyle.Regular;
            foreach (string s in tagStack)
            {
                if (s == "b")
                {
                    style = style | FontStyle.Bold;
                }
                else if (s == "i")
                {
                    style = style | FontStyle.Italic;
                }
                else if (s == "u")
                {
                    style = style | FontStyle.Underline;
                }
            }
            return style;
        }
        private float GetFontSizeInPixels()
        {
            //will end up using the last font size on the stack
            float fSize = 20.0f * (float)gv.squareSize / 100.0f;

            foreach (string s in tagStack)
            {
                if (s == "big")
                {
                    fSize = 28.0f * (float)gv.squareSize / 100.0f;
                }
                else if (s == "small")
                {
                    fSize = 16.0f * (float)gv.squareSize / 100.0f;
                }
            }
            return fSize;
        }
    }
}
