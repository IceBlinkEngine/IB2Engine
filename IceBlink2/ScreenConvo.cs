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
	    public Module mod;
	    public GameView gv;
	
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
		    mod = m;
		    gv = g;
		    setControlsStart();
	    }

	    public void setControlsStart()
	    {		
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
			    btnNew.X = ((x+5) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
			    btnNew.Y = 9 * gv.squareSize + (pH * 2);
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnPartyIndex.Add(btnNew);
		    }
	    }
	
	    //CONVO SCREEN
	    public void redrawConvo()
        {
            //gv.gCanvas.Clear(Color.Black);
    	    //canvas.drawColor(Color.BLACK);
    	
		    //drawConvoControls(canvas);
		    drawPortrait();
		    drawNpcNode();
		    drawPcNode();	 
		
		    if (currentConvo.PartyChat)
		    {
			    //DRAW EACH PC BUTTON
			    int cntPCs = 0;
			    foreach (IbbButton btn in btnPartyIndex)
			    {
				    if (cntPCs < mod.playerList.Count)
				    {
					    if (cntPCs == mod.selectedPartyLeader) {btn.glowOn = true;}
					    else {btn.glowOn = false;}					
					    btn.Draw();
				    }
				    cntPCs++;
			    }
		    }
        }
	    public void drawPortrait()
	    {		
		    int sX = gv.squareSize * 2;
		    int sY = (int)((float)gv.screenHeight / 100.0f) * 4;
            IbRect src = new IbRect(0, 0, convoBitmap.PixelSize.Width, convoBitmap.PixelSize.Height);
            IbRect dst = new IbRect(sX, sY, convoBitmap.PixelSize.Width * 2, convoBitmap.PixelSize.Height * 2);

            if (convoBitmap.PixelSize.Width == convoBitmap.PixelSize.Height)
            {
                dst = new IbRect(sX, sY, (int)(gv.squareSize * 2), (int)(gv.squareSize * 2));
            }
		    if (currentConvo.Narration)
            {
                if (!currentConvo.NpcPortraitBitmap.Equals("")) //Narration with image
                {
                    dst = new IbRect((gv.screenWidth / 2) - (gv.squareSize * 4), gv.squareSize / 2, gv.squareSize * 8, gv.squareSize * 4);
                }
                else //Narration without image
                {
                    //do narration without image setup                                      
                }            
            }
		    if (convoBitmap != null)
		    {
			    gv.DrawBitmap(convoBitmap, src, dst);
		    }
	    }
	    public void drawNpcNode()
	    {
            int startX = gv.squareSize * 5;
		    int startY = (int)((float)gv.screenHeight / 100.0f) * 4;
            int width = gv.squareSize * 12;
            int pH = (int)((float)gv.screenHeight / 100.0f);
		
		    if (currentConvo.Narration)
            {
                if (!currentConvo.NpcPortraitBitmap.Equals("")) //Narration with image
                {
                    //do narration with image setup
                    startX = gv.squareSize * 2;
                    startY = gv.squareSize * 4 + (pH * 10);
                    width = gv.squareSize * 16;
                }
                else //Narration without image
                {
                    //do narration without image setup                                      
                }            
            }
            //Node Rectangle Text
            string textToSpan = "";
            textToSpan = currentNpcNode;
            SizeF textSize = gv.cc.MeasureString(currentNpcNode, gv.drawFontReg, width);
            npcNodeEndY = startY + (int)textSize.Height;
            //IbRect rect = new IbRect(startX, startY, width, pH * 50);
            //gv.DrawText(textToSpan, rect, 1.0f, Color.White);

            htmltext.tbXloc = startX;
            htmltext.tbYloc = startY;
            htmltext.tbWidth = width;
            htmltext.tbHeight = pH * 50;
            htmltext.logLinesList.Clear();
            htmltext.AddHtmlTextToLog(textToSpan);
            htmltext.onDrawLogBox();
	    }
	    public void drawPcNode()
	    {          
		    currentPcNodeRectList.Clear();

            int pH = (int)((float)gv.screenHeight / 100.0f);
		    int pad = (int)((float)gv.screenHeight / 100.0f);
		    int startX = gv.squareSize * 3;
		    int sY = (int)((float)gv.screenHeight / 100.0f) * 4;
		    int startY = gv.squareSize * 5;		
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
                //Node Rectangle Text
                SizeF textSize = gv.cc.MeasureString(txt, gv.drawFontReg, width);
                //textSize.Height += textSize.Height;
                currentPcNodeRectList.Add(new IbRect(startX, startY + gv.oYshift, (int)textSize.Width, (int)textSize.Height));
                //IbRect rect = new IbRect(startX, startY, width, pH * 50);
                string textToSpan = txt;
                if (pcNodeGlow == cnt)
                {
                    textToSpan = "<font color='red'>" + txt + "</font>";
                    //gv.DrawText(txt, rect, 1.0f, Color.Red);
                }
                
                htmltext.tbXloc = startX;
                htmltext.tbYloc = startY;
                htmltext.tbWidth = width;
                htmltext.tbHeight = pH * 50;
                htmltext.logLinesList.Clear();
                htmltext.AddHtmlTextToLog(textToSpan);
                htmltext.onDrawLogBox();

                startY += (int)textSize.Height + pad;
                cnt++;
            }

            /*TODO
		    TextPaint tp = new TextPaint();
            tp.setColor(Color.WHITE);
            tp.setTextSize(gv.mUiTextPaint.getTextSize());
            tp.setTextAlign(Align.LEFT);
            tp.setAntiAlias(true);
            tp.setTypeface(gv.uiFont);
        
            int cnt = 1;
            foreach (string txt in currentPcNodeList)
            {
        	    Spanned htmlText = Html.fromHtml(txt);
        	    if (pcNodeGlow == cnt)
        	    {
        		    htmlText = Html.fromHtml("<font color='red'>" + txt + "</font>");
        	    }
                StaticLayout sl = new StaticLayout(htmlText, tp, width, Layout.Alignment.ALIGN_NORMAL, 1, 0, false);
                currentPcNodeRectList.add(new Rect(startX, startY, startX + sl.getWidth(), startY + sl.getHeight()));
                canvas.translate(startX, startY);
                sl.draw(canvas);
                canvas.translate(-startX, -startY);
                startY += sl.getHeight() + pad;
                cnt++;
            }
            */
	    }

	    public void onTouchConvo(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
		    pcNodeGlow = -1;
		
		    //int eventAction = event.getAction();
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
				
			    pcNodeGlow = -1;
			
			    cnt = 0;
			    foreach (IbRect r in currentPcNodeRectList)
			    {
				    if ((x >= r.Left) && (x <= r.Left + r.Width))
				    {
					    if ((y >= r.Top) && (y <= r.Top + r.Height))
					    {
						    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
						    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
						    selectedLine(cnt);
					    }
				    }
				    cnt++;
			    }
			
			    if (currentConvo.PartyChat)
			    {
				    for (int j = 0; j < mod.playerList.Count; j++)
				    {
					    if (btnPartyIndex[j].getImpact(x, y))
					    {
						    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
						    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
						    mod.selectedPartyLeader = j;
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
            //Toast.makeText(gv.gameContext, "pressed a keyCode: " + keyCode + " and event: " + event.toString(), Toast.LENGTH_SHORT).show();
            if (((KeyCode == Keys.D1) || (KeyCode == Keys.NumPad1)) && (1 <= nodeIndexList.Count))
            {
                selectedLine(0);
            }
            else if (((KeyCode == Keys.D2) || (KeyCode == Keys.NumPad2)) && (2 <= nodeIndexList.Count))
            {
                selectedLine(1);
            }
            else if (((KeyCode == Keys.D3) || (KeyCode == Keys.NumPad3)) && (3 <= nodeIndexList.Count))
            {
                selectedLine(2);
            }
            else if (((KeyCode == Keys.D4) || (KeyCode == Keys.NumPad4)) && (4 <= nodeIndexList.Count))
            {
                selectedLine(3);
            }
            else if (((KeyCode == Keys.D5) || (KeyCode == Keys.NumPad5)) && (5 <= nodeIndexList.Count))
            {
                selectedLine(4);
            }
            else if (((KeyCode == Keys.D6) || (KeyCode == Keys.NumPad6)) && (6 <= nodeIndexList.Count))
            {
                selectedLine(5);
            }
        }

	    //methods
	    public void startConvo()
        {
            //gv.TrackerSendEventConvo(currentConvo.ConvoFileName);

		    if (currentConvo.SpeakToMainPcOnly)
		    {
                int x = 0;
                foreach (Player pc in gv.mod.playerList)
                {
                    if (pc.mainPc)
                    {
                        mod.selectedPartyLeader = x;
                    }
                    x++;
                }
		    }
            if (mod.playerList[mod.selectedPartyLeader].charStatus.Equals("Dead"))
            {
                gv.cc.SwitchToNextAvailablePartyLeader();
            }
        
            //load all the current player token images to be used in party chat system
            int cntPCs = 0;
		    foreach (IbbButton btn in btnPartyIndex)
		    {
			    if (cntPCs < mod.playerList.Count)
			    {
				    btn.Img2 = gv.cc.LoadBitmap(mod.playerList[cntPCs].tokenFilename);						
			    }
			    cntPCs++;
		    }
		    //Remember who the party leader is so that when convo is over we can revert back to them
            originalSelectedPartyLeader = mod.selectedPartyLeader;
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
            //pictureBox1.Image = (Image)convoBitmap;
            //loadConvoPlusImage();              
            //refreshPcPortraits();        
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
            	    string filename = currentConvo.NpcPortraitBitmap;
                    int lastPeriodPos = filename.LastIndexOf('.');
                    string filenameNoExt = filename.Substring(0, lastPeriodPos);
                    convoBitmap = gv.cc.LoadBitmap(filenameNoExt);
                    if (convoBitmap == null)
                    {
                	    convoBitmap = gv.cc.LoadBitmap("npc_blob_portrait");
                    }
                }
                else
                {
                    string filename = currentConvo.GetContentNodeById(parentIdNum).NodePortraitBitmap;
                    int lastPeriodPos = filename.LastIndexOf('.');
                    string filenameNoExt = filename.Substring(0, lastPeriodPos);
                    convoBitmap = gv.cc.LoadBitmap(filenameNoExt);
                    if (convoBitmap == null)
                    {
                	    convoBitmap = gv.cc.LoadBitmap("npc_blob_portrait");
                    }
                }
            }
            catch (Exception ex)
            {
                convoBitmap = gv.cc.LoadBitmap("npc_blob_portrait");
            }
        }
	    private void SetNodeIsActiveFalseForAll()
        {
            foreach (ConvoSavedValues csv in mod.moduleConvoSavedValuesList)
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
                    if ((mod.returnCheck == false) && (conditional.c_not == false))
                    {
                        check = false;
                    }
                    if ((mod.returnCheck == true) && (conditional.c_not == true))
                    {
                        check = false;
                    }
                }
                else //this is an "or" set
                {
                    if ((mod.returnCheck == false) && (conditional.c_not == false))
                    {
                        check = false;
                    }
                    else if ((mod.returnCheck == true) && (conditional.c_not == true))
                    {
                        check = false;
                    }
                    else //in "or" statement, if find one true then done
                    {
                        check = true;
                        break;
                    }
                }
                //MessageBox.Show("script: " + conditional.c_script + "  variable: " + conditional.c_parameter_1 + "  value: " + conditional.c_parameter_2);
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
            mod.moduleConvoSavedValuesList.Add(newCSV);
        }
        private void doConvo(int prntIdNum) // load up the text for the NPC node and all PC responses
        {
            String selectedPcOptions = "";
            String comparePcOptions = "";
            currentNpcNode = "";
            currentPcNodeList.Clear();        
            nodeIndexList.Clear();
        
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
            	    currentPcNodeList.Add("<font color='lime'>" + trueCount + ") "  + "</font>" + pcNodeText);
            	    trueCount++;
                }
                cnt++;
            }

            //PARTY CHAT SYSTEM other PCs NODE STUFF
            //Iterate through all other PCs and see what node options they have and indicate if different
            int PcIndx = 0;
            int originalPartyLeader = mod.selectedPartyLeader; //remember who was the currently selected PC to check against for diffs
        
            //set all Img3 bitmaps to null to turn off convo plus bubble tag
            int cntPCs = 0;
		    foreach (IbbButton btn in btnPartyIndex)
		    {
			    if (cntPCs < mod.playerList.Count)
			    {
				    btn.Img3 = null;						
			    }
			    cntPCs++;
		    }
		
            foreach (Player pc in mod.playerList)
            {
                comparePcOptions = "";
                mod.selectedPartyLeader = PcIndx;
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
                    if (comparePcOptions.Equals(selectedPcOptions))
                    {
                	    //no new options for this PC so no plus bubble marker 
                        btnPartyIndex[PcIndx].btnNotificationOn = false;
                	    btnPartyIndex[PcIndx].Img3 = null;
                    } 
                    else //new options available so show bubble plus marker
                    {
                        btnPartyIndex[PcIndx].btnNotificationOn = true;
                	    btnPartyIndex[PcIndx].Img3 = gv.cc.LoadBitmap("convoplus");
                    }
                }
                PcIndx++;
            }
        
            //return back to original selected PC after making checks for different node options available
            mod.selectedPartyLeader = originalPartyLeader;

            //load node portrait and play node sound
            loadNodePortrait();
        }
        private void selectedLine(int btnIndex)
        {    	
            //#region send choosen text to the main screen log        
            if (btnIndex < nodeIndexList.Count)
            {
        	    int index = nodeIndexList[btnIndex];
        	    String NPCname = "";
	            ContentNode selectedNod = currentConvo.GetContentNodeById(parentIdNum).subNodes[index];
	            if ((selectedNod.NodeNpcName.Equals("")) || (selectedNod.NodeNpcName == null) || (selectedNod.NodeNpcName.Length <= 0))
	            {
	                NPCname = currentConvo.DefaultNpcName;
	            }
	            else
	            {
	                NPCname = selectedNod.NodeNpcName;
	            }
	            //String npcNode = replaceText(rtxtNPC.Text);
	            //String pcNode = replaceText(selectedNod.conversationText);
	            //doScriptBasedOnFilename("dsAdventureMapConvoLog.cs", npcNode, NPCname, pcNode, mod.playerList.get(mod.selectedPartyLeader).name);
	            //#endregion
	
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
	                mod.selectedPartyLeader = originalSelectedPartyLeader;
	        	    if ((gv.screenType.Equals("shop")) || (gv.screenType.Equals("title")) || (gv.screenType.Equals("combat")))
	        	    {
	        		    //leave as shop and launch shop screen
	        	    }
	        	    else
	        	    {
	        		    gv.screenType = "main";
	        		    if (gv.cc.calledConvoFromProp)
	        		    {
	        			    gv.cc.doPropTriggers();
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
            //Player pc = mod.playerList.get(mod.selectedPartyLeader);
    	    //for The Raventhal, always assumed that main PC is talking
            Player pc = mod.playerList[mod.selectedPartyLeader];
            string newString = originalText;
        
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
            return newString;
        }
    }
}
