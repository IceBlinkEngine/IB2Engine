using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class IbbHtmlTextBox
    {
        public GameView gv;
        public List<string> tagStack = new List<string>();
        public List<FormattedLine> logLinesList = new List<FormattedLine>();
        float xLoc = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;
        public bool showBoxBorder = false;

        public IbbHtmlTextBox(GameView g, int locX, int locY, int width, int height)
        {
            gv = g;
            //fontfamily = gv.family;
            //font = new Font(fontfamily, 20.0f * (float)gv.squareSize / 100.0f);
            //font = gv.drawFontReg;
            tbXloc = locX;
            tbYloc = locY;
            tbWidth = width;
            tbHeight = height;
            //brush.Color = Color.Red;
        }
        public IbbHtmlTextBox(GameView g)
        {
            gv = g;
            //fontfamily = gv.family;
            //font = new Font(fontfamily, 20.0f * (float)gv.squareSize / 100.0f);
            //font = gv.drawFontReg;
            //brush.Color = Color.Red;
        }

        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bmp, int x, int y)
        {
            //Rectangle src = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //Rectangle dst = new Rectangle(x + tbXloc, y + tbYloc + gv.oYshift, bmp.Width, bmp.Height);
            //g.DrawImage(bmp, dst, src, GraphicsUnit.Pixel);
            IbRect src = new IbRect(0, 0, bmp.PixelSize.Width, bmp.PixelSize.Height);
            IbRect dst = new IbRect(x + tbXloc, y + tbYloc, bmp.PixelSize.Width, bmp.PixelSize.Height);
            gv.DrawBitmap(bmp, src, dst);
        }
        public void DrawString(string text, float x, float y, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, SharpDX.Color fontColor, float fontHeight, bool isUnderlined)
        {
            if ((y > -2) && (y <= tbHeight - fontHeight))
            {
                gv.DrawText(text, x + tbXloc, y + tbYloc, fw, fs, 1.0f, fontColor, isUnderlined);
            }
        }

        public void AddHtmlTextToLog(string htmlText)
        {            
            htmlText = htmlText.Replace("\r\n", "<br>");
            htmlText = htmlText.Replace("\n\n", "<br>");
            htmlText = htmlText.Replace("\"", "'");

            if ((htmlText.EndsWith("<br>")) || (htmlText.EndsWith("<BR>")))
            {
                //ProcessHtmlString(htmlText, tbWidth);
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText, tbWidth, tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                //ProcessHtmlString(htmlText + "<br>", tbWidth);
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText + "<br>", tbWidth, tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
        }
        /*public void ProcessHtmlString(string text, int width)
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
                        newWord.fontWeight = GetFontWeight();
                        newWord.fontSize = GetFontSizeInPixels();
                        newWord.color = GetColor();
                        gv.textFormat = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, newWord.fontWeight, newWord.fontStyle, FontStretch.Normal, newWord.fontSize) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
                        gv.textLayout = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, newWord.text + " ", gv.textFormat, gv.Width, gv.Height);
                        //font = new Font(gv.family, newWord.fontSize, newWord.fontStyle);
                        float height = gv.textLayout.Metrics.Height;
                        float wordWidth = gv.textLayout.Metrics.WidthIncludingTrailingWhitespace;
                        if (height > lineHeight) { lineHeight = (int)height; }
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
                            newWord.fontWeight = GetFontWeight();
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
                        newWord.fontWeight = GetFontWeight();
                        newWord.fontSize = GetFontSizeInPixels();
                        newWord.color = GetColor();
                        gv.textFormat = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, newWord.fontWeight, newWord.fontStyle, FontStretch.Normal, newWord.fontSize) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
                        gv.textLayout = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, newWord.text + " ", gv.textFormat, gv.Width, gv.Height);
                        //font = new Font(gv.family, newWord.fontSize, newWord.fontStyle);
                        float wordWidth = gv.textLayout.Metrics.WidthIncludingTrailingWhitespace;
                        float height = gv.textLayout.Metrics.Height;
                        if (height > lineHeight) { lineHeight = (int)height; }

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
        }*/

        public void onDrawLogBox()
        {
            //only draw lines needed to fill textbox
            float xLoc = 0;
            float yLoc = 0;
            //loop through 5 lines from current index point
            for (int i = 0; i < logLinesList.Count; i++)
            {
                //loop through each line and print each word
                foreach (FormattedWord word in logLinesList[i].wordsList)
                {
                    if (gv.textFormat != null)
                    {
                        gv.textFormat.Dispose();
                        gv.textFormat = null;
                    }

                    if (gv.textLayout != null)
                    {
                        gv.textLayout.Dispose();
                        gv.textLayout = null;
                    }
                    gv.textFormat = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, word.fontWeight, word.fontStyle, FontStretch.Normal, word.fontSize) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
                    gv.textLayout = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, word.text + " ", gv.textFormat, gv.Width, gv.Height);
                    float ht = gv.textLayout.Metrics.Height;
                    float wd = gv.textLayout.Metrics.Width;
                    int difYheight = logLinesList[i].lineHeight - (int)word.fontSize;                    
                    DrawString(word.text + " ", xLoc, yLoc + difYheight, word.fontWeight, word.fontStyle, word.color, word.fontSize, word.underlined);
                    //gv.DrawRectangle(new IbRect((int)xLoc + tbXloc, (int)yLoc + difYheight + tbYloc, (int)wd, (int)ht), SharpDX.Color.White, 1);
                    xLoc += gv.textLayout.Metrics.WidthIncludingTrailingWhitespace;

                    //OLD STUFF
                    //print each word and move xLoc
                    //font = new Font(fontfamily, word.fontSize, word.fontStyle);
                    //int wordWidth = (int)(gv.gCanvas.MeasureString(word.text, font)).Width;
                    //int wordWidth = 12;
                    //brush.Color = word.color;
                    //int difYheight = logLinesList[i].lineHeight - (int)word.fontSize;
                    //DrawString(word.text, xLoc, yLoc + difYheight, word.fontWeight, word.fontStyle, word.color);
                    //xLoc += wordWidth;
                }
                xLoc = 0;
                yLoc += logLinesList[i].lineHeight;
            }

            //draw border for debug info
            if (showBoxBorder)
            {
                gv.DrawRectangle(new IbRect(tbXloc, tbYloc, tbWidth, tbHeight), Color.DimGray, 1);
            }
        }

        /*private Color GetColor()
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
        private SharpDX.DirectWrite.FontStyle GetFontStyle()
        {
            SharpDX.DirectWrite.FontStyle style = SharpDX.DirectWrite.FontStyle.Normal;
            foreach (string s in tagStack)
            {
                if (s == "i")
                {
                    style = style | SharpDX.DirectWrite.FontStyle.Italic;
                }
            }
            return style;
        }
        private SharpDX.DirectWrite.FontWeight GetFontWeight()
        {
            SharpDX.DirectWrite.FontWeight style = SharpDX.DirectWrite.FontWeight.Normal;
            foreach (string s in tagStack)
            {
                if (s == "b")
                {
                    style = style | SharpDX.DirectWrite.FontWeight.Bold;
                }
            }
            return style;
        }
        private float GetFontSizeInPixels()
        {
            float fSize = gv.drawFontRegHeight * (float)gv.squareSize / 100.0f;
            foreach (string s in tagStack)
            {
                if (s == "big")
                {
                    fSize = gv.drawFontLargeHeight * (float)gv.squareSize / 100.0f;
                }
                else if (s == "small")
                {
                    fSize = gv.drawFontSmallHeight * (float)gv.squareSize / 100.0f;
                }
            }
            return fSize;
        }*/
    }
}
