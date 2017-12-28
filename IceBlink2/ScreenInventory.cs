using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class ScreenInventory 
    {
	    //public gv.module gv.mod;
	    public GameView gv;
	    private int inventoryPageIndex = 0;
	    private int inventorySlotIndex = 0;
	    private int slotsPerPage = 20;
	    private List<IbbButton> btnInventorySlot = new List<IbbButton>();
	    private IbbButton btnInventoryLeft = null;
	    private IbbButton btnInventoryRight = null;
	    private IbbButton btnPageIndex = null;
	    private IbbButton btnHelp = null;
	    private IbbButton btnInfo = null;
	    private IbbButton btnReturn = null;
	    private IbbHtmlTextBox description;
	
	    public ScreenInventory(Module m, GameView g)
	    {
		    //gv.mod = m;
		    gv = g;
		    //setControlsStart();
	    }
	
	    public void setControlsStart()
	    {			
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;

		    if (btnInventoryLeft == null)
		    {
			    btnInventoryLeft = new IbbButton(gv, 1.0f);
			    btnInventoryLeft.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInventoryLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnInventoryLeft.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInventoryLeft.X = 8 * gv.squareSize;
			    btnInventoryLeft.Y = (1 * gv.squareSize) - (pH * 2);
                btnInventoryLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnPageIndex == null)
		    {
			    btnPageIndex = new IbbButton(gv, 1.0f);
			    btnPageIndex.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnPageIndex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPageIndex.Text = "1/10";
			    btnPageIndex.X = 9 * gv.squareSize;
			    btnPageIndex.Y = (1 * gv.squareSize) - (pH * 2);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnInventoryRight == null)
		    {
			    btnInventoryRight = new IbbButton(gv, 1.0f);
			    btnInventoryRight.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInventoryRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnInventoryRight.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInventoryRight.X = 10 * gv.squareSize;
			    btnInventoryRight.Y = (1 * gv.squareSize) - (pH * 2);
                btnInventoryRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		
		    if (btnReturn == null)
		    {
			    btnReturn = new IbbButton(gv, 1.2f);	
			    btnReturn.Text = "RETURN";
			    btnReturn.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnReturn.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnReturn.Y = 9 * gv.squareSize + pH * 2;
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnHelp == null)
		    {
			    btnHelp = new IbbButton(gv, 0.8f);	
			    btnHelp.Text = "HELP";
			    btnHelp.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnHelp.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnHelp.X = 0 * gv.squareSize + padW * 1 + gv.oXshift;
			    btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    if (btnInfo == null)
		    {
			    btnInfo = new IbbButton(gv, 0.8f);	
			    btnInfo.Text = "INFO";
			    btnInfo.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInfo.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInfo.X = (15 * gv.squareSize) - padW * 1 + gv.oXshift;
			    btnInfo.Y = 9 * gv.squareSize + pH * 2;
                btnInfo.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInfo.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
			    btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    if (y < 5)
			    {
				    btnNew.X = ((y+2+4) * gv.squareSize) + (padW * (y+1)) + gv.oXshift;
				    btnNew.Y = 2 * gv.squareSize;
			    }
			    else if ((y >=5 ) && (y < 10))
			    {
				    btnNew.X = ((y-5+2+4) * gv.squareSize) + (padW * ((y-5)+1)) + gv.oXshift;
				    btnNew.Y = 3 * gv.squareSize + padW;
			    }
			    else if ((y >=10 ) && (y < 15))
			    {
				    btnNew.X = ((y-10+2+4) * gv.squareSize) + (padW * ((y-10)+1)) + gv.oXshift;
				    btnNew.Y = 4 * gv.squareSize + (padW * 2);
			    }
			    else
			    {
				    btnNew.X = ((y-15+2+4) * gv.squareSize) + (padW * ((y-15)+1)) + gv.oXshift;
				    btnNew.Y = 5 * gv.squareSize + (padW * 3);
			    }

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnInventorySlot.Add(btnNew);
		    }			
	    }
	
        public void resetInventory(bool inCombat)
        {
            if (btnReturn == null)
            {
                setControlsStart();
            }

            for (int i = gv.mod.addedItemsRefs.Count - 1; i >= 0; i--)
            {
                for (int j = gv.mod.partyInventoryRefsList.Count - 1; j >= 0; j--)
                {

                    if (gv.mod.addedItemsRefs[i] == gv.mod.partyInventoryRefsList[j].tag)
                    {
                        //krahn
                        gv.mod.partyInventoryRefsList.RemoveAt(j);
                        //btnInventorySlot
                        //btn.Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                        //btnInventorySlot[i].Img3 = null;
                        break;
                    }
                    
                }
            }

            foreach (IbbButton b in btnInventorySlot)
            {
                b.Img3 = null;
            }

            gv.mod.addedItemsRefs.Clear();

            doItemStacking();
            int cntSlot = 0;

            //entrypoint
            Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
            if (!inCombat)
            {
                pc = gv.mod.playerList[gv.mod.selectedPartyLeader];
            }

            //make list on mod level that stores the added tags of pc
            //remove matchng refs first
            /*
            for (int i = gv.mod.partyInventoryRefsList.Count-1; i >= 0; i--)
            {
                if (gv.mod.partyInventoryRefsList[i].tag == pc.BodyRefs.tag)
                {

                }
            }
            */
            if (inventoryPageIndex == 0)
            {
                int insertCounter = 0;

                //if (inventoryPageIndex == 0)
                //{
                //Body
                if (pc.BodyRefs.tag != "none" && pc.BodyRefs.tag != "")
                {
                    gv.mod.partyInventoryRefsList.Insert(insertCounter, pc.BodyRefs);
                    gv.mod.addedItemsRefs.Add(pc.BodyRefs.tag);
                    btnInventorySlot[insertCounter].Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                    insertCounter++;
                }

                //weapon
                if (pc.MainHandRefs.tag != "none" && pc.MainHandRefs.tag != "")
                {
                    gv.mod.partyInventoryRefsList.Insert(insertCounter, pc.MainHandRefs);
                    gv.mod.addedItemsRefs.Add(pc.MainHandRefs.tag);
                    btnInventorySlot[insertCounter].Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                    insertCounter++;
                }

                //offhand
                if (pc.OffHandRefs.tag != "none" && pc.OffHandRefs.tag != "")
                {
                    gv.mod.partyInventoryRefsList.Insert(insertCounter, pc.OffHandRefs);
                    gv.mod.addedItemsRefs.Add(pc.OffHandRefs.tag);
                    btnInventorySlot[insertCounter].Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                    insertCounter++;
                }

                //head
                if (pc.HeadRefs.tag != "none" && pc.HeadRefs.tag != "")
                {
                    gv.mod.partyInventoryRefsList.Insert(insertCounter, pc.HeadRefs);
                    gv.mod.addedItemsRefs.Add(pc.HeadRefs.tag);
                    btnInventorySlot[insertCounter].Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                    insertCounter++;
                }

                //ring1
                if (pc.RingRefs.tag != "none" && pc.RingRefs.tag != "")
                {
                    gv.mod.partyInventoryRefsList.Insert(insertCounter, pc.RingRefs);
                    gv.mod.addedItemsRefs.Add(pc.RingRefs.tag);
                    btnInventorySlot[insertCounter].Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                    insertCounter++;
                }


                //ring2
                if (pc.Ring2Refs.tag != "none" && pc.Ring2Refs.tag != "")
                {
                    gv.mod.partyInventoryRefsList.Insert(insertCounter, pc.Ring2Refs);
                    gv.mod.addedItemsRefs.Add(pc.Ring2Refs.tag);
                    btnInventorySlot[insertCounter].Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                    insertCounter++;
                }

                //neck
                if (pc.NeckRefs.tag != "none" && pc.NeckRefs.tag != "")
                {
                    gv.mod.partyInventoryRefsList.Insert(insertCounter, pc.NeckRefs);
                    gv.mod.addedItemsRefs.Add(pc.NeckRefs.tag);
                    btnInventorySlot[insertCounter].Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                    insertCounter++;
                }

                //feet
                if (pc.FeetRefs.tag != "none" && pc.FeetRefs.tag != "")
                {
                    gv.mod.partyInventoryRefsList.Insert(insertCounter, pc.FeetRefs);
                    gv.mod.addedItemsRefs.Add(pc.FeetRefs.tag);
                    btnInventorySlot[insertCounter].Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                    insertCounter++;
                }
            }
            //}

            //int startCounter = cntSlot;
            //if (inventoryPageIndex != 0)
            //{
            //startCounter = 0;
            //}

            //foreach (IbbButton btn in btnInventorySlot)
            int startCounter = 0;
            for (int i = startCounter; i < btnInventorySlot.Count; i++)
            {
                if ((cntSlot + (inventoryPageIndex * slotsPerPage)) < gv.mod.partyInventoryRefsList.Count)
                {
                    Item it = gv.mod.getItemByResRefForInfo(gv.mod.partyInventoryRefsList[cntSlot + (inventoryPageIndex * slotsPerPage)].resref);
                    gv.cc.DisposeOfBitmap(ref btnInventorySlot[i].Img2);
                    btnInventorySlot[i].Img2 = gv.cc.LoadBitmap(it.itemImage);
                    ItemRefs itr = gv.mod.partyInventoryRefsList[cntSlot + (inventoryPageIndex * slotsPerPage)];

                    //if ()
                    //bockauf
                    //chargelogic
                    //check shop: no sepaartwd selling of charges items
                    //check zero charges items - hopefully not deleted

                    if ((it.onUseItemCastSpellTag == "none" || it.onUseItemCastSpellTag == "") && (it.onUseItemIBScript == "none" || it.onUseItemIBScript == "") && (it.onUseItem == "none" || it.onUseItem == ""))
                    {
                        if (itr.quantity > 1)
                        {
                            btnInventorySlot[i].Quantity = itr.quantity + "";
                            btnInventorySlot[i].btnOfChargedItem = false;
                        }
                        else
                        {
                            btnInventorySlot[i].Quantity = "";
                            btnInventorySlot[i].btnOfChargedItem = false;
                        }
                    }
                    //useable item
                    else if (itr.quantity != 1)
                    {
                        if (itr.quantity > 1)
                        {
                            btnInventorySlot[i].Quantity = (itr.quantity-1) + "";
                            //eg staff that can conjure three fireballs
                            if (!it.isStackable)
                            {
                                btnInventorySlot[i].btnOfChargedItem = true;
                            }
                            //eg potion
                            else
                            {
                                btnInventorySlot[i].btnOfChargedItem = false;
                            }
                        }
                        else
                        {
                            btnInventorySlot[i].Quantity = "0";
                            btnInventorySlot[i].btnOfChargedItem = true;
                        }
                    }
                }
                //no item in on button
                else
                {
                    btnInventorySlot[i].Img2 = null;
                    btnInventorySlot[i].Quantity = "";
                    btnInventorySlot[i].btnOfChargedItem = false;
                }
                cntSlot++;
            }
        }
	    //INVENTORY SCREEN (COMBAT and MAIN)
        public void redrawInventory()
        {
    	    //IF CONTROLS ARE NULL, CREATE THEM
    	    if (btnReturn == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    doItemStacking();
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
    	    int textH = (int)gv.drawFontRegHeight;
            int spacing = textH;
            int tabX = pW * 4;
    	    int tabX2 = 5 * gv.squareSize + pW * 2;
    	    int leftStartY = pH * 4;
    	    int tabStartY = 5 * gv.squareSize + pW * 10;
    	
            //DRAW TEXT		
		    locY = gv.squareSize + (pH * 2);
		    gv.DrawText("Party", locX + (gv.squareSize * 5) + pW * 2, locY);
            gv.DrawText("Inventory", locX + (gv.squareSize * 5) + pW * 2, locY += spacing);
		    locY = gv.squareSize + (pH * 2);
		    gv.DrawText("Party", tabX2 + (gv.squareSize * 6), locY);
            gv.DrawText(gv.mod.goldLabelPlural + ": " + gv.mod.partyGold, tabX2 + (gv.squareSize * 6), locY += spacing);

		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX
		    btnPageIndex.Draw();
		    btnInventoryLeft.Draw();
		    btnInventoryRight.Draw();		
		
		    //DRAW ALL INVENTORY SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnInventorySlot)
		    {
			    if (cntSlot == inventorySlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if (isSelectedItemSlotInPartyInventoryRange())
		    {
			    ItemRefs itRef = GetCurrentlySelectedItemRefs();
        	    Item it = gv.mod.getItemByResRefForInfo(itRef.resref);

                /*
                //Description
		        string textToSpan = "";
                //textToSpan = "Description:" + Environment.NewLine;
        	    //textToSpan += it.name + Environment.NewLine;
                //textToSpan = "<u>Description</u>" + "<BR>";
	            textToSpan += "<b><big>" + it.name + "</big></b><BR>";
	            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
	            {
	        	    textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<BR>";
	                textToSpan += "Attack Bonus: " + it.attackBonus + "<BR>";
	                textToSpan += "Attack Range: " + it.attackRange + "<BR>";
	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	                textToSpan += "Tap 'INFO' for Full Description<BR>";
	            }    
	            else if (!it.category.Equals("General"))
	            {
	        	    textToSpan += "AC Bonus: " + it.armorBonus + "<BR>";
	                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";
	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	                textToSpan += "Tap 'INFO' for Full Description<BR>";
	            }
	            else if (it.category.Equals("General"))
	            {
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	        	    textToSpan += "Tap 'INFO' for Full Description<BR>";
	            }
                */
                string textToSpan = gv.cc.buildItemInfoText(it, -3);
                description.tbXloc = (11 * gv.squareSize) + (pW * 5) + gv.oXshift;
                description.tbYloc = 2 * gv.squareSize;
                description.tbWidth = pW * 80;
                description.tbHeight = pH * 50;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
		    btnHelp.Draw();	
		    btnInfo.Draw();	
		    btnReturn.Draw();
        }
        public string isUseableBy(Item it)
        {
    	    string strg = "";
    	    foreach (PlayerClass cls in gv.mod.modulePlayerClassList)
    	    {
                string firstLetter = cls.name.Substring(0,1);
    		    foreach (ItemRefs stg in cls.itemsAllowed)
    		    {
    			    if (stg.resref.Equals(it.resref))
    			    {
    				    strg += firstLetter + ", ";
    			    }
    		    }
    	    }
    	    return strg;
        }
        public void onTouchInventory(MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
        {
            try
            {
                btnInventoryLeft.glowOn = false;
                btnInventoryRight.glowOn = false;
                btnHelp.glowOn = false;
                btnInfo.glowOn = false;
                btnReturn.glowOn = false;

                //int eventAction = event.getAction();
                switch (eventType)
                {
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)e.X;
                        int y = (int)e.Y;
                        if (btnInventoryLeft.getImpact(x, y))
                        {
                            btnInventoryLeft.glowOn = true;
                        }
                        else if (btnInventoryRight.getImpact(x, y))
                        {
                            btnInventoryRight.glowOn = true;
                        }
                        else if (btnHelp.getImpact(x, y))
                        {
                            btnHelp.glowOn = true;
                        }
                        else if (btnInfo.getImpact(x, y))
                        {
                            btnInfo.glowOn = true;
                        }
                        else if (btnReturn.getImpact(x, y))
                        {
                            btnReturn.glowOn = true;
                        }
                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)e.X;
                        y = (int)e.Y;

                        btnInventoryLeft.glowOn = false;
                        btnInventoryRight.glowOn = false;
                        btnHelp.glowOn = false;
                        btnInfo.glowOn = false;
                        btnReturn.glowOn = false;

                        for (int j = 0; j < slotsPerPage; j++)
                        {
                            if (btnInventorySlot[j].getImpact(x, y))
                            {
                                if (inventorySlotIndex == j)
                                {
                                    if (inCombat)
                                    {
                                        if (isSelectedItemSlotInPartyInventoryRange())
                                        {
                                            doItemAction(true);
                                        }
                                    }
                                    else
                                    {
                                        if (isSelectedItemSlotInPartyInventoryRange())
                                        {
                                            doItemAction(false);
                                        }
                                    }
                                }
                                inventorySlotIndex = j;
                            }
                        }
                        if (btnInventoryLeft.getImpact(x, y))
                        {
                            if (inventoryPageIndex > 0)
                            {
                                inventoryPageIndex--;
                                btnPageIndex.Text = (inventoryPageIndex + 1) + "/10";
                                resetInventory(inCombat);
                            }
                        }
                        else if (btnInventoryRight.getImpact(x, y))
                        {
                            if (inventoryPageIndex < 9)
                            {
                                inventoryPageIndex++;
                                btnPageIndex.Text = (inventoryPageIndex + 1) + "/10";
                                resetInventory(inCombat);
                            }
                        }
                        else if (btnHelp.getImpact(x, y))
                        {
                            tutorialMessageInventory(true);
                        }
                        else if (btnInfo.getImpact(x, y))
                        {
                            if (isSelectedItemSlotInPartyInventoryRange())
                            {
                                ItemRefs itRef = GetCurrentlySelectedItemRefs();
                                if (itRef == null) { return; }
                                Item it = gv.mod.getItemByResRef(itRef.resref);
                                if (it == null) { return; }
                                gv.cc.buildItemInfoText(it, -100);
                            }
                        }
                        else if (btnReturn.getImpact(x, y))
                        {
                            if (inCombat)
                            {
                                if (gv.screenCombat.canMove)
                                {
                                    gv.screenCombat.currentCombatMode = "move";
                                }
                                else
                                {
                                    gv.screenCombat.currentCombatMode = "attack";
                                }
                                gv.screenType = "combat";
                                doCleanUp();
                            }
                            else
                            {
                                gv.screenType = "main";
                                doCleanUp();
                            }
                        }
                        break;
                }
             }
            catch
            {
            }
         }
	
	    public void doCleanUp()
	    {
		    btnInventorySlot.Clear();
		    btnInventoryLeft = null;
		    btnInventoryRight = null;
		    btnPageIndex = null;
		    btnHelp = null;
		    btnInfo = null;
		    btnReturn = null;
	    }
	
	    public void doItemAction(bool inCombat)
	    {

            ItemRefs itRef = GetCurrentlySelectedItemRefs();
            bool isEquippedItem = false;
            foreach (string s in gv.mod.addedItemsRefs)
            {
                if (s == itRef.tag)
                {
                    isEquippedItem = true;
                    break;
                }
            }

            if (!isEquippedItem)
            {

                //todo: check wheter torach ahs use action, otehr wise add in

                //different item selector if item allows no use action at all
                Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                if ((it.onUseItem.Equals("none") && it.onUseItemIBScript.Equals("none") && it.onUseItemCastSpellTag.Equals("none")) || itRef.quantity == 0 || (inCombat && it.useableInSituation == "OutOfCombat") || (!inCombat && it.useableInSituation == "InCombat") || it.useableInSituation
                    == "Passive")
                {

                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    List<string> actionList = new List<string> { "View Item Description", "Drop Item"};
                    using (ItemListSelector itSel = new ItemListSelector(gv, actionList, "Item Action"))
                    {
                        itSel.IceBlinkButtonClose.Enabled = true;
                        itSel.IceBlinkButtonClose.Visible = true;
                        itSel.setupAll(gv);
                        var ret = itSel.ShowDialog();
                        //ItemRefs itRef = GetCurrentlySelectedItemRefs();
                        it = gv.mod.getItemByResRefForInfo(itRef.resref);

                        if (itSel.selectedIndex == 0) // selected to VIEW ITEM
                        {
                            gv.cc.buildItemInfoText(it, -100);
                        }
                        else if (itSel.selectedIndex == 1) // selected to DROP ITEM
                        {
                            DialogResult dlg = IBMessageBox.Show(gv, "Do you wish to drop this item forever?", enumMessageButton.YesNo);
                            if (dlg == DialogResult.Yes)
                            {
                                //drop item
                                itRef = GetCurrentlySelectedItemRefs();
                                it = gv.mod.getItemByResRef(itRef.resref);
                                if (!it.plotItem)
                                {
                                    gv.sf.RemoveItemFromInventory(itRef, 1);
                                }
                                else
                                {
                                    gv.sf.MessageBoxHtml("You can't drop this item.");
                                }
                            }
                        }

                    }
                    resetInventory(inCombat);
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                }
                else
                {
                    List<string> actionList = new List<string> { "Use Item", "Drop Item", "View Item Description" };

                    using (ItemListSelector itSel = new ItemListSelector(gv, actionList, "Item Action"))
                    {
                        itSel.IceBlinkButtonClose.Enabled = true;
                        itSel.IceBlinkButtonClose.Visible = true;
                        itSel.setupAll(gv);
                        var ret = itSel.ShowDialog();
                        //ItemRefs itRef = GetCurrentlySelectedItemRefs();
                        it = gv.mod.getItemByResRefForInfo(itRef.resref);
                        if ((itSel.selectedIndex == 0) && (((!it.onUseItem.Equals("none") && itRef.quantity != 0)) || ((!it.onUseItemIBScript.Equals("none")) && itRef.quantity != 0) || ((!it.onUseItemCastSpellTag.Equals("none") && itRef.quantity != 0))))
                        {
                            // selected to USE ITEM
                            List<string> pcNames = new List<string>();
                            pcNames.Add("cancel");

                            if (inCombat || it.isLightSource)
                            {
                                Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                                //check to see if use IBScript first
                                bool isClassBound = false;
                                foreach (PlayerClass pClass in gv.mod.modulePlayerClassList)
                                {
                                    foreach (ItemRefs iRef in pClass.itemsAllowed)
                                    {
                                        if (iRef == itRef)
                                        {
                                            isClassBound = true;
                                            break;
                                        }
                                    }
                                    if (isClassBound)
                                    {
                                        break;
                                    }
                                }
                                if ((pc.playerClass.containsItemRefsWithResRef(itRef.resref) || !isClassBound) && (!it.onlyUseableWhenEquipped) && (it.requiredLevel <= pc.classLevel) && gv.cc.checkRequirmentsMet(pc, it))
                                {
                                    if (inCombat && !it.isLightSource)
                                    {
                                        if (!it.onUseItem.Equals("none"))
                                        {
                                            //Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                                            doItemInventoryScriptBasedOnFilename(gv.mod.playerList[gv.screenCombat.currentPlayerIndex]);
                                            gv.screenCombat.currentCombatMode = "move";
                                            gv.screenType = "combat";
                                            gv.screenCombat.endPcTurn(false);
                                        }
                                        else if (!it.onUseItemIBScript.Equals("none"))
                                        {
                                            doItemInventoryIBScript(gv.screenCombat.currentPlayerIndex);
                                            gv.screenCombat.currentCombatMode = "move";
                                            gv.screenType = "combat";
                                            gv.screenCombat.endPcTurn(false);
                                        }
                                        else if (!it.onUseItemCastSpellTag.Equals("none"))
                                        {
                                            doItemInventoryCastSpellCombat(gv.screenCombat.currentPlayerIndex);
                                            gv.screenCombat.currentCombatMode = "cast";
                                            gv.screenType = "combat";
                                            //gv.cc.currentSelectedSpell = gv.mod.getSpellByTag(it.onUseItemCastSpellTag);
                                            //if (gv.cc.currentSelectedSpell.usesTurnToActivate)
                                            //{
                                            //gv.screenCombat.endPcTurn(false);
                                            //}
                                        }
                                    }
                                    //light source outside combat
                                    else if (!inCombat)
                                    {
                                        //check to see if use IBScript first
                                        if (!it.onUseItem.Equals("none"))
                                        {
                                            pc = gv.mod.playerList[gv.mod.selectedPartyLeader];
                                            doItemInventoryScriptBasedOnFilename(pc);
                                        }
                                        else if (!it.onUseItemIBScript.Equals("none"))
                                        {
                                            doItemInventoryIBScript(gv.mod.selectedPartyLeader);
                                        }
                                        else if (!it.onUseItemCastSpellTag.Equals("none"))
                                        {
                                            bool outsideCombat = !inCombat;
                                            doItemInventoryCastSpell(gv.mod.selectedPartyLeader, outsideCombat);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!pc.playerClass.containsItemRefsWithResRef(itRef.resref) && isClassBound)
                                    {
                                        //add message that this class cannot use this time
                                        IBMessageBox.Show(gv, "The item cannot be used by this player's class.");
                                    }
                                    else if (it.requiredLevel > pc.classLevel)
                                    {
                                        IBMessageBox.Show(gv, "Player level is not high enough to use this item.");
                                    }
                                    else if (!gv.cc.checkRequirmentsMet(pc, it))
                                    {
                                        IBMessageBox.Show(gv, "Item requirements not met.");
                                    }
                                    else
                                    {
                                        //or can only be used while equipped
                                        IBMessageBox.Show(gv, "The item can only be used while equipped.");
                                    }
                                }


                                /*
                                if (!pc.isTemporaryAllyForThisEncounterOnly)
                                {
                                    pcNames.Add(pc.name);
                                }
                                */
                            }
                            //not in combat and no light source item
                            else
                            {
                                foreach (Player pc in gv.mod.playerList)
                                {
                                    pcNames.Add(pc.name);
                                }
                                //}
                                using (ItemListSelector itSel2 = new ItemListSelector(gv, pcNames, "Selected PC to Use Item"))
                                {
                                    itSel2.IceBlinkButtonClose.Enabled = true;
                                    itSel2.IceBlinkButtonClose.Visible = true;
                                    itSel2.setupAll(gv);
                                    var ret2 = itSel2.ShowDialog();
                                    if (itSel2.selectedIndex > 0)
                                    {
                                        try
                                        {
                                            Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                                            itRef = GetCurrentlySelectedItemRefs();
                                            it = gv.mod.getItemByResRefForInfo(itRef.resref);
                                            bool isClassBound = false;
                                            foreach (PlayerClass pClass in gv.mod.modulePlayerClassList)
                                            {
                                                foreach (ItemRefs iRef in pClass.itemsAllowed)
                                                {
                                                    if (iRef == itRef)
                                                    {
                                                        isClassBound = true;
                                                        break;
                                                    }
                                                }
                                                if (isClassBound)
                                                {
                                                    break;
                                                }
                                            }
                                            if ((pc.playerClass.containsItemRefsWithResRef(itRef.resref) || !isClassBound) && it.requiredLevel <= pc.classLevel && gv.cc.checkRequirmentsMet(pc, it))
                                            {
                                                if (inCombat)
                                                {
                                                    //check to see if use IBScript first
                                                    if (!it.onUseItem.Equals("none"))
                                                    {
                                                        //Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                                                        doItemInventoryScriptBasedOnFilename(pc);
                                                        gv.screenCombat.currentCombatMode = "move";
                                                        gv.screenType = "combat";
                                                        gv.screenCombat.endPcTurn(false);
                                                    }
                                                    else if (!it.onUseItemIBScript.Equals("none"))
                                                    {
                                                        doItemInventoryIBScript(gv.screenCombat.currentPlayerIndex);
                                                        gv.screenCombat.currentCombatMode = "move";
                                                        gv.screenType = "combat";
                                                        gv.screenCombat.endPcTurn(false);
                                                    }
                                                    else if (!it.onUseItemCastSpellTag.Equals("none"))
                                                    {
                                                        doItemInventoryCastSpellCombat(gv.screenCombat.currentPlayerIndex);
                                                        gv.screenCombat.currentCombatMode = "cast";
                                                        gv.screenType = "combat";
                                                        //gv.cc.currentSelectedSpell = gv.mod.getSpellByTag(it.onUseItemCastSpellTag);
                                                        //if (gv.cc.currentSelectedSpell.usesTurnToActivate)
                                                        //{
                                                        //gv.screenCombat.endPcTurn(false);
                                                        //}
                                                    }
                                                }
                                                //outside combat
                                                else
                                                {
                                                    //check to see if use IBScript first
                                                    if (!it.onUseItem.Equals("none"))
                                                    {
                                                        pc = gv.mod.playerList[itSel2.selectedIndex - 1];
                                                        doItemInventoryScriptBasedOnFilename(pc);
                                                    }
                                                    else if (!it.onUseItemIBScript.Equals("none"))
                                                    {
                                                        doItemInventoryIBScript(itSel2.selectedIndex - 1);
                                                    }
                                                    else if (!it.onUseItemCastSpellTag.Equals("none"))
                                                    {
                                                        bool outsideCombat = !inCombat;
                                                        doItemInventoryCastSpell(itSel2.selectedIndex - 1, outsideCombat);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //item not allowed for class
                                                if (!pc.playerClass.containsItemRefsWithResRef(itRef.resref) && isClassBound)
                                                {
                                                    IBMessageBox.Show(gv, "The item cannot be used by this player's class.");
                                                }
                                                else if (it.requiredLevel > pc.classLevel)
                                                {
                                                    IBMessageBox.Show(gv, "Player level is not high enough to use this item.");
                                                }
                                                else if (!gv.cc.checkRequirmentsMet(pc, it))
                                                {
                                                    IBMessageBox.Show(gv, "Item requirements not met.");
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            gv.errorLog(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        else if (itSel.selectedIndex == 1) // selected to DROP ITEM
                        {
                            DialogResult dlg = IBMessageBox.Show(gv, "Do you wish to drop this item forever?", enumMessageButton.YesNo);
                            if (dlg == DialogResult.Yes)
                            {
                                //drop item
                                itRef = GetCurrentlySelectedItemRefs();
                                it = gv.mod.getItemByResRef(itRef.resref);
                                if (!it.plotItem)
                                {
                                    gv.sf.RemoveItemFromInventory(itRef, 1);
                                }
                                else
                                {
                                    gv.sf.MessageBoxHtml("You can't drop this item.");
                                }
                            }
                        }
                        else if (itSel.selectedIndex == 2) // selected to VIEW ITEM
                        {
                            gv.cc.buildItemInfoText(it, -100);
                        }
                    }
                    resetInventory(inCombat);
                }
            }

            //is equipped item
            else
            {
                //different item selector if item allows no use action at all
                Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                if ((it.onUseItem.Equals("none") && it.onUseItemIBScript.Equals("none") && it.onUseItemCastSpellTag.Equals("none")) || itRef.quantity == 0 || (inCombat && it.useableInSituation == "OutOfCombat") || (!inCombat && it.useableInSituation == "InCombat") || it.useableInSituation
                    == "Passive")
                {

                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    List<string> actionList = new List<string> { "View Item Description" };
                    using (ItemListSelector itSel = new ItemListSelector(gv, actionList, "Item Action"))
                    {
                        itSel.IceBlinkButtonClose.Enabled = true;
                        itSel.IceBlinkButtonClose.Visible = true;
                        itSel.setupAll(gv);
                        var ret = itSel.ShowDialog();
                        //ItemRefs itRef = GetCurrentlySelectedItemRefs();
                        it = gv.mod.getItemByResRefForInfo(itRef.resref);
                        
                        if (itSel.selectedIndex == 0) // selected to VIEW ITEM
                        {
                            gv.cc.buildItemInfoText(it, -100);
                        }
                    }
                    resetInventory(inCombat);
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                }
                else
                {

                    List<string> actionList = new List<string> { "Use Item", "View Item Description" };
                    using (ItemListSelector itSel = new ItemListSelector(gv, actionList, "Item Action"))
                    {
                        itSel.IceBlinkButtonClose.Enabled = true;
                        itSel.IceBlinkButtonClose.Visible = true;
                        itSel.setupAll(gv);
                        var ret = itSel.ShowDialog();
                        //ItemRefs itRef = GetCurrentlySelectedItemRefs();
                        it = gv.mod.getItemByResRefForInfo(itRef.resref);

                        bool classLevelRequirementMet = false;
                        bool attribueRequirementsMet = false;
                        if (inCombat)
                        {
                            Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                            if (pc.classLevel >= it.requiredLevel)
                            {
                                classLevelRequirementMet = true;
                            }
                            attribueRequirementsMet = gv.cc.checkRequirmentsMet(pc, it);
                        }
                        else
                        {

                            Player pc = gv.mod.playerList[gv.mod.selectedPartyLeader];
                            if (pc.classLevel >= it.requiredLevel)
                            {
                                classLevelRequirementMet = true;
                            }
                            attribueRequirementsMet = gv.cc.checkRequirmentsMet(pc, it);
                        }

                        if ((itSel.selectedIndex == 0) && (((!it.onUseItem.Equals("none") && itRef.quantity != 0)) || ((!it.onUseItemIBScript.Equals("none")) && itRef.quantity != 0) || ((!it.onUseItemCastSpellTag.Equals("none") && itRef.quantity != 0))) && classLevelRequirementMet && attribueRequirementsMet)
                        {
                            // selected to USE ITEM
                            List<string> pcNames = new List<string>();
                            pcNames.Add("cancel");
                            if (inCombat)
                            {
                                Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                                //if (!pc.isTemporaryAllyForThisEncounterOnly)
                                //{
                                pcNames.Add(pc.name);
                                //}
                            }
                            else
                            {

                                Player pc = gv.mod.playerList[gv.mod.selectedPartyLeader];
                                pcNames.Add(pc.name);

                            }

                            //pc choosing selector

                            //1. find out if current pc can use the item

                            //2. if he can, do it

                            //3. if he cant, cancel
                            if (inCombat)
                            {
                                //check to see if use IBScript first
                                if (!it.onUseItem.Equals("none"))
                                {
                                    //Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                                    doItemInventoryScriptBasedOnFilename(gv.mod.playerList[gv.screenCombat.currentPlayerIndex]);
                                    gv.screenCombat.currentCombatMode = "move";
                                    gv.screenType = "combat";
                                    gv.screenCombat.endPcTurn(false);
                                }
                                else if (!it.onUseItemIBScript.Equals("none"))
                                {
                                    doItemInventoryIBScript(gv.screenCombat.currentPlayerIndex);
                                    gv.screenCombat.currentCombatMode = "move";
                                    gv.screenType = "combat";
                                    gv.screenCombat.endPcTurn(false);
                                }
                                else if (!it.onUseItemCastSpellTag.Equals("none"))
                                {
                                    doItemInventoryCastSpellCombat(gv.screenCombat.currentPlayerIndex);
                                    gv.screenCombat.currentCombatMode = "cast";
                                    gv.screenType = "combat";

                                    gv.cc.currentSelectedSpell = gv.mod.getSpellByTag(it.onUseItemCastSpellTag);
                                    if (gv.cc.currentSelectedSpell.usesTurnToActivate)
                                    {
                                        //gv.screenCombat.endPcTurn(false);
                                        gv.screenCombat.continueTurn = false;
                                        gv.screenCombat.dontEndTurn = false;
                                    }
                                }
                            }
                            //outside combat
                            //Player pc = gv.mod.playerList[gv.mod.selectedPartyLeader];
                            else
                            {
                                //check to see if use IBScript first
                                if (!it.onUseItem.Equals("none"))
                                {
                                    //pc = gv.mod.playerList[itSel2.selectedIndex - 1];
                                    doItemInventoryScriptBasedOnFilename(gv.mod.playerList[gv.mod.selectedPartyLeader]);
                                }
                                else if (!it.onUseItemIBScript.Equals("none"))
                                {
                                    doItemInventoryIBScript(gv.mod.selectedPartyLeader);
                                }
                                else if (!it.onUseItemCastSpellTag.Equals("none"))
                                {
                                    bool outsideCombat = !inCombat;
                                    doItemInventoryCastSpell(gv.mod.selectedPartyLeader, outsideCombat);
                                }
                            }
                        }
                        else if (itSel.selectedIndex == 1) // selected to VIEW ITEM
                        {
                            gv.cc.buildItemInfoText(it, -100);
                        }
                        else if (!classLevelRequirementMet)
                        {
                            gv.sf.MessageBoxHtml("Class level is not high enough to use item.");
                        }
                        else if (!attribueRequirementsMet)
                        {
                            gv.sf.MessageBoxHtml("Item requirements not met.");
                        }
                    }
                    resetInventory(inCombat);
                }
            }
	    }
        public void doItemInventoryScriptBasedOnFilename(Player pc)
        {
            //punkrock
            if (isSelectedItemSlotInPartyInventoryRange())
            {
                ItemRefs itRef = GetCurrentlySelectedItemRefs();
                if (itRef.quantity != 0)
                {
                    gv.cc.doItemScriptBasedOnUseItem(pc, itRef, true);
                    Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                    if (it.destroyItemAfterOnUseItemIBScript && itRef.quantity == 1)
                    {
                        gv.screenItemSelector.unequipItemOnDestroy(it, pc, itRef);
                        gv.sf.RemoveItemFromInventory(itRef, 1);
                        //resetInventory(true);
                    }
                    if (itRef.quantity > 1)
                    {
                        itRef.quantity--;
                        if (itRef.quantity == 1)
                        {
                            itRef.quantity--;
                            if (it.destroyItemAfterOnUseItemIBScript)
                            {
                                gv.screenItemSelector.unequipItemOnDestroy(it, pc, itRef);
                                gv.sf.RemoveItemFromInventory(itRef, 1);
                            }
                        }
                    }
                }
            }

            bool inCombat = false;
            if (gv.screenType == "combatInventory")
            {
                inCombat = true;
            }
            resetInventory(inCombat);
        }

	    public void doItemInventoryIBScript(int pcIndex)
        {
    	    if (isSelectedItemSlotInPartyInventoryRange())
		    {
    		    ItemRefs itRef = GetCurrentlySelectedItemRefs();
    		    Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                if (itRef.quantity != 0)
                {
                    gv.mod.indexOfPCtoLastUseItem = pcIndex;
                    //do IBScript
                    gv.cc.doIBScriptBasedOnFilename(it.onUseItemIBScript, it.onUseItemIBScriptParms);
                    if (it.destroyItemAfterOnUseItemIBScript && itRef.quantity == 1)
                    {
                        gv.screenItemSelector.unequipItemOnDestroy(it, gv.mod.playerList[pcIndex],itRef);
                        gv.sf.RemoveItemFromInventory(itRef, 1);
                    }
                    if (itRef.quantity > 1)
                    {
                        itRef.quantity--;
                        if (itRef.quantity == 1)
                        {
                            itRef.quantity--;
                            if (it.destroyItemAfterOnUseItemIBScript)
                            {
                                gv.screenItemSelector.unequipItemOnDestroy(it, gv.mod.playerList[pcIndex], itRef);
                                gv.sf.RemoveItemFromInventory(itRef, 1);
                            }
                        }
                    }
                }	    	
		    }
            bool inCombat = false;
            if (gv.screenType == "combatInventory")
            {
                inCombat = true;
            }
            resetInventory(inCombat);
        }
        public void doItemInventoryCastSpellCombat(int pcIndex)
        {
            if (isSelectedItemSlotInPartyInventoryRange())
            {
                ItemRefs itRef = GetCurrentlySelectedItemRefs();
                Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                if (itRef.quantity != 0)
                {
                    gv.mod.indexOfPCtoLastUseItem = pcIndex;
                    gv.cc.currentSelectedSpell = gv.mod.getSpellByTag(it.onUseItemCastSpellTag);
                    if (it.destroyItemAfterOnUseItemCastSpell && itRef.quantity == 1)
                    {
                        //destroyer
                        //knacker2
                        gv.screenItemSelector.unequipItemOnDestroy(it, gv.mod.playerList[pcIndex], itRef);
                        gv.sf.RemoveItemFromInventory(itRef, 1);
                    }
                    if (itRef.quantity > 1)
                    {
                        itRef.quantity--;
                        if (itRef.quantity == 1)
                        {
                            itRef.quantity--;
                            if (it.destroyItemAfterOnUseItemCastSpell)
                            {
                                gv.screenItemSelector.unequipItemOnDestroy(it, gv.mod.playerList[pcIndex], itRef);
                                gv.sf.RemoveItemFromInventory(itRef, 1);
                            }
                        }
                    }
                }
            }
            bool inCombat = false;
            if (gv.screenType == "combatInventory")
            {
                inCombat = true;
            }
            resetInventory(inCombat);
        }
        public void doItemInventoryCastSpell(int pcIndex, bool outsideCombat)
        {
            if (isSelectedItemSlotInPartyInventoryRange())
            {
                ItemRefs itRef = GetCurrentlySelectedItemRefs();
                Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                if (itRef.quantity != 0)
                {
                    Spell sp = gv.mod.getSpellByTag(it.onUseItemCastSpellTag);
                    Player pc = gv.mod.playerList[pcIndex];
                    gv.mod.indexOfPCtoLastUseItem = pcIndex;
                    gv.cc.doSpellBasedOnScriptOrEffectTag(sp, it, pc, outsideCombat, false);
                    if (it.destroyItemAfterOnUseItemCastSpell && itRef.quantity == 1)
                    {
                        gv.screenItemSelector.unequipItemOnDestroy(it, gv.mod.playerList[pcIndex], itRef);
                        gv.sf.RemoveItemFromInventory(itRef, 1);
                    }
                    if (itRef.quantity > 1)
                    {
                        itRef.quantity--;
                        if (itRef.quantity == 1)
                        {
                            itRef.quantity--;
                            if (it.destroyItemAfterOnUseItemCastSpell)
                            {
                                gv.screenItemSelector.unequipItemOnDestroy(it, gv.mod.playerList[pcIndex], itRef);
                                gv.sf.RemoveItemFromInventory(itRef, 1);
                            }
                        }
                    }
                }
            }
            bool inCombat = false;
            if (gv.screenType == "combatInventory")
            {
                inCombat = true;
            }
            resetInventory(inCombat);
        }
        public int GetIndex()
	    {
		    return inventorySlotIndex + (inventoryPageIndex * slotsPerPage);
	    }
	
	    public void doItemStacking()
	    {
		    for (int i = 0; i < gv.mod.partyInventoryRefsList.Count; i++)
		    {
			    ItemRefs itr = gv.mod.partyInventoryRefsList[i];
			    Item itm = gv.mod.getItemByResRefForInfo(itr.resref);
			    if (itm.isStackable)
			    {
				    for (int j = gv.mod.partyInventoryRefsList.Count - 1; j >= 0; j--)
				    {
					    ItemRefs it = gv.mod.partyInventoryRefsList[j];
					    //do check to see if same resref and then stack and delete
					    if ((it.resref.Equals(itr.resref)) && (i != j))
					    {
						    itr.quantity += it.quantity;
						    gv.mod.partyInventoryRefsList.RemoveAt(j);
					    }
				    }
			    }
		    }
	    }
	    public ItemRefs GetCurrentlySelectedItemRefs()
	    {
		    return gv.mod.partyInventoryRefsList[GetIndex()];
	    }
	    public bool isSelectedItemSlotInPartyInventoryRange()
	    {
		    return GetIndex() < gv.mod.partyInventoryRefsList.Count;
	    }
	    public void tutorialMessageInventory(bool helpCall)
        {
    	    if ((gv.mod.showTutorialInventory) || (helpCall))
		    {
    		    gv.sf.MessageBoxHtml(gv.cc.stringMessageInventory);    		
			    gv.mod.showTutorialInventory = false;
		    }
        }
    }
}
