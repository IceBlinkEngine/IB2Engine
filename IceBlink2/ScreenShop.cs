using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenShop 
    {
	    //public gv.module gv.mod;
	    public GameView gv;

	    public List<IbbButton> btnInventorySlot = new List<IbbButton>();
	    public IbbButton btnInventoryLeft = null;
	    public IbbButton btnInventoryRight = null;
	    public IbbButton btnPageIndex = null;
	    public IbbButton btnShopLeft = null;
	    public IbbButton btnShopRight = null;
	    public IbbButton btnShopPageIndex = null;
	    public List<IbbButton> btnShopSlot = new List<IbbButton>();
	    private IbbButton btnHelp = null;
	    private IbbButton btnReturn = null;
        private IbbButton btnInfo = null;
        private IbbButton btnInfoShop = null;
        public int inventoryPageIndex = 0;
	    public int inventorySlotIndex = 0;
	    public int shopPageIndex = 0;
	    public int shopSlotIndex = 0;
	    public string currentShopTag = "";
	    public Shop currentShop = new Shop();
	    private string stringMessageShop = "";
        private IbbHtmlTextBox description;
	
        public ScreenShop(Module m, GameView g)
	    {
		    //gv.mod = m;
		    gv = g;

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
            gv.mod.addedItemsRefs.Clear();


            foreach (ItemRefs s in gv.mod.partyInventoryRefsList)
            {
                //do not add items currently equipped
                //dschungel
                bool allowAdding = true;
                foreach (Player p in gv.mod.playerList)
                {
                    if (p.AmmoRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.BodyRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.FeetRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.GlovesRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.HeadRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.MainHandRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.NeckRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.OffHandRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.RingRefs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                    if (p.Ring2Refs.tag == s.tag)
                    {
                        allowAdding = false;
                        break;
                    }
                }

                if (!allowAdding)
                {
                    gv.mod.partyInventoryRefsList.Remove(s);
                }
            }




            setControlsStart();
		    stringMessageShop = gv.cc.loadTextToString("data/MessageShop.txt");
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
			    btnInventoryLeft.X = 7 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f*pW);
			    btnInventoryLeft.Y = (int)(5f * gv.squareSize) - (pH * 2);
                btnInventoryLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnPageIndex == null)
		    {
			    btnPageIndex = new IbbButton(gv, 1.0f);
			    btnPageIndex.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnPageIndex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPageIndex.Text = "1/10";
			    btnPageIndex.X = 8 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f * pW);
			    btnPageIndex.Y = (int)(5f * gv.squareSize) - (pH * 2);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnInventoryRight == null)
		    {
			    btnInventoryRight = new IbbButton(gv, 1.0f);
			    btnInventoryRight.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInventoryRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnInventoryRight.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInventoryRight.X = 9 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f * pW);
			    btnInventoryRight.Y = (int)(5f * gv.squareSize) - (pH * 2);
                btnInventoryRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopLeft == null)
		    {
			    btnShopLeft = new IbbButton(gv, 1.0f);
			    btnShopLeft.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnShopLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnShopLeft.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopLeft.X = 7 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f * pW);
			    btnShopLeft.Y = (int)(0.5f * gv.squareSize) - (pH * 2);
                btnShopLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopPageIndex == null)
		    {
			    btnShopPageIndex = new IbbButton(gv, 1.0f);
			    btnShopPageIndex.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnShopPageIndex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopPageIndex.Text = "1/10";
			    btnShopPageIndex.X = 8 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f * pW);
			    btnShopPageIndex.Y = (int)(0.5f * gv.squareSize) - (pH * 2);
                btnShopPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopRight == null)
		    {
			    btnShopRight = new IbbButton(gv, 1.0f);
			    btnShopRight.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnShopRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnShopRight.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopRight.X = 9 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f * pW);
			    btnShopRight.Y = (int)(0.5f * gv.squareSize) - (pH * 2);
                btnShopRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnReturn == null)
		    {
			    btnReturn = new IbbButton(gv, 1.2f);	
			    btnReturn.Text = "EXIT SHOP";
			    btnReturn.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnReturn.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f) - (int)(0.5f*gv.squareSize);
			    btnReturn.Y = 9* gv.squareSize + pH * 2;
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
                    btnInfo.X = (5 * gv.squareSize) - padW * 1 + gv.oXshift;
                    btnInfo.Y = (int)(6.5f * gv.squareSize);
                    btnInfo.Height = (int)(gv.ibbheight * gv.screenDensity);
                    btnInfo.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                }

            if (btnInfoShop == null)
            {
                btnInfoShop = new IbbButton(gv, 0.8f);
                btnInfoShop.Text = "INFO";
                btnInfoShop.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnInfoShop.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnInfoShop.X = (5 * gv.squareSize) - padW * 1 + gv.oXshift;
                btnInfoShop.Y = (int)(2f * gv.squareSize) + 0*pH;
                btnInfoShop.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInfoShop.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }

            for (int j = 0; j < 10; j++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
			    btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    if (j < 5)
			    {
				    btnNew.X = ((j+2+4) * gv.squareSize) + (padW * (j+1)) + gv.oXshift;
				    btnNew.Y = (int)(6.5f * gv.squareSize);
			    }
			    else
			    {
				    btnNew.X = ((j-5+2+4) * gv.squareSize) + (padW * ((j-5)+1)) + gv.oXshift;
				    btnNew.Y = (int)((7.5f * gv.squareSize) + padW);
			    }
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnInventorySlot.Add(btnNew);
		    }
		    for (int j = 0; j < 10; j++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
			    btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    if (j < 5)
			    {
				    btnNew.X = ((j+2+4) * gv.squareSize) + (padW * (j+1)) + gv.oXshift;
				    btnNew.Y = 2 * gv.squareSize;
			    }
			    else
			    {
				    btnNew.X = ((j-5+2+4) * gv.squareSize) + (padW * ((j-5)+1)) + gv.oXshift;
				    btnNew.Y = 3 * gv.squareSize + padW;
			    }
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnShopSlot.Add(btnNew);
		    }
	    }

        public void redrawShop()
        {
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
            gv.mod.addedItemsRefs.Clear();

            this.doItemStackingParty();
    	    
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
    	    int textH = (int)gv.drawFontRegHeight;
            int spacing = textH;
    	    int tabX = pW * 4;
    	    int tabX2 = 5 * gv.squareSize + pW * 2;
    	    int leftStartY = pH * 4;
    	    int tabStartY = 9 * gv.squareSize + pH * 2;
    	    int tabShopStartY = 4 * gv.squareSize + pH * 2;
    	
    	    gv.DrawText(currentShop.shopName, 5 * gv.squareSize, locY + 1.5f*gv.squareSize, 1.0f, Color.DarkGray);

            int adder = 0;
            int highestNonStackable = -99;
            foreach (Player pc in gv.mod.playerList)
            {
                int mod = gv.sf.CalcShopBuyBackModifier(pc);
                if (mod > highestNonStackable)
                {
                    mod = highestNonStackable;
                }
            }

            if (highestNonStackable > -99) { adder = highestNonStackable; }

            //int totalBuyPerc = currentShop.buybackPercent + adder;
            int totalBuyPerc = currentShop.buybackPercent + currentShop.buybackModifier + adder;

            adder = 0;
            highestNonStackable = -99;
            foreach (Player pc in gv.mod.playerList)
            {
                int mod = gv.sf.CalcShopSellModifier(pc);
                if (mod > highestNonStackable)
                {
                    mod = highestNonStackable;
                }
            }

            if (highestNonStackable > -99) { adder = highestNonStackable; }
            int totalSellPerc = currentShop.sellPercent + currentShop.sellModifier + adder;

            gv.DrawText("Buys at: " + totalBuyPerc + "%, sells at: " + totalSellPerc + "%", 8 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f * pW), locY + 1.5f * gv.squareSize, 1.0f, Color.Blue);
           
            //8 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f * pW)


            //DRAW LEFT/RIGHT ARROWS and PAGE INDEX of SHOP
            btnShopPageIndex.Draw();
		    btnShopLeft.Draw();
		    btnShopRight.Draw();
            btnInfo.Draw();
            btnInfoShop.Draw();

            //DRAW ALL SHOP INVENTORY SLOTS of SHOP		
            int cntSlot = 0;
		    foreach (IbbButton btn in btnShopSlot)
		    {
			    if (cntSlot == shopSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
			    {
				    ItemRefs itrs = currentShop.shopItemRefs[cntSlot + (shopPageIndex * 10)];
				    Item it = gv.mod.getItemByResRefForInfo(itrs.resref);
                    gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = gv.cc.LoadBitmap(it.itemImage);	
				    if (itrs.quantity < it.groupSizeForSellingStackableItems)
    			    {
                        //less than the stack size for selling
                        //int cost = (itrs.quantity * it.value) / it.groupSizeForSellingStackableItems;
                        int cost = (itrs.quantity * storeSellValueForItem(it)) / it.groupSizeForSellingStackableItems;
                        btn.Text = "" + cost;
    			    }
    			    else //have more than the stack size for selling
    			    {
                        //int full = (itrs.quantity / it.groupSizeForSellingStackableItems) * it.value;
                        //int part = ((itrs.quantity % it.groupSizeForSellingStackableItems) * it.value) / it.groupSizeForSellingStackableItems;
                        int full = (itrs.quantity / it.groupSizeForSellingStackableItems) * storeSellValueForItem(it);
                        int part = ((itrs.quantity % it.groupSizeForSellingStackableItems) * storeSellValueForItem(it)) / it.groupSizeForSellingStackableItems;

                        int total = full + part;
    				    btn.Text = "" + total;
    			    }

                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    //ItemRefs itr = gv.mod.partyInventoryRefsList[cntSlot + (inventoryPageIndex * slotsPerPage)];

                    //if ()
                    //bockauf
                    //chargelogic
                    //check shop: no sepaartwd selling of charges items
                    //check zero charges items - hopefully not deleted




                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    if ((it.onUseItemCastSpellTag == "none" || it.onUseItemCastSpellTag == "") && (it.onUseItemIBScript == "none" || it.onUseItemIBScript == "") && (it.onUseItem == "none" || it.onUseItem == ""))
                    {
                        if (itrs.quantity > 1)
                        {
                            btn.Quantity = itrs.quantity + "";
                            btn.btnOfChargedItem = false;
                        }
                        else
                        {
                            btn.Quantity = "";
                            btn.btnOfChargedItem = false;
                        }
                    }
                    //useable item
                    else if (itrs.quantity != 1)
                    {
                        if (itrs.quantity > 1)
                        {
                            btn.Quantity = (itrs.quantity - 1) + "";
                            //eg staff that can conjure three fireballs
                            if (!it.isStackable)
                            {
                                btn.btnOfChargedItem = true;
                                btn.Text = storeSellValueForItem(it).ToString();
                            }
                            //eg potion
                            else
                            {
                                btn.btnOfChargedItem = false;
                            }
                        }
                        else
                        {
                            btn.Quantity = "0";
                            btn.btnOfChargedItem = true;
                            btn.Text = storeSellValueForItem(it).ToString();
                        }
                    }
                    /*
				    if (itrs.quantity > 1)
				    {
					    btn.Quantity = itrs.quantity + "";
				    }
				    else
				    {
					    btn.Quantity = "";
				    }
                    */
                }
			    else
			    {
				    btn.Img2 = null;
				    btn.Text = "";
				    btn.Quantity = "";
                    btn.btnOfChargedItem = false;
                }
                btn.btnWithGold = true;
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX of SHOP
		    locY = tabShopStartY;		
		    if ((shopSlotIndex + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
		    {
                //DRAW DESCRIPTION BOX
			    Item it = gv.mod.getItemByResRefForInfo(currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)].resref);

                /*string textToSpan = "<b><i><big>" + it.name + "</big></i></b><BR>";
	            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
	            {
	        	    textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<br>";
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	                textToSpan += "Attack Bonus: " + it.attackBonus + "<br>";
	                textToSpan += "Attack Range: " + it.attackRange + "<BR>";	        	
	            }    
	            else if (!it.category.Equals("General"))
	            {
	        	    textToSpan += "AC Bonus: " + it.armorBonus + "<br>";
	                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";	       
	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	            }
	            else if (it.category.Equals("General"))
	            {
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	            }
                */

                string textToSpan = gv.cc.buildItemInfoText(it, 3);
                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                description.tbXloc = 12 * gv.squareSize;
                description.tbYloc = 2 * gv.squareSize -1*pH;
                description.tbWidth = pW * 40;
                description.tbHeight = pH * 50;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
		
		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX
		    btnPageIndex.Draw();
		    btnInventoryLeft.Draw();
		    btnInventoryRight.Draw();		
		
		    //DRAW TEXT		
		    locY = (5 * gv.squareSize) + (pH * 2);
		    gv.DrawText("Party Inventory", gv.squareSize * 5, locY + gv.squareSize - 2*pH, 1.0f, Color.DarkGray);
            //gv.DrawText("Inventory", locX + gv.squareSize * 4, locY += spacing, 1.0f, Color.DarkGray);
		    //locY = (5 * gv.squareSize) + (pH * -2);
		    //gv.DrawText("Party", tabX2 + gv.squareSize * 5.5f - 0*pW, locY+pH, 1.0f, Color.Gold);
            //gv.DrawText(gv.mod.goldLabelPlural + ": " + gv.mod.partyGold, tabX2 + gv.squareSize * 5.5f - 0*pW, locY += (int)2*spacing, 1.0f, Color.Gold);
            gv.DrawText(gv.mod.goldLabelPlural + ": " + gv.mod.partyGold, 8 * gv.squareSize + (int)(0.5f * gv.squareSize) + (int)(0.75f * pW), locY + gv.squareSize - 2 * pH, 1.0f, Color.Gold);
            //gv.DrawText(gv.mod.goldLabelPlural + ": " + gv.mod.partyGold, 12 * gv.squareSize, locY += (int)2 * spacing, 1.0f, Color.Gold);
            //locY = (5 * gv.squareSize) + (pH * -2);

            //gv.DrawText("Store", tabX2 + gv.squareSize * 0.5f - 4 * pW, locY + pH, 1.25f, Color.Blue);
            //gv.DrawText("Margin" + ": " + getStoreMargin() + "%" , tabX2 + gv.squareSize * 0.5f - 4 * pW, locY += (int)2 * spacing, 1.25f, Color.Blue);
            //gv.DrawText("Shop", 5 *gv.squareSize, locY + pH, 1.0f, Color.Blue);
            //gv.DrawText("Margin" + ": " + getStoreMargin() + "%", 5*gv.squareSize, locY += (int)2 * spacing, 1.0f, Color.Blue);


            //DRAW ALL PARTY INVENTORY SLOTS		
            cntSlot = 0;
		    foreach (IbbButton btn in btnInventorySlot)
		    {
			    if (cntSlot == inventorySlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (inventoryPageIndex * 10)) < gv.mod.partyInventoryRefsList.Count)
			    {
				    ItemRefs itr = gv.mod.partyInventoryRefsList[cntSlot + (inventoryPageIndex * 10)];
				    Item it = gv.mod.getItemByResRefForInfo(itr.resref);
                    gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = gv.cc.LoadBitmap(it.itemImage);	
				    if (itr.quantity < it.groupSizeForSellingStackableItems)
    			    {
                        //less than the stack size for selling
                        //int cost = (itr.quantity * it.value) / it.groupSizeForSellingStackableItems;
                        int cost = (itr.quantity * storeBuyBackValueForItem(it)) / it.groupSizeForSellingStackableItems;
                        btn.Text = "" + cost;
    			    }
    			    else //have more than the stack size for selling
    			    {
                        //int full = (itr.quantity / it.groupSizeForSellingStackableItems) * it.value;
                        //int part = ((itr.quantity % it.groupSizeForSellingStackableItems) * it.value) / it.groupSizeForSellingStackableItems;
                        int full = (itr.quantity / it.groupSizeForSellingStackableItems) * storeBuyBackValueForItem(it);
                        int part = ((itr.quantity % it.groupSizeForSellingStackableItems) * storeBuyBackValueForItem(it)) / it.groupSizeForSellingStackableItems;

                        int total = full + part;
    				    btn.Text = "" + total;
    			    }
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    if ((it.onUseItemCastSpellTag == "none" || it.onUseItemCastSpellTag == "") && (it.onUseItemIBScript == "none" || it.onUseItemIBScript == "") && (it.onUseItem == "none" || it.onUseItem == ""))
                    {
                        if (itr.quantity > 1)
                        {
                            btn.Quantity = itr.quantity + "";
                            btn.btnOfChargedItem = false;
                        }
                        else
                        {
                            btn.Quantity = "";
                            btn.btnOfChargedItem = false;
                        }
                    }
                    //useable item
                    else if (itr.quantity != 1)
                    {
                        if (itr.quantity > 1)
                        {
                            btn.Quantity = (itr.quantity - 1) + "";
                            //eg staff that can conjure three fireballs
                            if (!it.isStackable)
                            {
                                btn.btnOfChargedItem = true;
                                btn.Text = storeBuyBackValueForItem(it).ToString();
                            }
                            //eg potion
                            else
                            {
                                btn.btnOfChargedItem = false;
                            }
                        }
                        else
                        {
                            btn.Quantity = "0";
                            btn.btnOfChargedItem = true;
                            btn.Text = storeBuyBackValueForItem(it).ToString();
                        }
                    }
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    /*				
				    if (itr.quantity > 1)
				    {
					    btn.Quantity = itr.quantity + "";
				    }
				    else
				    {
					    btn.Quantity = "";
				    }
                    */
                }
			    else
			    {
				    btn.Img2 = null;
				    btn.Text = "";
				    btn.Quantity = "";
                    btn.btnOfChargedItem = false;
                }
                btn.btnWithGold = true;
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if ((inventorySlotIndex + (inventoryPageIndex * 10)) < gv.mod.partyInventoryRefsList.Count)
		    {
			    ItemRefs itr = gv.mod.partyInventoryRefsList[inventorySlotIndex + (inventoryPageIndex * 10)];
			    Item it = gv.mod.getItemByResRefForInfo(itr.resref);

                /*
			    string textToSpan = "<b><i><big>" + it.name + "</big></i></b><BR>";
	            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
	            {
	        	    textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<br>";
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	                textToSpan += "Attack Bonus: " + it.attackBonus + "<br>";
	                textToSpan += "Attack Range: " + it.attackRange + "<BR>";	            
	            }    
	            else if (!it.category.Equals("General"))
	            {
	        	    textToSpan += "AC Bonus: " + it.armorBonus + "<br>";
	                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";	 
	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	            }
	            else if (it.category.Equals("General"))
	            {
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	            }
                */

                string textToSpan = gv.cc.buildItemInfoText(it, 2);

                description.tbXloc = 12 * gv.squareSize;
                description.tbYloc = 6 * gv.squareSize + 4*pH;
                description.tbWidth = pW * 40;
                description.tbHeight = pH * 50;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
				
		    btnHelp.Draw();		
		    btnReturn.Draw();
        }

        public int storeSellValueForItem(Item it)
        {
            //int sellPrice = (int)(it.value * ((float)currentShop.sellPercent / 100f));  
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Player pc in gv.mod.playerList)
            {
                int mod = gv.sf.CalcShopSellModifier(pc);
                if (mod > highestNonStackable)
                {
                    mod = highestNonStackable;
                }
            }
            
            if (highestNonStackable > -99) { adder = highestNonStackable; }
            
            //int totalSellPerc = currentShop.sellPercent + adder;
            int totalSellPerc = currentShop.sellPercent + currentShop.sellModifier + adder;

            int sellPrice = (int)(it.value * ((float)totalSellPerc / 100f));

            if (sellPrice< 1) { sellPrice = 1; }  
            return sellPrice;  
        }
          
        public int storeBuyBackValueForItem(Item it)
        {
            //int buyPrice = (int)(it.value * ((float)currentShop.buybackPercent / 100f));  
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Player pc in gv.mod.playerList)
            {
                int mod = gv.sf.CalcShopBuyBackModifier(pc);
                if (mod > highestNonStackable)
                {
                    mod = highestNonStackable;
                 }
            }
            
                        if (highestNonStackable > -99) { adder = highestNonStackable; }
            
            //int totalBuyPerc = currentShop.buybackPercent + adder;
            int totalBuyPerc = currentShop.buybackPercent + currentShop.buybackModifier + adder;
            int buyPrice = (int)(it.value * ((float)totalBuyPerc / 100f));

            if (buyPrice< 1) { buyPrice = 1; }
            if (buyPrice > storeSellValueForItem(it)) { buyPrice = storeSellValueForItem(it); }
            return buyPrice;  
        }  

	
        public string isUseableBy(Item it)
        {
    	    string strg = "";
    	    foreach (PlayerClass cls in gv.mod.modulePlayerClassList)
    	    {
    		    string firstLetter = cls.name.Substring(0,1);
    		    foreach (ItemRefs itr in cls.itemsAllowed)
    		    {
    			    if (itr.resref.Equals(it.resref))
    			    {
    				    strg += firstLetter + ", ";
    			    }
    		    }
    	    }
    	    return strg;
        }
        public void doItemStackingParty()
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

        public int getStoreMargin()
        {
            int margin = 0;

            int adder = 0;
            int highestNonStackable = -99;
            foreach (Player pc in gv.mod.playerList)
            {
                int mod = gv.sf.CalcShopBuyBackModifier(pc);
                if (mod > highestNonStackable)
                {
                    mod = highestNonStackable;
                }
            }

            if (highestNonStackable > -99) { adder = highestNonStackable; }

            //int totalBuyPerc = currentShop.buybackPercent + adder;
            int totalBuyPerc = currentShop.buybackPercent + currentShop.buybackModifier + adder;

            adder = 0;
            highestNonStackable = -99;
            foreach (Player pc in gv.mod.playerList)
            {
                int mod = gv.sf.CalcShopSellModifier(pc);
                if (mod > highestNonStackable)
                {
                    mod = highestNonStackable;
                }
            }

            if (highestNonStackable > -99) { adder = highestNonStackable; }

            //int totalBuyPerc = currentShop.buybackPercent + adder;
            int totalSellPerc = currentShop.sellPercent + currentShop.sellModifier + adder;

            margin = totalSellPerc - totalBuyPerc;

            return margin;
        }

        public void doItemStackingShop()
	    {
    	    //Not being used but leaving here just in case for future use
		    for (int i = 0; i < currentShop.shopItemRefs.Count; i++)
		    {
			    ItemRefs itr = currentShop.shopItemRefs[i];
			    for (int j = currentShop.shopItemRefs.Count - 1; j >= 0; j--)
			    {
				    ItemRefs it = currentShop.shopItemRefs[j];
				    //do check to see if same resref and then stack and delete
				    if ((it.resref.Equals(itr.resref)) && (i != j))
				    {
					    itr.quantity += it.quantity;
					    currentShop.shopItemRefs.RemoveAt(j);
				    }
			    }
		    }
	    }
    
        public void onTouchShop(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
            try
            {
                btnInventoryLeft.glowOn = false;
                btnInventoryRight.glowOn = false;
                btnHelp.glowOn = false;
                btnInfo.glowOn = false;
                btnInfoShop.glowOn = false;
                btnReturn.glowOn = false;
                btnShopLeft.glowOn = false;
                btnShopRight.glowOn = false;

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
                        else if (btnInfoShop.getImpact(x, y))
                        {
                            btnInfoShop.glowOn = true;
                        }
                        else if (btnReturn.getImpact(x, y))
                        {
                            btnReturn.glowOn = true;
                        }
                        else if (btnShopLeft.getImpact(x, y))
                        {
                            btnShopLeft.glowOn = true;
                        }
                        else if (btnShopRight.getImpact(x, y))
                        {
                            btnShopRight.glowOn = true;
                        }
                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)e.X;
                        y = (int)e.Y;

                        btnInventoryLeft.glowOn = false;
                        btnInventoryRight.glowOn = false;
                        btnHelp.glowOn = false;
                        btnInfoShop.glowOn = false;
                        btnInfo.glowOn = false;
                        btnReturn.glowOn = false;
                        btnShopLeft.glowOn = false;
                        btnShopRight.glowOn = false;

                        for (int j = 0; j < 10; j++)
                        {
                            if (btnInventorySlot[j].getImpact(x, y))
                            {
                                if (inventorySlotIndex == j)
                                {
                                    doInventoryActions();
                                }
                                inventorySlotIndex = j;
                            }
                        }
                        for (int j = 0; j < 10; j++)
                        {
                            if (btnShopSlot[j].getImpact(x, y))
                            {
                                if (shopSlotIndex == j)
                                {
                                    doShopActions();
                                }
                                shopSlotIndex = j;
                            }
                        }
                        if (btnInventoryLeft.getImpact(x, y))
                        {
                            if (inventoryPageIndex > 0)
                            {
                                inventoryPageIndex--;
                                btnPageIndex.Text = (inventoryPageIndex + 1) + "/10";
                            }
                        }
                        else if (btnInventoryRight.getImpact(x, y))
                        {
                            if (inventoryPageIndex < 9)
                            {
                                inventoryPageIndex++;
                                btnPageIndex.Text = (inventoryPageIndex + 1) + "/10";
                            }
                        }
                        else if (btnShopLeft.getImpact(x, y))
                        {
                            if (shopPageIndex > 0)
                            {
                                shopPageIndex--;
                                btnShopPageIndex.Text = (shopPageIndex + 1) + "/10";
                            }
                        }
                        else if (btnShopRight.getImpact(x, y))
                        {
                            if (shopPageIndex < 9)
                            {
                                shopPageIndex++;
                                btnShopPageIndex.Text = (shopPageIndex + 1) + "/10";
                            }
                        }
                        else if (btnHelp.getImpact(x, y))
                        {
                            tutorialMessageShop();
                        }
                        else if (btnInfo.getImpact(x, y))
                        {

                            //XXXtutorialMessageShop();
                            if ((inventorySlotIndex + (inventoryPageIndex * 10)) < gv.mod.partyInventoryRefsList.Count)
                            {

                                ItemRefs itRef = gv.mod.partyInventoryRefsList[inventorySlotIndex + (inventoryPageIndex * 10)];
                                //ItemRefs itRef = GetCurrentlySelectedItemRefs();
                                if (itRef == null) { return; }
                                Item it = gv.mod.getItemByResRef(itRef.resref);
                                if (it == null) { return; }
                                gv.cc.buildItemInfoText(it, -100);
                            }
                        }
                        else if (btnInfoShop.getImpact(x, y))
                        {
                            //halt1
                            //XXXtutorialMessageShop();
                            if ((shopSlotIndex + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
                            {

                                ItemRefs itRef = currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)];
                                //ItemRefs itRef = GetCurrentlySelectedItemRefs();
                                if (itRef == null) { return; }
                                Item it = gv.mod.getItemByResRef(itRef.resref);
                                if (it == null) { return; }
                                gv.cc.buildItemInfoText(it, -100);
                            }
                        }
                        else if (btnReturn.getImpact(x, y))
                        {
                            gv.screenType = "main";
                        }
                        break;
                }
            }
            catch
            { }
	    }
	
	    public void doInventoryActions()
	    {
		    if ((inventorySlotIndex + (inventoryPageIndex * 10)) < gv.mod.partyInventoryRefsList.Count)
		    {
                DialogResult dlg = IBMessageBox.Show(gv, "Do you wish to sell this item?", enumMessageButton.YesNo);
                if (dlg == DialogResult.Yes)
                {
                    //sell item
                    ItemRefs itr = gv.mod.partyInventoryRefsList[inventorySlotIndex + (inventoryPageIndex * 10)];
                    Item it = gv.mod.getItemByResRef(itr.resref);
                    if (it != null)
                    {
                        if (!it.plotItem)
                        {

                            bool chargedItem = false;
                            //reaper
                            //*********************************************
                            if ((it.onUseItemCastSpellTag == "none" || it.onUseItemCastSpellTag == "") && (it.onUseItemIBScript == "none" || it.onUseItemIBScript == "") && (it.onUseItem == "none" || it.onUseItem == ""))
                            {
                                if (itr.quantity > 1)
                                {
                                
                                    chargedItem = false;
                                }
                                else
                                {
                               
                                    chargedItem = false;
                                }
                            }
                            //useable item
                            else if (itr.quantity != 1)
                            {
                                if (itr.quantity > 1)
                                {
                                    //btn.Quantity = (itrs.quantity - 1) + "";
                                    //eg staff that can conjure three fireballs
                                    if (!it.isStackable)
                                    {
                                        //btn.btnOfChargedItem = true;
                                        chargedItem = true;
                                    }
                                    //eg potion
                                    else
                                    {
                                        //btn.btnOfChargedItem = false;
                                        chargedItem = false;
                                    }
                                }
                                else
                                {
                                    //btn.Quantity = "0";
                                    //btn.btnOfChargedItem = true;
                                    chargedItem = true;
                                }
                            }

                            //*********************************************

                            if (!chargedItem)
                            {
                                if (itr.quantity < it.groupSizeForSellingStackableItems)
                                {
                                    //less than the stack size for selling
                                    //gv.mod.partyGold += (itr.quantity * it.value) / it.groupSizeForSellingStackableItems;
                                    gv.mod.partyGold += (itr.quantity * storeBuyBackValueForItem(it)) / it.groupSizeForSellingStackableItems;

                                    ItemRefs itrCopy = itr.DeepCopy();
                                    itrCopy.quantity = itr.quantity;
                                    currentShop.shopItemRefs.Add(itrCopy);
                                    //remove item and tag from party inventory
                                    gv.sf.RemoveItemFromInventory(itr, itr.quantity);
                                }
                                else //have more than the stack size for selling
                                {
                                    //gv.mod.partyGold += it.value;
                                    gv.mod.partyGold += storeBuyBackValueForItem(it);
                                    ItemRefs itrCopy = itr.DeepCopy();
                                    itrCopy.quantity = it.groupSizeForSellingStackableItems;
                                    currentShop.shopItemRefs.Add(itrCopy);
                                    //remove item and tag from party inventory
                                    gv.sf.RemoveItemFromInventory(itr, it.groupSizeForSellingStackableItems);
                                }
                            }
                            //is charged item
                            else
                            {
                                gv.mod.partyGold += storeBuyBackValueForItem(it);
                                ItemRefs itrCopy = itr.DeepCopy();
                                currentShop.shopItemRefs.Add(itrCopy);
                                gv.sf.RemoveItemFromInventory(itr, itr.quantity);
                            }
                        }
                        else
                        {
                            gv.sf.MessageBoxHtml("You can't sell this item.");
                        }
                    }
                }
                if (dlg == DialogResult.No)
                {
                    //do nothing
                }
		    }
	    }
	
	    public void doShopActions()
	    {
		    if ((shopSlotIndex + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
		    {
                //check to see if have enough gold and whether number of light sources and rations do not exceed carry limits
	            Item it = gv.mod.getItemByResRef(currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)].resref);
                if (it != null)
                {
                    ///if (gv.mod.partyGold < it.value)
                    if (gv.mod.partyGold < storeSellValueForItem(it))
                    {
                        gv.sf.MessageBoxHtml("Your party does not have enough gold to purchase this item.");
                        return;
                    }

                    if ((it.isRation) && (gv.mod.numberOfRationsRemaining >= gv.mod.maxNumberOfRationsAllowed))
                    {
                        gv.sf.MessageBoxHtml("Too much encumbrance by rations - your party can carry only " + gv.mod.maxNumberOfRationsAllowed.ToString() + ".");
                        return;
                    }

                    int lightSourceCounter = 0;
                    if (it.isLightSource)
                    {
                        foreach (ItemRefs itRef in gv.mod.partyInventoryRefsList)
                        {

                            lightSourceCounter++;
                        }

                        if (lightSourceCounter >= gv.mod.maxNumberOfLightSourcesAllowed)
                        {
                            gv.sf.MessageBoxHtml("Too much encumbrance by light sources - your party can carry only " + gv.mod.maxNumberOfLightSourcesAllowed.ToString() + ".");
                            return;
                        }
                    }

                    /*
                    int lightSourceCounter = 0;
                    if (it.onUseItemIBScript.Equals("doPartyLight"))
                    {
                        foreach (ItemRefs itRef in gv.gv.mod.partyInventoryRefsList)
                        {
                            lightSourceCounter++;
                        }

                        if (lightSourceCounter >= gv.gv.mod.maxNumberOfLightSourcesAllowed)
                        {
                            gv.sf.MessageBoxHtml("Too much encumbrance by light sources - your party can carry only " + gv.gv.mod.maxNumberOfLightSourcesAllowed.ToString() + ".");
                            return;
                        }
                    }
                    */
                }
                DialogResult dlg = IBMessageBox.Show(gv, "Do you wish to buy this item?", enumMessageButton.YesNo);
                if (dlg == DialogResult.Yes)
                {
                    //buy item
                    ItemRefs itr = currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)];
                    it = gv.mod.getItemByResRef(itr.resref);
                    if (it != null)
                    {
                        if (it.isRation)
                        {
                            //gv.gv.mod.numberOfRationsRemaining = it.quantity;
                            if (it.quantity <= 1)
                            {
                                gv.mod.numberOfRationsRemaining++;
                            }
                            else
                            {
                                gv.mod.numberOfRationsRemaining += it.quantity;
                            }
                        }
                        if (itr.quantity < it.groupSizeForSellingStackableItems)
                        {
                            //less than the stack size for selling
                            //gv.mod.partyGold -= (itr.quantity * it.value) / it.groupSizeForSellingStackableItems;
                            gv.mod.partyGold -= (itr.quantity * storeSellValueForItem(it)) / it.groupSizeForSellingStackableItems;

                            //add item and tag to party inventory
                            gv.mod.partyInventoryRefsList.Add(itr.DeepCopy());
                            //remove tag from shop list
                            currentShop.shopItemRefs.Remove(itr);
                        }
                        else //have more than the stack size for selling
                        {
                            //subtract gold from party
                            //gv.mod.partyGold -= it.value;
                            gv.mod.partyGold -= storeSellValueForItem(it);
                            //add item and tag to party inventory
                            gv.mod.partyInventoryRefsList.Add(itr.DeepCopy());
                            //remove tag from shop list
                            currentShop.shopItemRefs.Remove(itr);
                        }
                    }
                }
                if (dlg == DialogResult.No)
                {
                    //do nothing
                }
		    }
	    }
	
	    public void tutorialMessageShop()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageShop);    	
        }
    }
}
