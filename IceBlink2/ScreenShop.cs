using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class ScreenShop 
    {
	    public Module mod;
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
		    mod = m;
		    gv = g;
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
			    btnInventoryLeft.X = 7 * gv.squareSize;
			    btnInventoryLeft.Y = (5 * gv.squareSize) - (pH * 2);
                btnInventoryLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnPageIndex == null)
		    {
			    btnPageIndex = new IbbButton(gv, 1.0f);
			    btnPageIndex.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnPageIndex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPageIndex.Text = "1/10";
			    btnPageIndex.X = 8 * gv.squareSize;
			    btnPageIndex.Y = (5 * gv.squareSize) - (pH * 2);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnInventoryRight == null)
		    {
			    btnInventoryRight = new IbbButton(gv, 1.0f);
			    btnInventoryRight.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInventoryRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnInventoryRight.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInventoryRight.X = 9 * gv.squareSize;
			    btnInventoryRight.Y = (5 * gv.squareSize) - (pH * 2);
                btnInventoryRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopLeft == null)
		    {
			    btnShopLeft = new IbbButton(gv, 1.0f);
			    btnShopLeft.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnShopLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnShopLeft.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopLeft.X = 7 * gv.squareSize;
			    btnShopLeft.Y = (1 * gv.squareSize) - (pH * 2);
                btnShopLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopPageIndex == null)
		    {
			    btnShopPageIndex = new IbbButton(gv, 1.0f);
			    btnShopPageIndex.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnShopPageIndex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopPageIndex.Text = "1/10";
			    btnShopPageIndex.X = 8 * gv.squareSize;
			    btnShopPageIndex.Y = (1 * gv.squareSize) - (pH * 2);
                btnShopPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopRight == null)
		    {
			    btnShopRight = new IbbButton(gv, 1.0f);
			    btnShopRight.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnShopRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnShopRight.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopRight.X = 9 * gv.squareSize;
			    btnShopRight.Y = (1 * gv.squareSize) - (pH * 2);
                btnShopRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnReturn == null)
		    {
			    btnReturn = new IbbButton(gv, 1.2f);	
			    btnReturn.Text = "EXIT SHOP";
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
		    for (int j = 0; j < 10; j++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
			    btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    if (j < 5)
			    {
				    btnNew.X = ((j+2+4) * gv.squareSize) + (padW * (j+1)) + gv.oXshift;
				    btnNew.Y = 6 * gv.squareSize;
			    }
			    else
			    {
				    btnNew.X = ((j-5+2+4) * gv.squareSize) + (padW * ((j-5)+1)) + gv.oXshift;
				    btnNew.Y = 7 * gv.squareSize + padW;
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
    	    this.doItemStackingParty();
    	    //this.doItemStackingShop();
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
    	    int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            int spacing = textH;
    	    //int spacing = (int)gv.mSheetTextPaint.getTextSize() + pH;
    	    int tabX = pW * 4;
    	    int tabX2 = 5 * gv.squareSize + pW * 2;
    	    int leftStartY = pH * 4;
    	    int tabStartY = 9 * gv.squareSize + pH * 2;
    	    int tabShopStartY = 4 * gv.squareSize + pH * 2;
    	
    	    //canvas.drawColor(Color.DKGRAY);
    	    //gv.mSheetTextPaint.setColor(Color.LTGRAY);
		    gv.DrawText(currentShop.shopName, 7 * gv.squareSize, locY, 1.4f, Color.DarkGray);
		
	
		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX of SHOP
		    btnShopPageIndex.Draw();
		    btnShopLeft.Draw();
		    btnShopRight.Draw();		
		
		    //DRAW ALL INVENTORY SLOTS of SHOP		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnShopSlot)
		    {
			    if (cntSlot == shopSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
			    {
				    ItemRefs itrs = currentShop.shopItemRefs[cntSlot + (shopPageIndex * 10)];
				    Item it = mod.getItemByResRefForInfo(itrs.resref);
				    btn.Img2 = gv.cc.LoadBitmap(it.itemImage);	
				    if (itrs.quantity < it.groupSizeForSellingStackableItems)
    			    {
    				    //less than the stack size for selling
    				    int cost = (itrs.quantity * it.value) / it.groupSizeForSellingStackableItems;
    				    btn.Text = "" + cost;
    			    }
    			    else //have more than the stack size for selling
    			    {
    				    int full = (itrs.quantity / it.groupSizeForSellingStackableItems) * it.value;
    				    int part = ((itrs.quantity % it.groupSizeForSellingStackableItems) * it.value) / it.groupSizeForSellingStackableItems;
    				    int total = full + part;
    				    btn.Text = "" + total;
    			    }
				
				    //btn.Quantity = itrs.quantity + "";
				    if (itrs.quantity > 1)
				    {
					    btn.Quantity = itrs.quantity + "";
				    }
				    else
				    {
					    btn.Quantity = "";
				    }
			    }
			    else
			    {
				    btn.Img2 = null;
				    btn.Text = "";
				    btn.Quantity = "";
			    }
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX of SHOP
		    locY = tabShopStartY;		
		    if ((shopSlotIndex + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
		    {
                //DRAW DESCRIPTION BOX
			    Item it = mod.getItemByResRefForInfo(currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)].resref);
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
	        	    //textToSpan += it.desc;
	            }
                //IbRect rect = new IbRect(tabX, locY, pW * 80, pH * 50);
                //gv.DrawText(textToSpan, rect, 1.0f, Color.White);

                description.tbXloc = 12 * gv.squareSize;
                description.tbYloc = 2 * gv.squareSize;
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
		    //gv.mSheetTextPaint.setColor(Color.LTGRAY);
		    gv.DrawText("Party", locX + gv.squareSize * 4, locY, 1.0f, Color.DarkGray);
            gv.DrawText("Inventory", locX + gv.squareSize * 4, locY += spacing, 1.0f, Color.DarkGray);
		    locY = (5 * gv.squareSize) + (pH * 2);
		    //gv.mSheetTextPaint.setColor(Color.YELLOW);
            gv.DrawText("Party", tabX2 + gv.squareSize * 5, locY, 1.0f, Color.Yellow);
            gv.DrawText("Gold: " + mod.partyGold, tabX2 + gv.squareSize * 5, locY += spacing, 1.0f, Color.Yellow);
		
		    //DRAW ALL INVENTORY SLOTS		
		    cntSlot = 0;
		    foreach (IbbButton btn in btnInventorySlot)
		    {
			    if (cntSlot == inventorySlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (inventoryPageIndex * 10)) < mod.partyInventoryRefsList.Count)
			    {
				    ItemRefs itr = mod.partyInventoryRefsList[cntSlot + (inventoryPageIndex * 10)];
				    Item it = mod.getItemByResRefForInfo(itr.resref);
				    btn.Img2 = gv.cc.LoadBitmap(it.itemImage);	
				    if (itr.quantity < it.groupSizeForSellingStackableItems)
    			    {
    				    //less than the stack size for selling
    				    int cost = (itr.quantity * it.value) / it.groupSizeForSellingStackableItems;
    				    btn.Text = "" + cost;
    			    }
    			    else //have more than the stack size for selling
    			    {
    				    int full = (itr.quantity / it.groupSizeForSellingStackableItems) * it.value;
    				    int part = ((itr.quantity % it.groupSizeForSellingStackableItems) * it.value) / it.groupSizeForSellingStackableItems;
    				    int total = full + part;
    				    btn.Text = "" + total;
    			    }				
				    //btn.Quantity = itr.quantity + "";
				    if (itr.quantity > 1)
				    {
					    btn.Quantity = itr.quantity + "";
				    }
				    else
				    {
					    btn.Quantity = "";
				    }
			    }
			    else
			    {
				    btn.Img2 = null;
				    btn.Text = "";
				    btn.Quantity = "";
			    }
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if ((inventorySlotIndex + (inventoryPageIndex * 10)) < mod.partyInventoryRefsList.Count)
		    {
			    ItemRefs itr = mod.partyInventoryRefsList[inventorySlotIndex + (inventoryPageIndex * 10)];
			    Item it = mod.getItemByResRefForInfo(itr.resref);
			    //TextPaint tp = new TextPaint();
	            //tp.setColor(Color.WHITE);
	            //tp.setTextSize(gv.mSheetTextPaint.getTextSize());
	            //tp.setTextAlign(Align.LEFT);
	            //tp.setAntiAlias(true);
	            //tp.setTypeface(gv.uiFont);	        
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
                //IbRect rect = new IbRect(tabX, locY, pW * 80, pH * 50);
                //gv.DrawText(textToSpan, rect, 1.0f, Color.White);

                description.tbXloc = 12 * gv.squareSize;
                description.tbYloc = 6 * gv.squareSize;
                description.tbWidth = pW * 40;
                description.tbHeight = pH * 50;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
				
		    btnHelp.Draw();		
		    btnReturn.Draw();
        }
	
        public string isUseableBy(Item it)
        {
    	    string strg = "";
    	    foreach (PlayerClass cls in mod.modulePlayerClassList)
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
		    for (int i = 0; i < mod.partyInventoryRefsList.Count; i++)
		    {
			    ItemRefs itr = mod.partyInventoryRefsList[i];
			    Item itm = mod.getItemByResRefForInfo(itr.resref);
			    if (itm.isStackable)
			    {
				    for (int j = mod.partyInventoryRefsList.Count - 1; j >= 0; j--)
				    {
					    ItemRefs it = mod.partyInventoryRefsList[j];
					    //do check to see if same resref and then stack and delete
					    if ((it.resref.Equals(itr.resref)) && (i != j))
					    {
						    itr.quantity += it.quantity;
						    mod.partyInventoryRefsList.RemoveAt(j);
					    }
				    }
			    }
		    }
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
		    btnInventoryLeft.glowOn = false;
		    btnInventoryRight.glowOn = false;
		    btnHelp.glowOn = false;
		    btnReturn.glowOn = false;
		    btnShopLeft.glowOn = false;
		    btnShopRight.glowOn = false;	
		
		    //int eventAction = event.getAction();
		    switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) e.X;
			    int y = (int) e.Y;
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
			    btnReturn.glowOn = false;
			    btnShopLeft.glowOn = false;
			    btnShopRight.glowOn = false;	
			
			    for (int j = 0; j < 10; j++)
			    {
				    if (btnInventorySlot[j].getImpact(x, y))
				    {
					    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
					    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
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
					    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
					    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
					    if (shopSlotIndex == j)
					    {
						    doShopActions();
					    }
					    shopSlotIndex = j;
				    }
			    }
			    if (btnInventoryLeft.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (inventoryPageIndex > 0)
				    {
					    inventoryPageIndex--;
					    btnPageIndex.Text = (inventoryPageIndex + 1) + "/10";
				    }
			    }
			    else if (btnInventoryRight.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (inventoryPageIndex < 9)
				    {
					    inventoryPageIndex++;
					    btnPageIndex.Text = (inventoryPageIndex + 1) + "/10";
				    }
			    }
			    else if (btnShopLeft.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (shopPageIndex > 0)
				    {
					    shopPageIndex--;
					    btnShopPageIndex.Text = (shopPageIndex + 1) + "/10";
				    }
			    }
			    else if (btnShopRight.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (shopPageIndex < 9)
				    {
					    shopPageIndex++;
					    btnShopPageIndex.Text = (shopPageIndex + 1) + "/10";
				    }
			    }
			    else if (btnHelp.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    tutorialMessageShop();
			    }
			    else if (btnReturn.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.screenType = "main";	
			    }
			    break;		
		    }
	    }
	
	    public void doInventoryActions()
	    {
		    if ((inventorySlotIndex + (inventoryPageIndex * 10)) < mod.partyInventoryRefsList.Count)
		    {
                DialogResult dlg = IBMessageBox.Show(gv, "Do you wish to sell this item?", enumMessageButton.YesNo);
                if (dlg == DialogResult.Yes)
                {
                    //sell item
                    ItemRefs itr = mod.partyInventoryRefsList[inventorySlotIndex + (inventoryPageIndex * 10)];
                    Item it = mod.getItemByResRef(itr.resref);
                    if (it != null)
                    {
                        if (!it.plotItem)
                        {
                            if (itr.quantity < it.groupSizeForSellingStackableItems)
                            {
                                //less than the stack size for selling
                                mod.partyGold += (itr.quantity * it.value) / it.groupSizeForSellingStackableItems;
                                ItemRefs itrCopy = itr.DeepCopy();
                                itrCopy.quantity = itr.quantity;
                                currentShop.shopItemRefs.Add(itrCopy);
                                //remove item and tag from party inventory
                                gv.sf.RemoveItemFromInventory(itr, itr.quantity);
                            }
                            else //have more than the stack size for selling
                            {
                                mod.partyGold += it.value;
                                ItemRefs itrCopy = itr.DeepCopy();
                                itrCopy.quantity = it.groupSizeForSellingStackableItems;
                                currentShop.shopItemRefs.Add(itrCopy);
                                //remove item and tag from party inventory
                                gv.sf.RemoveItemFromInventory(itr, it.groupSizeForSellingStackableItems);
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
                /*
			    // Strings to Show In Dialog with Radio Buttons
			    final CharSequence[] items = {" Sell Item "," View Item Description "};				            
	            // Creating and Building the Dialog 
	            AlertDialog.Builder builder = new AlertDialog.Builder(gv.gameContext);
	            builder.setTitle("Party Item Action");
	            builder.setItems(items, new DialogInterface.OnClickListener() 
	            {
	                public void onClick(DialogInterface dialog, int item) 
	                {
	            	    if (item == 0)
	            	    {	      
	            		    // selected to SELL ITEM
	            		    final CharSequence[] items = {" YES "," NO "};				            
	                        AlertDialog.Builder builder = new AlertDialog.Builder(gv.gameContext);
	                        builder.setTitle("Are you sure you wish to sell the item?");
	                        builder.setItems(items, new DialogInterface.OnClickListener() 
	                        {
	    	                    public void onClick(DialogInterface dialog, int item) 
	    	                    {
	    	                	    if (item == 0) // selected YES
	    	                	    {		    	                		
	    	                		    //sell item
	    	                		    ItemRefs itr = mod.partyInventoryRefsList.get(inventorySlotIndex + (inventoryPageIndex * 10));
	    	                		    Item it = mod.getItemByResRef(itr.resref);
	    	                		    if (it != null)
	    	                		    {
		    	                		    if (!it.plotItem)
		    	                		    {
		    	                			    if (itr.quantity < it.groupSizeForSellingStackableItems)
		    	                			    {
		    	                				    //less than the stack size for selling
		    	                				    mod.partyGold += (itr.quantity * it.value) / it.groupSizeForSellingStackableItems;
		    	                				    ItemRefs itrCopy = itr.DeepCopy();
				    	                		    itrCopy.quantity = itr.quantity;
				    	                		    currentShop.shopItemRefs.add(itrCopy);
				    	                		    //remove item and tag from party inventory
				    	                		    gv.sf.RemoveItemFromInventory(itr, itr.quantity);
		    	                			    }
		    	                			    else //have more than the stack size for selling
		    	                			    {
		    	                				    mod.partyGold += it.value;
		    	                				    ItemRefs itrCopy = itr.DeepCopy();
				    	                		    itrCopy.quantity = it.groupSizeForSellingStackableItems;
				    	                		    currentShop.shopItemRefs.add(itrCopy);
				    	                		    //remove item and tag from party inventory
				    	                		    gv.sf.RemoveItemFromInventory(itr, it.groupSizeForSellingStackableItems);
		    	                			    }
		    	                		    }
		    	                		    else
		    	                		    {
		    	                			    gv.sf.MessageBoxHtml("You can't sell this item.");
		    	                		    }
	    	                		    }
	    	                	    }
	    	                	    else if (item == 1) // selected NO
	    	                	    {	                		
	    	                		    //do nothing
	    	                	    }
	    	                        gv.ActionDialog.dismiss();
	    	                        gv.invalidate();
	    	                    }
	                        });
	                        gv.ActionDialog = builder.create();
	                        gv.ActionDialog.show();
	            	    }
	            	    else if (item == 1) // selected to VIEW ITEM
	            	    {	           
	            		    ItemRefs itr = mod.partyInventoryRefsList.get(inventorySlotIndex + (inventoryPageIndex * 10));
	            		    Item it = mod.getItemByResRefForInfo(itr.resref);
	            		    String textToSpan = "<u>Description</u>" + "<BR>";
	        	            textToSpan += "<b><i><big>" + it.name + "</big></i></b><BR>";
	        	            if ((it.category.equals("Melee")) || (it.category.equals("Ranged")))
	        	            {
	        	        	    textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<BR>";
	        	                textToSpan += "Attack Bonus: " + it.attackBonus + "<BR>";
	        	                textToSpan += "Attack Range: " + it.attackRange + "<BR>";
	        	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	        	                //textToSpan += "Area of Effect: " + it.AreaOfEffect + "<BR>";
	        	                textToSpan += "<BR>";
	        	                if (!it.descFull.equals(""))
	        	                {
	        	            	    textToSpan += it.descFull;
	        	                }
	        	                else
	        	                {
	        	            	    textToSpan += it.desc;
	        	                }
	        	            }    
	        	            else if (!it.category.equals("General"))
	        	            {
	        	        	    textToSpan += "AC Bonus: " + it.armorBonus + "<BR>";
	        	                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";
	        	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	        	                textToSpan += "<BR>";
	        	                if (!it.descFull.equals(""))
	        	                {
	        	            	    textToSpan += it.descFull;
	        	                }
	        	                else
	        	                {
	        	            	    textToSpan += it.desc;
	        	                }
	        	            }
	        	            else if (it.category.equals("General"))
	        	            {
	        	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	        	                textToSpan += "<BR>";
	        	        	    if (!it.descFull.equals(""))
	        	                {
	        	            	    textToSpan += it.descFull;
	        	                }
	        	                else
	        	                {
	        	            	    textToSpan += it.desc;
	        	                }
	        	            }
	        	            gv.sf.MessageBoxHtml(textToSpan);
	            	    }
	                    gv.ItemDialog.dismiss();
	                    gv.invalidate();
	                }
	            });
	            gv.ItemDialog = builder.create();
	            gv.ItemDialog.show();
                */
		    }
	    }
	
	    public void doShopActions()
	    {
		    if ((shopSlotIndex + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
		    {
                //check to see if have enough gold
	            Item it = mod.getItemByResRef(currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)].resref);
                if (it != null)
                {
                    if (mod.partyGold < it.value)
                    {
                        gv.sf.MessageBoxHtml("Your party does not have enough gold to purchase this item.");
                        return;
                    }
                }
                DialogResult dlg = IBMessageBox.Show(gv, "Do you wish to buy this item?", enumMessageButton.YesNo);
                if (dlg == DialogResult.Yes)
                {
                    //buy item
                    ItemRefs itr = currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)];
                    it = mod.getItemByResRef(itr.resref);
                    if (it != null)
                    {
                        if (itr.quantity < it.groupSizeForSellingStackableItems)
                        {
                            //less than the stack size for selling
                            mod.partyGold -= (itr.quantity * it.value) / it.groupSizeForSellingStackableItems;
                            //add item and tag to party inventory
                            mod.partyInventoryRefsList.Add(itr.DeepCopy());
                            //mod.partyInventoryTagList.add(it.tag);
                            //remove tag from shop list
                            currentShop.shopItemRefs.Remove(itr);
                        }
                        else //have more than the stack size for selling
                        {
                            //subtract gold from party
                            mod.partyGold -= it.value;
                            //add item and tag to party inventory
                            mod.partyInventoryRefsList.Add(itr.DeepCopy());
                            //mod.partyInventoryTagList.add(it.tag);
                            //remove tag from shop list
                            currentShop.shopItemRefs.Remove(itr);
                        }
                    }
                }
                if (dlg == DialogResult.No)
                {
                    //do nothing
                }
                /*
			    // Strings to Show In Dialog with Radio Buttons
			    final CharSequence[] items = {" Buy Item "," View Item Description "};				            
	            // Creating and Building the Dialog 
	            AlertDialog.Builder builder = new AlertDialog.Builder(gv.gameContext);
	            builder.setTitle("Shop Item Action");
	            builder.setItems(items, new DialogInterface.OnClickListener() 
	            {
	                public void onClick(DialogInterface dialog, int item) 
	                {
	            	    if (item == 0)
	            	    {
	            		    //check to see if have enough gold
	            		    Item it = mod.getItemByResRef(currentShop.shopItemRefs.get(shopSlotIndex + (shopPageIndex * 10)).resref);
	            		    if (it != null)
	            		    {
		            		    if (mod.partyGold >= it.value)
		            		    {
			            		    // selected to BUY ITEM
			            		    final CharSequence[] items = {" YES "," NO "};				            
			                        AlertDialog.Builder builder = new AlertDialog.Builder(gv.gameContext);
			                        builder.setTitle("Are you sure you wish to buy the item?");
			                        builder.setItems(items, new DialogInterface.OnClickListener() 
			                        {
			    	                    public void onClick(DialogInterface dialog, int item) 
			    	                    {
			    	                	    if (item == 0) // selected YES
			    	                	    {	                		
			    	                		    //buy item
			    	                		    ItemRefs itr = currentShop.shopItemRefs.get(shopSlotIndex + (shopPageIndex * 10));
			    	                		    Item it = mod.getItemByResRef(itr.resref);
			    	                		    if (it != null)
			    	                		    {
			    	                			    if (itr.quantity < it.groupSizeForSellingStackableItems)
			    	                			    {
			    	                				    //less than the stack size for selling
			    	                				    mod.partyGold -= (itr.quantity * it.value) / it.groupSizeForSellingStackableItems;
			    	                				    //add item and tag to party inventory
					    	                		    mod.partyInventoryRefsList.add(itr.DeepCopy());
					    	                            //mod.partyInventoryTagList.add(it.tag);
					    	                            //remove tag from shop list
					    	                		    currentShop.shopItemRefs.remove(itr);
			    	                			    }
			    	                			    else //have more than the stack size for selling
			    	                			    {
			    	                				    //subtract gold from party
					    	                		    mod.partyGold -= it.value;		    	                		
					    	                		    //add item and tag to party inventory
					    	                		    mod.partyInventoryRefsList.add(itr.DeepCopy());
					    	                            //mod.partyInventoryTagList.add(it.tag);
					    	                            //remove tag from shop list
					    	                		    currentShop.shopItemRefs.remove(itr);
			    	                			    }
				    	                		
			    	                		    }
			    	                	    }
			    	                	    else if (item == 1) // selected NO
			    	                	    {	                		
			    	                		    //do nothing
			    	                	    }
			    	                        gv.ActionDialog.dismiss();
			    	                        gv.invalidate();
			    	                    }
			                        });
			                        gv.ActionDialog = builder.create();
			                        gv.ActionDialog.show();
		            		    }
		            		    else
		            		    {
		            			    gv.sf.MessageBoxHtml("Your party does not have enough gold to purchase this item.");	
		            		    }
	            		    }
	            	    }
	            	    else if (item == 1) // selected to VIEW ITEM
	            	    {	           
	            		    Item it = mod.getItemByResRef(currentShop.shopItemRefs.get(shopSlotIndex + (shopPageIndex * 10)).resref);
	            		    String textToSpan = "<u>Description</u>" + "<BR>";
	        	            textToSpan += "<b><i><big>" + it.name + "</big></i></b><BR>";
	        	            if ((it.category.equals("Melee")) || (it.category.equals("Ranged")))
	        	            {
	        	        	    textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<BR>";
	        	                textToSpan += "Attack Bonus: " + it.attackBonus + "<BR>";
	        	                textToSpan += "Attack Range: " + it.attackRange + "<BR>";
	        	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	        	                //textToSpan += "Area of Effect: " + it.AreaOfEffect + "<BR>";
	        	                textToSpan += "<BR>";
	        	                if (!it.descFull.equals(""))
	        	                {
	        	            	    textToSpan += it.descFull;
	        	                }
	        	                else
	        	                {
	        	            	    textToSpan += it.desc;
	        	                }
	        	            }    
	        	            else if (!it.category.equals("General"))
	        	            {
	        	        	    textToSpan += "AC Bonus: " + it.armorBonus + "<BR>";
	        	                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";
	        	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	        	                textToSpan += "<BR>";
	        	                if (!it.descFull.equals(""))
	        	                {
	        	            	    textToSpan += it.descFull;
	        	                }
	        	                else
	        	                {
	        	            	    textToSpan += it.desc;
	        	                }
	        	            }
	        	            else if (it.category.equals("General"))
	        	            {
	        	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	        	                textToSpan += "<BR>";
	        	        	    if (!it.descFull.equals(""))
	        	                {
	        	            	    textToSpan += it.descFull;
	        	                }
	        	                else
	        	                {
	        	            	    textToSpan += it.desc;
	        	                }
	        	            }
	        	            gv.sf.MessageBoxHtml(textToSpan);
	            	    }
	                    gv.ItemDialog.dismiss();
	                    gv.invalidate();
	                }
	            });
	            gv.ItemDialog = builder.create();
	            gv.ItemDialog.show();
                */
		    }
	    }
	
	    public void tutorialMessageShop()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageShop);    	
        }
    }
}
