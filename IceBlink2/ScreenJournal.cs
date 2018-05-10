using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenJournal 
    {

	    //public gv.module gv.mod;
	    public GameView gv;
	    private int journalScreenQuestIndex = 0;
	    private int journalScreenEntryIndex = 0;	
	    private Bitmap journalBack;
	    private IbbButton btnReturnJournal = null;
	    public IbbButton ctrlUpArrow = null;
	    public IbbButton ctrlDownArrow = null;
	    public IbbButton ctrlLeftArrow = null;
	    public IbbButton ctrlRightArrow = null;
        private IbbHtmlTextBox description;
	
	    public ScreenJournal(Module m, GameView g)
	    {
		    //gv.mod = m;
		    gv = g;
	    }
	    public void setControlsStart()
	    {		
		    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;		
		    int xShift = gv.squareSize/2;
		    int yShift = gv.squareSize/2;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;

		    if (ctrlUpArrow == null)
		    {
			    ctrlUpArrow = new IbbButton(gv, 1.0f);
			    ctrlUpArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlUpArrow.Img2 = gv.cc.LoadBitmap("ctrl_up_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_up_arrow);
			    ctrlUpArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlUpArrow.X = 12 * gv.squareSize;
			    ctrlUpArrow.Y = 1 * gv.squareSize + pH * 2;
                ctrlUpArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlUpArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (ctrlDownArrow == null)
		    {
			    ctrlDownArrow = new IbbButton(gv, 1.0f);	
			    ctrlDownArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlDownArrow.Img2 = gv.cc.LoadBitmap("ctrl_down_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_down_arrow);
			    ctrlDownArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlDownArrow.X = 12 * gv.squareSize;
			    ctrlDownArrow.Y = 2 * gv.squareSize + pH * 3;
                ctrlDownArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlDownArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (ctrlLeftArrow == null)
		    {
			    ctrlLeftArrow = new IbbButton(gv, 1.0f);
			    ctrlLeftArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlLeftArrow.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    ctrlLeftArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlLeftArrow.X = 10 * gv.squareSize + xShift;
			    ctrlLeftArrow.Y = pH * 34;
                ctrlLeftArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlLeftArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (ctrlRightArrow == null)
		    {
			    ctrlRightArrow = new IbbButton(gv, 1.0f);
			    ctrlRightArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlRightArrow.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    ctrlRightArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlRightArrow.X = 11 * gv.squareSize + pW * 2 + xShift;
			    ctrlRightArrow.Y = pH * 34;
                ctrlRightArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlRightArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }				
		    if (btnReturnJournal == null)
		    {
			    btnReturnJournal = new IbbButton(gv, 1.2f);	
			    btnReturnJournal.Text = "RETURN";
			    btnReturnJournal.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnReturnJournal.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturnJournal.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnReturnJournal.Y = 9 * gv.squareSize + pH * 2;
                btnReturnJournal.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturnJournal.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		
	    }

        public void redrawJournal()
        {
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = pH * 5;
            int locX = 4 * gv.squareSize;
            int spacing = (int)gv.drawFontRegHeight + pH;
            int leftStartY = pH * 4;
    	    int tabStartY = pH * 40;
    	
    	    //IF BACKGROUND IS NULL, LOAD IMAGE
    	    if (journalBack == null)
    	    {
                gv.cc.DisposeOfBitmap(ref journalBack);
                journalBack = gv.cc.LoadBitmap("journalback");
    	    }
    	    //IF BUTTONS ARE NULL, LOAD BUTTONS
    	    if (btnReturnJournal == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    //DRAW BACKGROUND IMAGE
            IbRect src = new IbRect(0, 0, journalBack.PixelSize.Width, journalBack.PixelSize.Height);
            IbRect dst = new IbRect(2 * gv.squareSize, 0, (gv.squaresInWidth - 4) * gv.squareSize, (gv.squaresInHeight - 1) * gv.squareSize);
            gv.DrawBitmap(journalBack, src, dst);
        
            //MAKE SURE NO OUT OF INDEX ERRORS
    	    if (gv.mod.partyJournalQuests.Count > 0)
    	    {
	    	    if (journalScreenQuestIndex >= gv.mod.partyJournalQuests.Count)
	    	    {
	    		    journalScreenQuestIndex = 0;
	    	    }    	
	    	    if (journalScreenEntryIndex >= gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count)
	    	    {
	    		    journalScreenEntryIndex = 0;
	    	    }
    	    }
			
    	    //DRAW QUESTS
            Color color = Color.Black;
		    gv.DrawText("Active Quests:", locX, locY += leftStartY, 1.0f, color);
		    gv.DrawText("--------------", locX, locY += spacing, 1.0f, color);
            
            //draw Faction info first
            bool drawFactionQuest = false;
            if (gv.mod.moduleFactionsList != null)
            {
                foreach (Faction f in gv.mod.moduleFactionsList)
                {
                    if (f.showThisFactionInJournal)
                    {
                        drawFactionQuest = true;
                        break;
                    }
                }
            }

            if (drawFactionQuest)
            {
                //now add the faction quest and an entry for each drawn faction

                //1. No need to add the quest if it already exists
                bool factionQuestExistsAlready = false;
                foreach (JournalQuest jQ in gv.mod.partyJournalQuests)
                {
                    if (jQ.Tag == "factionQuest001")
                    {
                        factionQuestExistsAlready = true;
                        jQ.Entries.Clear();
                        break;
                    }
                }

                if (!factionQuestExistsAlready)
                {
                    JournalQuest factionQuest = new JournalQuest();
                    factionQuest.Tag = "factionQuest001";
                    factionQuest.Name = "Factions";
                    factionQuest.Entries.Clear();
                    gv.mod.partyJournalQuests.Add(factionQuest);
                }

                //2. update entries of faction quest
                foreach (JournalQuest jQ in gv.mod.partyJournalQuests)
                {
                    if (jQ.Tag == "factionQuest001")
                    {
                        int idCounter = 0;
                        foreach (Faction f in gv.mod.moduleFactionsList)
                        {
                            if (f.showThisFactionInJournal)
                            {
                                JournalEntry factionEntry = new JournalEntry();
                                factionEntry.EntryId = idCounter;
                                factionEntry.EntryTitle = f.name;
                                factionEntry.EntryText = "";
                                //Rank 1 (27 of 99, +1 every 24h), +4 buff to AC/toHit/Saves
                                if (f.showRankInJournal)
                                {
                                    if (f.displayRankInWords)
                                    {
                                        if (f.rank == 1)
                                        {
                                            factionEntry.EntryText += f.nameRank1 + " ";
                                        }
                                        else if (f.rank == 2)
                                        {
                                            factionEntry.EntryText += f.nameRank2 + " ";
                                        }
                                        else if (f.rank == 3)
                                        {
                                            factionEntry.EntryText += f.nameRank3 + " ";
                                        }
                                        else if (f.rank == 4)
                                        {
                                            factionEntry.EntryText += f.nameRank4 + " ";
                                        }
                                        else if (f.rank == 5)
                                        {
                                            factionEntry.EntryText += f.nameRank5 + " ";
                                        }
                                        else if (f.rank == 6)
                                        {
                                            factionEntry.EntryText += f.nameRank6 + " ";
                                        }
                                        else if (f.rank == 7)
                                        {
                                            factionEntry.EntryText += f.nameRank7 + " ";
                                        }
                                        else if (f.rank == 8)
                                        {
                                            factionEntry.EntryText += f.nameRank8 + " ";
                                        }
                                        else if (f.rank == 9)
                                        {
                                            factionEntry.EntryText += f.nameRank9 + " ";
                                        }
                                        else if (f.rank == 10)
                                        {
                                            factionEntry.EntryText += f.nameRank10 + " ";
                                        }
                                    }
                                    else
                                    {
                                        if (f.rank == 1)
                                        {
                                            factionEntry.EntryText += "Rank 1 ";
                                        }
                                        else if (f.rank == 2)
                                        {
                                            factionEntry.EntryText += "Rank 2 ";
                                        }
                                        else if (f.rank == 3)
                                        {
                                            factionEntry.EntryText += "Rank 3 ";
                                        }
                                        else if (f.rank == 4)
                                        {
                                            factionEntry.EntryText += "Rank 4 ";
                                        }
                                        else if (f.rank == 5)
                                        {
                                            factionEntry.EntryText += "Rank 5 ";
                                        }
                                        else if (f.rank == 6)
                                        {
                                            factionEntry.EntryText += "Rank 6 ";
                                        }
                                        else if (f.rank == 7)
                                        {
                                            factionEntry.EntryText += "Rank 7 ";
                                        }
                                        else if (f.rank == 8)
                                        {
                                            factionEntry.EntryText += "Rank 8 ";
                                        }
                                        else if (f.rank == 9)
                                        {
                                            factionEntry.EntryText += "Rank 9 ";
                                        }
                                        else if (f.rank == 9)
                                        {
                                            factionEntry.EntryText += "Rank 9 ";
                                        }
                                        else if (f.rank == 10)
                                        {
                                            factionEntry.EntryText += "Rank 10 ";
                                        }
                                    }
                                }

                                if ((f.showStrengthInJournal || f.showChangeRateInJournal) && (f.showRankInJournal))
                                {
                                    factionEntry.EntryText += "(";
                                }
                                if (f.showStrengthInJournal)
                                {
                                    if (f.rank == 1)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank2.ToString();
                                    }
                                    else if (f.rank == 2)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank3.ToString();
                                    }
                                    else if (f.rank == 3)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank4.ToString();
                                    }
                                    else if (f.rank == 4)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank5.ToString();
                                    }
                                    else if (f.rank == 5)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank6.ToString();
                                    }
                                    else if (f.rank == 6)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank7.ToString();
                                    }
                                    else if (f.rank == 7)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank8.ToString();
                                    }
                                    else if (f.rank == 8)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank9.ToString();
                                    }
                                    else if (f.rank == 9)
                                    {
                                        factionEntry.EntryText += f.strength.ToString() + " of " + f.factionStrengthRequiredForRank10.ToString();
                                    }
                                    else if (f.rank == 10)
                                    {
                                        factionEntry.EntryText += f.strength.ToString();
                                    }
                                }

                                if ((f.showStrengthInJournal && !f.showChangeRateInJournal) && f.showRankInJournal)
                                {
                                    factionEntry.EntryText += ") ";
                                }
                              
                                if ((f.showChangeRateInJournal) && (f.showRankInJournal || f.showStrengthInJournal))
                                {
                                    if (f.showRankInJournal && f.showStrengthInJournal)
                                    {
                                        factionEntry.EntryText += ", " + f.amountOfFactionStrengthChangePerInterval.ToString() + " every " + f.intervalOfFactionStrengthChangeInHours.ToString() + "h)";
                                    }
                                    else if (f.showRankInJournal && !f.showStrengthInJournal)
                                    {
                                        factionEntry.EntryText += f.amountOfFactionStrengthChangePerInterval.ToString() + " every " + f.intervalOfFactionStrengthChangeInHours.ToString() + "h)";
                                    }
                                    else if (!f.showRankInJournal && f.showStrengthInJournal)
                                    {
                                        factionEntry.EntryText += ", " + f.amountOfFactionStrengthChangePerInterval.ToString() + " every " + f.intervalOfFactionStrengthChangeInHours.ToString() + "h";
                                    }
                                    else if (f.showRankInJournal && !f.showStrengthInJournal)
                                    {
                                        factionEntry.EntryText += f.amountOfFactionStrengthChangePerInterval.ToString() + " every " + f.intervalOfFactionStrengthChangeInHours.ToString() + "h)";
                                    }
                                }
                                else if (f.showChangeRateInJournal)
                                {
                                    factionEntry.EntryText += f.amountOfFactionStrengthChangePerInterval.ToString() + " every " + f.intervalOfFactionStrengthChangeInHours.ToString() + "h";
                                }

                                if (f.rank == 1)
                                {
                                    if (f.accumulatedBuffStrengthRank1 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank1.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 2)
                                {
                                    if (f.accumulatedBuffStrengthRank2 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank2.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 3)
                                {
                                    if (f.accumulatedBuffStrengthRank3 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank3.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 4)
                                {
                                    if (f.accumulatedBuffStrengthRank4 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank4.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 5)
                                {
                                    if (f.accumulatedBuffStrengthRank5 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank5.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 6)
                                {
                                    if (f.accumulatedBuffStrengthRank6 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank6.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 7)
                                {
                                    if (f.accumulatedBuffStrengthRank7 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank7.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 8)
                                {
                                    if (f.accumulatedBuffStrengthRank8 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank8.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 9)
                                {
                                    if (f.accumulatedBuffStrengthRank9 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank9.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }
                                else if (f.rank == 10)
                                {
                                    if (f.accumulatedBuffStrengthRank10 != 0)
                                    {
                                        factionEntry.EntryText += ", +" + f.accumulatedBuffStrengthRank10.ToString() + " buff to AC/toHit/Saves";
                                        //factionEntry.EntryText += "<br>";
                                    }
                                }

                                factionEntry.EntryText += "<br>";
                                factionEntry.EntryText += "<br>";
                                factionEntry.EntryText += f.factionDescriptionInJournal;
                                jQ.Entries.Add(factionEntry);
                                idCounter++;
                            }
                        }
                        break;
                    }
                }
            }
           
		    if (gv.mod.partyJournalQuests.Count > 0)
    	    {
                /*
                int cnt = 0;
			    foreach (JournalQuest jq in gv.mod.partyJournalQuests)
			    {
                    if (journalScreenQuestIndex == cnt) { color = Color.Lime; }
				    else { color = Color.Black; }	
                    gv.DrawText(jq.Name, locX, locY += spacing, 1.0f, color);
				    cnt++;
			    }
                */
                int minQuestNumber = journalScreenQuestIndex - 3;
                int maxQuestNumber = journalScreenQuestIndex + 3;
                if (minQuestNumber < 0)
                {
                    maxQuestNumber -= minQuestNumber;
                    if (maxQuestNumber > gv.mod.partyJournalQuests.Count - 1)
                    {
                        maxQuestNumber = gv.mod.partyJournalQuests.Count - 1;
                    }
                        minQuestNumber = 0;
                }
                if (maxQuestNumber > gv.mod.partyJournalQuests.Count-1)
                {
                    minQuestNumber -= (maxQuestNumber - (gv.mod.partyJournalQuests.Count - 1));
                    if (minQuestNumber < 0)
                    {
                        minQuestNumber = 0;
                    }
                    maxQuestNumber = gv.mod.partyJournalQuests.Count - 1;
                }
              

                for (int i = minQuestNumber; i <= maxQuestNumber; i++)
                {
                    if (journalScreenQuestIndex == i) { color = Color.Lime; }
                    else { color = Color.Black; }
                    gv.DrawText(gv.mod.partyJournalQuests[i].Name, locX, locY += spacing, 1.0f, color);
                }    

    	    }
		
		    //DRAW QUEST ENTRIES
		    locY = tabStartY;
		    gv.DrawText("Quest Entry:", locX, locY, 1.0f, Color.Black);
		    gv.DrawText("--------------", locX, locY += spacing, 1.0f, Color.Black);	
		    if (gv.mod.partyJournalQuests.Count > 0)
    	    {
                //Description
                string textToSpan = "<font color='black'><i><b>" + gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries[journalScreenEntryIndex].EntryTitle + "</b></i></font><br>";
                textToSpan += gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries[journalScreenEntryIndex].EntryText;

                string textToSpanReplaced = replaceText(textToSpan);
                       
                int yLoc = pH * 18;

                description.tbXloc = locX;
                description.tbYloc = locY + spacing;
                description.tbWidth = pW * 60;
                description.tbHeight = pH * 45;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpanReplaced);
                description.onDrawLogBox();
    	    }
		
		    //DRAW ALL CONTROLS
		    ctrlUpArrow.Draw();
		    ctrlDownArrow.Draw();
		    ctrlLeftArrow.Draw();
		    ctrlRightArrow.Draw();
		    btnReturnJournal.Draw();
        }

        public string replaceText(string originalText)
        { 
            string newString = originalText;
            
            //countdown, other global int
            foreach (GlobalInt gi in gv.mod.moduleGlobalInts)
            {
                if (newString.Contains("<" + gi.Key + ">") && gi.Key.Contains("AutomaticCountDown"))
                //if (newString.Contains("<" + gi.Key + "AutomaticCountDown" + ">"))
                {
                    int days = (gi.Value) / (24 * 60);
                    int hours = ((gi.Value) % (24 * 60)) / 60;
                    int minutes = ((gi.Value) % (24 * 60)) % 60;

                    string timeText = "";
                    if ((days > 0) && (hours > 0) && (minutes > 0))
                    {
                        timeText = days + " day(s), " + hours + " hour(s), " + minutes + " minute(s)";
                    }
                    else if ((days > 0) && (hours <= 0) && (minutes > 0))
                    {
                        timeText = days + " day(s), " + minutes + " minute(s)";
                    }
                    else if ((days > 0) && (hours > 0) && (minutes <= 0))
                    {
                        timeText = days + " day(s), " + hours + " hour(s)";
                    }
                    else if ((days > 0) && (hours <= 0) && (minutes <= 0))
                    {
                        timeText = days + " day(s), ";
                    }
                    else if ((days <= 0) && (hours > 0) && (minutes > 0))
                    {
                        timeText = hours + " hour(s), " + minutes + " minute(s)";
                    }
                    else if ((days <= 0) && (hours <= 0) && (minutes > 0))
                    {
                        timeText = minutes + " minute(s)";
                    }
                    else if ((days <= 0) && (hours > 0) && (minutes <= 0))
                    {
                        timeText = hours + " hour(s)";
                    }

                    newString = newString.Replace("<" + gi.Key + ">", timeText);
                }

                else if (newString.Contains("<" + gi.Key + ">") && gi.Key.Contains("DateInformation"))
                //else if (newString.Contains("<" + gi.Key + "DateInformation"  + ">"))
                {
                    int timeofday = (gi.Value) % (24 * 60);
                    int hour = timeofday / 60;
                    int minute = timeofday % 60;
                    string sMinute = minute + "";
                    if (minute < 10)
                    {
                        sMinute = "0" + minute;
                    }

                    string timeText = "none";

                    string modifiedMonthDayCounterNumberToDisplay = "none";
                    string modifiedMonthDayCounterAddendumToDisplay = "none";
                    string modifiedMonthNameToDisplay = "none";
                    string modifiedWeekDayNameToDisplay = "none";

                    int modifiedTimeInThisYear = 0;
                    int modifiedCurrentYear = 0;
                    int modifiedCurrentMonth = 0;
                    int modifiedCurrentDay = 0;
                    int modifiedCurrentWeekDay = 0;
                    int modifiedCurrentMonthDay = 0;

                    modifiedTimeInThisYear = (gi.Value) % 483840;

                    //note: our ranges strat at 0 here, while our usual displayed counting starts at 1
                    modifiedCurrentYear = (gi.Value) / 483840;
                    decimal.Round(modifiedCurrentYear, 0);
                    modifiedCurrentMonth = ((modifiedTimeInThisYear) / 40320);
                    decimal.Round(modifiedCurrentMonth, 0);
                    modifiedCurrentDay = ((modifiedTimeInThisYear) / 1440);
                    decimal.Round(modifiedCurrentDay, 0);
                    modifiedCurrentWeekDay = (modifiedCurrentDay % 7);
                    modifiedCurrentMonthDay = (modifiedCurrentDay % 28);

                    //XXX
                    if (modifiedCurrentMonth == 0)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfFirstMonth;
                    }
                    else if (modifiedCurrentMonth == 1)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfSecondMonth;
                    }
                    else if (modifiedCurrentMonth == 2)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfThirdMonth;
                    }
                    else if (modifiedCurrentMonth == 3)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfFourthMonth;
                    }
                    else if (modifiedCurrentMonth == 4)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfFifthMonth;
                    }
                    else if (modifiedCurrentMonth == 5)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfSixthMonth;
                    }
                    else if (modifiedCurrentMonth == 6)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfSeventhMonth;
                    }
                    else if (modifiedCurrentMonth == 7)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfEighthMonth;
                    }
                    else if (modifiedCurrentMonth == 8)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfNinthMonth;
                    }
                    else if (modifiedCurrentMonth == 9)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfTenthMonth;
                    }
                    else if (modifiedCurrentMonth == 10)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfEleventhMonth;
                    }
                    else if (modifiedCurrentMonth == 11)
                    {
                        modifiedMonthNameToDisplay = gv.mod.nameOfTwelfthMonth;
                    }

                    modifiedMonthDayCounterNumberToDisplay = (modifiedCurrentMonthDay + 1).ToString();
                    if (modifiedCurrentMonthDay == 0)
                    {
                        modifiedMonthDayCounterAddendumToDisplay = "st";
                    }
                    else if (modifiedCurrentMonthDay == 1)
                    {
                        modifiedMonthDayCounterAddendumToDisplay = "nd";
                    }
                    else if (modifiedCurrentMonthDay == 2)
                    {
                        modifiedMonthDayCounterAddendumToDisplay = "rd";
                    }
                    else if (modifiedCurrentMonthDay == 20)
                    {
                        modifiedMonthDayCounterAddendumToDisplay = "st";
                    }
                    else if (modifiedCurrentMonthDay == 21)
                    {
                        modifiedMonthDayCounterAddendumToDisplay = "nd";
                    }
                    else if (modifiedCurrentMonthDay == 22)
                    {
                        modifiedMonthDayCounterAddendumToDisplay = "rd";
                    }
                    else
                    {
                        modifiedMonthDayCounterAddendumToDisplay = "th";
                    }

                    if (modifiedCurrentWeekDay == 0)
                    {
                        modifiedWeekDayNameToDisplay = gv.mod.nameOfFirstDayOfTheWeek;
                    }
                    else if (modifiedCurrentWeekDay == 1)
                    {
                        modifiedWeekDayNameToDisplay = gv.mod.nameOfSecondDayOfTheWeek;
                    }
                    else if (modifiedCurrentWeekDay == 2)
                    {
                        modifiedWeekDayNameToDisplay = gv.mod.nameOfThirdDayOfTheWeek;
                    }
                    else if (modifiedCurrentWeekDay == 3)
                    {
                        modifiedWeekDayNameToDisplay = gv.mod.nameOfFourthDayOfTheWeek;
                    }
                    else if (modifiedCurrentWeekDay == 4)
                    {
                        modifiedWeekDayNameToDisplay = gv.mod.nameOfFifthDayOfTheWeek;
                    }
                    else if (modifiedCurrentWeekDay == 5)
                    {
                        modifiedWeekDayNameToDisplay = gv.mod.nameOfSixthDayOfTheWeek;
                    }
                    else if (modifiedCurrentWeekDay == 6)
                    {
                        modifiedWeekDayNameToDisplay = gv.mod.nameOfSeventhDayOfTheWeek;
                    }

                    timeText = hour + ":" + sMinute + ", " + modifiedWeekDayNameToDisplay + ", " + modifiedMonthDayCounterNumberToDisplay + modifiedMonthDayCounterAddendumToDisplay + " of " + modifiedMonthNameToDisplay + " " + modifiedCurrentYear.ToString();
                    newString = newString.Replace("<" + gi.Key + ">", timeText);

                }

                else if (newString.Contains("<" + gi.Key + ">"))
                {
                    newString = newString.Replace("<" + gi.Key + ">", gi.Value.ToString());
                }
            }

            //work with contains, go throuh all global strings and add <>
            foreach (GlobalString gs in gv.mod.moduleGlobalStrings)
            {
                if (newString.Contains("<" + gs.Key + ">"))
                {
                    newString = newString.Replace("<" + gs.Key + ">", gs.Value);
                }
            }

            return newString;
        }

        public void onTouchJournal(MouseEventArgs e, MouseEventType.EventType eventType)
        {//1
            try
            {//2 
                ctrlUpArrow.glowOn = false;
                ctrlDownArrow.glowOn = false;
                ctrlLeftArrow.glowOn = false;
                ctrlRightArrow.glowOn = false;
                btnReturnJournal.glowOn = false;

                switch (eventType)
                {//3
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)e.X;
                        int y = (int)e.Y;
                        if (ctrlUpArrow.getImpact(x, y))
                        {
                            ctrlUpArrow.glowOn = true;
                        }
                        else if (ctrlDownArrow.getImpact(x, y))
                        {
                            ctrlDownArrow.glowOn = true;
                        }
                        else if (ctrlLeftArrow.getImpact(x, y))
                        {
                            ctrlLeftArrow.glowOn = true;
                        }
                        else if (ctrlRightArrow.getImpact(x, y))
                        {
                            ctrlRightArrow.glowOn = true;
                        }
                        else if (btnReturnJournal.getImpact(x, y))
                        {
                            btnReturnJournal.glowOn = true;
                        }

                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)e.X;
                        y = (int)e.Y;

                        ctrlUpArrow.glowOn = false;
                        ctrlDownArrow.glowOn = false;
                        ctrlLeftArrow.glowOn = false;
                        ctrlRightArrow.glowOn = false;
                        btnReturnJournal.glowOn = false;

                        if (ctrlUpArrow.getImpact(x, y))
                        {
                            if (journalScreenQuestIndex > 0)
                            {
                                journalScreenQuestIndex--;
                                journalScreenEntryIndex = gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1;
                            }
                        }
                        else if (ctrlDownArrow.getImpact(x, y))
                        {
                            if (journalScreenQuestIndex < gv.mod.partyJournalQuests.Count - 1)
                            {
                                journalScreenQuestIndex++;
                                journalScreenEntryIndex = gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1;
                            }
                        }
                        else if (ctrlLeftArrow.getImpact(x, y))
                        {
                            if (journalScreenEntryIndex > 0)
                            {
                                journalScreenEntryIndex--;
                            }
                        }
                        else if (ctrlRightArrow.getImpact(x, y))
                        {
                            if (journalScreenEntryIndex < gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1)
                            {
                                journalScreenEntryIndex++;
                            }
                        }
                        else if (btnReturnJournal.getImpact(x, y))
                        {
                            gv.screenType = "main";
                            journalBack = null;
                            btnReturnJournal = null;
                            ctrlUpArrow = null;
                            ctrlDownArrow = null;
                            ctrlLeftArrow = null;
                            ctrlRightArrow = null;
                        }
                        break;
                }//3
            }//2
            catch
            { }
        }	    
    }
}
