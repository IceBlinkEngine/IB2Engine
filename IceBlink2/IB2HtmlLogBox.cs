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
        public List<string> tagStack = new List<string>();
        public List<FormattedLine> logLinesList = new List<FormattedLine>();
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
            if ((y > -2) && (y <= tbHeight - fontHeight))
            {
                gv.DrawText(text, x + tbXloc + gv.pS, y, fw, fs, 1.0f, fontColor, isUnderlined);
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
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText, tbWidth, tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText + "<br>", tbWidth, tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            scrollToEnd();
        }
        public void onDrawLogBox(IB2Panel parentPanel)
        {
            //ratio of #lines to #pixels
            float ratio = (float)(logLinesList.Count) / (float)(tbHeight);
            if (ratio < 1.0f) { ratio = 1.0f; }
            if (moveDeltaY != 0)
            {
                int lineMove = (startY + moveDeltaY) * (int)ratio;
                SetCurrentTopLineAbsoluteIndex(lineMove);
            }
            //only draw lines needed to fill textbox
            float xLoc = 0;
            float yLoc = 3.0f;
            int maxLines = currentTopLineIndex + numberOfLinesToShow;
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
                    DrawString(word.text + " ", parentPanel.currentLocX + xLoc, parentPanel.currentLocY + yLoc + difYheight, word.fontWeight, word.fontStyle, word.color, word.fontSize, word.underlined);
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
            if ((e.X > tbXloc) && (e.X < tbWidth + tbXloc) && (e.Y > tbYloc) && (e.Y < tbHeight + tbYloc))
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
                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

                if (numberOfTextLinesToMove != 0)
                {
                    SetCurrentTopLineIndex(-numberOfTextLinesToMove);
                    //gv.Invalidate();
                    gv.Render();
                }
            }
        }        
    }
}
