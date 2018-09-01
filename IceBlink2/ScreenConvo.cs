using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class ScreenConvo 
    {
	    //publicgv.modulegv.mod;
	    public GameView gv;

        public int currentLineIndex = 1;
	
	    public List<IbbButton> btnPartyIndex = new List<IbbButton>();
	
	    //Convo STUFF
	    public Convo currentConvo = new Convo();
	    public string currentNpcNode = "";
	    public string currentPcNode = "";
	    public List<string> currentPcNodeList = new List<string>();
	    public List<IbRect> currentPcNodeRectList = new List<IbRect>();
	    public int pcNodeGlow = -1;
	    public int npcNodeEndY = 0;
	    public int originalSelectedPartyLeader = 0;
	    public int parentIdNum = 0;    
        public Bitmap convoBitmap;
        public Bitmap convoPlusBitmap;
        private bool doActions = true;
        public List<int> nodeIndexList = new List<int>();
        private IbbHtmlTextBox htmltext;

	    public ScreenConvo(Module m, GameView g)
	    {
		    //mod = m;
		    gv = g;
		    setControlsStart();
	    }

	    public void setControlsStart()
	    {
            currentLineIndex = 1;
            pcNodeGlow = 1;
            int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            htmltext = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            htmltext.showBoxBorder = false;

		    for (int x = 0; x < 6; x++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
			    btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnNew.X = ((x+6) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
			    btnNew.Y = 9 * gv.squareSize + (pH * 2);
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnPartyIndex.Add(btnNew);
		    }
	    }
	
	    //CONVO SCREEN
	    public void redrawConvo()
        {
            drawPortrait();
		    drawNpcNode();
		    drawPcNode();	 
		
		    if (currentConvo.PartyChat)
		    {
			    //DRAW EACH PC BUTTON
			    int cntPCs = 0;
			    foreach (IbbButton btn in btnPartyIndex)
			    {
				    if (cntPCs <gv.mod.playerList.Count)
				    {
                        if (gv.mod.playerList[cntPCs].hp >= 0)
                        {
                            if (cntPCs == gv.mod.selectedPartyLeader) { btn.glowOn = true; }
                            else { btn.glowOn = false; }
                            btn.Draw();
                        }
				    }
				    cntPCs++;
			    }
		    }
        }
	    public void drawPortrait()
	    {
            int pH = (int)((float)gv.screenHeight / 100.0f);
		    int sX = gv.squareSize * 1;
		    int sY = pH * 4;
            IbRect src = new IbRect(0, 0, convoBitmap.PixelSize.Width, convoBitmap.PixelSize.Height);
            //IbRect dst = new IbRect(sX, sY, (int)(convoBitmap.PixelSize.Width * 2 * gv.screenDensity * 1.4), (int)(convoBitmap.PixelSize.Height * 2 * gv.screenDensity * 1.4));
            IbRect dst = new IbRect(sX - (int)(gv.squareSize * 0.5f), sY - (int)(gv.squareSize * 0.5f) + pH, (int)(gv.squareSize * 3.5f), (int)(gv.squareSize *5));


            if (convoBitmap.PixelSize.Width == convoBitmap.PixelSize.Height)
            {
                dst = new IbRect(sX, sY, (int)(gv.squareSize * 2 *1.4), (int)(gv.squareSize * 2 * 1.4));
            }
		    if (currentConvo.Narration)
            {
                if (!currentConvo.NpcPortraitBitmap.Equals("")) //Narration with image
                {
                    dst = new IbRect((gv.screenWidth / 2) - (int)(gv.squareSize * 5.7f), gv.squareSize / 2, (int)(gv.squareSize * 8 * 1.4f), (int)(gv.squareSize * 4 * 1.4f));
                }
                else //Narration without image
                {
                    //do narration without image setup                                      
                }            
            }
		    if (convoBitmap != null)
		    {
			    gv.DrawBitmap(convoBitmap, src, dst);
                if ((gv.mod.useUIBackground) && (!currentConvo.Narration))
                {
                    IbRect srcFrame = new IbRect(0, 0, gv.cc.ui_portrait_frame.PixelSize.Width, gv.cc.ui_portrait_frame.PixelSize.Height);
                    IbRect dstFrame = new IbRect(dst.Left - (int)(10 * gv.screenDensity),
                                            dst.Top - (int)(10 * gv.screenDensity),
                                            (int)((float)dst.Width) + (int)(20 * gv.screenDensity),
                                            (int)((float)dst.Height) + (int)(20 * gv.screenDensity));
                    gv.DrawBitmap(gv.cc.ui_portrait_frame, srcFrame, dstFrame);
                }
            }
	    }
	    public void drawNpcNode()
	    {
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            //int startX = gv.squareSize * 3 + (pW * 3);
            int startX = gv.squareSize * 4 + (pW * 4) - (int)(gv.squareSize * 0.5f);
            int startY = pH * 4 - (int)(gv.squareSize * 0.5f);
            int width = gv.screenWidth - startX - (pW * 5);
		
		    if (currentConvo.Narration)
            {
                if (!currentConvo.NpcPortraitBitmap.Equals("")) //Narration with image
                {
                    //do narration with image setup
                    startX = gv.squareSize * 1;
                    //startY = gv.squareSize * 5;
                    startY = (int)(gv.squareSize * 6.6f);
                    width = gv.screenWidth - startX - startX;
                }
                else //Narration without image
                {
                    //do narration without image setup                                      
                }            
            }
            //Node Rectangle Text
            string textToSpan = "";
            textToSpan = currentNpcNode;            
            
            htmltext.tbXloc = startX;
            htmltext.tbYloc = startY;
            htmltext.tbWidth = width;
            htmltext.tbHeight = pH * 50;
            htmltext.logLinesList.Clear();
            htmltext.AddHtmlTextToLog(textToSpan);
            htmltext.onDrawLogBox();
            float totalHeight = 0;
            foreach (FormattedLine fl in htmltext.logLinesList)
            {
                totalHeight += fl.lineHeight;
            }
            npcNodeEndY = startY + (int)totalHeight;
	    }
	    public void drawPcNode()
	    {          
		    currentPcNodeRectList.Clear();

            int pH = (int)((float)gv.screenHeight / 100.0f);
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pad = (int)((float)gv.screenHeight / 100.0f);
		    int startX = gv.squareSize * 1 - (int)(gv.squareSize * 0.5f);
            //int startX = gv.squareSize * 4 + (pW * 3);
            int sY = (int)((float)gv.screenHeight / 100.0f) * 4;
		    //int startY = gv.squareSize * 4;
            int startY = gv.squareSize * 5 + 5*pH - (int)(gv.squareSize * 0.5f);
            int width = gv.screenWidth - startX - startX;

		    if (currentConvo.Narration)
            {
                if (!currentConvo.NpcPortraitBitmap.Equals("")) //Narration with image
                {
                    //do narration with image setup
                    startY = (int)((float)gv.screenHeight / 100.0f) * 50;
                }
                else //Narration without image
                {
                    //do narration without image setup... different startY value                                      
                }            
            }
		    if (startY <= npcNodeEndY)
		    {
			    startY = npcNodeEndY + (pad * 4);
		    }

            int cnt = 1;
            foreach (string txt in currentPcNodeList)
            {
                string textToSpan = txt;
                if (pcNodeGlow == cnt)
                {
                    textToSpan = "<font color='red'>" + txt + "</font>";
                }
                else
                {
                    textToSpan = "<font color='white'>" + txt + "</font>";
                }
                
                htmltext.tbXloc = startX;
                htmltext.tbYloc = startY;
                htmltext.tbWidth = width;
                htmltext.tbHeight = pH * 50;
                htmltext.logLinesList.Clear();
                htmltext.AddHtmlTextToLog(textToSpan);
                htmltext.onDrawLogBox();
                
                float totalHeight = 0;
                float totalWidth = htmltext.tbWidth;
                foreach (FormattedLine fl in htmltext.logLinesList)
                {
                    totalHeight += fl.lineHeight;
                }
                currentPcNodeRectList.Add(new IbRect(startX, startY + gv.oYshift, (int)totalWidth, (int)totalHeight));

                startY += (int)totalHeight + pad;
                cnt++;
            }
	    }

	    public void onTouchConvo(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
		    //pcNodeGlow = -1;
		
		    switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) e.X;
			    int y = (int) e.Y;
				
			    int cnt = 1;
			    foreach (IbRect r in currentPcNodeRectList)
			    {
				    if ((x >= r.Left) && (x <= r.Left + r.Width))
				    {
					    if ((y >= r.Top) && (y <= r.Top + r.Height))
					    {
						    pcNodeGlow = cnt;
					    }
				    }
				    cnt++;
			    }
											
			    break;
			
		    case MouseEventType.EventType.MouseUp:
			    x = (int) e.X;
			    y = (int) e.Y;
				
			    //pcNodeGlow = -1;
			
			    cnt = 0;
			    foreach (IbRect r in currentPcNodeRectList)
			    {
				    if ((x >= r.Left) && (x <= r.Left + r.Width))
				    {
					    if ((y >= r.Top) && (y <= r.Top + r.Height))
					    {
						    selectedLine(cnt);
					    }
				    }
				    cnt++;
			    }
			
			    if (currentConvo.PartyChat)
			    {
				    for (int j = 0; j <gv.mod.playerList.Count; j++)
				    {
					    if (btnPartyIndex[j].getImpact(x, y))
					    {
                            gv.screenMainMap.updateTraitsPanel();
                            gv.mod.selectedPartyLeader = j;
                            currentLineIndex = 1;
                            pcNodeGlow = 1;
                            gv.screenMainMap.updateTraitsPanel();
                            doActions = false;
			                doConvo(parentIdNum);
					    }
				    }
			    }
											
			    break;	
		    }
	    }
        public void onKeyUp(Keys KeyCode)
        {
            if (((KeyCode == Keys.D1) || (KeyCode == Keys.NumPad1)) && (1 <= nodeIndexList.Count))
            {
                selectedLine(0);
                currentLineIndex = 1;
            }
            else if (((KeyCode == Keys.D2) || (KeyCode == Keys.NumPad2)) && (2 <= nodeIndexList.Count))
            {
                selectedLine(1);
                currentLineIndex = 2;
            }
            else if (((KeyCode == Keys.D3) || (KeyCode == Keys.NumPad3)) && (3 <= nodeIndexList.Count))
            {
                selectedLine(2);
                currentLineIndex = 3;
            }
            else if (((KeyCode == Keys.D4) || (KeyCode == Keys.NumPad4)) && (4 <= nodeIndexList.Count))
            {
                selectedLine(3);
                currentLineIndex = 4;
            }
            else if (((KeyCode == Keys.D5) || (KeyCode == Keys.NumPad5)) && (5 <= nodeIndexList.Count))
            {
                selectedLine(4);
                currentLineIndex = 5;
            }
            else if (((KeyCode == Keys.D6) || (KeyCode == Keys.NumPad6)) && (6 <= nodeIndexList.Count))
            {
                selectedLine(5);
                currentLineIndex = 6;
            }
            else if (((KeyCode == Keys.D7) || (KeyCode == Keys.NumPad7)) && (7 <= nodeIndexList.Count))
            {
                selectedLine(6);
                currentLineIndex = 7;
            }
            else if (((KeyCode == Keys.D8) || (KeyCode == Keys.NumPad8)) && (8 <= nodeIndexList.Count))
            {
                selectedLine(7);
                currentLineIndex = 8;
            }
            else if (((KeyCode == Keys.D9) || (KeyCode == Keys.NumPad9)) && (9 <= nodeIndexList.Count))
            {
                selectedLine(8);
                currentLineIndex = 9;
            }
            else if (((KeyCode == Keys.Space) || (KeyCode == Keys.Return)))
            {
                selectedLine(currentLineIndex-1);
                //currentLineIndex = 9;
            }
            else if ((KeyCode == Keys.S) || (KeyCode == Keys.Down))
            {
                currentLineIndex++;
                if (currentLineIndex > currentPcNodeRectList.Count)
                {
                    currentLineIndex = 1;
                }
                pcNodeGlow = currentLineIndex;
            }
            else if ((KeyCode == Keys.W) || (KeyCode == Keys.Up))
            {
                currentLineIndex--;
                if (currentLineIndex < 1)
                {
                    currentLineIndex = currentPcNodeRectList.Count;
                }
                pcNodeGlow = currentLineIndex;
            }

            /*
            if (btnPartyIndex[j].getImpact(x, y))
            {
                gv.mod.selectedPartyLeader = j;
                currentLineIndex = 1;
                pcNodeGlow = 1;
                gv.screenMainMap.updateTraitsPanel();
                doActions = false;
                doConvo(parentIdNum);
            }
            */
            else if ((KeyCode == Keys.D) || (KeyCode == Keys.Right))
            {
                //to do: dead members / unconscious members
                //gv.mod.selectedPartyLeader++;
                //if (gv.mod.playerList[gv.mod.selectedPartyLeader].hp <= 0 )
                //if (gv.mod.selectedPartyLeader)
                bool leaderFound = false;
                while (!leaderFound)
                {
                    gv.mod.selectedPartyLeader++;
                    if (gv.mod.selectedPartyLeader >= gv.mod.playerList.Count)
                    {
                        gv.mod.selectedPartyLeader = 0;
                    }
                    if (gv.mod.playerList[gv.mod.selectedPartyLeader].hp >= 0)
                    {
                        leaderFound = true;
                    }
                }
                gv.screenMainMap.updateTraitsPanel();
                currentLineIndex = 1;
                pcNodeGlow = 1;
                gv.screenMainMap.updateTraitsPanel();
                doActions = false;
                doConvo(parentIdNum);
            }
            else if ((KeyCode == Keys.A) || (KeyCode == Keys.Left))
            {
                //gv.mod.selectedPartyLeader--;
                bool leaderFound = false;
                while (!leaderFound)
                {
                    gv.mod.selectedPartyLeader--;
                    if (gv.mod.selectedPartyLeader < 0)
                    {
                        gv.mod.selectedPartyLeader = gv.mod.playerList.Count - 1; ;
                    }
                    if (gv.mod.playerList[gv.mod.selectedPartyLeader].hp >= 0)
                    {
                        leaderFound = true;
                    }
                }
                gv.screenMainMap.updateTraitsPanel();
                currentLineIndex = 1;
                pcNodeGlow = 1;
                gv.screenMainMap.updateTraitsPanel();
                doActions = false;
                doConvo(parentIdNum);
            }
        }

	    //methods
	    public void startConvo()
        {
            currentLineIndex = 1;
            pcNodeGlow = 1;
            if (currentConvo.SpeakToMainPcOnly)
		    {
                int x = 0;
                foreach (Player pc in gv.mod.playerList)
                {
                    if (pc.mainPc)
                    {
                       gv.mod.selectedPartyLeader = x;
                       gv.screenMainMap.updateTraitsPanel();
                    }
                    x++;
                }
		    }
            if (gv.mod.playerList[gv.mod.selectedPartyLeader].isDead())
            {
                gv.cc.SwitchToNextAvailablePartyLeader();
            }
        
            //load all the current player token images to be used in party chat system
            int cntPCs = 0;
		    foreach (IbbButton btn in btnPartyIndex)
		    {
			    if (cntPCs <gv.mod.playerList.Count)
			    {
                    if (gv.mod.playerList[cntPCs].hp >= 0)
                    {
                        gv.cc.DisposeOfBitmap(ref btn.Img2);
                        btn.Img2 = gv.cc.LoadBitmap(gv.mod.playerList[cntPCs].tokenFilename);
                    }
			    }
			    cntPCs++;
		    }
		    //Remember who the party leader is so that when convo is over we can revert back to them
            originalSelectedPartyLeader =gv.mod.selectedPartyLeader;
            parentIdNum = 0;
		    SetNodeIsActiveFalseForAll();
            parentIdNum = getParentIdNum(parentIdNum);  
                
            if (currentConvo.Narration)
            {
                if (!currentConvo.NpcPortraitBitmap.Equals("")) //Narration with image
                {
                    //do narration with image setup
                }
                else //Narration without image
                {
                    //do narration without image setup                                      
                }            
            }
            // load image for convo
            loadNodePortrait();              
            doActions = true;            
            doConvo(parentIdNum); // load up the text for the NPC node and all PC responses
        }
	    private void loadNodePortrait()
        {		
            // load image for convo
            try
            {
                if ((currentConvo.GetContentNodeById(parentIdNum).NodePortraitBitmap.Equals("")) 
                    || (currentConvo.GetContentNodeById(parentIdNum).NodePortraitBitmap == null))
                {
                    if (currentConvo.NpcPortraitBitmap.Equals(""))
                    {
                        convoBitmap = gv.cc.GetFromBitmapList("npc_blob_portrait");
                    }
                    else
                    {
                        string filename = currentConvo.NpcPortraitBitmap;
                        string filenameNoExt = filename;
                        if (filename.Contains("."))
                        {
                            int lastPeriodPos = filename.LastIndexOf('.');
                            filenameNoExt = filename.Substring(0, lastPeriodPos);
                        }
                        //gv.cc.DisposeOfBitmap(ref convoBitmap);
                        //convoBitmap = gv.cc.LoadBitmap(filenameNoExt);
                        //if (convoBitmap == null)
                        //{
                        //    gv.cc.DisposeOfBitmap(ref convoBitmap);
                        //    convoBitmap = gv.cc.LoadBitmap("npc_blob_portrait");
                        //}
                        convoBitmap = gv.cc.GetFromBitmapList(filenameNoExt);
                    }
                }
                else
                {
                    string filename = currentConvo.GetContentNodeById(parentIdNum).NodePortraitBitmap;
                    string filenameNoExt = filename;
                    if (filename.Contains("."))
                    {
                        int lastPeriodPos = filename.LastIndexOf('.');
                        filenameNoExt = filename.Substring(0, lastPeriodPos);
                    }
                    //gv.cc.DisposeOfBitmap(ref convoBitmap);
                    //convoBitmap = gv.cc.LoadBitmap(filenameNoExt);
                    //if (convoBitmap == null)
                    //{
                    //    gv.cc.DisposeOfBitmap(ref convoBitmap);
                    //    convoBitmap = gv.cc.LoadBitmap("npc_blob_portrait");
                    //}
                    convoBitmap = gv.cc.GetFromBitmapList(filenameNoExt);
                }
            }
            catch (Exception ex)
            {
                //gv.cc.DisposeOfBitmap(ref convoBitmap);
                //convoBitmap = gv.cc.LoadBitmap("npc_blob_portrait");
                convoBitmap = gv.cc.GetFromBitmapList("npc_blob_portrait");
                gv.errorLog(ex.ToString());
            }
        }
	    private void SetNodeIsActiveFalseForAll()
        {
            foreach (ConvoSavedValues csv in gv.mod.moduleConvoSavedValuesList)
            {
                if (csv.ConvoFileName.Equals(currentConvo.ConvoFileName))
                {
            	    currentConvo.GetContentNodeById(csv.NodeNotActiveIdNum).NodeIsActive = false;
                }
            }
        }
	
	    private bool nodePassesConditional(ContentNode pnode)
	    {
		    //check to see if passes conditional
            bool check = true;
            ContentNode chkNode = new ContentNode();
            chkNode = pnode;
            // cycle through the conditions of each node and check for true
            // if one node is a link, then go to the linked node and check its conditional
            if (pnode.isLink)
            {
                chkNode = currentConvo.GetContentNodeById(pnode.linkTo);
            }
            bool AndStatments = true;
            foreach (Condition conditional in chkNode.conditions)
            {
                if (!conditional.c_and)
                {
                    AndStatments = false;
                    break;
                }
            }
            foreach (Condition conditional in chkNode.conditions)
            {
        	    gv.cc.doScriptBasedOnFilename(conditional.c_script, conditional.c_parameter_1, conditional.c_parameter_2, conditional.c_parameter_3, conditional.c_parameter_4);
                if (AndStatments) //this is an "and" set
                {
                    if ((gv.mod.returnCheck == false) && (conditional.c_not == false))
                    {
                        check = false;
                    }
                    if ((gv.mod.returnCheck == true) && (conditional.c_not == true))
                    {
                        check = false;
                    }
                }
                else //this is an "or" set
                {
                    if ((gv.mod.returnCheck == false) && (conditional.c_not == false))
                    {
                        check = false;
                    }
                    else if ((gv.mod.returnCheck == true) && (conditional.c_not == true))
                    {
                        check = false;
                    }
                    else //in "or" statement, if find one true then done
                    {
                        check = true;
                        break;
                    }
                }
            }
            return check;
	    }
		
	    private int getParentIdNum(int childIdNum) // Gets the first NPC node idNum that returns a true conditional
        {
            //first determine which NPC subNode to use by cycling through all children of parentNode until one returns true from conditionals
            foreach (ContentNode npcNode in currentConvo.GetContentNodeById(childIdNum).subNodes)
            {            
                bool check = nodePassesConditional(npcNode);
                if ((check == true) && (npcNode.NodeIsActive))
                {
                    if (npcNode.ShowOnlyOnce)
                    {
                	    npcNode.NodeIsActive = false;
                        saveNodeIsActiveFalseToModule(npcNode);
                    }
                    return npcNode.idNum;
                }
            }
            if (currentConvo.GetContentNodeById(childIdNum).subNodes[0].ShowOnlyOnce)
            {
        	    currentConvo.GetContentNodeById(childIdNum).subNodes[0].NodeIsActive = false;
                saveNodeIsActiveFalseToModule(currentConvo.GetContentNodeById(childIdNum).subNodes[0]);
            }
            return currentConvo.GetContentNodeById(childIdNum).subNodes[0].idNum;
        }
	    private void saveNodeIsActiveFalseToModule(ContentNode nod)
        {
            ConvoSavedValues newCSV = new ConvoSavedValues();
            newCSV.ConvoFileName = currentConvo.ConvoFileName;
            newCSV.NodeNotActiveIdNum = nod.idNum;
           gv.mod.moduleConvoSavedValuesList.Add(newCSV);
        }
        private void doConvo(int prntIdNum) // load up the text for the NPC node and all PC responses
        {
            String selectedPcOptions = "";
            String comparePcOptions = "";
            List<string> existingDifferentAnswerCombinations = new List<string>();
            currentNpcNode = "";
            currentPcNodeList.Clear();        
            nodeIndexList.Clear();
            pcNodeGlow = 1;
            currentLineIndex = 1;

            //NPC NODE STUFF
            //if the NPC node is a link, move to the actual node
            if (currentConvo.GetContentNodeById(prntIdNum).isLink)
            {
        	    parentIdNum = currentConvo.GetContentNodeById(prntIdNum).linkTo;
        	    prntIdNum = currentConvo.GetContentNodeById(prntIdNum).linkTo;
            }
        
            if (doActions)
            {
                foreach (Action action in currentConvo.GetContentNodeById(prntIdNum).actions)
                {
            	    gv.cc.doScriptBasedOnFilename(action.a_script, action.a_parameter_1, action.a_parameter_2, action.a_parameter_3, action.a_parameter_4);
                }
            }
            currentNpcNode = replaceText(currentConvo.GetContentNodeById(prntIdNum).conversationText);
            //Alterntaive system for speech bubbles
            if (gv.mod.useAlternativeSpeechBubbleSystem)
            {

                //have six strings for convo options (six bubble pos)

                //go through each living pc and save its convo options code on the pc (new property), string

                //go through pcs again
                //go through all bubble groups (strings) for this pc
                //check wheter pc convo option (string) matches any bubble pos string already
                //if it is: next pc
                //if it is not:
                //go through bubble group strings again (not nested) and add pc's convo options to first empty "" bubble pos string , then next pc

                //go through pc again
                //go thorugh bubble groups strings
                //if pc convo option matches first bubble group string found, draw img3 with the offset that is matching this bubble group string

                string posRightDown = "";
                string posRightMiddle = "";
                string posRightUp = "";
                string posLeftDown = "";
                string posLeftMiddle = "";
                string posLeftUp = "";

                //PC NODE STUFF
                 //Loop through all PC nodes and check to see if they should be visible
                int cnt = 0;
                int trueCount = 1;
                foreach (ContentNode pcNode in currentConvo.GetContentNodeById(prntIdNum).subNodes)
                {
                    bool check = nodePassesConditional(pcNode);
                    if (check == true)
                    {
                        selectedPcOptions += cnt + "";
                        nodeIndexList.Add(cnt);
                        String pcNodeText = replaceText(pcNode.conversationText);
                        currentPcNodeList.Add("<font color='lime'>" + trueCount + ") " + "</font>" + pcNodeText);
                        trueCount++;
                    }
                    cnt++;
                }

                //PARTY CHAT SYSTEM other PCs NODE STUFF
                //Iterate through all other PCs and see what node options they have and indicate if different
                //int PcIndx = 0;
                int originalPartyLeader = gv.mod.selectedPartyLeader; //remember who was the currently selected PC to check against for diffs

                //set all Img3 bitmaps to null to turn off convo plus bubble tag
                int cntPCs = 0;
                foreach (IbbButton btn in btnPartyIndex)
                {
                    if (cntPCs < gv.mod.playerList.Count)
                    {
                        btn.Img3 = null;
                    }
                    cntPCs++;
                }

                //score convoCodes for each pc
                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                int PcIndx = 0;
                foreach (Player pc in gv.mod.playerList)
                {
                    comparePcOptions = "";
                    gv.mod.selectedPartyLeader = PcIndx;
                    //gv.screenMainMap.updateTraitsPanel();
                   
                        //loop through all nodes and check to see if they should be visible
                        int cntr = 0;
                        foreach (ContentNode pcNode in currentConvo.GetContentNodeById(prntIdNum).subNodes)
                        {
                            bool check = nodePassesConditional(pcNode);
                            if (check == true)
                            {
                                comparePcOptions += cntr + "";
                            }
                            cntr++;
                        }

                    gv.mod.playerList[PcIndx].convoCode = comparePcOptions;
                    PcIndx++;
                }

                //return back to original selected PC after making checks for different node options available
                gv.mod.selectedPartyLeader = originalPartyLeader;


                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                bool allTheSame = true;

                foreach (Player pc in gv.mod.playerList)
                {
                    foreach (Player pc2 in gv.mod.playerList)
                    {
                        if (pc.convoCode != pc2.convoCode)
                        {
                            allTheSame = false;
                        }
                    }
                }

                string shortestConvoCode = "none";
                int shortestConvoCodeLength = 100000;
                foreach (Player pc in gv.mod.playerList)
                {
                    if (pc.convoCode.Length < shortestConvoCodeLength)
                    {
                        shortestConvoCodeLength = pc.convoCode.Length;
                        shortestConvoCode = pc.convoCode;
                    }
                }

                foreach (Player pc in gv.mod.playerList)
                {
                   if (posRightDown == pc.convoCode)
                   {
                        continue;
                   }
                    if (posRightMiddle == pc.convoCode)
                    {
                        continue;
                    }
                    if (posRightUp == pc.convoCode)
                    {
                        continue;
                    }
                    if (posLeftDown == pc.convoCode)
                    {
                        continue;
                    }
                    if (posLeftMiddle == pc.convoCode)
                    {
                        continue;
                    }
                    if (posLeftUp == pc.convoCode)
                    {
                        continue;
                    }

                    if ((pc.convoCode.Length <= 1) || allTheSame || pc.convoCode == shortestConvoCode)
                    {
                        if (posRightDown == "")
                        { 
                            posRightDown = pc.convoCode;
                            continue;
                        }
                    }

                    if (posRightMiddle == "")
                    {
                        posRightMiddle = pc.convoCode;
                        continue;
                    }
                    if (posRightUp == "")
                    {
                        posRightUp = pc.convoCode;
                        continue;
                    }
                    if (posLeftDown == "")
                    {
                        posLeftDown = pc.convoCode;
                        continue;
                    }
                    if (posLeftMiddle == "")
                    {
                        posLeftMiddle = pc.convoCode;
                        continue;
                    }
                    if (posLeftUp == "")
                    {
                        posLeftUp = pc.convoCode;
                        continue;
                    }
                }
                PcIndx = 0;
                foreach (Player pc in gv.mod.playerList)
                {
                    if (pc.hp >= 0)
                    {
                        btnPartyIndex[PcIndx].btnNotificationOn = true;
                        gv.cc.DisposeOfBitmap(ref btnPartyIndex[PcIndx].Img3);
                        btnPartyIndex[PcIndx].Img3 = gv.cc.LoadBitmap("convoplus");
                        
                    }
                    //btnPartyIndex[PcIndx].Image3YOffSet = (int)(-gv.squareSize / 4f);
                    else
                    {
                        gv.cc.DisposeOfBitmap(ref btnPartyIndex[PcIndx].Img3);
                        btnPartyIndex[PcIndx].Img3 = null;
                    }


                    if (gv.mod.playerList[PcIndx].convoCode == posRightDown)
                    {
                        gv.cc.DisposeOfBitmap(ref btnPartyIndex[PcIndx].Img3);
                        btnPartyIndex[PcIndx].Img3 = null;
                        ///btnPartyIndex[PcIndx].Image3YOffSet = 0;
                        //btnPartyIndex[PcIndx].Image3XOffSet = 0;
                    }
                    if (gv.mod.playerList[PcIndx].convoCode == posRightMiddle)
                    {
                        btnPartyIndex[PcIndx].Image3YOffSet = 0;
                        btnPartyIndex[PcIndx].Image3XOffSet = 0;
                    }
                    if (gv.mod.playerList[PcIndx].convoCode == posRightUp)
                    {
                        btnPartyIndex[PcIndx].Image3YOffSet = (int)(-gv.squareSize / 4f);
                        btnPartyIndex[PcIndx].Image3XOffSet = 0;
                    }
                    if (gv.mod.playerList[PcIndx].convoCode == posLeftDown)
                    {
                        btnPartyIndex[PcIndx].Image3YOffSet = (int)(-gv.squareSize / 2f);
                        btnPartyIndex[PcIndx].Image3XOffSet = 0;
                    }
                    if (gv.mod.playerList[PcIndx].convoCode == posLeftMiddle)
                    {
                        btnPartyIndex[PcIndx].Image3YOffSet = 0;
                        btnPartyIndex[PcIndx].Image3XOffSet = (int)(-gv.squareSize / 2f);
                    }
                    if (gv.mod.playerList[PcIndx].convoCode == posLeftMiddle)
                    {
                        btnPartyIndex[PcIndx].Image3YOffSet = (int)(-gv.squareSize / 4f);
                        btnPartyIndex[PcIndx].Image3XOffSet = (int)(-gv.squareSize / 2f);
                    }
                    PcIndx++;
                }


            }
            //Original speech bubble system
            else
            {
                //PC NODE STUFF
                //Loop through all PC nodes and check to see if they should be visible
                int cnt = 0; 
                int trueCount = 1;
                foreach (ContentNode pcNode in currentConvo.GetContentNodeById(prntIdNum).subNodes)
                {
                    bool check = nodePassesConditional(pcNode);
                    if (check == true)
                    {
                        selectedPcOptions += cnt + "";
                        nodeIndexList.Add(cnt);
                        String pcNodeText = replaceText(pcNode.conversationText);
                        currentPcNodeList.Add("<font color='lime'>" + trueCount + ") " + "</font>" + pcNodeText);
                        trueCount++;
                    }
                    cnt++;
                }

                //PARTY CHAT SYSTEM other PCs NODE STUFF
                //Iterate through all other PCs and see what node options they have and indicate if different
                int PcIndx = 0;
                int originalPartyLeader = gv.mod.selectedPartyLeader; //remember who was the currently selected PC to check against for diffs

                //set all Img3 bitmaps to null to turn off convo plus bubble tag
                int cntPCs = 0;
                foreach (IbbButton btn in btnPartyIndex)
                {
                    if (cntPCs < gv.mod.playerList.Count)
                    {
                        btn.Img3 = null;
                    }
                    cntPCs++;
                }

                foreach (Player pc in gv.mod.playerList)
                {
                    comparePcOptions = "";
                    gv.mod.selectedPartyLeader = PcIndx;
                    gv.screenMainMap.updateTraitsPanel();
                    if (PcIndx != originalPartyLeader)
                    {
                        //loop through all nodes and check to see if they should be visible
                        int cntr = 0;
                        foreach (ContentNode pcNode in currentConvo.GetContentNodeById(prntIdNum).subNodes)
                        {
                            bool check = nodePassesConditional(pcNode);
                            if (check == true)
                             {
                                comparePcOptions += cntr + "";
                            }
                            cntr++;
                        }
                        //compare this PC to the selectedPartyLeader's options
                        if ((comparePcOptions.Equals(selectedPcOptions)) || pc.hp < 0)
                        {
                            //no new options for this PC so no plus bubble marker 
                            btnPartyIndex[PcIndx].btnNotificationOn = false;
                            btnPartyIndex[PcIndx].Img3 = null;
                        }
                        else //new options available so show bubble plus marker
                        {
                            btnPartyIndex[PcIndx].btnNotificationOn = true;
                            gv.cc.DisposeOfBitmap(ref btnPartyIndex[PcIndx].Img3);
                            btnPartyIndex[PcIndx].Img3 = gv.cc.LoadBitmap("convoplus");
                            //btnPartyIndex[PcIndx].Image3YOffSet = (int)(-gv.squareSize / 4f);
                        }
                    }
                    PcIndx++;
                }

                //return back to original selected PC after making checks for different node options available
                gv.mod.selectedPartyLeader = originalPartyLeader;
            }

            gv.screenMainMap.updateTraitsPanel();

            //load node portrait and play node sound
            loadNodePortrait();
        }
        private void selectedLine(int btnIndex)
        {    	                 
            if (btnIndex < nodeIndexList.Count)
            {
        	    int index = nodeIndexList[btnIndex];
                string NPCname = "";
	            ContentNode selectedNod = currentConvo.GetContentNodeById(parentIdNum).subNodes[index];
	            if ((selectedNod.NodeNpcName.Equals("")) || (selectedNod.NodeNpcName == null) || (selectedNod.NodeNpcName.Length <= 0))
	            {
	                NPCname = currentConvo.DefaultNpcName;
	            }
	            else
	            {
	                NPCname = selectedNod.NodeNpcName;
	            }
	            string npcNode = replaceText(currentConvo.GetContentNodeById(parentIdNum).conversationText);
	            string pcNode = replaceText(selectedNod.conversationText);
	            //write to log
                gv.cc.addLogText("<font color='yellow'>" + NPCname + ": </font>" +
                                 "<font color='silver'>" + npcNode + "<br>" + "</font>" +
                                 "<font color='aqua'>" +gv.mod.playerList[gv.mod.selectedPartyLeader].name + ": </font>" +
                                 "<font color='silver'>" + pcNode + "</font>");
	
	            int childIdNum = currentConvo.GetContentNodeById(parentIdNum).subNodes[index].idNum;
	            // if PC node choosen was a linked node, then return the idNum of the linked node
	            if (currentConvo.GetContentNodeById(parentIdNum).subNodes[index].isLink)
	            {
	                childIdNum = currentConvo.GetContentNodeById(currentConvo.GetContentNodeById(parentIdNum).subNodes[index].linkTo).idNum;
	            }
	            //doAction() for current selected PC Node (all actions for node)
	            foreach (Action action in currentConvo.GetContentNodeById(childIdNum).actions)
	            {
	        	    gv.cc.doScriptBasedOnFilename(action.a_script, action.a_parameter_1, action.a_parameter_2, action.a_parameter_3, action.a_parameter_4);
	            }
	            if (currentConvo.GetContentNodeById(childIdNum).subNodes.Count < 1)
	            {
                    gv.cc.addLogText("[end convo]<br><br>"); //add a blank line to main screen log at the end of a conversation
                    if (currentConvo.ConvoFileName != "order")
                    {
                        gv.mod.selectedPartyLeader = originalSelectedPartyLeader;
                    }
                    gv.screenMainMap.updateTraitsPanel();
                    if ((gv.screenType.Equals("shop")) || (gv.screenType.Equals("title")) || (gv.screenType.Equals("combat")))
	        	    {
	        		    //leave as shop and launch shop screen
	        	    }
	        	    else
	        	    {
	        		    gv.screenType = "main";
	        		    if (gv.cc.calledConvoFromProp)
	        		    {
                            //gv.mod.isRecursiveDoTriggerCallMovingProp = true;
                            //gv.mod.blockTriggerMovingProp = true;
                            //gv.mod.isRecursiveCall = true;
                            gv.cc.doPropTriggers();
                            //gv.mod.isRecursiveCall = true;
                        }
	        		    else
	        		    {
	        			    gv.cc.doTrigger();
	        		    }
	        	    }
	            }
	            else
	            {
	                parentIdNum = getParentIdNum(childIdNum);
	                doActions = true;
	                doConvo(parentIdNum);
	            }
            }
        }    
        public string replaceText(string originalText)
        {
            //for The Raventhal, always assumed that main PC is talking
            Player pc =gv.mod.playerList[gv.mod.selectedPartyLeader];
            string newString = originalText;

            if (gv.mod.playerList.Count > 5)
            {
                newString = newString.Replace("<player6>", gv.mod.playerList[5].name);
            }
            if (gv.mod.playerList.Count > 4)
            {
                newString = newString.Replace("<player5>", gv.mod.playerList[4].name);
            }
            if (gv.mod.playerList.Count > 3)
            {
                newString = newString.Replace("<player4>", gv.mod.playerList[3].name);
            }
            if (gv.mod.playerList.Count > 2)
            {
                newString = newString.Replace("<player3>", gv.mod.playerList[2].name);
            }
            if (gv.mod.playerList.Count > 1)
            {
                newString = newString.Replace("<player2>", gv.mod.playerList[1].name);
            }
            newString = newString.Replace("<player1>", gv.mod.playerList[0].name);

            newString = newString.Replace("<FirstName>", pc.name);
            newString = newString.Replace("<FullName>", pc.name);
            newString = newString.Replace("<Class>", pc.playerClass.name);
            newString = newString.Replace("<class>", pc.playerClass.name.ToLower());
            newString = newString.Replace("<Race>", pc.race.name);
            newString = newString.Replace("<race>", pc.race.name.ToLower());

            if (pc.isMale)
            {
                newString = newString.Replace("<Sir/Madam>", "Sir");
                newString = newString.Replace("<sir/madam>", "sir");
                newString = newString.Replace("<His/Her>", "His");
                newString = newString.Replace("<his/her>", "his");
                newString = newString.Replace("<Him/Her>", "Him");
                newString = newString.Replace("<him/her>", "him");
                newString = newString.Replace("<He/She>", "He");
                newString = newString.Replace("<he/she>", "he");
                newString = newString.Replace("<Boy/Girl>", "Boy");
                newString = newString.Replace("<boy/girl>", "boy");
                newString = newString.Replace("<Lad/Lass>", "Lad");
                newString = newString.Replace("<lad/lass>", "lad");
                newString = newString.Replace("<Man/Woman>", "Man");
                newString = newString.Replace("<man/woman>", "man");
                newString = newString.Replace("<Brother/Sister>", "Brother");
                newString = newString.Replace("<brother/sister>", "brother");
            }
            else
            {
                newString = newString.Replace("<Sir/Madam>", "Madam");
                newString = newString.Replace("<sir/madam>", "madam");
                newString = newString.Replace("<His/Her>", "Her");
                newString = newString.Replace("<his/her>", "her");
                newString = newString.Replace("<Him/Her>", "Her");
                newString = newString.Replace("<him/her>", "her");
                newString = newString.Replace("<He/She>", "She");
                newString = newString.Replace("<he/she>", "she");
                newString = newString.Replace("<Boy/Girl>", "Girl");
                newString = newString.Replace("<boy/girl>", "girl");
                newString = newString.Replace("<Lad/Lass>", "Lass");
                newString = newString.Replace("<lad/lass>", "lass");
                newString = newString.Replace("<Man/Woman>", "Woman");
                newString = newString.Replace("<man/woman>", "woman");
                newString = newString.Replace("<Brother/Sister>", "Sister");
                newString = newString.Replace("<brother/sister>", "sister");
            }

            //countdown, other global int
            foreach (GlobalInt gi in gv.mod.moduleGlobalInts)
            {
                if (newString.Contains("<" + gi.Key + ">") && gi.Key.Contains("AutomaticCountDown"))
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
    }
}
