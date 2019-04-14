using Newtonsoft.Json;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class IB2HtmlLogBox
    {
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        [JsonIgnore]
        public List<string> tagStack = new List<string>();
        [JsonIgnore]
        public List<FormattedLine> logLinesList = new List<FormattedLine>();
        [JsonIgnore]
        public int currentTopLineIndex = 0;
        public int numberOfLinesToShow = 17;
        public float xLoc = 0;
        public int startY = 0;
        public int moveDeltaY = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;

        public IB2HtmlLogBox()
        {

        }

        public IB2HtmlLogBox(GameView g)
        {
            gv = g;
        }

        public void setupIB2HtmlLogBox(GameView g)
        {
            gv = g;
        }

        public void DrawString(string text, float x, float y, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, SharpDX.Color fontColor, float fontHeight, bool isUnderlined)
        {
            if ((y > -2) && (y <= (int)(tbHeight * gv.screenDensity) - fontHeight))
            {
                //hurgh21
                if (gv.mod.useMinimalisticUI)
                {
                    gv.DrawText(text, x + (int)(tbXloc * gv.screenDensity) + 2 * gv.pS, y, fw, fs, 1.0f, fontColor, isUnderlined);
                }
                else 
                {
                    gv.DrawText(text, x + (int)(tbXloc * gv.screenDensity) + gv.pS, y, fw, fs, 1.0f, fontColor, isUnderlined);
                }
            }
        }

        public void AddHtmlTextToLog(string htmlText)
        {
            //Remove any '\r\n' hard returns from message
            htmlText = htmlText.Replace("\r\n", "<br>");
            htmlText = htmlText.Replace("\n\n", "<br>");
            htmlText = htmlText.Replace("\"", "'");

            if ((htmlText.EndsWith("<br>")) || (htmlText.EndsWith("<BR>")))
            {
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText, (int)(tbWidth * gv.screenDensity), tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText + "<br>", (int)(tbWidth * gv.screenDensity), tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            gv.mod.logFadeCounter = 120;
            gv.mod.logOpacity = 1f;
            scrollToEnd();
        }
        public void onDrawLogBox(IB2Panel parentPanel)
        {
            //ratio of #lines to #pixels
            float ratio = (float)(logLinesList.Count) / (float)(tbHeight * gv.screenDensity);
            if (ratio < 1.0f) { ratio = 1.0f; }
            if (moveDeltaY != 0)
            {
                int lineMove = (startY + moveDeltaY) * (int)ratio;
                SetCurrentTopLineAbsoluteIndex(lineMove);
            }
            //only draw lines needed to fill textbox
            float xLoc = 0.0f;
            float yLoc = 3.0f;
            int maxLines = 0;

            if (gv.screenType.Equals("combat") && (gv.mod.useMinimalisticUI))
            {
                numberOfLinesToShow = 20;
            }

            if (gv.screenType.Equals("combat") && (gv.screenCombat.showIniBar) && (!gv.mod.useMinimalisticUI))
            {
                numberOfLinesToShow = 22;
            }

            if (gv.screenType.Equals("combat") && (!gv.screenCombat.showIniBar) && (!gv.mod.useMinimalisticUI))
            {
                numberOfLinesToShow = 26;
            }

            if (!gv.screenType.Equals("combat"))
            {
                numberOfLinesToShow = 22;
            }


            maxLines = currentTopLineIndex + numberOfLinesToShow;
            
            if (maxLines > logLinesList.Count) { maxLines = logLinesList.Count; }
            for (int i = currentTopLineIndex; i < maxLines; i++)
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
                    int difYheight = logLinesList[i].lineHeight - (int)word.fontSize;
                    if (word.underlined)
                    {
                        gv.textLayout.SetUnderline(true, new TextRange(0, word.text.Length - 1));
                    }
                    int xLoc2 = (int)((parentPanel.currentLocX * gv.screenDensity + xLoc));
                    int yLoc2 = (int)((parentPanel.currentLocY * gv.screenDensity + yLoc + difYheight));
                    int logOpac = (int)(255f * gv.mod.logOpacity);
                    word.color.A = (byte)(logOpac);
                    int yPositionModifier = 0;
                    
                    if (gv.mod.useComplexCoordinateSystem && !gv.screenType.Equals("combat"))
                    {
                        yPositionModifier = gv.squareSize - gv.pS;
                    }
                    
                    if (gv.screenType.Equals("combat") && (!gv.mod.useMinimalisticUI) && (gv.screenCombat.showIniBar))
                    {
                        yPositionModifier = gv.squareSize + 4 * gv.pS;
                    }
                   
                    DrawString(word.text + " ", xLoc2, yLoc2 + yPositionModifier, word.fontWeight, word.fontStyle, word.color, word.fontSize, word.underlined);
                    xLoc += gv.textLayout.Metrics.WidthIncludingTrailingWhitespace;
                }
                xLoc = 0;
                yLoc += logLinesList[i].lineHeight;
            }            
            //draw border for debug info
            //gv.DrawRectangle(new IbRect(parentPanel.currentLocX + tbXloc, parentPanel.currentLocY + tbYloc, tbWidth, tbHeight), SharpDX.Color.DimGray, 1);
        }

        public void scrollToEnd()
        {
            SetCurrentTopLineIndex(logLinesList.Count);
            //gv.Invalidate();
        }
        public void SetCurrentTopLineIndex(int changeValue)
        {
            currentTopLineIndex += changeValue;
            if (currentTopLineIndex > logLinesList.Count - numberOfLinesToShow)
            {
                currentTopLineIndex = logLinesList.Count - numberOfLinesToShow;
            }
            if (currentTopLineIndex < 0)
            {
                currentTopLineIndex = 0;
            }
        }
        public void SetCurrentTopLineAbsoluteIndex(int absoluteValue)
        {
            currentTopLineIndex = absoluteValue;
            if (currentTopLineIndex > logLinesList.Count - numberOfLinesToShow)
            {
                currentTopLineIndex = logLinesList.Count - numberOfLinesToShow;
            }
            if (currentTopLineIndex < 0)
            {
                currentTopLineIndex = 0;
            }
        }        
        private bool isMouseWithinTextBox(MouseEventArgs e)
        {
            if ((e.X > (int)(tbXloc * gv.screenDensity)) && (e.X < (int)(tbWidth * gv.screenDensity) + (int)(tbXloc * gv.screenDensity)) && (e.Y > (int)(tbYloc * gv.screenDensity)) && (e.Y < (int)(tbHeight * gv.screenDensity) + (int)(tbYloc * gv.screenDensity)))
            {
                return true;
            }
            return false;
        }        
        public void onMouseWheel(object sender, MouseEventArgs e)
        {
            if (isMouseWithinTextBox(e))
            {
                // Update the drawing based upon the mouse wheel scrolling. 
                gv.mod.logFadeCounter = 120;
                gv.mod.logOpacity = 1f;
                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

                if (numberOfTextLinesToMove != 0)
                {
                    SetCurrentTopLineIndex(-numberOfTextLinesToMove);
                    //gv.Invalidate();
                    //bloodbus
                    //gv.Render(0);
                }
            }
        }        
    }
}
