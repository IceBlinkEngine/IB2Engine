using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenCombat 
    {
	    public Module mod;
	    public GameView gv;
	
	    //COMBAT STUFF
	    private bool isPlayerTurn = true;
	    public bool canMove = true;
	    public int currentPlayerIndex = 0;
        public int creatureIndex = 0;
        public int currentMoveOrderIndex = 0;
        public List<MoveOrder> moveOrderList = new List<MoveOrder>();
        public int initialMoveOrderListSize = 0;
        public float currentMoves = 0;
        public float creatureMoves = 0;
        public Coordinate UpperLeftSquare = new Coordinate();
	    public string currentCombatMode = "info"; //info, move, cast, attack
	    public Coordinate targetHighlightCenterLocation = new Coordinate();
	    public Coordinate creatureTargetLocation = new Coordinate();
	    private int encounterXP = 0;
	    private Creature creatureToAnimate = null;
	    private Player playerToAnimate = null;
	    private bool drawHitAnimation = false;
	    private bool drawMissAnimation = false;
	    private Coordinate hitAnimationLocation = new Coordinate();
	    public int spellSelectorIndex = 0;
	    public List<string> spellSelectorSpellTagList = new List<string>();
	    private bool drawProjectileAnimation = false;
	    private Coordinate projectileAnimationLocation = new Coordinate();
	    private bool drawEndingAnimation = false;
	    private Coordinate endingAnimationLocation = new Coordinate();
        public bool drawDeathAnimation = true;
        public List<Coordinate> deathAnimationLocations = new List<Coordinate>();
	    private int animationFrameIndex = 0;
	    public PathFinderEncounters pf;
	    public bool floatyTextOn = false;
	    public AnimationState animationState = AnimationState.None;
	    private Bitmap projectile;
        private bool projectileFacingUp = true;
	    private Bitmap ending_fx;
        private Bitmap mapBitmap;

        private IbbButton btnSelect = null;
	    private IbbButton btnMove = null;
	    private IbbButton btnAttack = null;
	    private IbbButton btnCast = null;
	    private IbbButton btnSkipTurn = null;
	    private IbbButton btnSwitchWeapon = null;
        private IbbButton btnMoveCounter = null;
	    public IbbToggleButton tglHP = null;
	    public IbbToggleButton tglSP = null;
        public IbbToggleButton tglMoveOrder = null;
	    public IbbToggleButton tglSpeed = null;
	    public IbbToggleButton tglSoundFx = null;
	    public IbbToggleButton tglKill = null;
	    public IbbToggleButton tglHelp = null;
	    public IbbToggleButton tglGrid = null;
        public int mapStartLocXinPixels;
        public float moveCost = 1.0f;
        //public float diagonalMoveCost = 1.5f; //using the property from module instead
        public List<Sprite> spriteList = new List<Sprite>();


        public ScreenCombat(Module m, GameView g)
	    {
		    mod = m;
		    gv = g;
            mapStartLocXinPixels = 6 * gv.squareSize;
		    setControlsStart();
            setToggleButtonsStart();
	    }
	
	    public void setControlsStart()
	    {		
		    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;
            int hotkeyShift = 0;
            if (gv.useLargeLayout)
            {
                hotkeyShift = 1;
            }


            if (btnSelect == null)
		    {
			    btnSelect = new IbbButton(gv, 0.8f);	
			    btnSelect.Text = "SELECT";
			    btnSelect.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnSelect.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnSelect.X = gv.cc.pnlArrows.LocX + 1 * gv.squareSize + gv.squareSize / 2;
                btnSelect.Y = gv.cc.pnlArrows.LocY + 1 * gv.squareSize + gv.pS;
                btnSelect.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSelect.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }			
		    if (btnMove == null)
		    {
			    btnMove = new IbbButton(gv, 0.8f);
			    btnMove.Img = gv.cc.LoadBitmap("btn_small");
                btnMove.ImgOn = gv.cc.LoadBitmap("btn_small_on");
                btnMove.ImgOff = gv.cc.LoadBitmap("btn_small_off");
			    btnMove.Glow = gv.cc.LoadBitmap("btn_small_glow");
			    btnMove.Text = "MOVE";
                btnMove.HotKey = "M";
                btnMove.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 3) * gv.squareSize;
                btnMove.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnMove.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnMove.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
            if (btnMoveCounter == null)
            {
                btnMoveCounter = new IbbButton(gv, 1.2f);
                btnMoveCounter.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnMoveCounter.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnMoveCounter.Text = "0";
                btnMoveCounter.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 6) * gv.squareSize;
                btnMoveCounter.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnMoveCounter.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnMoveCounter.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
		    if (btnAttack == null)
		    {
			    btnAttack = new IbbButton(gv, 0.8f);
			    btnAttack.Img = gv.cc.LoadBitmap("btn_small");
                btnAttack.ImgOn = gv.cc.LoadBitmap("btn_small_on");
                btnAttack.ImgOff = gv.cc.LoadBitmap("btn_small_off");
			    btnAttack.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnAttack.Text = "ATTACK";
                btnAttack.HotKey = "A";
                btnAttack.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 4) * gv.squareSize;
                btnAttack.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnAttack.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAttack.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    if (btnCast == null)
		    {
			    btnCast = new IbbButton(gv, 0.8f);
			    btnCast.Img = gv.cc.LoadBitmap("btn_small");
                btnCast.ImgOn = gv.cc.LoadBitmap("btn_small_on");
                btnCast.ImgOff = gv.cc.LoadBitmap("btn_small_off");
			    btnCast.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnCast.Text = "CAST";
                btnCast.HotKey = "C";
                btnCast.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 5) * gv.squareSize;
                btnCast.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnCast.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnCast.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    if (btnSkipTurn == null)
		    {
			    btnSkipTurn = new IbbButton(gv, 0.8f);
			    btnSkipTurn.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnSkipTurn.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnSkipTurn.Text = "SKIP";
                btnSkipTurn.HotKey = "S";
                btnSkipTurn.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 2) * gv.squareSize;
                btnSkipTurn.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnSkipTurn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSkipTurn.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    if (btnSwitchWeapon == null)
		    {
			    btnSwitchWeapon = new IbbButton(gv, 0.8f);
                btnSwitchWeapon.HotKey = "P";
			    btnSwitchWeapon.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnSwitchWeapon.Img2 = gv.cc.LoadBitmap("btnparty"); // BitmapFactory.decodeResource(getResources(), R.drawable.btnparty);
			    btnSwitchWeapon.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnSwitchWeapon.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 0) * gv.squareSize;
                btnSwitchWeapon.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnSwitchWeapon.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSwitchWeapon.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
					
		    }
	    }
        public void setToggleButtonsStart()
        {
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;
			
		    if (tglGrid == null)
		    {
			    tglGrid = new IbbToggleButton(gv);
			    tglGrid.ImgOn = gv.cc.LoadBitmap("tgl_grid_on");
			    tglGrid.ImgOff = gv.cc.LoadBitmap("tgl_grid_off");
                tglGrid.X = gv.cc.pnlToggles.LocX + 1 * gv.squareSize + gv.squareSize / 4;
                tglGrid.Y = gv.cc.pnlToggles.LocY + 1 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglGrid.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglGrid.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
			    tglGrid.toggleOn = true;
		    }
		    if (tglHP == null)
		    {
			    tglHP = new IbbToggleButton(gv);
			    tglHP.ImgOn = gv.cc.LoadBitmap("tgl_hp_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_hp_on);
			    tglHP.ImgOff = gv.cc.LoadBitmap("tgl_hp_off"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_hp_off);
                tglHP.X = gv.cc.pnlToggles.LocX + 1 * gv.squareSize + gv.squareSize / 4;
                tglHP.Y = gv.cc.pnlToggles.LocY + 0 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglHP.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglHP.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
		    }
		    if (tglSP == null)
		    {
			    tglSP = new IbbToggleButton(gv);
			    tglSP.ImgOn = gv.cc.LoadBitmap("tgl_sp_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_on);
			    tglSP.ImgOff = gv.cc.LoadBitmap("tgl_sp_off"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_off);
                tglSP.X = gv.cc.pnlToggles.LocX + 2 * gv.squareSize + gv.squareSize / 4;
                tglSP.Y = gv.cc.pnlToggles.LocY + 0 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglSP.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglSP.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
		    }
            if (tglMoveOrder == null)
            {
                tglMoveOrder = new IbbToggleButton(gv);
                tglMoveOrder.ImgOn = gv.cc.LoadBitmap("tgl_mo_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_on);
                tglMoveOrder.ImgOff = gv.cc.LoadBitmap("tgl_mo_off"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_off);
                tglMoveOrder.X = gv.cc.pnlToggles.LocX + 3 * gv.squareSize + gv.squareSize / 4;
                tglMoveOrder.Y = gv.cc.pnlToggles.LocY + 0 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglMoveOrder.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglMoveOrder.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
            }
		    if (tglSpeed == null)
		    {
			    tglSpeed = new IbbToggleButton(gv);
			    tglSpeed.ImgOn = gv.cc.LoadBitmap("tgl_speed_4"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_on);
			    tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_4"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_off);
                tglSpeed.X = gv.cc.pnlToggles.LocX + 3 * gv.squareSize + gv.squareSize / 4;
                tglSpeed.Y = gv.cc.pnlToggles.LocY + 2 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglSpeed.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglSpeed.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
		    }
		    if (tglSoundFx == null)
		    {
			    tglSoundFx = new IbbToggleButton(gv);
			    tglSoundFx.ImgOn = gv.cc.LoadBitmap("tgl_sound_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_on);
			    tglSoundFx.ImgOff = gv.cc.LoadBitmap("tgl_sound_off"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_off);
                tglSoundFx.X = gv.cc.pnlToggles.LocX + 3 * gv.squareSize + gv.squareSize / 4;
                tglSoundFx.Y = gv.cc.pnlToggles.LocY + 1 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglSoundFx.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglSoundFx.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
		    }
		    if (tglHelp == null)
		    {
			    tglHelp = new IbbToggleButton(gv);
			    tglHelp.ImgOn = gv.cc.LoadBitmap("tgl_help_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_on);
			    tglHelp.ImgOff = gv.cc.LoadBitmap("tgl_help_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_off);
                tglHelp.X = gv.cc.pnlToggles.LocX + 2 * gv.squareSize + gv.squareSize / 4;
                tglHelp.Y = gv.cc.pnlToggles.LocY + 2 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglHelp.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglHelp.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
		    }
		    if (tglKill == null)
		    {
			    tglKill = new IbbToggleButton(gv);
			    tglKill.ImgOn = gv.cc.LoadBitmap("tgl_kill_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_on);
			    tglKill.ImgOff = gv.cc.LoadBitmap("tgl_kill_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sp_off);
                tglKill.X = gv.cc.pnlToggles.LocX + 1 * gv.squareSize + gv.squareSize / 4;
                tglKill.Y = gv.cc.pnlToggles.LocY + 2 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglKill.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglKill.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
		    }
        }
        public void resetToggleButtons()
        {
            if (mod.combatAnimationSpeed == 100)
            {
                gv.cc.DisposeOfBitmap(ref tglSpeed.ImgOff);
                tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_1");
            }
            else if (mod.combatAnimationSpeed == 50)
            {
                gv.cc.DisposeOfBitmap(ref tglSpeed.ImgOff);
                tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_2");
            }
            else if (mod.combatAnimationSpeed == 25)
            {
                gv.cc.DisposeOfBitmap(ref tglSpeed.ImgOff);
                tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_4");
            }
            else if (mod.combatAnimationSpeed == 10)
            {
                gv.cc.DisposeOfBitmap(ref tglSpeed.ImgOff);
                tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_10");
            }

            if (mod.playMusic)
            {
                gv.cc.tglSound.toggleOn = true;
            }
            else
            {
                gv.cc.tglSound.toggleOn = false;
            }

            if (mod.playSoundFx)
            {
                tglSoundFx.toggleOn = true;
            }
            else
            {
                tglSoundFx.toggleOn = false;
            }
        }
	    public void tutorialMessageCombat(bool helpCall)
        {
    	    if ((mod.showTutorialCombat) || (helpCall))
		    {
			    gv.sf.MessageBoxHtml(
					    "<big><b>COMBAT</b></big><br><br>" +
			
					    "<b>1. Player's Turn:</b> Each player takes a turn. The current player will be highlighted with a" +
					    " light blue box. You can Move one square (or stay put) and make one additional action such" + 
					    " as ATTACK, CAST, use item, or end turn (SKIP button).<br><br>" +
					
					    "<b>2. Info Mode:</b> Info mode is the default mode. In this mode you can tap on a token (player or enemy image) to show" + 
					    " some of their stats (HP, SP, etc.). If none of the buttons are highlighted, then you are in 'Info' mode. If you are" +
					    " in 'move' mode and want to return to 'info' mode, tap on the move button to unselect it and return to 'info' mode. Same" +
					    " concept works for 'attack' mode back to 'info' mode.<br><br>" +
					
					    "<b>3. Move:</b> After pressing move, you may move one square and then do one more action or press 'SKIP' to end this Player's" +
					    " turn. You move by pressing one of the arrow direction buttons or tapping on a square adjacent to the PC.<br><br>" +
					
					    "<b>3. Attack:</b> After pressing attack, move the target selection square by pressing the arrow keys or tapping on any map square." + 
					    " Once you have selected a valid target (box will be green), press the 'TARGET' button or tap on the targeted map square (green box)" +
					    " again to complete the action.<br><br>" +
					
					    "<b>4. Cast:</b> After pressing cast and selecting a spell from the spell selection screen, move the target selection square by" +
					    " pressing the arrow keys or tapping on any map square. Once you have selected a valid target (box will be green), press the" + 
					    " 'TARGET' button or tap on the targeted map square (green box) again to complete the action.<br><br>" +
					
					    "<b>5. Skip:</b> The 'SKIP' button will end the current player's turn.<br><br>" +
					
					    "<b>6. Use Item:</b> press the inventory button (image of a bag) to show the party inventory screen. Only the current Player" +
					    " may use an item from this screen during combat.<br><br>" +
					
					    "<small><b>Note:</b> Also, check out the 'Player's Guide' in the settings menu (the rusty gear looking button)</small>"
					    );
			    mod.showTutorialCombat = false;
		    }
        }
	    
        public void doAnimationController()
	    {
            gv.Render();
		    if (animationState == AnimationState.None)
		    {
			    return;
            }
            #region PcMeleeAttackAnimation
            else if (animationState == AnimationState.PcMeleeAttackAnimation)
		    {
			    creatureToAnimate = null;
	    	    playerToAnimate = null;
                //gv.Invalidate();
                gv.Render();
	    	    Player pc = mod.playerList[currentPlayerIndex];
	    	    doCombatAttack(pc);
            }
            #endregion
            #region CreatureHitAnimation
            else if (animationState == AnimationState.CreatureHitAnimation)
		    {
			    drawHitAnimation = false;
        	    hitAnimationLocation = new Coordinate();
                //gv.Invalidate();
                gv.Render();
                if (deathAnimationLocations.Count > 0)
                {
                    drawDeathAnimation = true;
                    animationFrameIndex = 0;
                    animationState = AnimationState.DeathAnimation;
                    gv.postDelayed("doAnimation", (mod.combatAnimationSpeed));
                    //play death ending sound
                    //gv.PlaySound(gv.sf.SpellToCast.spellEndSound);
                }
                else
                {
                    //check for end of encounter
                    checkEndEncounter();
                    //end PC's turn
                    gv.touchEnabled = true;
                    animationState = AnimationState.None;
                    endPcTurn(true);
                }
            }
            #endregion
            #region CreatureMissedAnimation
            else if (animationState == AnimationState.CreatureMissedAnimation)
		    {
			    drawMissAnimation = false;
        	    hitAnimationLocation = new Coordinate();
                //gv.Invalidate();
                gv.Render();
	    	    //check for end of encounter
                checkEndEncounter();
                gv.touchEnabled = true;
                animationState = AnimationState.None;
			    //end PC's turn
                endPcTurn(true);
            }
            #endregion
            #region CreatureMeleeAttackAnimation
            else if (animationState == AnimationState.CreatureMeleeAttackAnimation)
		    {
			    creatureToAnimate = null;
	    	    playerToAnimate = null;
                //gv.Invalidate();
                gv.Render();
	    	    doStandardCreatureAttack();
            }
            #endregion
            #region PcHitAnimation
            else if (animationState == AnimationState.PcHitAnimation)
		    {
			    drawHitAnimation = false;
        	    hitAnimationLocation = new Coordinate();
                //gv.Invalidate();
                gv.Render();
                if (deathAnimationLocations.Count > 0)
                {
                    drawDeathAnimation = true;
                    animationFrameIndex = 0;
                    animationState = AnimationState.DeathAnimation;
                    gv.postDelayed("doAnimation", (mod.combatAnimationSpeed));
                    //play death ending sound
                    //gv.PlaySound(gv.sf.SpellToCast.spellEndSound);
                }
                else
                {
                    animationState = AnimationState.None;
                    endCreatureTurn();
                }
            }
            #endregion
            #region PcMissedAnimation
            else if (animationState == AnimationState.PcMissedAnimation)
            {
                drawMissAnimation = false;
                hitAnimationLocation = new Coordinate();
                //gv.Invalidate();
                gv.Render();
                animationState = AnimationState.None;
                endCreatureTurn();
            }
            #endregion
            #region PcRangedAttackAnimation
            else if (animationState == AnimationState.PcRangedAttackAnimation)
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                //gv.Invalidate();
                gv.Render();
                Player pc = mod.playerList[currentPlayerIndex];
                //do projectile next
                drawProjectileAnimation = true;
                projectileAnimationLocation = new Coordinate(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY));
                //load projectile image
                gv.cc.DisposeOfBitmap(ref projectile);
                projectile = gv.cc.LoadBitmap(mod.getItemByResRefForInfo(pc.AmmoRefs.resref).projectileSpriteFilename);
                if (pc.combatLocY < targetHighlightCenterLocation.Y)
                {
                    projectileFacingUp = false;
//TODO                    projectile = gv.cc.FlipHorz(projectile);
                }
                else
                {
                    projectileFacingUp = true;
                }
                //reset animation frame counter
                animationFrameIndex = 0;
                //gv.Invalidate();
                gv.Render();
                animationState = AnimationState.PcRangedProjectileAnimation;
                gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
            }
            #endregion
            #region PcRangedProjectileAnimation
            else if (animationState == AnimationState.PcRangedProjectileAnimation)
            {
                Player pc = mod.playerList[currentPlayerIndex];
                //if at target, do ending
                if ((projectileAnimationLocation.X == getPixelLocX(targetHighlightCenterLocation.X)) && (projectileAnimationLocation.Y == getPixelLocY(targetHighlightCenterLocation.Y)))
                {
                    drawProjectileAnimation = false;
                    projectileAnimationLocation = new Coordinate();
                    drawEndingAnimation = true;
                    endingAnimationLocation = new Coordinate(getPixelLocX(targetHighlightCenterLocation.X), getPixelLocY(targetHighlightCenterLocation.Y));
                    animationFrameIndex = 0;
                    gv.cc.DisposeOfBitmap(ref ending_fx);
                    ending_fx = gv.cc.LoadBitmap(mod.getItemByResRefForInfo(pc.AmmoRefs.resref).spriteEndingFilename);
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.PcRangedEndingAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                }
                else //if not at target, get new coordinate and keep going
                {
                    //if frame is last, reset
                    animationFrameIndex++;
                    if (animationFrameIndex > 4) { animationFrameIndex = 0; }
                    //get next coordinate
                    projectileAnimationLocation = GetNextProjectileCoordinate(new Coordinate(pc.combatLocX, pc.combatLocY), targetHighlightCenterLocation);
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.PcRangedProjectileAnimation;
                    gv.postDelayed("doAnimation", (int) (2 * (0.5f) * mod.combatAnimationSpeed));
                }
            }
            #endregion
            #region PcRangedEndingAnimation
            else if (animationState == AnimationState.PcRangedEndingAnimation)
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if ((animationFrameIndex < 3) && (!mod.getItemByResRefForInfo(pc.MainHandRefs.resref).spriteEndingFilename.Equals("none")))
                {
                    animationFrameIndex++;
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.PcRangedEndingAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                }
                else //if not at target, get new coordinate and keep going
                {
                    drawEndingAnimation = false;
                    endingAnimationLocation = new Coordinate();
                    animationFrameIndex = 0;
                    //loop through 4 times
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.None;
                    gv.touchEnabled = true;
                    //Player pc = mod.playerList.get(currentPlayerIndex);
                    doCombatAttack(pc);
                }
            }
            #endregion
            #region PcCastAttackAnimation
            else if (animationState == AnimationState.PcCastAttackAnimation)
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                //gv.Invalidate();
                gv.Render();
                Player pc = mod.playerList[currentPlayerIndex];
                //do projectile next
                drawProjectileAnimation = true;
                projectileAnimationLocation = new Coordinate(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY));
                //load projectile image
                gv.cc.DisposeOfBitmap(ref projectile);
                projectile = gv.cc.LoadBitmap(gv.cc.currentSelectedSpell.spriteFilename);
                if (pc.combatLocY < targetHighlightCenterLocation.Y)
                {
                    projectileFacingUp = false;
//TODO                    projectile = gv.cc.FlipHorz(projectile);
                }
                else
                {
                    projectileFacingUp = true;
                }
                //reset animation frame counter
                animationFrameIndex = 0;
                //gv.Invalidate();
                gv.Render();
                animationState = AnimationState.PcCastProjectileAnimation;
                gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                //play cast projectile sound
                gv.PlaySound(gv.cc.currentSelectedSpell.spellStartSound);
            }
            #endregion
            #region PcCastProjectileAnimation
            else if (animationState == AnimationState.PcCastProjectileAnimation)
            {
                Player pc = mod.playerList[currentPlayerIndex];
                //if at target, do ending
                if ((projectileAnimationLocation.X == getPixelLocX(targetHighlightCenterLocation.X)) && (projectileAnimationLocation.Y == getPixelLocY(targetHighlightCenterLocation.Y)))
                {
                    drawProjectileAnimation = false;
                    projectileAnimationLocation = new Coordinate();
                    drawEndingAnimation = true;
                    endingAnimationLocation = new Coordinate(getPixelLocX(targetHighlightCenterLocation.X), getPixelLocY(targetHighlightCenterLocation.Y));
                    animationFrameIndex = 0;
                    gv.cc.DisposeOfBitmap(ref ending_fx);
                    ending_fx = gv.cc.LoadBitmap(gv.cc.currentSelectedSpell.spriteEndingFilename);
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.PcCastEndingAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                    //play cast ending sound
                    gv.PlaySound(gv.cc.currentSelectedSpell.spellEndSound);
                }
                else //if not at target, get new coordinate and keep going
                {
                    //if frame is last, reset
                    animationFrameIndex++;
                    if (animationFrameIndex > 4) { animationFrameIndex = 0; }
                    //get next coordinate
                    projectileAnimationLocation = GetNextProjectileCoordinate(new Coordinate(pc.combatLocX, pc.combatLocY), targetHighlightCenterLocation);
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.PcCastProjectileAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                }
            }
            #endregion
            #region PcCastEndingAnimation
            else if (animationState == AnimationState.PcCastEndingAnimation)
            {
                if (animationFrameIndex < 3)
                {
                    animationFrameIndex++;
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.PcCastEndingAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                }
                else //if not at target, get new coordinate and keep going
                {
                    drawEndingAnimation = false;
                    endingAnimationLocation = new Coordinate();
                    animationFrameIndex = 0;
                    //loop through 4 times
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.None;
                    gv.touchEnabled = true;
                    Player pc = mod.playerList[currentPlayerIndex];
                    doCombatCast(pc);
                }
            }
            #endregion
            #region CreatureRangedAttackAnimation
            else if (animationState == AnimationState.CreatureRangedAttackAnimation)
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                //gv.Invalidate();
                gv.Render();
                Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                //do projectile next
                drawProjectileAnimation = true;
                projectileAnimationLocation = new Coordinate(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY));
                //load projectile image
                gv.cc.DisposeOfBitmap(ref projectile);
                projectile = gv.cc.LoadBitmap(crt.cr_projSpriteFilename);
                if (crt.combatLocY < creatureTargetLocation.Y)
                {
                    projectileFacingUp = false;
//TODO                    projectile = gv.cc.FlipHorz(projectile);
                }
                else
                {
                    projectileFacingUp = true;
                }
                //reset animation frame counter
                animationFrameIndex = 0;
                //gv.Invalidate();
                gv.Render();
                animationState = AnimationState.CreatureRangedProjectileAnimation;
                gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
            }
            #endregion
            #region CreatureRangedProjectileAnimation
            else if (animationState == AnimationState.CreatureRangedProjectileAnimation)
            {
                Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                //if at target, do ending
                if ((projectileAnimationLocation.X == getPixelLocX(creatureTargetLocation.X)) && (projectileAnimationLocation.Y == getPixelLocY(creatureTargetLocation.Y)))
                {
                    drawProjectileAnimation = false;
                    projectileAnimationLocation = new Coordinate();
                    drawEndingAnimation = true;
                    endingAnimationLocation = new Coordinate(getPixelLocX(creatureTargetLocation.X), getPixelLocY(creatureTargetLocation.Y));
                    animationFrameIndex = 0;
                    gv.cc.DisposeOfBitmap(ref ending_fx);
                    ending_fx = gv.cc.LoadBitmap(crt.cr_spriteEndingFilename);
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.CreatureRangedEndingAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                }
                else //if not at target, get new coordinate and keep going
                {
                    //if frame is last, reset
                    animationFrameIndex++;
                    if (animationFrameIndex > 4) { animationFrameIndex = 0; }
                    //get next coordinate
                    projectileAnimationLocation = GetNextProjectileCoordinate(new Coordinate(crt.combatLocX, crt.combatLocY), creatureTargetLocation);
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.CreatureRangedProjectileAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                }
            }
            #endregion
            #region CreatureRangedEndingAnimation
            else if (animationState == AnimationState.CreatureRangedEndingAnimation)
            {
                Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                if ((animationFrameIndex < 3) && (!crt.cr_spriteEndingFilename.Equals("none")))
                {
                    animationFrameIndex++;
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.CreatureRangedEndingAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f)* mod.combatAnimationSpeed));
                }
                else //if not at target, get new coordinate and keep going
                {
                    drawEndingAnimation = false;
                    endingAnimationLocation = new Coordinate();
                    animationFrameIndex = 0;
                    //loop through 4 times
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.None;
                    //touchEnabled = true;
                    doStandardCreatureAttack();
                }
            }
            #endregion
            #region CreatureCastAttackAnimation
            else if (animationState == AnimationState.CreatureCastAttackAnimation)
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                //gv.Invalidate();
                gv.Render();
                Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                //do projectile next
                drawProjectileAnimation = true;
                projectileAnimationLocation = new Coordinate(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY));
                //load projectile image
                gv.cc.DisposeOfBitmap(ref projectile);
                projectile = gv.cc.LoadBitmap(gv.sf.SpellToCast.spriteFilename);
                if (crt.combatLocY < creatureTargetLocation.Y)
                {
                    projectileFacingUp = false;
//TODO                    projectile = gv.cc.FlipHorz(projectile);
                }
                else
                {
                    projectileFacingUp = true;
                }
                //reset animation frame counter
                animationFrameIndex = 0;
                //gv.Invalidate();
                gv.Render();
                animationState = AnimationState.CreatureCastProjectileAnimation;
                gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                //play cast start sound
                gv.PlaySound(gv.sf.SpellToCast.spellStartSound);
            }
            #endregion
            #region CreatureCastProjectileAnimation
            else if (animationState == AnimationState.CreatureCastProjectileAnimation)
            {
                Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                //if at target, do ending
                if ((projectileAnimationLocation.X == getPixelLocX(creatureTargetLocation.X)) && (projectileAnimationLocation.Y == getPixelLocY(creatureTargetLocation.Y)))
                {
                    drawProjectileAnimation = false;
                    projectileAnimationLocation = new Coordinate();
                    drawEndingAnimation = true;
                    endingAnimationLocation = new Coordinate(getPixelLocX(creatureTargetLocation.X), getPixelLocY(creatureTargetLocation.Y));
                    animationFrameIndex = 0;
                    gv.cc.DisposeOfBitmap(ref ending_fx);
                    ending_fx = gv.cc.LoadBitmap(gv.sf.SpellToCast.spriteEndingFilename);
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.CreatureCastEndingAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                    //play cast ending sound
                    gv.PlaySound(gv.sf.SpellToCast.spellEndSound);
                }
                else //if not at target, get new coordinate and keep going
                {
                    //if frame is last, reset
                    animationFrameIndex++;
                    if (animationFrameIndex > 4) { animationFrameIndex = 0; }
                    //get next coordinate
                    projectileAnimationLocation = GetNextProjectileCoordinate(new Coordinate(crt.combatLocX, crt.combatLocY), creatureTargetLocation);
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.CreatureCastProjectileAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                }
            }
            #endregion
            #region CreatureCastEndingAnimation
            else if (animationState == AnimationState.CreatureCastEndingAnimation)
            {
                if (animationFrameIndex < 3)
                {
                    animationFrameIndex++;
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.CreatureCastEndingAnimation;
                    gv.postDelayed("doAnimation", (int)(2 * (0.5f) * mod.combatAnimationSpeed));
                }
                else //if not at target, get new coordinate and keep going
                {
                    drawEndingAnimation = false;
                    endingAnimationLocation = new Coordinate();
                    animationFrameIndex = 0;
                    //loop through 4 times
                    //gv.Invalidate();
                    gv.Render();
                    animationState = AnimationState.None;
                    //touchEnabled = true;
                    doCreatureSpell();
                }
            }
            #endregion
            #region CreatureThink
            else if (animationState == AnimationState.CreatureThink)
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                //gv.Invalidate();
                gv.Render();
                doCreatureTurnAfterDelay();
            }
            #endregion
            #region CreatureMove
            else if (animationState == AnimationState.CreatureMove)
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                //gv.Invalidate();
                gv.Render();
                Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                //CreatureDoesAttack(crt);
                if (moveCost == mod.diagonalMoveCost)
                {
                    creatureMoves += mod.diagonalMoveCost;
                    moveCost = 1.0f;
                }
                else
                {
                    creatureMoves++;
                }
                //reatureMoves++;
                doCreatureNextAction();
                //endCreatureTurn();
            }
            #endregion
            #region DeathAnimation
            else if (animationState == AnimationState.DeathAnimation)
            {
                if (animationFrameIndex < 3) //keep animating
                {
                    animationFrameIndex++;
                    gv.Render();
                    animationState = AnimationState.DeathAnimation;
                    gv.postDelayed("doAnimation", (int)(1 * mod.combatAnimationSpeed));
                }
                else //done with death animations
                {
                    drawDeathAnimation = false;
                    deathAnimationLocations.Clear();
                    animationFrameIndex = 0;
                    gv.Render();
                    if (isPlayerTurn)
                    {
                        //check for end of encounter
                        checkEndEncounter();
                        //end PC's turn
                        gv.touchEnabled = true;
                        animationState = AnimationState.None;
                        endPcTurn(true);
                    }
                    else
                    {
                        animationState = AnimationState.None;
                        endCreatureTurn();
                    }
                }
            }
            #endregion
        }
        public void doFloatyTextLoop()
	    {
		    gv.postDelayed("doFloatyText", 200);
	    }

        public void doCombatSetup()
        {
            if (mod.playMusic)
            {
                gv.stopMusic();
                gv.stopAmbient();
                gv.startCombatMusic();
            }
            gv.screenType = "combat";
            resetToggleButtons();
            //Load map if used
            if (mod.currentEncounter.UseMapImage)
            {
                gv.cc.DisposeOfBitmap(ref mapBitmap);
                mapBitmap = gv.cc.LoadBitmap(mod.currentEncounter.MapImage);
            }
            else //loads only the tiles that are used on this encounter map
            {
                //TODO gv.cc.LoadTileBitmapList();
            }
            //Load up all creature stuff
            foreach (CreatureRefs crf in mod.currentEncounter.encounterCreatureRefsList)
            {
                //find this creatureRef in mod creature list
                foreach (Creature c in gv.mod.moduleCreaturesList)
                {
                    if (crf.creatureResRef.Equals(c.cr_resref))
                    {
                        //copy it and add to encounters creature object list
                        try
                        {
                            Creature copy = c.DeepCopy();
                            copy.cr_tag = crf.creatureTag;
                            gv.cc.DisposeOfBitmap(ref copy.token);
                            copy.token = gv.cc.LoadBitmap(copy.cr_tokenFilename);
                            copy.combatLocX = crf.creatureStartLocationX;
                            copy.combatLocY = crf.creatureStartLocationY;
                            mod.currentEncounter.encounterCreatureList.Add(copy);
                        }
                        catch (Exception ex)
                        {
                            gv.errorLog(ex.ToString());
                        }
                    }
                }
            }
            //Place all PCs
            for (int index = 0; index < mod.playerList.Count; index++)
            {
                mod.playerList[index].combatLocX = mod.currentEncounter.encounterPcStartLocations[index].X;
                mod.playerList[index].combatLocY = mod.currentEncounter.encounterPcStartLocations[index].Y;
            }
            isPlayerTurn = true;
            currentPlayerIndex = 0;
            creatureIndex = 0;
            currentMoveOrderIndex = 0;
            currentCombatMode = "info";
            drawDeathAnimation = false;
            encounterXP = 0;
            foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
            {
                encounterXP += crtr.cr_XP;
            }
            pf = new PathFinderEncounters(mod);
            tutorialMessageCombat(false);
            //IBScript Setup Combat Hook (run only once)
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnSetupCombatIBScript, gv.mod.currentEncounter.OnSetupCombatIBScriptParms);
            //IBScript Start Combat Round Hook
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnStartCombatRoundIBScript, gv.mod.currentEncounter.OnStartCombatRoundIBScriptParms);
            //determine initiative
            calcualteMoveOrder();
            //do turn controller
            turnController();
            //startPcTurn();
        }
        public void calcualteMoveOrder()
        {
            moveOrderList.Clear();
            //go through each PC and creature and make initiative roll
            foreach (Player pc in mod.playerList)
            {
                int roll = gv.sf.RandInt(100) + (((pc.dexterity - 10) / 2) * 5);
                MoveOrder newMO = new MoveOrder();
                newMO.PcOrCreature = pc;
                newMO.rank = roll;
                moveOrderList.Add(newMO);

            }
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                int roll = gv.sf.RandInt(100) + (crt.initiativeBonus * 5);
                MoveOrder newMO = new MoveOrder();
                newMO.PcOrCreature = crt;
                newMO.rank = roll;
                moveOrderList.Add(newMO);
            }
            initialMoveOrderListSize = moveOrderList.Count;
            //sort PCs and creatures based on results
            moveOrderList = moveOrderList.OrderByDescending(x => x.rank).ToList();
            //assign moveOrder to PC and Creature property
            int cnt = 0;
            foreach (MoveOrder m in moveOrderList)
            {
                if (m.PcOrCreature is Player)
                {
                    Player pc = (Player)m.PcOrCreature;
                    pc.moveOrder = cnt;
                }
                else
                {
                    Creature crt = (Creature)m.PcOrCreature;
                    crt.moveOrder = cnt;
                }
                cnt++;
            }
        }
        public void turnController()
        {
            //redraw screen
            gv.Render();
            if (currentMoveOrderIndex >= initialMoveOrderListSize)
            {
                //hit the end so start the next round
                startNextRoundStuff();
                return;
            }
            //get the next PC or Creature based on currentMoveOrderIndex and moveOrder property
            int idx = 0;
            foreach (Player pc in mod.playerList)
            {
                if (pc.moveOrder == currentMoveOrderIndex)
                {
                    //highlight the portrait of the pc whose current turn it is
                    gv.cc.ptrPc0.glowOn = false;
                    gv.cc.ptrPc1.glowOn = false;
                    gv.cc.ptrPc2.glowOn = false;
                    gv.cc.ptrPc3.glowOn = false;
                    gv.cc.ptrPc4.glowOn = false;
                    gv.cc.ptrPc5.glowOn = false;
            
                    if (idx == 0) 
                    { 
                        gv.cc.ptrPc0.glowOn = true;
                    }
                    if (idx == 1)
                    {   gv.cc.ptrPc1.glowOn = true; 
                    }
                    if (idx == 2)
                    { 
                        gv.cc.ptrPc2.glowOn = true; 
                    }
                    if (idx == 3)
                    { 
                        gv.cc.ptrPc3.glowOn = true; 
                    }
                    if (idx == 4)
                    { 
                        gv.cc.ptrPc4.glowOn = true; 
                    }
                    if (idx == 5)
                    { 
                        gv.cc.ptrPc5.glowOn = true; 
                    }

                    //write the pc's name to log whsoe turn it is
                    gv.cc.addLogText("<font color='blue'>It's the turn of " + pc.name + ". </font><BR>");

                    //change creatureIndex or currentPlayerIndex
                    currentPlayerIndex = idx;
                    //set isPlayerTurn 
                    isPlayerTurn = true;
                    
                    currentCombatMode = "info";
                    currentMoveOrderIndex++;
                    gv.Render();
                    //go to start PlayerTurn or start CreatureTurn
                    if ((pc.isHeld()) || (pc.isDead()))
                    {
                        endPcTurn(false);
                    }
                    else
                    {
                        startPcTurn();
                    }
                    return;
                }
                idx++;
            }
            idx = 0;
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if (crt.moveOrder == currentMoveOrderIndex)
                {

                    gv.cc.addLogText("<font color='blue'>It's the turn of " + crt.cr_name + ". </font><BR>");
                    //change creatureIndex or currentPlayerIndex
                    creatureIndex = idx;
                    //set isPlayerTurn
                    isPlayerTurn = false;
                    
                    gv.touchEnabled = false;
                    currentCombatMode = "info";
                    currentMoveOrderIndex++;
                    gv.Render();
                    //go to start PlayerTurn or start CreatureTurn
                    if ((crt.hp > 0) && (!crt.isHeld()))
                    {
                        doCreatureTurn();
                    }
                    else
                    {
                        endCreatureTurn();
                    }                    
                    return;
                }
                idx++;
            }            
            //didn't find one so increment moveOrderIndex and try again
            currentMoveOrderIndex++;
            turnController();
        }        
        public void startNextRoundStuff()
        {
            currentMoveOrderIndex = 0;
            gv.sf.dsWorldTime();
            doHardToKillTrait();
            doBattleRegenTrait();
            foreach (Player pc in mod.playerList)
            {
                RunAllItemCombatRegenerations(pc);
            }
            applyEffectsCombat();
            //IBScript Start Combat Round Hook
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnStartCombatRoundIBScript, gv.mod.currentEncounter.OnStartCombatRoundIBScriptParms);
            turnController();
        }
        public void doBattleRegenTrait()
        {
            foreach (Player pc in mod.playerList)
            {
                if (gv.sf.hasTrait(pc, "battleregen"))
                {
                    if (pc.hp <= -20)
                    {
                        //MessageBox("Can't heal a dead character!");
                        gv.cc.addLogText("<font color='red'>" + "BattleRegen off for dead character!" + "</font>" +
                                "<BR>");
                    }
                    else
                    {
                        pc.hp += 1;
                        if (pc.hp > pc.hpMax)
                        {
                            pc.hp = pc.hpMax;
                        }
                        if ((pc.hp > 0) && (pc.charStatus.Equals("Dead")))
                        {
                            pc.charStatus = "Alive";
                        }
                        gv.cc.addLogText("<font color='lime'>" + pc.name + " gains 1 HPs (BattleRegen Trait)" + "</font><BR>");
                    }
                }
            }
        }
        public void doHardToKillTrait()
        {
            foreach (Player pc in mod.playerList)
            {
                if (gv.sf.hasTrait(pc, "hardtokill"))
                {
                    //hard to kill
                    if (pc.hp < 0)
                    {
                        //50% chance to jump back up to 1/4 hpMax
                        int roll = gv.sf.RandInt(100);
                        if (roll > 50)
                        {
                            pc.charStatus = "Alive";
                            pc.hp = pc.hpMax / 10;
                            //do damage to all
                            gv.cc.addLogText("<font color='lime'>" + pc.name + " jumps back up (Hard to Kill trait).</font><br>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "roll = " + roll + " (" + roll + " > 50)</font><BR>");
                            }
                        }
                        else
                        {
                            gv.cc.addLogText("<font color='lime'>" + pc.name + " stays down (Hard to Kill trait).</font><br>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "roll = " + roll + " (" + roll + " < 51)</font><BR>");
                            }
                        }
                    }
                }
            }
        }
        public void RunAllItemCombatRegenerations(Player pc)
        {
            try
            {
                if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.BodyRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.BodyRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.MainHandRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.MainHandRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.OffHandRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.OffHandRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.OffHandRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.OffHandRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.RingRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.RingRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.RingRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.RingRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.HeadRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.HeadRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.HeadRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.HeadRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.NeckRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.NeckRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.NeckRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.NeckRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.FeetRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.FeetRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.FeetRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.FeetRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.Ring2Refs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.Ring2Refs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.Ring2Refs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.Ring2Refs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.AmmoRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.AmmoRefs.resref).hpRegenPerRoundInCombat);
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }
        public void doRegenSp(Player pc, int increment)
        {
            pc.sp += increment;
            if (pc.sp > pc.spMax) { pc.sp = pc.spMax; }
            gv.cc.addLogText("<font color='lime'>" + pc.name + " regens " + increment + "sp</font><br>");

        }
        public void doRegenHp(Player pc, int increment)
        {
            pc.hp += increment;
            if (pc.hp > pc.hpMax) { pc.hp = pc.hpMax; }
            gv.cc.addLogText("<font color='lime'>" + pc.name + " regens " + increment + "hp</font><br>");
        }
        public void applyEffectsCombat()
        {
            try
            {
                //maybe reorder all based on their order property            
                foreach (Player pc in mod.playerList)
                {
                    foreach (Effect ef in pc.effectsList)
                    {
                        //decrement duration of all
                        ef.durationInUnits -= gv.mod.TimePerRound;
                        if (!ef.usedForUpdateStats) //not used for stat updates
                        {
                            gv.cc.doEffectScript(pc, ef);
                        }
                    }
                }
                foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
                {
                    foreach (Effect ef in crtr.cr_effectsList)
                    {
                        //increment duration of all
                        ef.durationInUnits -= gv.mod.TimePerRound;
                        if (!ef.usedForUpdateStats) //not used for stat updates
                        {
                            //do script for each effect
                            gv.cc.doEffectScript(crtr, ef);
                        }
                    }
                }
                //if remaining duration <= 0, remove from list
                foreach (Player pc in mod.playerList)
                {
                    for (int i = pc.effectsList.Count; i > 0; i--)
                    {
                        if (pc.effectsList[i - 1].durationInUnits <= 0)
                        {
                            pc.effectsList.RemoveAt(i - 1);
                        }
                    }
                }
                foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
                {
                    for (int i = crtr.cr_effectsList.Count; i > 0; i--)
                    {
                        if (crtr.cr_effectsList[i - 1].durationInUnits <= 0)
                        {
                            crtr.cr_effectsList.RemoveAt(i - 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(gv, ex.ToString());
                gv.errorLog(ex.ToString());
            }
            checkEndEncounter();
        }

        //COMBAT	
        #region PC Combat Stuff
        public void decrementAmmo(Player pc)
        {
            if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                    && (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
            {
                ItemRefs itr = mod.getItemRefsInInventoryByResRef(pc.AmmoRefs.resref);
                if (itr != null)
                {
                    //decrement by one
                    itr.quantity--;
                    if (gv.sf.hasTrait(pc, "rapidshot"))
                    {
                        itr.quantity--;
                    }
                    if (gv.sf.hasTrait(pc, "rapidshot2"))
                    {
                        itr.quantity--;
                    }
                    //if equal to zero, remove from party inventory and from all PCs ammo slot
                    if (itr.quantity < 1)
                    {
                        foreach (Player p in mod.playerList)
                        {
                            if (p.AmmoRefs.resref.Equals(itr.resref))
                            {
                                p.AmmoRefs = new ItemRefs();
                            }
                        }
                        mod.partyInventoryRefsList.Remove(itr);
                    }
                }
            }
        }
        public void startPcTurn()
        {
            CalculateUpperLeft();
            gv.Render();
            isPlayerTurn = true;
            gv.touchEnabled = true;
            currentCombatMode = "move";
            //gv.screenType = "combat";
            //gv.cc.logScrollOffset = 0;
            Player pc = mod.playerList[currentPlayerIndex];
            gv.sf.UpdateStats(pc);
            currentMoves = 0;
            //do onTurn IBScript
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnStartCombatTurnIBScript, gv.mod.currentEncounter.OnStartCombatTurnIBScriptParms);
                        
            if ((pc.isHeld()) || (pc.isDead()) || (pc.isUnconcious()))
            {
                endPcTurn(false);
            }
            if (pc.isImmobile())
            {
                currentMoves = 99;
            }
        }
        public void doCombatAttack(Player pc)
        {
            if (isInRange(pc))
            {
                
                Item itChk = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                if (itChk != null)
                {
                    if (itChk.automaticallyHitsTarget) //if AoE type attack and automatically hits
                    {
                        //if using ranged and have ammo, use ammo properties
                        if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                        && (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                        {
                            itChk = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                            if (itChk != null)
                            {
                                //always decrement ammo by one whether a hit or miss
                                this.decrementAmmo(pc);

                                if (!itChk.onScoringHitCastSpellTag.Equals("none"))
                                {
                                    doItemOnHitCastSpell(itChk.onScoringHitCastSpellTag, itChk, targetHighlightCenterLocation);
                                }
                            }
                        }
                        else if (!itChk.onScoringHitCastSpellTag.Equals("none"))
                        {
                            doItemOnHitCastSpell(itChk.onScoringHitCastSpellTag, itChk, targetHighlightCenterLocation);
                        }
                        
                        drawHitAnimation = true;
                        hitAnimationLocation = new Coordinate(getPixelLocX(targetHighlightCenterLocation.X), getPixelLocY(targetHighlightCenterLocation.Y));
                        gv.Render();
                        animationState = AnimationState.CreatureHitAnimation;
                        gv.postDelayed("doAnimation", (int)(4 * (0.5f) * mod.combatAnimationSpeed));
                        return;
                    }
                }
                

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    if ((crt.combatLocX == targetHighlightCenterLocation.X) && (crt.combatLocY == targetHighlightCenterLocation.Y))
                    {
                        int attResult = 0; //0=missed, 1=hit, 2=killed
                        int numAtt = 1;
                        //boolean hasCleave = false;
                        int crtLocX = crt.combatLocX;
                        int crtLocY = crt.combatLocY;

                        if ((gv.sf.hasTrait(pc, "twoAttack")) && (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee")))
                        {
                            numAtt = 2;
                        }
                        if ((gv.sf.hasTrait(pc, "rapidshot")) && (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")))
                        {
                            numAtt = 2;
                        }
                        if ((gv.sf.hasTrait(pc, "rapidshot2")) && (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")))
                        {
                            numAtt = 3;
                        }
                        for (int i = 0; i < numAtt; i++)
                        {
                            if ((gv.sf.hasTrait(pc, "cleave")) && (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee")))
                            {
                                attResult = doActualCombatAttack(pc, crt, i);
                                if (attResult == 2) //2=killed, 1=hit, 0=missed
                                {
                                    Creature crt2 = GetNextAdjacentCreature(pc);
                                    if (crt2 != null)
                                    {
                                        crtLocX = crt2.combatLocX;
                                        crtLocY = crt2.combatLocY;
                                        floatyTextOn = true;
                                        gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), "cleave", "green");
                                        gv.postDelayed("doFloatyText", 100);
                                        attResult = doActualCombatAttack(pc, crt2, i);
                                    }
                                    break; //do not try and attack same creature that was just killed
                                }
                            }
                            else
                            {
                                attResult = doActualCombatAttack(pc, crt, i);
                                if (attResult == 2) //2=killed, 1=hit, 0=missed
                                {
                                    break; //do not try and attack same creature that was just killed
                                }
                            }

                        }
                        if (attResult > 0) //2=killed, 1=hit, 0=missed
                        {
                            drawHitAnimation = true;
                            hitAnimationLocation = new Coordinate(getPixelLocX(crtLocX), getPixelLocY(crtLocY));
                            gv.Render();
                            animationState = AnimationState.CreatureHitAnimation;
                            gv.postDelayed("doAnimation", (int)(4 * (0.5f) * mod.combatAnimationSpeed));
                        }
                        else
                        {
                            drawMissAnimation = true;
                            hitAnimationLocation = new Coordinate(getPixelLocX(crtLocX), getPixelLocY(crtLocY));
                            gv.Render();
                            animationState = AnimationState.CreatureMissedAnimation;
                            gv.postDelayed("doAnimation", (int)(4 * (0.5f) * mod.combatAnimationSpeed));
                        }
                        return;
                    }
                }
            }
        }
        public int doActualCombatAttack(Player pc, Creature crt, int attackNumber)
        {
            //always decrement ammo by one whether a hit or miss
            this.decrementAmmo(pc);

            int attackRoll = gv.sf.RandInt(20);
            int attackMod = CalcPcAttackModifier(pc, crt);
            int attack = attackRoll + attackMod;
            int defense = CalcCreatureDefense(pc, crt);
            int damage = CalcPcDamageToCreature(pc, crt);

            bool automaticallyHits = false;
            Item itChk = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
            if (itChk != null)
            {
                automaticallyHits = itChk.automaticallyHitsTarget;
            }
            //natural 20 always hits
            if ((attack >= defense) || (attackRoll == 20) || (automaticallyHits == true)) //HIT
            {
                crt.hp = crt.hp - damage;
                gv.cc.addLogText("<font color='aqua'>" + pc.name + "</font>" +
                        "<font color='white'>" + " attacks " + "</font>" +
                        "<font color='silver'>" + crt.cr_name + "</font>" +
                        "<BR>");
                gv.cc.addLogText("<font color='white'>" + " and HITS (" + "</font>" +
                        "<font color='lime'>" + damage + "</font>" +
                        "<font color='white'>" + " damage)" + "</font>" +
                        "<BR>");
                gv.cc.addLogText("<font color='white'>" + attackRoll + " + " + attackMod + " >= " + defense + "</font>" +
                        "<BR>");

                Item it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                if (it != null)
                {                    
                    doOnHitScriptBasedOnFilename(it.onScoringHit, crt, pc);
                    if (!it.onScoringHitCastSpellTag.Equals("none"))
                    {
                        doItemOnHitCastSpell(it.onScoringHitCastSpellTag, it, crt);
                    }
                }

                it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                if (it != null)
                {
                    doOnHitScriptBasedOnFilename(it.onScoringHit, crt, pc);
                    if (!it.onScoringHitCastSpellTag.Equals("none"))
                    {
                        doItemOnHitCastSpell(it.onScoringHitCastSpellTag, it, crt);
                    }
                }
                                
                //play attack sound for melee (not ranged)
                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                {
                    gv.PlaySound(mod.getItemByResRefForInfo(pc.MainHandRefs.resref).itemOnUseSound);
                }

                //Draw floaty text showing damage above Creature
                int txtH = (int)gv.drawFontRegHeight;
                int shiftUp = 0 - (attackNumber * txtH);
                floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), damage + "", shiftUp);
                gv.postDelayed("doFloatyText", 100);

                if (crt.hp <= 0)
                {
                    deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                    gv.cc.addLogText("<font color='lime'>You killed the " + crt.cr_name + "</font><BR>");
                    try
                    {
                        for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                        {
                            if (mod.currentEncounter.encounterCreatureList[x] == crt)
                            {
                                try
                                {
                                    //do OnDeath IBScript
                                    gv.cc.doIBScriptBasedOnFilename(crt.onDeathIBScript, crt.onDeathIBScriptParms);
                                    mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                                    mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                                }
                                catch (Exception ex)
                                {
                                    gv.errorLog(ex.ToString());
                                }
                            }
                        }
                        //mod.currentEncounter.encounterCreatureList.remove(crt);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                    return 2; //killed
                }
                else
                {
                    return 1; //hit
                }
            }
            else //MISSED
            {
                //play attack sound for melee (not ranged)
                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                {
                    gv.PlaySound(mod.getItemByResRefForInfo(pc.MainHandRefs.resref).itemOnUseSound);
                }
                gv.cc.addLogText("<font color='aqua'>" + pc.name + "</font>" +
                        "<font color='white'>" + " attacks " + "</font>" +
                        "<font color='gray'>" + crt.cr_name + "</font>" +
                        "<BR>");
                gv.cc.addLogText("<font color='white'>" + " and MISSES" + "</font>" +
                        "<BR>");
                gv.cc.addLogText("<font color='white'>" + attackRoll + " + " + attackMod + " < " + defense + "</font>" +
                        "<BR>");
                return 0; //missed
            }
        }
        public void doItemOnHitCastSpell(string tag, Item it, object trg)
        {
            Spell sp = gv.mod.getSpellByTag(tag);
            if (sp == null) { return; }
            gv.cc.doSpellBasedOnScriptOrEffectTag(sp, it, trg);
        }
        public void doCombatCast(Player pc)
        {
            object target = getCastTarget(pc);
            //gv.cc.doSpellBasedOnTag(gv.cc.currentSelectedSpell.tag, pc, target);
            gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, pc, target);
            if (deathAnimationLocations.Count > 0)
            {
                drawDeathAnimation = true;
                animationFrameIndex = 0;
                animationState = AnimationState.DeathAnimation;
                gv.postDelayed("doAnimation", (mod.combatAnimationSpeed));
                //play death ending sound
                //gv.PlaySound(gv.sf.SpellToCast.spellEndSound);
            }
            else
            {
                //check for end of encounter
                checkEndEncounter();
                //end PC's turn
                gv.touchEnabled = true;
                animationState = AnimationState.None;
                endPcTurn(true);
            }
        }
        public void endPcTurn(bool endStealthMode)
        {
            gv.Render();
            //remove stealth if endStealthMode = true		
            Player pc = mod.playerList[currentPlayerIndex];
            if (endStealthMode)
            {
                pc.steathModeOn = false;
            }
            else //else test to see if enter/stay in stealth mode if has trait
            {
                doStealthModeCheck(pc);
            }
            canMove = true;
            turnController();
        }
        public void doStealthModeCheck(Player pc)
        {
            int skillMod = 0;
            if (pc.knownTraitsTags.Contains("stealth4"))
            {
                Trait tr = mod.getTraitByTag("stealth4");
                skillMod = tr.skillModifier;
            }
            else if (pc.knownTraitsTags.Contains("stealth3"))
            {
                Trait tr = mod.getTraitByTag("stealth3");
                skillMod = tr.skillModifier;
            }
            else if (pc.knownTraitsTags.Contains("stealth2"))
            {
                Trait tr = mod.getTraitByTag("stealth2");
                skillMod = tr.skillModifier;
            }
            else if (pc.knownTraitsTags.Contains("stealth"))
            {
                Trait tr = mod.getTraitByTag("stealth");
                skillMod = tr.skillModifier;
            }
            else
            {
                //PC doesn't have stealth trait
                pc.steathModeOn = false;
                return;
            }
            int attMod = (pc.dexterity - 10) / 2;
            int roll = gv.sf.RandInt(20);
            int DC = 18; //eventually change to include area modifiers, proximity to enemies, etc.
            if (roll + attMod + skillMod >= DC)
            {
                pc.steathModeOn = true;
                gv.cc.addLogText("<font color='lime'> stealth ON: " + roll + "+" + attMod + "+" + skillMod + ">=" + DC + "</font><BR>");
            }
            else
            {
                pc.steathModeOn = false;
                gv.cc.addLogText("<font color='lime'> stealth OFF: " + roll + "+" + attMod + "+" + skillMod + "<" + DC + "</font><BR>");
            }
        }
        public void doPlayerCombatFacing(Player pc, int tarX, int tarY)
        {
            if ((tarX == pc.combatLocX) && (tarY > pc.combatLocY)) { pc.combatFacing = 2; }
            if ((tarX > pc.combatLocX) && (tarY > pc.combatLocY)) { pc.combatFacing = 3; }
            if ((tarX < pc.combatLocX) && (tarY > pc.combatLocY)) { pc.combatFacing = 1; }
            if ((tarX == pc.combatLocX) && (tarY < pc.combatLocY)) { pc.combatFacing = 8; }
            if ((tarX > pc.combatLocX) && (tarY < pc.combatLocY)) { pc.combatFacing = 9; }
            if ((tarX < pc.combatLocX) && (tarY < pc.combatLocY)) { pc.combatFacing = 7; }
            if ((tarX > pc.combatLocX) && (tarY == pc.combatLocY)) { pc.combatFacing = 6; }
            if ((tarX < pc.combatLocX) && (tarY == pc.combatLocY)) { pc.combatFacing = 4; }
        }
        #endregion

        #region Creature Combat Stuff
	    public void doCreatureTurn()
	    {
		    canMove = true;
		    Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];            
		    //do onStartTurn IBScript
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnStartCombatTurnIBScript, gv.mod.currentEncounter.OnStartCombatTurnIBScriptParms);
            creatureMoves = 0;
            doCreatureNextAction();
	    }
        public void doCreatureNextAction()
        {
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
            CalculateUpperLeftCreature();
            if ((crt.hp > 0) && (!crt.isHeld()))
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                gv.Render();
                animationState = AnimationState.CreatureThink;
                //gv.postDelayed("doAnimation", 5 * mod.combatAnimationSpeed);
                gv.postDelayed("doAnimation", (int)(1.25f * mod.combatAnimationSpeed));

            }
            else
            { 
                endCreatureTurn();
            }
        }
	    public void doCreatureTurnAfterDelay()
	    {
		    Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
		
		    gv.sf.ActionToTake = null;
		    gv.sf.SpellToCast = null;

            if (crt.isImmobile())
            {
                creatureMoves = 99;
            }

            //determine the action to take
            doCreatureAI(crt);

            //do the action (melee/ranged, cast spell, use trait, etc.)
            if (gv.sf.ActionToTake == null)
            {
        	    endCreatureTurn();
            }
            if (gv.sf.ActionToTake.Equals("Attack"))
            {
                Player pc = targetClosestPC(crt);
        	    gv.sf.CombatTarget = pc;	                
                CreatureDoesAttack(crt);
            }
            else if (gv.sf.ActionToTake.Equals("Move"))
            {
                if ((creatureMoves + 0.5f) < crt.moveDistance)
                {
                    CreatureMoves();
                }
                else
                {
                    endCreatureTurn();
                }
            }
            else if (gv.sf.ActionToTake.Equals("Cast"))
            {
                if ((gv.sf.SpellToCast != null) && (gv.sf.CombatTarget != null))
                {
                    CreatureCastsSpell(crt);
                }
            }
	    }
	    public void CreatureMoves()
	    {
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
            if (creatureMoves + 0.5f < crt.moveDistance)
		    {
			    Player pc = targetClosestPC(crt);
			    //run pathFinder to get new location
			    if (pc != null)
			    {
				    pf.resetGrid();
				    Coordinate newCoor = pf.findNewPoint(crt, new Coordinate(pc.combatLocX, pc.combatLocY));
				    if ((newCoor.X == -1) && (newCoor.Y == -1))
				    {
					    //didn't find a path, don't move
                        gv.Render();
			    	    endCreatureTurn();
					    return;
				    }
				   
                    //it's a diagonal move
                    if ((crt.combatLocX != newCoor.X) && (crt.combatLocY != newCoor.Y))
                    {
                        //enough  move points availbale to do the diagonal move
                        if ((crt.moveDistance - creatureMoves) >= mod.diagonalMoveCost)
                        {
                            if ((newCoor.X < crt.combatLocX) && (!crt.combatFacingLeft)) //move left
                            {
                                crt.combatFacingLeft = true;
                            }
                            else if ((newCoor.X > crt.combatLocX) && (crt.combatFacingLeft)) //move right
                            {
                                crt.combatFacingLeft = false;
                            }
                            //CHANGE FACING BASED ON MOVE
                            doCreatureCombatFacing(crt, newCoor.X, newCoor.Y);
                            moveCost = mod.diagonalMoveCost;
                            crt.combatLocX = newCoor.X;
                            crt.combatLocY = newCoor.Y;
                            canMove = false;
                            animationState = AnimationState.CreatureMove;
                            //gv.postDelayed("doAnimation", 2 * mod.combatAnimationSpeed);
                            gv.postDelayed("doAnimation", (int)(0.5f * mod.combatAnimationSpeed));

                        }
                        
                        //try to move horizontally or vertically instead if most points are not enough for diagonal move
                        else if ((crt.moveDistance - creatureMoves) >= 1)
                        {
                            pf.resetGrid();
                            //block the originial diagonal target square and calculate again
                            mod.nonAllowedDiagonalSquareX = newCoor.X;
                            mod.nonAllowedDiagonalSquareY = newCoor.Y;
                            newCoor = pf.findNewPoint(crt, new Coordinate(pc.combatLocX, pc.combatLocY));
                            if ((newCoor.X == -1) && (newCoor.Y == -1))
                            {
                                //didn't find a path, don't move
                                gv.Render();
                                endCreatureTurn();
                                return;
                            }
                            if ((newCoor.X < crt.combatLocX) && (!crt.combatFacingLeft)) //move left
                            {
                                crt.combatFacingLeft = true;
                            }
                            else if ((newCoor.X > crt.combatLocX) && (crt.combatFacingLeft)) //move right
                            {
                                crt.combatFacingLeft = false;
                            }
                            //CHANGE FACING BASED ON MOVE
                            doCreatureCombatFacing(crt, newCoor.X, newCoor.Y);
                            moveCost = 1;
                            crt.combatLocX = newCoor.X;
                            crt.combatLocY = newCoor.Y;
                            canMove = false;
                            animationState = AnimationState.CreatureMove;
                            //gv.postDelayed("doAnimation", 2 * mod.combatAnimationSpeed);
                            gv.postDelayed("doAnimation", (int)(0.5f * mod.combatAnimationSpeed));

                        }
                        //less than one move point, no move
                        else
                        {
                            canMove = false;
                            animationState = AnimationState.CreatureMove;
                            //gv.postDelayed("doAnimation", 2 * mod.combatAnimationSpeed);
                            gv.postDelayed("doAnimation", (int)(0.5f * mod.combatAnimationSpeed));

                        }
                    }
                   //it's a horizontal or vertical move
                    else
                    {
                        if ((newCoor.X < crt.combatLocX) && (!crt.combatFacingLeft)) //move left
                        {
                            crt.combatFacingLeft = true;
                        }
                        else if ((newCoor.X > crt.combatLocX) && (crt.combatFacingLeft)) //move right
                        {
                            crt.combatFacingLeft = false;
                        }
                        //CHANGE FACING BASED ON MOVE
                        doCreatureCombatFacing(crt, newCoor.X, newCoor.Y);
                        crt.combatLocX = newCoor.X;
                        crt.combatLocY = newCoor.Y;
                        canMove = false;
                        animationState = AnimationState.CreatureMove;
                        //gv.postDelayed("doAnimation", 2 * mod.combatAnimationSpeed);
                        gv.postDelayed("doAnimation", (int)(0.5f * mod.combatAnimationSpeed));

                    }
			    }
			    else //no target found
			    {
                    gv.Render();
		    	    endCreatureTurn();
				    return;
			    }
		    }
            //less than a move point left, no move
		    else
		    {
                gv.Render();
	    	    endCreatureTurn();
			    return;
		    }
	    }
	    public void CreatureDoesAttack(Creature crt)
        {
            if (gv.sf.CombatTarget != null)
            {
	            Player pc = (Player)gv.sf.CombatTarget;
                //Uses Map Pixel Locations
	            int endX = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
			    int endY = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
			    int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
			    int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
	            // determine if ranged or melee
	            if ((crt.cr_category.Equals("Ranged")) 
	        		    && (CalcDistance(crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY) <= crt.cr_attRange)
	        		    && (isVisibleLineOfSight(new Coordinate(endX, endY), new Coordinate(startX, startY))))
	            {
	                //play attack sound for ranged
	        	    gv.PlaySound(crt.cr_attackSound);
	                if ((pc.combatLocX < crt.combatLocX) && (!crt.combatFacingLeft)) //attack left
        		    {
        			    crt.combatFacingLeft = true;
        		    }
        		    else if ((pc.combatLocX > crt.combatLocX) && (crt.combatFacingLeft)) //attack right
        		    {
        			    crt.combatFacingLeft = false;
        		    }
                    //CHANGE FACING BASED ON ATTACK
                    doCreatureCombatFacing(crt, pc.combatLocX, pc.combatLocY);
	                if (crt.hp > 0)
	                {
	            	    creatureToAnimate = crt;
	    	            playerToAnimate = null;
                        gv.Render();
	    	            creatureTargetLocation = new Coordinate(pc.combatLocX, pc.combatLocY);
	    	            animationState = AnimationState.CreatureRangedAttackAnimation;
                        gv.sf.CreateAoeSquaresList(crt, pc, AreaOfEffectShape.Circle, 0);
                        //int speed = gv.sf.GetGlobalInt("animationSpeed");
                        //if (speed < 1)
                        //{
                        //    speed = 50;
                        //}
                        gv.postDelayed("doAnimation", (int)(5 * (0.5f)* mod.combatAnimationSpeed));
	                }
	                else
	                {
	                    //skip this guys turn
	                }
	            }
	            else if ((crt.cr_category.Equals("Melee")) 
	        		    && (CalcDistance(crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY) <= crt.cr_attRange))
	            {
	        	    if ((pc.combatLocX < crt.combatLocX) && (!crt.combatFacingLeft)) //attack left
        		    {
        			    crt.combatFacingLeft = true;
        		    }
        		    else if ((pc.combatLocX > crt.combatLocX) && (crt.combatFacingLeft)) //attack right
        		    {
        			    crt.combatFacingLeft = false;
        		    }
                    //CHANGE FACING BASED ON ATTACK
                    doCreatureCombatFacing(crt, pc.combatLocX, pc.combatLocY);
	                if (crt.hp > 0)
	                {
	            	    creatureToAnimate = crt;
	    	            playerToAnimate = null;
                        gv.Render();
	    	            animationState = AnimationState.CreatureMeleeAttackAnimation;
                        //int speed = gv.sf.GetGlobalInt("animationSpeed");
                        //if (speed == -1)
                        //{
                        //    speed = 50;
                        //}
                        gv.postDelayed("doAnimation", (int)(5 * (0.5f) * mod.combatAnimationSpeed));
	                }
	                else
	                {
	                    //skip this guys turn
	                }
	            }
	            else //not in range for attack so MOVE
	            {
	        	    CreatureMoves();
	            }
            }
            else //no target so move instead
            {
        	    CreatureMoves();
            }
        }
        public void CreatureCastsSpell(Creature crt)
        {
            Coordinate pnt = new Coordinate();
            if (gv.sf.CombatTarget is Player)
            {
                Player pc = (Player)gv.sf.CombatTarget;
                pnt = new Coordinate(pc.combatLocX, pc.combatLocY);
            }
            else if (gv.sf.CombatTarget is Creature)
            {
                Creature crtTarget = (Creature)gv.sf.CombatTarget;
                pnt = new Coordinate(crtTarget.combatLocX, crtTarget.combatLocY);
            }
            else if (gv.sf.CombatTarget is Coordinate)
            {
                pnt = (Coordinate)gv.sf.CombatTarget;
            }
            else //do not understand, what is the target
            {
        	    //Toast.makeText(gv.gameContext, "can't figure out target.", Toast.LENGTH_SHORT).show();
        	    return;
            }
            //Using Map Pixel Locations
            int endX = pnt.X * gv.squareSize + (gv.squareSize / 2);
		    int endY = pnt.Y * gv.squareSize + (gv.squareSize / 2);
		    int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
		    int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
            if ((getDistance(pnt, new Coordinate(crt.combatLocX, crt.combatLocY)) <= gv.sf.SpellToCast.range) 
        		    && (isVisibleLineOfSight(new Coordinate(endX, endY), new Coordinate(startX, startY))))
            {
                if ((pnt.X < crt.combatLocX) && (!crt.combatFacingLeft)) //attack left
    		    {
    			    crt.combatFacingLeft = true;
    		    }
    		    else if ((pnt.X > crt.combatLocX) && (crt.combatFacingLeft)) //attack right
    		    {
    			    crt.combatFacingLeft = false;
    		    }
                //CHANGE FACING BASED ON ATTACK
                doCreatureCombatFacing(crt, pnt.X, pnt.Y);
        	    creatureTargetLocation = pnt;
        	    creatureToAnimate = crt;
	            playerToAnimate = null;
                gv.Render();
	            animationState = AnimationState.CreatureCastAttackAnimation;
                /*int speed = gv.sf.GetGlobalInt("animationSpeed");
                if (speed == -1)
                {
                    speed = 50;
                }*/
                //gv.postDelayed("doAnimation", 5 * mod.combatAnimationSpeed);
                gv.postDelayed("doAnimation", (int)(1.25f * mod.combatAnimationSpeed));

	                    
            }
            else
            {
                //#region Do a Melee or Ranged Attack
                Player pc = targetClosestPC(crt);
                gv.sf.CombatTarget = pc;	                
                CreatureDoesAttack(crt);
            }
        }
        public void doCreatureSpell()
        {
    	    Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
    	    //gv.cc.doSpellBasedOnTag(gv.sf.SpellToCast.tag, crt, gv.sf.CombatTarget);
            gv.cc.doSpellBasedOnScriptOrEffectTag(gv.sf.SpellToCast, crt, gv.sf.CombatTarget);
            if (deathAnimationLocations.Count > 0)
            {
                drawDeathAnimation = true;
                animationFrameIndex = 0;
                animationState = AnimationState.DeathAnimation;
                gv.postDelayed("doAnimation", (mod.combatAnimationSpeed));
                //play death ending sound
                //gv.PlaySound(gv.sf.SpellToCast.spellEndSound);
            }
            else
            {
                endCreatureTurn();
            }
        }
	    public void doCreatureAI(Creature crt)
	    {
		    //These are the current generic AI types
            //BasicAttacker:          basic attack (ranged or melee)
            //Healer:                 heal Friend(s) until out of SP
            //BattleHealer:           heal Friend(s) and/or attack
            //DamageCaster:           cast damage spells
            //BattleDamageCaster:     cast damage spells and/or attack
            //DebuffCaster:           cast debuff spells
            //BattleDebuffCaster:     cast debuff spells and/or attack
            //GeneralCaster:          cast any of their known spells by random
            //BattleGeneralCaster:    cast any of their known spells by random and/or attack

            if (crt.cr_ai.Equals("BasicAttacker"))
            {
                if (gv.mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " is a BasicAttacker</font><BR>");
                }
                BasicAttacker(crt);
            }        
            else if (crt.cr_ai.Equals("GeneralCaster"))
            {
                if (gv.mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " is a GeneralCaster</font><BR>");
                }
                GeneralCaster(crt);
            }        
            else
            {
                if (gv.mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " is a BasicAttacker</font><BR>");
                }
                BasicAttacker(crt);
            }
	    }
	    public void BasicAttacker(Creature crt)
        {
            Player pc = targetClosestPC(crt);
            gv.sf.CombatTarget = pc;
            gv.sf.ActionToTake = "Attack";
        }
        public void GeneralCaster(Creature crt)
        {
    	    gv.sf.SpellToCast = null;
            //just pick a random spell from KnownSpells
            //try a few times to pick a random spell that has enough SP
            for (int i = 0; i < 10; i++)
            {
                int rnd = gv.sf.RandInt(crt.knownSpellsTags.Count);
                Spell sp = mod.getSpellByTag(crt.knownSpellsTags[rnd-1]);
                if (sp != null)
                {
                    if (sp.costSP <= crt.sp)
                    {
                	    gv.sf.SpellToCast = sp;
                        
                        if (gv.sf.SpellToCast.spellTargetType.Equals("Enemy"))
                        {
                            Player pc = targetClosestPC(crt);
                            gv.sf.CombatTarget = pc;
                            gv.sf.ActionToTake = "Cast";
                            break;
                        }
                        else if (gv.sf.SpellToCast.spellTargetType.Equals("PointLocation"))
                        {
                    	    Coordinate bestLoc = targetBestPointLocation(crt);
                    	    if (bestLoc == new Coordinate(-1,-1))
                    	    {
                    		    //didn't find a target so use closest PC
                    		    Player pc = targetClosestPC(crt);
                        	    gv.sf.CombatTarget = new Coordinate(pc.combatLocX, pc.combatLocY);
                    	    }
                    	    else
                    	    {
                    		    gv.sf.CombatTarget = targetBestPointLocation(crt);
                    	    }                    	
                    	    gv.sf.ActionToTake = "Cast";
                            break;
                        }
                        else if (gv.sf.SpellToCast.spellTargetType.Equals("Friend"))
                        {
                            //target is another creature (currently assumed that spell is a heal spell)
                            Creature targetCrt = GetCreatureWithMostDamaged();
                            if (targetCrt != null)
                            {
                        	    gv.sf.CombatTarget = targetCrt;
                        	    gv.sf.ActionToTake = "Cast";
                                break;
                            }
                            else //didn't find a target that needs HP
                            {
                        	    gv.sf.SpellToCast = null;
                        	    continue;
                            }
                        }
                        else if (gv.sf.SpellToCast.spellTargetType.Equals("Self"))
                        {
                            //target is self (currently assumed that spell is a heal spell)
                            Creature targetCrt = crt;
                            if (targetCrt != null)
                            {
                        	    gv.sf.CombatTarget = targetCrt;
                        	    gv.sf.ActionToTake = "Cast";
                                break;
                            }
                        }
                        else //didn't find a target so set to null so that will use attack instead
                        {
                    	    gv.sf.SpellToCast = null;
                        }
                    }
                }
            }
            if (gv.sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
            {
        	    Player pc = targetClosestPC(crt);
        	    gv.sf.CombatTarget = pc;
        	    gv.sf.ActionToTake = "Attack";
            }
        }
        public void endCreatureTurn()
        {
            gv.Render();
            canMove = true;
            gv.sf.ActionToTake = null;
            gv.sf.SpellToCast = null;
            if (checkEndEncounter())
            {
                return;
            }
            turnController();
        }
        public void doStandardCreatureAttack()
        {
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
            Player pc = (Player)gv.sf.CombatTarget;

            bool hit = false;
            for (int i = 0; i < crt.cr_numberOfAttacks; i++)
            {
                //this reduces the to hit bonus for each further creature attack by an additional -5
                //creatureMultAttackPenalty = 5 * i;            
                bool hitreturn = doActualCreatureAttack(pc, crt, i);
                if (hitreturn) { hit = true; }
                if (pc.hp <= 0)
                {
                    break; //do not try and attack same PC that was just killed
                }
            }

            //play attack sound for melee
            if (!crt.cr_category.Equals("Ranged"))
            {
                gv.PlaySound(crt.cr_attackSound);
            }

            if (hit)
            {
                drawHitAnimation = true;
                hitAnimationLocation = new Coordinate(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY));
                gv.Render();
                animationState = AnimationState.PcHitAnimation;
                gv.postDelayed("doAnimation", (int)(4 * (0.5f) * mod.combatAnimationSpeed));
            }
            else
            {
                drawMissAnimation = true;
                hitAnimationLocation = new Coordinate(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY)); 
                gv.Render();
                animationState = AnimationState.PcMissedAnimation;
                gv.postDelayed("doAnimation", (int)(4 * (0.5f) * mod.combatAnimationSpeed));
            }
        }
        public bool doActualCreatureAttack(Player pc, Creature crt, int attackNumber)
        {
            int attackRoll = gv.sf.RandInt(20);
            int attackMod = CalcCreatureAttackModifier(crt, pc);
            int defense = CalcPcDefense(pc, crt);
            int damage = CalcCreatureDamageToPc(pc, crt);
            int attack = attackRoll + attackMod;

            if ((attack >= defense) || (attackRoll == 20))
            {
                pc.hp = pc.hp - damage;
                gv.cc.addLogText("<font color='silver'>" + crt.cr_name + "</font>" +
                        "<font color='white'>" + " attacks " + "</font>" +
                        "<font color='aqua'>" + pc.name + "</font><BR>");
                gv.cc.addLogText("<font color='white'>" + " and HITS (" + "</font>" +
                        "<font color='red'>" + damage + "</font>" +
                        "<font color='white'>" + " damage)" + "</font><BR>");
                gv.cc.addLogText("<font color='white'>" + attackRoll + " + " + attackMod + " >= " + defense + "</font><BR>");

                doOnHitScriptBasedOnFilename(crt.onScoringHit, crt, pc);
                if (!crt.onScoringHitCastSpellTag.Equals("none"))
                {
                    doCreatureOnHitCastSpell(crt, pc);
                }
                
                //Draw floaty text showing damage above PC
                int txtH = (int)gv.drawFontRegHeight;
                int shiftUp = 0 - (attackNumber * txtH);
                floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), damage + "", shiftUp);
                gv.postDelayed("doFloatyText", 100);

                if (pc.hp <= 0)
                {
                    deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                    gv.cc.addLogText("<font color='red'>" + pc.name + " drops down unconsciously!" + "</font><BR>");
                    pc.charStatus = "Dead";
                }
                return true;
            }
            else
            {
                gv.cc.addLogText("<font color='silver'>" + crt.cr_name + "</font>" +
                        "<font color='white'>" + " attacks " + "</font>" +
                        "<font color='aqua'>" + pc.name + "</font><BR>");
                gv.cc.addLogText("<font color='white'>" + " and MISSES" + "</font><BR>");
                gv.cc.addLogText("<font color='white'>" + attackRoll + " + " + attackMod + " < " + defense + "</font><BR>");
                return false;
            }
        }
        public void doCreatureCombatFacing(Creature crt, int tarX, int tarY)
        {
            if ((tarX == crt.combatLocX) && (tarY > crt.combatLocY)) { crt.combatFacing = 2; }
            if ((tarX > crt.combatLocX) && (tarY > crt.combatLocY)) { crt.combatFacing = 3; }
            if ((tarX < crt.combatLocX) && (tarY > crt.combatLocY)) { crt.combatFacing = 1; }
            if ((tarX == crt.combatLocX) && (tarY < crt.combatLocY)) { crt.combatFacing = 8; }
            if ((tarX > crt.combatLocX) && (tarY < crt.combatLocY)) { crt.combatFacing = 9; }
            if ((tarX < crt.combatLocX) && (tarY < crt.combatLocY)) { crt.combatFacing = 7; }
            if ((tarX > crt.combatLocX) && (tarY == crt.combatLocY)) { crt.combatFacing = 6; }
            if ((tarX < crt.combatLocX) && (tarY == crt.combatLocY)) { crt.combatFacing = 4; }
        }
        #endregion

        public void doOnHitScriptBasedOnFilename(string filename, Creature crt, Player pc)
        {
            if (!filename.Equals("none"))
            {
                try
                {
                    if (filename.Equals("onHitBeetleFire.cs"))
                    {
                        float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
                        float damage = (1 * gv.sf.RandInt(2)) + 0;
                        int fireDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                        + " fireDam = " + fireDam + "</font>" +
                                        "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + pc.name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='red'>" + fireDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        pc.hp -= fireDam;
                    }

                    else if (filename.Equals("onHitMaceOfStunning.cs"))
                    {
                        int tryHold = gv.sf.RandInt(100);
                        if (tryHold > 50)
                        {
                            //attempt to hold PC
                            int saveChkRoll = gv.sf.RandInt(20);
                            int saveChk = saveChkRoll + crt.fortitude;
                            int DC = 15;
                            if (saveChk >= DC) //passed save check
                            {
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " avoids stun (" + saveChkRoll + " + " + crt.fortitude + " >= " + DC + ")</font><BR>");
                            }
                            else
                            {
                                gv.cc.addLogText("<font color='red'>" + crt.cr_name + " is stunned by mace (" + saveChkRoll + " + " + crt.fortitude + " < " + DC + ")</font><BR>");
                                crt.cr_status = "Held";
                                Effect ef = mod.getEffectByTag("hold");
                                crt.AddEffectByObject(ef, 1);
                            }
                        }
                    }
                    else if (filename.Equals("onHitBeetleAcid.cs"))
                    {
                        float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalAcid / 100f));
                        float damage = (1 * gv.sf.RandInt(2)) + 0;
                        int acidDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                    + " acidDam = " + acidDam + "</font>" +
                                    "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + pc.name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='lime'>" + acidDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        pc.hp -= acidDam;

                        //attempt to hold PC
                        int saveChkRoll = gv.sf.RandInt(20);
                        //int saveChk = saveChkRoll + target.Will;
                        int saveChk = saveChkRoll + pc.fortitude;
                        int DC = 10;
                        if (saveChk >= DC) //passed save check
                        {
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " avoids the acid stun (" + saveChkRoll + " + " + pc.fortitude + " >= " + DC + ")</font><BR>");
                        }
                        else
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " is held by an acid stun (" + saveChkRoll + " + " + pc.fortitude + " < " + DC + ")</font><BR>");
                            pc.charStatus = "Held";
                            Effect ef = mod.getEffectByTag("hold");
                            pc.AddEffectByObject(ef, 1);
                        }
                    }
                    else if (filename.Equals("onHitOneFire.cs"))
                    {
                        float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                        float damage = 1.0f;
                        int fireDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                        + " fireDam = " + fireDam + "</font>" +
                                        "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + crt.cr_name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='red'>" + fireDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        crt.hp -= fireDam;
                    }
                    else if (filename.Equals("onHitOneTwoFire.cs"))
                    {
                        float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                        float damage = (1 * gv.sf.RandInt(2)) + 0;
                        int fireDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                        + " fireDam = " + fireDam + "</font>" +
                                        "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + crt.cr_name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='red'>" + fireDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        crt.hp -= fireDam;
                    }
                    else if (filename.Equals("onHitTwoThreeFire.cs"))
                    {
                        float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                        float damage = (1 * gv.sf.RandInt(2)) + 1;
                        int fireDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                        + " fireDam = " + fireDam + "</font>" +
                                        "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + crt.cr_name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='red'>" + fireDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        crt.hp -= fireDam;
                    }
                    else if (filename.Equals("onHitPcPoisonedLight.cs"))
                    {
                        int saveChkRoll = gv.sf.RandInt(20);
                        int saveChk = saveChkRoll + pc.reflex;
                        int DC = 13;
                        if (saveChk >= DC) //passed save check
                        {
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " avoids being poisoned" + "</font>" +
                                    "<BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font>" +
                                            "<BR>");
                            }
                        }
                        else //failed check
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " is poisoned" + "</font>" + "<BR>");
                            Effect ef = mod.getEffectByTag("poisonedLight");
                            pc.AddEffectByObject(ef, 1);
                        }
                    }
                    else if (filename.Equals("onHitPcPoisonedMedium.cs"))
                    {
                        int saveChkRoll = gv.sf.RandInt(20);
                        int saveChk = saveChkRoll + pc.reflex;
                        int DC = 16;
                        if (saveChk >= DC) //passed save check
                        {
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " avoids being poisoned" + "</font>" +
                                    "<BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font>" +
                                            "<BR>");
                            }
                        }
                        else //failed check
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " is poisoned" + "</font>" + "<BR>");
                            Effect ef = mod.getEffectByTag("poisonedMedium");
                            pc.AddEffectByObject(ef, 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    gv.errorLog(ex.ToString());
                }
            }
        }
        public void doCreatureOnHitCastSpell(Creature crt, Player pc)
        {
            Spell sp = gv.mod.getSpellByTag(crt.onScoringHitCastSpellTag);
            if (sp == null) { return; }
            gv.cc.doSpellBasedOnScriptOrEffectTag(sp, crt, pc);
        }
        public bool checkEndEncounter()
        {
            int foundOneCrtr = 0;
            foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
            {
                if (crtr.hp > 0)
                {
                    foundOneCrtr = 1;
                }
            }
            if ((foundOneCrtr == 0) && (gv.screenType.Equals("combat")))
            {
                gv.touchEnabled = true;
                // give gold drop
                if (mod.currentEncounter.goldDrop > 0)
                {
                    gv.cc.addLogText("<font color='yellow'>The party finds " + mod.currentEncounter.goldDrop + " " + mod.goldLabelPlural + ".<BR></font>");
                }
                mod.partyGold += mod.currentEncounter.goldDrop;
                // give InventoryList
                if (mod.currentEncounter.encounterInventoryRefsList.Count > 0)
                {

                    string s = "<font color='fuchsia'>" + "The party has found:<BR>";
                    foreach (ItemRefs itRef in mod.currentEncounter.encounterInventoryRefsList)
                    {
                        mod.partyInventoryRefsList.Add(itRef.DeepCopy());
                        s += itRef.name + "<BR>";
                        //find this creatureRef in mod creature list

                    }
                    gv.cc.addLogText(s + "</font>" + "<BR>");
                }

                int giveEachXP = encounterXP / mod.playerList.Count;
                gv.cc.addLogText("fuchsia", "Each receives " + giveEachXP + " XP");
                foreach (Player givePcXp in mod.playerList)
                {
                    givePcXp.XP = givePcXp.XP + giveEachXP;
                }
                btnSelect.Text = "SELECT";
                gv.screenType = "main";
                if (mod.playMusic)
                {
                    gv.stopCombatMusic();
                    gv.startMusic();
                    gv.startAmbient();
                }
                //do END ENCOUNTER IBScript
                gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnEndCombatIBScript, gv.mod.currentEncounter.OnEndCombatIBScriptParms);
                if (gv.cc.calledEncounterFromProp)
                {
                    gv.cc.doPropTriggers();
                }
                else
                {
                    gv.cc.doTrigger();
                }
                return true;
            }

            int foundOnePc = 0;
            foreach (Player pc in mod.playerList)
            {
                if (pc.hp > 0)
                {
                    foundOnePc = 1;
                }
            }
            if (foundOnePc == 0)
            {
                gv.touchEnabled = true;
                gv.sf.MessageBox("Your party has been defeated!");
                if (mod.playMusic)
                {
                    gv.stopCombatMusic();
                    gv.startMusic();
                    gv.startAmbient();
                }
                gv.resetGame();
                gv.screenType = "title";
                return true;
            }
            return false;
        }

        //COMBAT SCREEN UPDATE
        public void Update(int elapsed)
        {
            foreach (Sprite spr in spriteList)
            {
                spr.Update(elapsed);
            }
            //remove sprite if hit end of life
            for (int x = spriteList.Count - 1; x >= 0; x--)
            {
                if (spriteList[x].timeToLiveInMilliseconds <= 0)
                {
                    try
                    {
                        spriteList.RemoveAt(x);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                }
            }
        }
        #region Combat Draw
        public void redrawCombat()
        {
            if (mod.com_showGrid)
            {
        	    tglGrid.toggleOn = true;
            }
            else
            {
        	    tglGrid.toggleOn = false;
            }
    	    //gv.drawLog();
    	    drawCombatMap();
            if (gv.mod.useCombatSmoothMovement == false)
            {
                drawCombatCreatures();
            }
            else
            {
                drawMovingCombatCreatures();
            }
            drawCombatPlayers();
            DrawHitAnimation();
            DrawMissAnimation();
            DrawProjectileAnimation();
            DrawEndingAnimation();
            DrawDeathAnimation();
            if (mod.currentEncounter.UseDayNightCycle)
            {
                drawOverlayTints();
            }            
            if ((!drawProjectileAnimation) && (!drawEndingAnimation) && (!drawHitAnimation) && (!drawMissAnimation) && (!drawDeathAnimation))
            {
                drawTargetHighlight();
                drawLosTrail();
            }
            if (mod.useUIBackground)
            {
                drawPanels();
            }
            drawSprites();
            gv.drawLog();
            drawFloatyText();
            drawHPText();
            drawSPText();
            drawFloatyTextList();
            drawCombatControls();
            drawPortraits();
        }
        public void drawPanels()
        {
            gv.cc.pnlLog.Draw();
            gv.cc.pnlToggles.Draw();
            gv.cc.pnlPortraits.Draw();
            gv.cc.pnlArrows.Draw();
            gv.cc.pnlHotkeys.Draw();
        }
        public void drawCombatControls()
	    {
		    gv.cc.ctrlUpArrow.Draw();
		    gv.cc.ctrlDownArrow.Draw();
		    gv.cc.ctrlLeftArrow.Draw();
		    gv.cc.ctrlRightArrow.Draw();
		    gv.cc.ctrlUpRightArrow.Draw();
		    gv.cc.ctrlDownLeftArrow.Draw();
		    gv.cc.ctrlUpLeftArrow.Draw();
		    gv.cc.ctrlDownRightArrow.Draw();
		    tglHP.Draw();
		    tglSP.Draw();
            tglMoveOrder.Draw();
		    tglSpeed.Draw();
		    tglSoundFx.Draw();
		    tglHelp.Draw();
		    tglGrid.Draw();
		    if (mod.debugMode)
            {
			    tglKill.Draw();
            }
		    gv.cc.tglSound.Draw();		
		    btnSwitchWeapon.Draw();
		
		    if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
		    {
			    btnSelect.Text = "TARGET";
		    }
		    else
		    {
			    btnSelect.Text = "SELECT";
		    }
		    btnSelect.Draw();
		
		    if (canMove)
		    {
			    if (currentCombatMode.Equals("move"))
			    {
                    btnMove.btnState = buttonState.On;
			    }
			    else
			    {
                    btnMove.btnState = buttonState.Normal;
			    }
		    }
		    else
		    {
                btnMove.btnState = buttonState.Off;
		    }
		    btnMove.Draw();
		    gv.cc.btnInventory.Draw();
		    if (currentCombatMode.Equals("attack"))
		    {
                btnAttack.btnState = buttonState.On;
		    }
		    else
		    {
                btnAttack.btnState = buttonState.Normal;
		    }
		    btnAttack.Draw();
		    if (currentCombatMode.Equals("cast"))
		    {
                btnCast.btnState = buttonState.On;
		    }
		    else
		    {
                btnCast.btnState = buttonState.Normal;
		    }
		    btnCast.Draw();
		    btnSkipTurn.Draw();
            Player pc = mod.playerList[currentPlayerIndex];
            float movesLeft = pc.moveDistance - currentMoves;
            if (movesLeft < 0) { movesLeft = 0; }
            btnMoveCounter.Text = movesLeft.ToString();
            btnMoveCounter.Draw();
	    }
        public void drawPortraits()
        {
            if (mod.playerList.Count > 0)
            {
                gv.cc.ptrPc0.Img = mod.playerList[0].portrait;
                gv.cc.ptrPc0.TextHP = mod.playerList[0].hp + "/" + mod.playerList[0].hpMax;
                gv.cc.ptrPc0.TextSP = mod.playerList[0].sp + "/" + mod.playerList[0].spMax;
                gv.cc.ptrPc0.Draw();
            }
            if (mod.playerList.Count > 1)
            {
                gv.cc.ptrPc1.Img = mod.playerList[1].portrait;
                gv.cc.ptrPc1.TextHP = mod.playerList[1].hp + "/" + mod.playerList[1].hpMax;
                gv.cc.ptrPc1.TextSP = mod.playerList[1].sp + "/" + mod.playerList[1].spMax;
                gv.cc.ptrPc1.Draw();
            }
            if (mod.playerList.Count > 2)
            {
                gv.cc.ptrPc2.Img = mod.playerList[2].portrait;
                gv.cc.ptrPc2.TextHP = mod.playerList[2].hp + "/" + mod.playerList[2].hpMax;
                gv.cc.ptrPc2.TextSP = mod.playerList[2].sp + "/" + mod.playerList[2].spMax;
                gv.cc.ptrPc2.Draw();
            }
            if (mod.playerList.Count > 3)
            {
                gv.cc.ptrPc3.Img = mod.playerList[3].portrait;
                gv.cc.ptrPc3.TextHP = mod.playerList[3].hp + "/" + mod.playerList[3].hpMax;
                gv.cc.ptrPc3.TextSP = mod.playerList[3].sp + "/" + mod.playerList[3].spMax;
                gv.cc.ptrPc3.Draw();
            }
            if (mod.playerList.Count > 4)
            {
                gv.cc.ptrPc4.Img = mod.playerList[4].portrait;
                gv.cc.ptrPc4.TextHP = mod.playerList[4].hp + "/" + mod.playerList[4].hpMax;
                gv.cc.ptrPc4.TextSP = mod.playerList[4].sp + "/" + mod.playerList[4].spMax;
                gv.cc.ptrPc4.Draw();
            }
            if (mod.playerList.Count > 5)
            {
                gv.cc.ptrPc5.Img = mod.playerList[5].portrait;
                gv.cc.ptrPc5.TextHP = mod.playerList[5].hp + "/" + mod.playerList[5].hpMax;
                gv.cc.ptrPc5.TextSP = mod.playerList[5].sp + "/" + mod.playerList[5].spMax;
                gv.cc.ptrPc5.Draw();
            }
        }
        public void drawCombatMap()
	    {
		    //row = y
		    //col = x
		    if (mod.currentEncounter.UseMapImage)
		    { 
                int sqrsizeW = mapBitmap.PixelSize.Width / this.mod.currentEncounter.MapSizeX;
                int sqrsizeH = mapBitmap.PixelSize.Height / this.mod.currentEncounter.MapSizeY;
                IbRect src = new IbRect(UpperLeftSquare.X * sqrsizeW, UpperLeftSquare.Y * sqrsizeH, sqrsizeW * 9, sqrsizeH * 9);
                IbRect dst = new IbRect(0 + gv.oXshift + mapStartLocXinPixels, 0, gv.squareSize * 9, gv.squareSize * 9);
	            gv.DrawBitmap(mapBitmap, src, dst);
	            //draw grid
	            if (mod.com_showGrid)
                {
                    src = new IbRect(0, 0, gv.squareSizeInPixels / 2, gv.squareSizeInPixels / 2);
                    dst = new IbRect(0 + mapStartLocXinPixels, 0, gv.squareSize, gv.squareSize);
                    for (int x = UpperLeftSquare.X; x < this.mod.currentEncounter.MapSizeX; x++)
		            {
                        for (int y = UpperLeftSquare.Y; y < this.mod.currentEncounter.MapSizeY; y++)
		                {
                            if (!IsInVisibleCombatWindow(x, y))
                            {
                                continue;
                            }

                            int tlX = ((x - UpperLeftSquare.X) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                            int tlY = (y - UpperLeftSquare.Y) * gv.squareSize;
                            int brX = gv.squareSize;
                            int brY = gv.squareSize;

                            dst = new IbRect(tlX, tlY, brX, brY);
                            if (mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x].LoSBlocked)
		                    {
		                	    gv.DrawBitmap(gv.cc.losBlocked, src, dst);
		                    }
                            if (mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x].Walkable != true)
		                    {
                                gv.DrawBitmap(gv.cc.walkBlocked, src, dst);
		                    }
		                    else
		                    {
                                gv.DrawBitmap(gv.cc.walkPass, src, dst);
		                    }
                            if ((pf.values != null) && (mod.debugMode))
                            {
                                //gv.DrawText(pf.values[x, y].ToString(), (x - UpperLeftSquare.X) * gv.squareSize + gv.oXshift + mapStartLocXinPixels, (y - UpperLeftSquare.Y) * gv.squareSize);
                            }
		                }
		            }
                }
		    }
		    else //using tiles
		    {
                //implemented loading only those tiles that are on current encounter map here
                /*TODO gv.cc.tileBitmapList.Clear();
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\tiles"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\tiles", "*.png");
                    foreach (string file in files)
                    {
                        string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                        foreach (TileEnc t in gv.mod.currentEncounter.encounterTiles)
                        {
                            if (t.Layer1Filename == fileNameWithOutExt || t.Layer2Filename == fileNameWithOutExt || t.Layer3Filename == fileNameWithOutExt)
                            {
                                gv.cc.tileBitmapList.Add(fileNameWithOutExt, gv.cc.LoadBitmap(fileNameWithOutExt));
                                break;
                            }
                        }
                    }
                }*/
                int minX = UpperLeftSquare.X - 5;
                if (minX < 0) { minX = 0; }
                int minY = UpperLeftSquare.Y - 5;
                if (minY < 0) { minY = 0; }
                int maxX = UpperLeftSquare.X + gv.playerOffset + gv.playerOffset + 1;
                if (maxX > this.mod.currentEncounter.MapSizeX) { maxX = this.mod.currentEncounter.MapSizeX; }
                int maxY = UpperLeftSquare.Y + gv.playerOffset + gv.playerOffset + 1;
                if (maxY > this.mod.currentEncounter.MapSizeY) { maxY = this.mod.currentEncounter.MapSizeY; }

                //XXXXXXXXXXX
                /*
                #region Draw Layer 1
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];

                        try
                        {

                            bool tileBitmapIsLoadedAlready = false;
                            int indexOfLoadedTile = -1;
                            for (int i = 0; i < gv.mod.loadedTileBitmapsNames.Count; i++)
                            {
                                if (gv.mod.loadedTileBitmapsNames[i] == tile.Layer1Filename)
                                {
                                    tileBitmapIsLoadedAlready = true;
                                    indexOfLoadedTile = i;
                                    break;
                                }
                            }

                            //hurghx
                            if (!tileBitmapIsLoadedAlready)
                            {
                                gv.mod.loadedTileBitmapsNames.Add(tile.Layer1Filename);
                                tile.tileBitmap1 = gv.cc.LoadBitmap(tile.Layer1Filename);

                                int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                                int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
                                float scalerX = tile.tileBitmap1.PixelSize.Width / 100;
                                float scalerY = tile.tileBitmap1.PixelSize.Height / 100;
                                int brX = (int)(gv.squareSize * scalerX);
                                int brY = (int)(gv.squareSize * scalerY);
                                IbRect src = new IbRect(0, 0, tile.tileBitmap1.PixelSize.Width, tile.tileBitmap1.PixelSize.Height);
                                IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);

                                gv.mod.loadedTileBitmaps.Add(tile.tileBitmap1);
                                gv.DrawBitmap(tile.tileBitmap1, src, dst);
                            }
                            else
                            {
                                int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                                int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
                                float scalerX = gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Width / 100;
                                float scalerY = gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Height / 100;
                                int brX = (int)(gv.squareSize * scalerX);
                                int brY = (int)(gv.squareSize * scalerY);
                                IbRect src = new IbRect(0, 0, gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Width, gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Height);
                                IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);

                                gv.DrawBitmap(gv.mod.loadedTileBitmaps[indexOfLoadedTile], src, dst);
                            }

                            //gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer1Filename], src, dst);
                        }
                        catch { }
                    }
                }
                #endregion
                */


                //XXXXXXXXXXX
                #region Draw Layer1
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        TileEnc tile = mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x];
                        try {
                            //insert1                        
                            bool tileBitmapIsLoadedAlready = false;
                            int indexOfLoadedTile = -1;
                            for (int i = 0; i < gv.mod.loadedTileBitmapsNames.Count; i++)
                            {
                                if (gv.mod.loadedTileBitmapsNames[i] == tile.Layer1Filename)
                                {
                                    tileBitmapIsLoadedAlready = true;
                                    indexOfLoadedTile = i;
                                    break;
                                }
                            }

                            //insert2
                            if (!tileBitmapIsLoadedAlready)
                            {
                                gv.mod.loadedTileBitmapsNames.Add(tile.Layer1Filename);
                                tile.tileBitmap1 = gv.cc.LoadBitmap(tile.Layer1Filename);
                                gv.mod.loadedTileBitmaps.Add(tile.tileBitmap1);

                                IbRect srcLyr = getSourceIbRect(
                                x,
                                y,
                                UpperLeftSquare.X,
                                UpperLeftSquare.Y,
                                tile.tileBitmap1.PixelSize.Width,
                                tile.tileBitmap1.PixelSize.Height);
                                if (srcLyr != null)
                                {
                                    int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                                    int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                                    int tlX = ((x - UpperLeftSquare.X + shiftX) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                                    int tlY = (y - UpperLeftSquare.Y + shiftY) * gv.squareSize;
                                    float scalerX = srcLyr.Width / 100;
                                    float scalerY = srcLyr.Height / 100;
                                    int brX = (int)(gv.squareSize * scalerX);
                                    int brY = (int)(gv.squareSize * scalerY);
                                    IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                                    gv.DrawBitmap(tile.tileBitmap1, srcLyr, dstLyr);
                                }
                            }
                            else
                            {
                                IbRect srcLyr = getSourceIbRect(
                                x,
                                y,
                                UpperLeftSquare.X,
                                UpperLeftSquare.Y,
                                gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Width,
                                gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Height);
                                if (srcLyr != null)
                                {
                                    int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                                    int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                                    int tlX = ((x - UpperLeftSquare.X + shiftX) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                                    int tlY = (y - UpperLeftSquare.Y + shiftY) * gv.squareSize;
                                    float scalerX = srcLyr.Width / 100;
                                    float scalerY = srcLyr.Height / 100;
                                    int brX = (int)(gv.squareSize * scalerX);
                                    int brY = (int)(gv.squareSize * scalerY);
                                    IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                                    gv.DrawBitmap(gv.mod.loadedTileBitmaps[indexOfLoadedTile], srcLyr, dstLyr);
                                }

                            }
                        }
                        catch
                        { }                  
                    }
	            }
                #endregion

                #region Draw Layer2
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        TileEnc tile = mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x];
                        try
                        {
                            //insert1                        
                            bool tileBitmapIsLoadedAlready = false;
                            int indexOfLoadedTile = -1;
                            for (int i = 0; i < gv.mod.loadedTileBitmapsNames.Count; i++)
                            {
                                if (gv.mod.loadedTileBitmapsNames[i] == tile.Layer2Filename)
                                {
                                    tileBitmapIsLoadedAlready = true;
                                    indexOfLoadedTile = i;
                                    break;
                                }
                            }

                            //insert2
                            if (!tileBitmapIsLoadedAlready)
                            {
                                gv.mod.loadedTileBitmapsNames.Add(tile.Layer2Filename);
                                tile.tileBitmap2 = gv.cc.LoadBitmap(tile.Layer2Filename);
                                gv.mod.loadedTileBitmaps.Add(tile.tileBitmap2);

                                IbRect srcLyr = getSourceIbRect(
                                x,
                                y,
                                UpperLeftSquare.X,
                                UpperLeftSquare.Y,
                                tile.tileBitmap2.PixelSize.Width,
                                tile.tileBitmap2.PixelSize.Height);
                                if (srcLyr != null)
                                {
                                    int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                                    int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                                    int tlX = ((x - UpperLeftSquare.X + shiftX) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                                    int tlY = (y - UpperLeftSquare.Y + shiftY) * gv.squareSize;
                                    float scalerX = srcLyr.Width / 100;
                                    float scalerY = srcLyr.Height / 100;
                                    int brX = (int)(gv.squareSize * scalerX);
                                    int brY = (int)(gv.squareSize * scalerY);
                                    IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                                    gv.DrawBitmap(tile.tileBitmap2, srcLyr, dstLyr);
                                }
                            }
                            else
                            {
                                IbRect srcLyr = getSourceIbRect(
                                x,
                                y,
                                UpperLeftSquare.X,
                                UpperLeftSquare.Y,
                                gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Width,
                                gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Height);
                                if (srcLyr != null)
                                {
                                    int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                                    int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                                    int tlX = ((x - UpperLeftSquare.X + shiftX) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                                    int tlY = (y - UpperLeftSquare.Y + shiftY) * gv.squareSize;
                                    float scalerX = srcLyr.Width / 100;
                                    float scalerY = srcLyr.Height / 100;
                                    int brX = (int)(gv.squareSize * scalerX);
                                    int brY = (int)(gv.squareSize * scalerY);
                                    IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                                    gv.DrawBitmap(gv.mod.loadedTileBitmaps[indexOfLoadedTile], srcLyr, dstLyr);
                                }

                            }
                        }
                        catch
                        { }
                    }
                }
                #endregion

                #region Draw Layer3
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        TileEnc tile = mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x];
                        try
                        {
                            //insert1                        
                            bool tileBitmapIsLoadedAlready = false;
                            int indexOfLoadedTile = -1;
                            for (int i = 0; i < gv.mod.loadedTileBitmapsNames.Count; i++)
                            {
                                if (gv.mod.loadedTileBitmapsNames[i] == tile.Layer3Filename)
                                {
                                    tileBitmapIsLoadedAlready = true;
                                    indexOfLoadedTile = i;
                                    break;
                                }
                            }

                            //insert2
                            if (!tileBitmapIsLoadedAlready)
                            {
                                gv.mod.loadedTileBitmapsNames.Add(tile.Layer3Filename);
                                tile.tileBitmap3 = gv.cc.LoadBitmap(tile.Layer3Filename);
                                gv.mod.loadedTileBitmaps.Add(tile.tileBitmap3);

                                IbRect srcLyr = getSourceIbRect(
                                x,
                                y,
                                UpperLeftSquare.X,
                                UpperLeftSquare.Y,
                                tile.tileBitmap3.PixelSize.Width,
                                tile.tileBitmap3.PixelSize.Height);
                                if (srcLyr != null)
                                {
                                    int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                                    int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                                    int tlX = ((x - UpperLeftSquare.X + shiftX) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                                    int tlY = (y - UpperLeftSquare.Y + shiftY) * gv.squareSize;
                                    float scalerX = srcLyr.Width / 100;
                                    float scalerY = srcLyr.Height / 100;
                                    int brX = (int)(gv.squareSize * scalerX);
                                    int brY = (int)(gv.squareSize * scalerY);
                                    IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                                    gv.DrawBitmap(tile.tileBitmap3, srcLyr, dstLyr);
                                }
                            }
                            else
                            {
                                IbRect srcLyr = getSourceIbRect(
                                x,
                                y,
                                UpperLeftSquare.X,
                                UpperLeftSquare.Y,
                                gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Width,
                                gv.mod.loadedTileBitmaps[indexOfLoadedTile].PixelSize.Height);
                                if (srcLyr != null)
                                {
                                    int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                                    int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                                    int tlX = ((x - UpperLeftSquare.X + shiftX) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                                    int tlY = (y - UpperLeftSquare.Y + shiftY) * gv.squareSize;
                                    float scalerX = srcLyr.Width / 100;
                                    float scalerY = srcLyr.Height / 100;
                                    int brX = (int)(gv.squareSize * scalerX);
                                    int brY = (int)(gv.squareSize * scalerY);
                                    IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                                    gv.DrawBitmap(gv.mod.loadedTileBitmaps[indexOfLoadedTile], srcLyr, dstLyr);
                                }

                            }
                        }
                        catch
                        { }
                    }
                }
                #endregion

                /*
                #region Draw Layer2
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        TileEnc tile = mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x];
                        IbRect srcLyr = getSourceIbRect(
                            x, 
                            y, 
                            UpperLeftSquare.X, 
                            UpperLeftSquare.Y, 
                            gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Width, 
                            gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Height);

                        if (srcLyr != null)
                        {
                            int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                            int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                            int tlX = ((x - UpperLeftSquare.X + shiftX) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                            int tlY = (y - UpperLeftSquare.Y + shiftY) * gv.squareSize;
                            float scalerX = srcLyr.Width / 100;
                            float scalerY = srcLyr.Height / 100;
                            int brX = (int)(gv.squareSize * scalerX);
                            int brY = (int)(gv.squareSize * scalerY);
                            IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                            gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer2Filename], srcLyr, dstLyr);
                        }
                    }
                }
                #endregion
                #region Draw Layer3
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        TileEnc tile = mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x];
                        IbRect srcLyr = getSourceIbRect(
                            x, 
                            y, 
                            UpperLeftSquare.X, 
                            UpperLeftSquare.Y, 
                            gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Width, 
                            gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Height);

                        if (srcLyr != null)
                        {
                            int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                            int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                            int tlX = ((x - UpperLeftSquare.X + shiftX) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                            int tlY = (y - UpperLeftSquare.Y + shiftY) * gv.squareSize;
                            float scalerX = srcLyr.Width / 100;
                            float scalerY = srcLyr.Height / 100;
                            int brX = (int)(gv.squareSize * scalerX);
                            int brY = (int)(gv.squareSize * scalerY);
                            IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                            gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer3Filename], srcLyr, dstLyr);
                        }
                    }
                }
                #endregion
                */

                #region Draw Grid
                //I brought the pix width and height of source back to normal
                if (mod.com_showGrid)
                {
                    for (int x = UpperLeftSquare.X; x < this.mod.currentEncounter.MapSizeX; x++)
                    {
                        for (int y = UpperLeftSquare.Y; y < this.mod.currentEncounter.MapSizeY; y++)
                        {
                            if (!IsInVisibleCombatWindow(x, y))
                            {
                                continue;
                            }

                            TileEnc tile = mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x];

                            int tlX = ((x - UpperLeftSquare.X) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
                            int tlY = (y - UpperLeftSquare.Y) * gv.squareSize;
                            int brX = gv.squareSize;
                            int brY = gv.squareSize;

                            IbRect srcGrid = new IbRect(0, 0, gv.squareSizeInPixels, gv.squareSizeInPixels);
                            IbRect dstGrid = new IbRect(tlX, tlY, brX, brY);
                            if (mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x].LoSBlocked)
                            {
                                gv.DrawBitmap(gv.cc.losBlocked, srcGrid, dstGrid);
                            }
                            if (mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x].Walkable != true)
                            {
                                gv.DrawBitmap(gv.cc.walkBlocked, srcGrid, dstGrid);
                            }
                            else
                            {
                                gv.DrawBitmap(gv.cc.walkPass, srcGrid, dstGrid);
                            }
                        }
                    }
                }
                #endregion
                #region Draw Pathfinding Numbers
                for (int x = UpperLeftSquare.X; x < this.mod.currentEncounter.MapSizeX; x++)
                {
                    for (int y = UpperLeftSquare.Y; y < this.mod.currentEncounter.MapSizeY; y++)
                    {
                        if (!IsInVisibleCombatWindow(x, y))
                        {
                            continue;
                        }
                        if ((pf.values != null) && (mod.debugMode))
                        {
                            //gv.DrawText(pf.values[x, y].ToString(), (x - UpperLeftSquare.X) * gv.squareSize + gv.oXshift + mapStartLocXinPixels, (y - UpperLeftSquare.Y) * gv.squareSize);
                        }
                    }
                }
                #endregion
            }
        }
        public IbRect getSourceIbRect(int xSqr, int ySqr, int UpperLeftXsqr, int UpperLeftYsqr, int tileWinPixels, int tileHinPixels)
        {
            IbRect src = new IbRect(0, 0, tileWinPixels, tileHinPixels);

            int tileWsqrs = tileWinPixels / gv.squareSizeInPixels;
            int tileHsqrs = tileHinPixels / gv.squareSizeInPixels;
            int BottomRightX = UpperLeftXsqr + gv.playerOffset + gv.playerOffset + 1;
            int BottomRightY = UpperLeftYsqr + gv.playerOffset + gv.playerOffset + 1;

            //left side
            int startX = UpperLeftXsqr - xSqr;
            if (startX < 0) { startX = 0; }
            //if startX >= tileW then it is off the map
            if (startX >= tileWsqrs) { return null; }

            //top side
            int startY = UpperLeftYsqr - ySqr;
            if (startY < 0) { startY = 0; }
            //if startY >= tileY then it is off the map
            if (startY >= tileHsqrs) { return null; }

            //right side
            int endX = BottomRightX - xSqr;
            if (endX > tileWsqrs) { endX = tileWsqrs; }
            //if endX <=0 then it is off the map
            if (endX <= 0) { return null; }

            //bottom side
            int endY = BottomRightY - ySqr;
            if (endY > tileHsqrs) { endY = tileHsqrs; }
            //if endY <=0 then it is off the map
            if (endY <= 0) { return null; }
            
            return new IbRect(startX * gv.squareSizeInPixels, startY * gv.squareSizeInPixels, (endX - startX) * gv.squareSizeInPixels, (endY - startY) * gv.squareSizeInPixels);            
        }
        public void drawCombatPlayers()
	    {
		    Player p = mod.playerList[currentPlayerIndex];
            if (IsInVisibleCombatWindow(p.combatLocX, p.combatLocY))
            {
                IbRect src = new IbRect(0, 0, gv.cc.turn_marker.PixelSize.Width, gv.cc.turn_marker.PixelSize.Width);
                IbRect dst = new IbRect(getPixelLocX(p.combatLocX), getPixelLocY(p.combatLocY), gv.squareSize, gv.squareSize);
                if (isPlayerTurn)
                {
                    gv.DrawBitmap(gv.cc.turn_marker, src, dst);
                }
            }
		    foreach (Player pc in mod.playerList)
		    {
                if (IsInVisibleCombatWindow(pc.combatLocX, pc.combatLocY))
                {
                    IbRect src = new IbRect(0, 0, pc.token.PixelSize.Width, pc.token.PixelSize.Width);
                    //check if drawing animation of player
                    if ((playerToAnimate != null) && (playerToAnimate == pc))
                    {
                        src = new IbRect(0, pc.token.PixelSize.Width, pc.token.PixelSize.Width, pc.token.PixelSize.Width);
                    }
                    IbRect dst = new IbRect(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY), gv.squareSize, gv.squareSize);
                    gv.DrawBitmap(pc.token, src, dst, !pc.combatFacingLeft, false);
                    src = new IbRect(0, 0, pc.token.PixelSize.Width, pc.token.PixelSize.Width);
                    foreach (Effect ef in pc.effectsList)
                    {
                        Bitmap fx = gv.cc.LoadBitmap(ef.spriteFilename);
                        src = new IbRect(0, 0, fx.PixelSize.Width, fx.PixelSize.Width);
                        gv.DrawBitmap(fx, src, dst);
                        gv.cc.DisposeOfBitmap(ref fx);
                    }
                    if ((pc.isDead()) || (pc.isUnconcious()))
                    {
                        src = new IbRect(0, 0, gv.cc.pc_dead.PixelSize.Width, gv.cc.pc_dead.PixelSize.Width);
                        gv.DrawBitmap(gv.cc.pc_dead, src, dst);
                    }
                    if (pc.steathModeOn)
                    {
                        src = new IbRect(0, 0, gv.cc.pc_stealth.PixelSize.Width, gv.cc.pc_stealth.PixelSize.Width);
                        gv.DrawBitmap(gv.cc.pc_stealth, src, dst);
                    }
                    //PLAYER FACING
                    src = new IbRect(0, 0, gv.cc.facing1.PixelSize.Width, gv.cc.facing1.PixelSize.Height);
                    if (pc.combatFacing == 8) { gv.DrawBitmap(gv.cc.facing8, src, dst); }
                    else if (pc.combatFacing == 9) { gv.DrawBitmap(gv.cc.facing9, src, dst); }
                    else if (pc.combatFacing == 6) { gv.DrawBitmap(gv.cc.facing6, src, dst); }
                    else if (pc.combatFacing == 3) { gv.DrawBitmap(gv.cc.facing3, src, dst); }
                    else if (pc.combatFacing == 2) { gv.DrawBitmap(gv.cc.facing2, src, dst); }
                    else if (pc.combatFacing == 1) { gv.DrawBitmap(gv.cc.facing1, src, dst); }
                    else if (pc.combatFacing == 4) { gv.DrawBitmap(gv.cc.facing4, src, dst); }
                    else if (pc.combatFacing == 7) { gv.DrawBitmap(gv.cc.facing7, src, dst); }
                    else { } //didn't find one


                    if (tglMoveOrder.toggleOn)
                    {
                        int mo = pc.moveOrder + 1;
                        drawText(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY) - (int)gv.drawFontRegHeight, mo.ToString(), Color.White);
                    }
                }
		    }
	    }
	    public void DrawHitAnimation()
	    {
		    if (drawHitAnimation)
		    {
                IbRect src = new IbRect(0, 0, gv.cc.hitSymbol.PixelSize.Width, gv.cc.hitSymbol.PixelSize.Height);
                IbRect dst = new IbRect(hitAnimationLocation.X, hitAnimationLocation.Y, gv.squareSize, gv.squareSize);
                gv.DrawBitmap(gv.cc.hitSymbol, src, dst);							
		    }
	    }
	    public void DrawMissAnimation()
	    {
		    if (drawMissAnimation)
		    {
                IbRect src = new IbRect(0, 0, gv.cc.missSymbol.PixelSize.Width, gv.cc.missSymbol.PixelSize.Height);
                IbRect dst = new IbRect(hitAnimationLocation.X, hitAnimationLocation.Y, gv.squareSize, gv.squareSize);
                gv.DrawBitmap(gv.cc.missSymbol, src, dst);
		    }
	    }
	    public void DrawProjectileAnimation()
	    {
		    if (drawProjectileAnimation)
		    {
                IbRect src = new IbRect(animationFrameIndex * projectile.PixelSize.Height, 0, projectile.PixelSize.Height, projectile.PixelSize.Height);
                IbRect dst = new IbRect(projectileAnimationLocation.X, projectileAnimationLocation.Y, gv.squareSize, gv.squareSize); 
			    gv.DrawBitmap(projectile, src, dst, false, !projectileFacingUp);		
		    }
	    }
        public void DrawEndingAnimation()
        {
            if ((drawEndingAnimation) && (ending_fx != null))
            {
                foreach (Coordinate coor in gv.sf.AoeSquaresList)
                {
                    if (!IsInVisibleCombatWindow(coor.X, coor.Y))
                    {
                        continue;
                    }
                    int height = ending_fx.PixelSize.Height;
                    IbRect src = new IbRect(animationFrameIndex * height, 0, height, height);
                    IbRect dst = new IbRect(getPixelLocX(coor.X), getPixelLocY(coor.Y), gv.squareSize, gv.squareSize);
                    gv.DrawBitmap(ending_fx, src, dst);
                }
            }
        }
        public void DrawDeathAnimation()
        {
            if ((drawDeathAnimation) && (gv.cc.death_fx != null))
            {
                foreach (Coordinate coor in deathAnimationLocations)
                {
                    if (!IsInVisibleCombatWindow(coor.X, coor.Y))
                    {
                        continue;
                    }
                    int height = gv.cc.death_fx.PixelSize.Height;
                    IbRect src = new IbRect(animationFrameIndex * height, 0, height, height);
                    IbRect dst = new IbRect(getPixelLocX(coor.X), getPixelLocY(coor.Y), gv.squareSize, gv.squareSize);
                    gv.DrawBitmap(gv.cc.death_fx, src, dst);
                }
            }
        }
        public void drawLosTrail()
	    {
		    Player p = mod.playerList[currentPlayerIndex];
		    if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
		    {
                //Uses the Screen Pixel Locations
			    int endX = getPixelLocX(targetHighlightCenterLocation.X) + (gv.squareSize / 2);
			    int endY = getPixelLocY(targetHighlightCenterLocation.Y) + (gv.squareSize / 2) + gv.oYshift;                
			    int startX = getPixelLocX(p.combatLocX) + (gv.squareSize / 2);
			    int startY = getPixelLocY(p.combatLocY) + (gv.squareSize / 2) + gv.oYshift;
                //Uses the Map Pixel Locations
                int endX2 = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                int endY2 = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);
                int startX2 = p.combatLocX * gv.squareSize + (gv.squareSize / 2);
                int startY2 = p.combatLocY * gv.squareSize + (gv.squareSize / 2);
			    
                //check if target is within attack distance, use green if true, red if false
			    if (isVisibleLineOfSight(new Coordinate(endX2,endY2), new Coordinate(startX2,startY2))) 
                {
                    drawVisibleLineOfSightTrail(new Coordinate(endX,endY), new Coordinate(startX,startY), Color.Lime, 2);                    
                }
			    else 
                {
                    drawVisibleLineOfSightTrail(new Coordinate(endX, endY), new Coordinate(startX, startY), Color.Red, 2); 
                }
		    }
	    }
	    public void drawCombatCreatures()
	    {
		    if (mod.currentEncounter.encounterCreatureList.Count > 0)
		    {
                if (!isPlayerTurn)
                {
                    Creature cr = mod.currentEncounter.encounterCreatureList[creatureIndex];
                    if (IsInVisibleCombatWindow(cr.combatLocX, cr.combatLocY))
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.turn_marker.PixelSize.Width, gv.cc.turn_marker.PixelSize.Height);
                        IbRect dst = new IbRect(getPixelLocX(cr.combatLocX), getPixelLocY(cr.combatLocY), gv.squareSize, gv.squareSize);
                        gv.DrawBitmap(gv.cc.turn_marker, src, dst);
                    }
                }
		    }
		    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
		    {
                if (!IsInVisibleCombatWindow(crt.combatLocX, crt.combatLocY))
                {
                    continue;
                }
			    IbRect src = new IbRect(0, 0, crt.token.PixelSize.Width, crt.token.PixelSize.Width);
			    if ((creatureToAnimate != null) && (creatureToAnimate == crt))
			    {
                    src = new IbRect(0, crt.token.PixelSize.Width, crt.token.PixelSize.Width, crt.token.PixelSize.Width);
			    }
                IbRect dst = new IbRect(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), gv.squareSize, gv.squareSize);
                if (crt.token.PixelSize.Width > 100)
			    {
                    dst = new IbRect(getPixelLocX(crt.combatLocX) - (gv.squareSize / 2), getPixelLocY(crt.combatLocY) - (gv.squareSize / 2), gv.squareSize * 2, gv.squareSize * 2);
			    }
			    gv.DrawBitmap(crt.token, src, dst, !crt.combatFacingLeft, false);
			    foreach (Effect ef in crt.cr_effectsList)
			    {
				    Bitmap fx = gv.cc.LoadBitmap(ef.spriteFilename);
                    src = new IbRect(0, 0, fx.PixelSize.Width, fx.PixelSize.Width);
				    gv.DrawBitmap(fx, src, dst);
                    gv.cc.DisposeOfBitmap(ref fx);
                }
                //CREATURE FACING
                src = new IbRect(0, 0, gv.cc.facing1.PixelSize.Width, gv.cc.facing1.PixelSize.Height);
                if (crt.combatFacing == 8) { gv.DrawBitmap(gv.cc.facing8, src, dst); }
                else if (crt.combatFacing == 9) { gv.DrawBitmap(gv.cc.facing9, src, dst); }
                else if (crt.combatFacing == 6) { gv.DrawBitmap(gv.cc.facing6, src, dst); }
                else if (crt.combatFacing == 3) { gv.DrawBitmap(gv.cc.facing3, src, dst); }
                else if (crt.combatFacing == 2) { gv.DrawBitmap(gv.cc.facing2, src, dst); }
                else if (crt.combatFacing == 1) { gv.DrawBitmap(gv.cc.facing1, src, dst); }
                else if (crt.combatFacing == 4) { gv.DrawBitmap(gv.cc.facing4, src, dst); }
                else if (crt.combatFacing == 7) { gv.DrawBitmap(gv.cc.facing7, src, dst); }
                else { } //didn't find one

                if (tglMoveOrder.toggleOn)
                {
                    int mo = crt.moveOrder + 1;
                    drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY) - (int)gv.drawFontRegHeight, mo.ToString(), Color.White);
                }
		    }
	    }
        public void drawMovingCombatCreatures()
        {
            if (mod.currentEncounter.encounterCreatureList.Count > 0)
            {
                if (!isPlayerTurn)
                {
                    Creature cr = mod.currentEncounter.encounterCreatureList[creatureIndex];
                    if (IsInVisibleCombatWindow(cr.combatLocX, cr.combatLocY))
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.turn_marker.PixelSize.Width, gv.cc.turn_marker.PixelSize.Height);
                        IbRect dst = new IbRect(getPixelLocX(cr.combatLocX), getPixelLocY(cr.combatLocY), gv.squareSize, gv.squareSize);
                        gv.DrawBitmap(gv.cc.turn_marker, src, dst);
                    }
                }
            }
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if (!IsInVisibleCombatWindow(crt.combatLocX, crt.combatLocY))
                {
                    continue;
                }
                IbRect src = new IbRect(0, 0, crt.token.PixelSize.Width, crt.token.PixelSize.Width);
                if ((creatureToAnimate != null) && (creatureToAnimate == crt))
                {
                    src = new IbRect(0, crt.token.PixelSize.Width, crt.token.PixelSize.Width, crt.token.PixelSize.Width);
                }
                IbRect dst = new IbRect(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), gv.squareSize, gv.squareSize);
                if (crt.token.PixelSize.Width > 100)
                {
                    dst = new IbRect(getPixelLocX(crt.combatLocX) - (gv.squareSize / 2), getPixelLocY(crt.combatLocY) - (gv.squareSize / 2), gv.squareSize * 2, gv.squareSize * 2);
                }
                gv.DrawBitmap(crt.token, src, dst, !crt.combatFacingLeft, false);
                foreach (Effect ef in crt.cr_effectsList)
                {
                    Bitmap fx = gv.cc.LoadBitmap(ef.spriteFilename);
                    src = new IbRect(0, 0, fx.PixelSize.Width, fx.PixelSize.Width);
                    gv.DrawBitmap(fx, src, dst);
                    gv.cc.DisposeOfBitmap(ref fx);
                }
                //CREATURE FACING
                src = new IbRect(0, 0, gv.cc.facing1.PixelSize.Width, gv.cc.facing1.PixelSize.Height);
                if (crt.combatFacing == 8) { gv.DrawBitmap(gv.cc.facing8, src, dst); }
                else if (crt.combatFacing == 9) { gv.DrawBitmap(gv.cc.facing9, src, dst); }
                else if (crt.combatFacing == 6) { gv.DrawBitmap(gv.cc.facing6, src, dst); }
                else if (crt.combatFacing == 3) { gv.DrawBitmap(gv.cc.facing3, src, dst); }
                else if (crt.combatFacing == 2) { gv.DrawBitmap(gv.cc.facing2, src, dst); }
                else if (crt.combatFacing == 1) { gv.DrawBitmap(gv.cc.facing1, src, dst); }
                else if (crt.combatFacing == 4) { gv.DrawBitmap(gv.cc.facing4, src, dst); }
                else if (crt.combatFacing == 7) { gv.DrawBitmap(gv.cc.facing7, src, dst); }
                else { } //didn't find one

                if (tglMoveOrder.toggleOn)
                {
                    int mo = crt.moveOrder + 1;
                    drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY) - (int)gv.drawFontRegHeight, mo.ToString(), Color.White);
                }
            }
        }
	    public void drawTargetHighlight()
	    {
		    Player pc = mod.playerList[currentPlayerIndex];
		    if (currentCombatMode.Equals("attack"))
		    {
                /*int x = getPixelLocX(targetHighlightCenterLocation.X);
                int y = getPixelLocY(targetHighlightCenterLocation.Y);
                IbRect src = new IbRect(0, 0, gv.cc.highlight_green.PixelSize.Width, gv.cc.highlight_green.PixelSize.Height);
                IbRect dst = new IbRect(x, y, gv.squareSize, gv.squareSize);
                if (isValidAttackTarget(pc))
                {
                    gv.DrawBitmap(gv.cc.highlight_green, src, dst);
                }
                else
                {
                    gv.DrawBitmap(gv.cc.highlight_red, src, dst);
                }*/
                Item it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                //if using ranged and have ammo, use ammo properties
                if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                        && (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                {
                    //ranged weapon with ammo
                    it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                }
                if (it == null)
                {
                    it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                }
                //set squares list
                gv.sf.CreateAoeSquaresList(pc, targetHighlightCenterLocation, it.aoeShape, it.AreaOfEffect);
                foreach (Coordinate coor in gv.sf.AoeSquaresList)
                {
                    if (!IsInVisibleCombatWindow(coor.X, coor.Y))
                    {
                        continue;
                    }
                    //Color colr = Color.Lime;
                    bool hl_green = true;
                    int endX2 = coor.X * gv.squareSize + (gv.squareSize / 2);
                    int endY2 = coor.Y * gv.squareSize + (gv.squareSize / 2);
                    int startX2 = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                    int startY2 = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);

                    if (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX2, startY2)))
                    {
                        //colr = Color.Lime;
                        hl_green = true;
                    }
                    else
                    {
                        //colr = Color.Red;
                        hl_green = false;
                    }
                    if ((coor.X == targetHighlightCenterLocation.X) && (coor.Y == targetHighlightCenterLocation.Y))
                    {
                        int startX3 = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                        int startY3 = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
                        if ((isValidAttackTarget(pc)) && (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX3, startY3))))
                        {
                            //colr = Color.Lime;
                            hl_green = true;
                        }
                        else
                        {
                            //colr = Color.Red;
                            hl_green = false;
                        }
                    }

                    //int cornerRadius = gv.squareSize / 5;
                    //int penWidth = 3;                    
                    int x = getPixelLocX(coor.X);
                    int y = getPixelLocY(coor.Y);
                    //gv.DrawRoundRectangle(new IbRect(x, y, gv.squareSize, gv.squareSize), cornerRadius, colr, penWidth);
                    IbRect src = new IbRect(0, 0, gv.cc.highlight_green.PixelSize.Width, gv.cc.highlight_green.PixelSize.Height);
                    IbRect dst = new IbRect(x, y, gv.squareSize, gv.squareSize);
                    if (hl_green)
                    {
                        gv.DrawBitmap(gv.cc.highlight_green, src, dst);
                    }
                    else
                    {
                        gv.DrawBitmap(gv.cc.highlight_red, src, dst);
                    }
                }
            }
		    else if (currentCombatMode.Equals("cast"))
		    {
                //set squares list
                gv.sf.CreateAoeSquaresList(pc, targetHighlightCenterLocation, gv.cc.currentSelectedSpell.aoeShape, gv.cc.currentSelectedSpell.aoeRadius);
                foreach (Coordinate coor in gv.sf.AoeSquaresList)
                {
                    if (!IsInVisibleCombatWindow(coor.X,coor.Y))
                    {
                        continue;
                    }
                    //Color colr = Color.Lime;
                    bool hl_green = true;
                    int endX2 = coor.X * gv.squareSize + (gv.squareSize / 2);
                    int endY2 = coor.Y * gv.squareSize + (gv.squareSize / 2);
                    int startX2 = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                    int startY2 = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);

                    if ((isValidCastTarget(pc)) && (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX2, startY2))))
                    {
                        //colr = Color.Lime;
                        hl_green = true;
                    }
                    else
                    {
                        //colr = Color.Red;
                        hl_green = false;
                    }
                    if ((coor.X == targetHighlightCenterLocation.X) && (coor.Y == targetHighlightCenterLocation.Y))
                    {
                        int startX3 = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                        int startY3 = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
                        if ((isValidCastTarget(pc)) && (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX3, startY3))))
                        {
                            //colr = Color.Lime;
                            hl_green = true;
                        }
                        else
                        {
                            //colr = Color.Red;
                            hl_green = false;
                        }
                    }

                    //int cornerRadius = gv.squareSize / 5;
                    //int penWidth = 3;                    
                    int x = getPixelLocX(coor.X);
                    int y = getPixelLocY(coor.Y);
                    //gv.DrawRoundRectangle(new IbRect(x, y, gv.squareSize, gv.squareSize), cornerRadius, colr, penWidth);
                    IbRect src = new IbRect(0, 0, gv.cc.highlight_green.PixelSize.Width, gv.cc.highlight_green.PixelSize.Height);
                    IbRect dst = new IbRect(x, y, gv.squareSize, gv.squareSize);
                    if (hl_green)
                    {
                        gv.DrawBitmap(gv.cc.highlight_green, src, dst);
                    }
                    else
                    {
                        gv.DrawBitmap(gv.cc.highlight_red, src, dst);
                    }
                }                
		    }
        }	            
        public void drawFloatyText()
	    {            
		    int txtH = (int)gv.drawFontRegHeight;
		
		    for (int x = -2; x <= 2; x++)
		    {
			    for (int y = -2; y <= 2; y++)
			    {
                    gv.DrawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X + x, gv.cc.floatyTextLoc.Y + txtH + y, 1.0f, Color.Black);
                    gv.DrawText(gv.cc.floatyText2, gv.cc.floatyTextLoc.X + x, gv.cc.floatyTextLoc.Y + (txtH * 2) + y, 1.0f, Color.Black);
                    gv.DrawText(gv.cc.floatyText3, gv.cc.floatyTextLoc.X + x, gv.cc.floatyTextLoc.Y + (txtH * 3) + y, 1.0f, Color.Black);	
			    }
		    }		
		    gv.DrawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X, gv.cc.floatyTextLoc.Y + txtH, 1.0f, Color.Yellow);
            gv.DrawText(gv.cc.floatyText2, gv.cc.floatyTextLoc.X, gv.cc.floatyTextLoc.Y + txtH * 2, 1.0f, Color.Yellow);
            gv.DrawText(gv.cc.floatyText3, gv.cc.floatyTextLoc.X, gv.cc.floatyTextLoc.Y + txtH * 3, 1.0f, Color.Yellow);            
	    }
	    public void drawHPText()
	    {		
		    if (tglHP.toggleOn)
		    {
			    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
			    {
                    if (IsInVisibleCombatWindow(crt.combatLocX, crt.combatLocY))
                    {
                        drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), crt.hp + "/" + crt.hpMax, Color.Red);
                    }
			    }
			    foreach (Player pc in mod.playerList)
			    {
                    if (IsInVisibleCombatWindow(pc.combatLocX, pc.combatLocY))
                    {
                        drawText(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY), pc.hp + "/" + pc.hpMax, Color.Red);
                    }
			    }
		    }
	    }
	    public void drawSPText()
	    {		
		    if (tglSP.toggleOn)
		    {
			    int txtH = (int)gv.drawFontRegHeight;
			    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
			    {
                    if (IsInVisibleCombatWindow(crt.combatLocX, crt.combatLocY))
                    {
                        drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY) + txtH, "sp: " + crt.sp, Color.Yellow);
                    }
			    }
			    foreach (Player pc in mod.playerList)
			    {
                    if (IsInVisibleCombatWindow(pc.combatLocX, pc.combatLocY))
                    {
                        drawText(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY) + txtH, pc.sp + "/" + pc.spMax, Color.Yellow);
                    }
			    }
		    }
	    }
	    public void drawText(int xLoc, int yLoc, string text, Color colr)
	    {
		    int txtH = (int)gv.drawFontRegHeight;

		    for (int x = -2; x <= 2; x++)
		    {
			    for (int y = -2; y <= 2; y++)
			    {
                    gv.DrawText(text, xLoc + x, yLoc + txtH + y, 1.0f, Color.Black);
			    }
		    }		
		    gv.DrawText(text, xLoc, yLoc + txtH, 1.0f, colr);
	    }
	    public void drawFloatyTextList()
	    {
		    if (floatyTextOn)
		    {
                int txtH = (int)gv.drawFontRegHeight;
					
			    foreach (FloatyText ft in gv.cc.floatyTextList)
			    {
				    for (int x = -2; x <= 2; x++)
				    {
					    for (int y = -2; y <= 2; y++)
					    {
                            gv.DrawText(ft.value, ft.location.X - (UpperLeftSquare.X * gv.squareSize) + x + mapStartLocXinPixels, ft.location.Y - (UpperLeftSquare.Y * gv.squareSize) + y, 1.0f, Color.Black);
					    }
				    }
                    Color colr = Color.Yellow;
				    if (ft.color.Equals("yellow"))
				    {				
					    colr = Color.Yellow;
				    }
				    else if (ft.color.Equals("blue"))
				    {
                        colr = Color.Blue;
				    }
				    else if (ft.color.Equals("green"))
				    {
                        colr = Color.Lime;
				    }
				    else
				    {
                        colr = Color.Red;
				    }
                    gv.DrawText(ft.value, ft.location.X - (UpperLeftSquare.X * gv.squareSize) + mapStartLocXinPixels, ft.location.Y - (UpperLeftSquare.Y * gv.squareSize), 1.0f, colr);
			    }
		    }
	    }
        public void drawOverlayTints()
        {
            IbRect src = new IbRect(0, 0, gv.cc.tint_sunset.PixelSize.Width, gv.cc.tint_sunset.PixelSize.Height);
            IbRect dst = new IbRect(gv.oXshift + mapStartLocXinPixels, 0, gv.squareSize * (gv.playerOffset + gv.playerOffset + 1), gv.squareSize * (gv.playerOffset + gv.playerOffset + 1));
            int dawn = 5 * 60;
            int sunrise = 6 * 60;
            int day = 7 * 60;
            int sunset = 17 * 60;
            int dusk = 18 * 60;
            int night = 20 * 60;
            int time = gv.mod.WorldTime % 1440;
            if ((time >= dawn) && (time < sunrise))
            {
                gv.DrawBitmap(gv.cc.tint_dawn, src, dst);
            }
            else if ((time >= sunrise) && (time < day))
            {
                gv.DrawBitmap(gv.cc.tint_sunrise, src, dst);
            }
            else if ((time >= day) && (time < sunset))
            {
                //no tint for day
            }
            else if ((time >= sunset) && (time < dusk))
            {
                gv.DrawBitmap(gv.cc.tint_sunset, src, dst);
            }
            else if ((time >= dusk) && (time < night))
            {
                gv.DrawBitmap(gv.cc.tint_dusk, src, dst);
            }
            else if ((time >= night) || (time < dawn))
            {
                gv.DrawBitmap(gv.cc.tint_night, src, dst);
            }

        }
        public void drawSprites()
        {
            foreach (Sprite spr in spriteList)
            {
                spr.Draw(gv);
            }
        }
        #endregion

        #region Keyboard Input
        public void onKeyUp(Keys keyData)
        {
            if (keyData == Keys.M)
            {
                if (canMove)
                {
                    currentCombatMode = "move";
                    gv.screenType = "combat";
                }
            }
            else if (keyData == Keys.A)
            {
                Player pc = mod.playerList[currentPlayerIndex];
                currentCombatMode = "attack";
                gv.screenType = "combat";
                setTargetHighlightStartLocation(pc);
            }
            else if (keyData == Keys.P)
            {
                if (currentPlayerIndex > mod.playerList.Count - 1)
                {
                    return;
                }
                gv.cc.partyScreenPcIndex = currentPlayerIndex;
                gv.screenParty.resetPartyScreen();
                gv.screenType = "combatParty";
            }
            else if (keyData == Keys.I)
            {
                gv.screenType = "combatInventory";
                gv.screenInventory.resetInventory();
            }
            else if (keyData == Keys.S)
            {
                gv.screenType = "combat";
                endPcTurn(false);
            }
            else if (keyData == Keys.C)
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if (pc.knownSpellsTags.Count > 0)
                {
                    currentCombatMode = "castSelector";
                    gv.screenType = "combatCast";
                    gv.screenCastSelector.castingPlayerIndex = currentPlayerIndex;
                    spellSelectorIndex = 0;
                    setTargetHighlightStartLocation(pc);
                }
                else
                {
                    //TODO Toast.makeText(gv.gameContext, "PC has no Spells", Toast.LENGTH_SHORT).show();
                }
            }

            #region Move Map
            if (keyData == Keys.Up)
            {
                if (UpperLeftSquare.Y > 0)
                {
                    UpperLeftSquare.Y--;
                }
                return;
            }
            else if (keyData == Keys.Left)
            {
                if (UpperLeftSquare.X > 0)
                {
                    UpperLeftSquare.X--;
                }
                return;
            }
            else if (keyData == Keys.Down)
            {
                if (UpperLeftSquare.Y < mod.currentEncounter.MapSizeY - gv.playerOffset - gv.playerOffset - 1)
                {
                    UpperLeftSquare.Y++;
                }
                return;
            }
            else if (keyData == Keys.Right)
            {
                if (UpperLeftSquare.X < mod.currentEncounter.MapSizeX - gv.playerOffset - gv.playerOffset - 1)
                {
                    UpperLeftSquare.X++;
                }
                return;
            }
            #endregion
            #region Move PC mode
            if (currentCombatMode.Equals("move"))
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if (keyData == Keys.NumPad7) 
                {
                    MoveUpLeft(pc);
                }
                else if (keyData == Keys.NumPad8)
                {
                    MoveUp(pc);
                }
                else if (keyData == Keys.NumPad9)
                {
                    MoveUpRight(pc);
                }
                else if (keyData == Keys.NumPad4)
                {
                    MoveLeft(pc);
                }
                else if (keyData == Keys.NumPad5)
                {
                    CenterScreenOnPC();
                }
                else if (keyData == Keys.NumPad6)
                {
                    MoveRight(pc);
                }
                else if (keyData == Keys.NumPad1)
                {
                    MoveDownLeft(pc);
                }
                else if (keyData == Keys.NumPad2)
                {
                    MoveDown(pc);
                }
                else if (keyData == Keys.NumPad3)
                {
                    MoveDownRight(pc);
                }
                return;
            }
            #endregion
            #region Move Targeting Mode
            if (currentCombatMode.Equals("attack"))
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if (keyData == Keys.NumPad5)
                {
                    TargetAttackPressed(pc);
                    return;
                }                
            }
            if (currentCombatMode.Equals("cast"))
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if (keyData == Keys.NumPad5)
                {
                    TargetCastPressed(pc);
                    return;
                }                
            }
            if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
            {
                if (keyData == Keys.NumPad7)
                {
                    MoveTargetHighlight(7);
                }
                else if (keyData == Keys.NumPad8)
                {
                    MoveTargetHighlight(8);
                }
                else if (keyData == Keys.NumPad9)
                {
                    MoveTargetHighlight(9);
                }
                else if (keyData == Keys.NumPad4)
                {
                    MoveTargetHighlight(4);
                }
                else if (keyData == Keys.NumPad6)
                {
                    MoveTargetHighlight(6);
                }
                else if (keyData == Keys.NumPad1)
                {
                    MoveTargetHighlight(1);
                }
                else if (keyData == Keys.NumPad2)
                {
                    MoveTargetHighlight(2);
                }
                else if (keyData == Keys.NumPad3)
                {
                    MoveTargetHighlight(3);
                }
                return;
            }
            #endregion
        }
        #endregion

        #region Mouse Input
        public void onTouchCombat(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                    int x = (int)e.X;
                    int y = (int)e.Y;

                    int gridx = (int)(e.X - gv.oXshift - mapStartLocXinPixels) / gv.squareSize;
                    int gridy = (int)(e.Y - (gv.squareSize / 2)) / gv.squareSize;

                    #region FloatyText
                    gv.cc.floatyText = "";
                    gv.cc.floatyText2 = "";
                    gv.cc.floatyText3 = "";
                    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                    {
                        if ((crt.combatLocX == gridx + UpperLeftSquare.X) && (crt.combatLocY == gridy + UpperLeftSquare.Y))
                        {
                            gv.cc.floatyText = crt.cr_name;
                            gv.cc.floatyText2 = "HP:" + crt.hp + " SP:" + crt.sp;
                            gv.cc.floatyText3 = "AC:" + crt.AC + " " + crt.cr_status;
                            gv.cc.floatyTextLoc = new Coordinate(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY));
                        }
                    }
                    foreach (Player pc in mod.playerList)
                    {
                        if ((pc.combatLocX == gridx + UpperLeftSquare.X) && (pc.combatLocY == gridy + UpperLeftSquare.Y))
                        {
                            string am = "";
                            ItemRefs itr = mod.getItemRefsInInventoryByResRef(pc.AmmoRefs.resref);
                            if (itr != null)
                            {
                                am = itr.quantity + "";
                            }
                            else
                            {
                                am = "";
                            }

                            gv.cc.floatyText = pc.name;
                            int actext = 0;
                            if (mod.ArmorClassAscending) { actext = pc.AC; }
                            else { actext = 20 - pc.AC; }
                            gv.cc.floatyText2 = "AC:" + actext + " " + pc.charStatus;
                            gv.cc.floatyText3 = "Ammo: " + am;
                            gv.cc.floatyTextLoc = new Coordinate(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY));

                        }
                    }
                    #endregion
                    #region Toggles
                    if (tglHP.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        if (tglHP.toggleOn)
                        {
                            tglHP.toggleOn = false;
                        }
                        else
                        {
                            tglHP.toggleOn = true;
                        }
                    }
                    if (tglSP.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        if (tglSP.toggleOn)
                        {
                            tglSP.toggleOn = false;
                        }
                        else
                        {
                            tglSP.toggleOn = true;
                        }
                    }
                    if (tglMoveOrder.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        if (tglMoveOrder.toggleOn)
                        {
                            tglMoveOrder.toggleOn = false;
                        }
                        else
                        {
                            tglMoveOrder.toggleOn = true;
                        }
                    }
                    if (tglSpeed.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        
                        if (mod.combatAnimationSpeed == 100)
                        {
                            mod.combatAnimationSpeed = 50;
                            gv.cc.addLogText("lime", "combat speed: 2x");
                            gv.cc.DisposeOfBitmap(ref tglSpeed.ImgOff);
                            tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_2");
                        }
                        else if (mod.combatAnimationSpeed == 50)
                        {
                            mod.combatAnimationSpeed = 25;
                            gv.cc.addLogText("lime", "combat speed: 4x");
                            gv.cc.DisposeOfBitmap(ref tglSpeed.ImgOff);
                            tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_4");
                        }
                        else if (mod.combatAnimationSpeed == 25)
                        {
                            mod.combatAnimationSpeed = 10;
                            gv.cc.addLogText("lime", "combat speed: 10x");
                            gv.cc.DisposeOfBitmap(ref tglSpeed.ImgOff);
                            tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_10");
                        }
                        else if (mod.combatAnimationSpeed == 10)
                        {
                            mod.combatAnimationSpeed = 100;
                            gv.cc.addLogText("lime", "combat speed: 1x");
                            gv.cc.DisposeOfBitmap(ref tglSpeed.ImgOff);
                            tglSpeed.ImgOff = gv.cc.LoadBitmap("tgl_speed_1");
                        }
                    }
                    if (gv.cc.tglSound.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        if (gv.cc.tglSound.toggleOn)
                        {
                            gv.cc.tglSound.toggleOn = false;
                            mod.playMusic = false;
                            gv.stopCombatMusic();
                            //addLogText("lime","Music Off");
                        }
                        else
                        {
                            gv.cc.tglSound.toggleOn = true;
                            mod.playMusic = true;
                            gv.startCombatMusic();
                            //addLogText("lime","Music On");
                        }
                    }
                    if (tglSoundFx.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        if (tglSoundFx.toggleOn)
                        {
                            tglSoundFx.toggleOn = false;
                            mod.playSoundFx = false;
                            //gv.stopCombatMusic();
                            //addLogText("lime","Music Off");
                        }
                        else
                        {
                            tglSoundFx.toggleOn = true;
                            mod.playSoundFx = true;
                            //gv.startCombatMusic();
                            //addLogText("lime","Music On");
                        }
                    }
                    if (tglGrid.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        if (tglGrid.toggleOn)
                        {
                            tglGrid.toggleOn = false;
                            mod.com_showGrid = false;
                        }
                        else
                        {
                            tglGrid.toggleOn = true;
                            mod.com_showGrid = true;
                        }
                    }
                    if (tglHelp.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        tutorialMessageCombat(true);
                    }
                    if ((tglKill.getImpact(x, y)) && (mod.debugMode))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        mod.currentEncounter.encounterCreatureList.Clear();
                        mod.currentEncounter.encounterCreatureRefsList.Clear();
                        checkEndEncounter();
                    }
                    #endregion

                    if (btnSwitchWeapon.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}

                        if (currentPlayerIndex > mod.playerList.Count - 1)
                        {
                            return;
                        }
                        gv.cc.partyScreenPcIndex = currentPlayerIndex;
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "combatParty";
                    }
                    break;
            }
            //if MoveMode(move), AttackMode(attack), CastMode(cast), or InfoMode(info)
            if (currentCombatMode.Equals("info"))
            {
                onTouchCombatInfo(e, eventType);
            }
            else if (currentCombatMode.Equals("move"))
            {
                onTouchCombatMove(e, eventType);
            }
            else if (currentCombatMode.Equals("attack"))
            {
                onTouchCombatAttack(e, eventType);
            }
            else if (currentCombatMode.Equals("castSelector"))
            {
                //onTouchCombatCastSelector(event);
            }
            else if (currentCombatMode.Equals("cast"))
            {
                onTouchCombatCast(e, eventType);
            }
            else
            {
                //info mode
            }
        }
        public void onTouchCombatInfo(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            //TODOgv.cc.onTouchLog();
            Player pc = mod.playerList[currentPlayerIndex];

            btnMove.glowOn = false;
            gv.cc.btnInventory.glowOn = false;
            btnAttack.glowOn = false;
            btnCast.glowOn = false;
            btnSkipTurn.glowOn = false;
            //btnKill.glowOn = false;
            //btnCombatHelp.glowOn = false;

            //int eventAction = event.getAction();
            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                case MouseEventType.EventType.MouseMove:
                    int x = (int)e.X;
                    int y = (int)e.Y;
                    if (btnMove.getImpact(x, y))
                    {
                        btnMove.glowOn = true;
                    }
                    else if (gv.cc.btnInventory.getImpact(x, y))
                    {
                        gv.cc.btnInventory.glowOn = true;
                    }
                    else if (btnAttack.getImpact(x, y))
                    {
                        btnAttack.glowOn = true;
                    }
                    else if (btnCast.getImpact(x, y))
                    {
                        btnCast.glowOn = true;
                    }
                    else if (btnSkipTurn.getImpact(x, y))
                    {
                        btnSkipTurn.glowOn = true;
                    }
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)e.X;
                    y = (int)e.Y;

                    btnMove.glowOn = false;
                    gv.cc.btnInventory.glowOn = false;
                    btnAttack.glowOn = false;
                    btnCast.glowOn = false;
                    btnSkipTurn.glowOn = false;

                    //BUTTONS			
                    if (btnMove.getImpact(x, y))
                    {
                        if (canMove)
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            currentCombatMode = "move";
                            gv.screenType = "combat";
                        }
                    }
                    else if (gv.cc.btnInventory.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        gv.screenType = "combatInventory";
                        gv.screenInventory.resetInventory();
                    }
                    else if (btnAttack.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        currentCombatMode = "attack";
                        gv.screenType = "combat";
                        setTargetHighlightStartLocation(pc);
                    }
                    else if (btnCast.getImpact(x, y))
                    {
                        if (pc.knownSpellsTags.Count > 0)
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            currentCombatMode = "castSelector";
                            gv.screenType = "combatCast";
                            gv.screenCastSelector.castingPlayerIndex = currentPlayerIndex;
                            spellSelectorIndex = 0;
                            setTargetHighlightStartLocation(pc);
                        }
                        else
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            //TODO Toast.makeText(gv.gameContext, "PC has no Spells", Toast.LENGTH_SHORT).show();
                        }
                    }
                    else if (btnSkipTurn.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        gv.screenType = "combat";
                        endPcTurn(false);
                    }
                    break;
            }
        }
        public void onTouchCombatMove(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            //gv.cc.onTouchLog();
            Player pc = mod.playerList[currentPlayerIndex];

            gv.cc.ctrlUpArrow.glowOn = false;
            gv.cc.ctrlDownArrow.glowOn = false;
            gv.cc.ctrlLeftArrow.glowOn = false;
            gv.cc.ctrlRightArrow.glowOn = false;
            gv.cc.ctrlUpRightArrow.glowOn = false;
            gv.cc.ctrlDownRightArrow.glowOn = false;
            gv.cc.ctrlUpLeftArrow.glowOn = false;
            gv.cc.ctrlDownLeftArrow.glowOn = false;
            btnMove.glowOn = false;
            gv.cc.btnInventory.glowOn = false;
            btnAttack.glowOn = false;
            btnCast.glowOn = false;
            btnSkipTurn.glowOn = false;

            //int eventAction = event.getAction();
            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                case MouseEventType.EventType.MouseMove:
                    int x = (int)e.X;
                    int y = (int)e.Y;
                    if (gv.cc.ctrlUpArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlLeftArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlUpRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlUpLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpLeftArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownLeftArrow.glowOn = true;
                    }
                    else if (btnMove.getImpact(x, y))
                    {
                        btnMove.glowOn = true;
                    }
                    else if (gv.cc.btnInventory.getImpact(x, y))
                    {
                        gv.cc.btnInventory.glowOn = true;
                    }
                    else if (btnAttack.getImpact(x, y))
                    {
                        btnAttack.glowOn = true;
                    }
                    else if (btnCast.getImpact(x, y))
                    {
                        btnCast.glowOn = true;
                    }
                    else if (btnSkipTurn.getImpact(x, y))
                    {
                        btnSkipTurn.glowOn = true;
                    }
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)e.X;
                    y = (int)e.Y;

                    gv.cc.ctrlUpArrow.glowOn = false;
                    gv.cc.ctrlDownArrow.glowOn = false;
                    gv.cc.ctrlLeftArrow.glowOn = false;
                    gv.cc.ctrlRightArrow.glowOn = false;
                    gv.cc.ctrlUpRightArrow.glowOn = false;
                    gv.cc.ctrlDownRightArrow.glowOn = false;
                    gv.cc.ctrlUpLeftArrow.glowOn = false;
                    gv.cc.ctrlDownLeftArrow.glowOn = false;
                    btnMove.glowOn = false;
                    gv.cc.btnInventory.glowOn = false;
                    btnAttack.glowOn = false;
                    btnCast.glowOn = false;
                    btnSkipTurn.glowOn = false;

                    //TOUCH ON MAP AREA
                    int gridx = (int)(e.X - gv.oXshift - mapStartLocXinPixels) / gv.squareSize;
                    int gridy = (int)(e.Y - (gv.squareSize / 2)) / gv.squareSize;
                    //int gridx = (int)e.X / gv.squareSize - 4;
                    //int gridy = (int)(e.Y - (gv.squareSize / 2)) / gv.squareSize;

                    if (gridy < mod.currentEncounter.MapSizeY)
                    {
                        gv.cc.floatyText = "";
                        gv.cc.floatyText2 = "";
                        gv.cc.floatyText3 = "";
                        //Check for second tap so TARGET
                    }

                    //BUTTONS
                    if ((gv.cc.ctrlUpArrow.getImpact(x, y)) || ((gridx + UpperLeftSquare.X == pc.combatLocX) && (gridy + UpperLeftSquare.Y == pc.combatLocY - 1)))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveUp(pc);
                    }
                    else if ((gv.cc.ctrlDownArrow.getImpact(x, y)) || ((gridx + UpperLeftSquare.X == pc.combatLocX) && (gridy + UpperLeftSquare.Y == pc.combatLocY + 1)))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveDown(pc);
                    }
                    else if ((gv.cc.ctrlLeftArrow.getImpact(x, y)) || ((gridx + UpperLeftSquare.X == pc.combatLocX - 1) && (gridy + UpperLeftSquare.Y == pc.combatLocY)))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveLeft(pc);
                    }
                    else if ((gv.cc.ctrlRightArrow.getImpact(x, y)) || ((gridx + UpperLeftSquare.X == pc.combatLocX + 1) && (gridy + UpperLeftSquare.Y == pc.combatLocY)))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveRight(pc);
                    }
                    else if ((gv.cc.ctrlUpRightArrow.getImpact(x, y)) || ((gridx + UpperLeftSquare.X == pc.combatLocX + 1) && (gridy + UpperLeftSquare.Y == pc.combatLocY - 1)))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveUpRight(pc);
                    }
                    else if ((gv.cc.ctrlDownRightArrow.getImpact(x, y)) || ((gridx + UpperLeftSquare.X == pc.combatLocX + 1) && (gridy + UpperLeftSquare.Y == pc.combatLocY + 1)))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveDownRight(pc);
                    }
                    else if ((gv.cc.ctrlUpLeftArrow.getImpact(x, y)) || ((gridx + UpperLeftSquare.X == pc.combatLocX - 1) && (gridy + UpperLeftSquare.Y == pc.combatLocY - 1)))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveUpLeft(pc);
                    }
                    else if ((gv.cc.ctrlDownLeftArrow.getImpact(x, y)) || ((gridx + UpperLeftSquare.X == pc.combatLocX - 1) && (gridy + UpperLeftSquare.Y == pc.combatLocY + 1)))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveDownLeft(pc);
                    }
                    else if (btnMove.getImpact(x, y))
                    {
                        if (canMove)
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            currentCombatMode = "info";
                            gv.screenType = "combat";
                            //Toast.makeText(gameContext, "Move Mode", Toast.LENGTH_SHORT).show();
                        }
                    }
                    else if (gv.cc.btnInventory.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        gv.screenType = "combatInventory";
                        gv.screenInventory.resetInventory();
                        //Toast.makeText(gameContext, "Inventory Button", Toast.LENGTH_SHORT).show();
                    }
                    else if (btnAttack.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        currentCombatMode = "attack";
                        gv.screenType = "combat";
                        setTargetHighlightStartLocation(pc);
                    }
                    else if (btnCast.getImpact(x, y))
                    {
                        if (pc.knownSpellsTags.Count > 0)
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            currentCombatMode = "castSelector";
                            gv.screenType = "combatCast";
                            gv.screenCastSelector.castingPlayerIndex = currentPlayerIndex;
                            spellSelectorIndex = 0;
                            setTargetHighlightStartLocation(pc);
                        }
                        else
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            //TODO Toast.makeText(gv.gameContext, "PC has no Spells", Toast.LENGTH_SHORT).show();
                        }
                    }
                    else if (btnSkipTurn.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        gv.screenType = "combat";
                        endPcTurn(false);
                    }
                    break;
            }
        }
        public void onTouchCombatAttack(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            Player pc = mod.playerList[currentPlayerIndex];

            gv.cc.ctrlUpArrow.glowOn = false;
            gv.cc.ctrlDownArrow.glowOn = false;
            gv.cc.ctrlLeftArrow.glowOn = false;
            gv.cc.ctrlRightArrow.glowOn = false;
            gv.cc.ctrlUpRightArrow.glowOn = false;
            gv.cc.ctrlDownRightArrow.glowOn = false;
            gv.cc.ctrlUpLeftArrow.glowOn = false;
            gv.cc.ctrlDownLeftArrow.glowOn = false;
            btnSelect.glowOn = false;
            btnMove.glowOn = false;
            gv.cc.btnInventory.glowOn = false;
            btnAttack.glowOn = false;
            btnCast.glowOn = false;
            btnSkipTurn.glowOn = false;

            //int eventAction = event.getAction();
            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                case MouseEventType.EventType.MouseMove:
                    int x = (int)e.X;
                    int y = (int)e.Y;
                    if (gv.cc.ctrlUpArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlLeftArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlUpRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlUpLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpLeftArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownLeftArrow.glowOn = true;
                    }
                    else if (btnSelect.getImpact(x, y))
                    {
                        btnSelect.glowOn = true;
                    }
                    else if (btnMove.getImpact(x, y))
                    {
                        btnMove.glowOn = true;
                    }
                    else if (gv.cc.btnInventory.getImpact(x, y))
                    {
                        gv.cc.btnInventory.glowOn = true;
                    }
                    else if (btnAttack.getImpact(x, y))
                    {
                        btnAttack.glowOn = true;
                    }
                    else if (btnCast.getImpact(x, y))
                    {
                        btnCast.glowOn = true;
                    }
                    else if (btnSkipTurn.getImpact(x, y))
                    {
                        btnSkipTurn.glowOn = true;
                    }
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)e.X;
                    y = (int)e.Y;

                    gv.cc.ctrlUpArrow.glowOn = false;
                    gv.cc.ctrlDownArrow.glowOn = false;
                    gv.cc.ctrlLeftArrow.glowOn = false;
                    gv.cc.ctrlRightArrow.glowOn = false;
                    gv.cc.ctrlUpRightArrow.glowOn = false;
                    gv.cc.ctrlDownRightArrow.glowOn = false;
                    gv.cc.ctrlUpLeftArrow.glowOn = false;
                    gv.cc.ctrlDownLeftArrow.glowOn = false;
                    btnSelect.glowOn = false;
                    btnMove.glowOn = false;
                    gv.cc.btnInventory.glowOn = false;
                    btnAttack.glowOn = false;
                    btnCast.glowOn = false;
                    btnSkipTurn.glowOn = false;

                    //TOUCH ON MAP AREA
                    int gridx = ((int)(e.X - gv.oXshift - mapStartLocXinPixels) / gv.squareSize) + UpperLeftSquare.X;
                    int gridy = ((int)(e.Y - (gv.squareSize / 2)) / gv.squareSize) + UpperLeftSquare.Y;

                    if (IsInVisibleCombatWindow(gridx, gridy))
                    {
                        gv.cc.floatyText = "";
                        gv.cc.floatyText2 = "";
                        gv.cc.floatyText3 = "";
                        //Check for second tap so TARGET
                        if ((gridx == targetHighlightCenterLocation.X) && (gridy == targetHighlightCenterLocation.Y))
                        {
                            TargetAttackPressed(pc);                            
                        }
                        //targetHighlightCenterLocation.Y = gridy + UpperLeftSquare.Y;
                        //targetHighlightCenterLocation.X = gridx + UpperLeftSquare.X;
                        targetHighlightCenterLocation.Y = gridy;
                        targetHighlightCenterLocation.X = gridx;
                    }

                    //BUTTONS
                    if (gv.cc.ctrlUpArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(8);
                    }
                    else if (gv.cc.ctrlDownArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(2);
                    }
                    else if (gv.cc.ctrlLeftArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(4);
                    }
                    else if (gv.cc.ctrlRightArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(6);
                    }
                    else if (gv.cc.ctrlUpRightArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(9);
                    }
                    else if (gv.cc.ctrlDownRightArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(3);
                    }
                    else if (gv.cc.ctrlUpLeftArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(7);
                    }
                    else if (gv.cc.ctrlDownLeftArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(1);
                    }
                    else if (btnSelect.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        TargetAttackPressed(pc);                        
                    }
                    else if (btnMove.getImpact(x, y))
                    {
                        if (canMove)
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            currentCombatMode = "move";
                            gv.screenType = "combat";
                        }
                    }
                    else if (gv.cc.btnInventory.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        gv.screenType = "combatInventory";
                        gv.screenInventory.resetInventory();
                    }
                    else if (btnAttack.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        currentCombatMode = "info";
                        gv.screenType = "combat";
                        setTargetHighlightStartLocation(pc);
                    }
                    else if (btnCast.getImpact(x, y))
                    {
                        if (pc.knownSpellsTags.Count > 0)
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            currentCombatMode = "castSelector";
                            gv.screenType = "combatCast";
                            gv.screenCastSelector.castingPlayerIndex = currentPlayerIndex;
                            spellSelectorIndex = 0;
                            setTargetHighlightStartLocation(pc);
                        }
                        else
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            //TODO Toast.makeText(gv.gameContext, "PC has no Spells", Toast.LENGTH_SHORT).show();
                        }
                    }
                    else if (btnSkipTurn.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        endPcTurn(false);
                    }
                    break;
            }
        }
        public void onTouchCombatCast(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            //gv.cc.onTouchLog();
            Player pc = mod.playerList[currentPlayerIndex];

            gv.cc.ctrlUpArrow.glowOn = false;
            gv.cc.ctrlDownArrow.glowOn = false;
            gv.cc.ctrlLeftArrow.glowOn = false;
            gv.cc.ctrlRightArrow.glowOn = false;
            gv.cc.ctrlUpRightArrow.glowOn = false;
            gv.cc.ctrlDownRightArrow.glowOn = false;
            gv.cc.ctrlUpLeftArrow.glowOn = false;
            gv.cc.ctrlDownLeftArrow.glowOn = false;
            btnSelect.glowOn = false;
            btnMove.glowOn = false;
            gv.cc.btnInventory.glowOn = false;
            btnAttack.glowOn = false;
            btnCast.glowOn = false;
            btnSkipTurn.glowOn = false;

            //int eventAction = event.getAction();
            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                case MouseEventType.EventType.MouseMove:
                    int x = (int)e.X;
                    int y = (int)e.Y;
                    if (gv.cc.ctrlUpArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlLeftArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlUpRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownRightArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownRightArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlUpLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlUpLeftArrow.glowOn = true;
                    }
                    else if (gv.cc.ctrlDownLeftArrow.getImpact(x, y))
                    {
                        gv.cc.ctrlDownLeftArrow.glowOn = true;
                    }
                    else if (btnSelect.getImpact(x, y))
                    {
                        btnSelect.glowOn = true;
                    }
                    else if (btnMove.getImpact(x, y))
                    {
                        btnMove.glowOn = true;
                    }
                    else if (gv.cc.btnInventory.getImpact(x, y))
                    {
                        gv.cc.btnInventory.glowOn = true;
                    }
                    else if (btnAttack.getImpact(x, y))
                    {
                        btnAttack.glowOn = true;
                    }
                    else if (btnCast.getImpact(x, y))
                    {
                        btnCast.glowOn = true;
                    }
                    else if (btnSkipTurn.getImpact(x, y))
                    {
                        btnSkipTurn.glowOn = true;
                    }
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)e.X;
                    y = (int)e.Y;

                    gv.cc.ctrlUpArrow.glowOn = false;
                    gv.cc.ctrlDownArrow.glowOn = false;
                    gv.cc.ctrlLeftArrow.glowOn = false;
                    gv.cc.ctrlRightArrow.glowOn = false;
                    gv.cc.ctrlUpRightArrow.glowOn = false;
                    gv.cc.ctrlDownRightArrow.glowOn = false;
                    gv.cc.ctrlUpLeftArrow.glowOn = false;
                    gv.cc.ctrlDownLeftArrow.glowOn = false;
                    btnSelect.glowOn = false;
                    btnMove.glowOn = false;
                    gv.cc.btnInventory.glowOn = false;
                    btnAttack.glowOn = false;
                    btnCast.glowOn = false;
                    btnSkipTurn.glowOn = false;

                    //TOUCH ON MAP AREA
                    int gridx = ((int)(e.X - gv.oXshift - mapStartLocXinPixels) / gv.squareSize) + UpperLeftSquare.X;
                    int gridy = ((int)(e.Y - (gv.squareSize / 2)) / gv.squareSize) + UpperLeftSquare.Y;

                    if (IsInVisibleCombatWindow(gridx, gridy))
                    //if (gridy < mod.currentEncounter.MapSizeY)
                    {
                        gv.cc.floatyText = "";
                        gv.cc.floatyText2 = "";
                        gv.cc.floatyText3 = "";
                        //Check for second tap so TARGET
                        if ((gridx == targetHighlightCenterLocation.X) && (gridy == targetHighlightCenterLocation.Y))
                        {
                            TargetCastPressed(pc);
                        }
                        targetHighlightCenterLocation.Y = gridy;
                        targetHighlightCenterLocation.X = gridx;
                    }

                    //BUTTONS
                    if (gv.cc.ctrlUpArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(8);
                    }
                    else if (gv.cc.ctrlDownArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(2);
                    }
                    else if (gv.cc.ctrlLeftArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(4);
                    }
                    else if (gv.cc.ctrlRightArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(6);
                    }
                    else if (gv.cc.ctrlUpRightArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(9);
                    }
                    else if (gv.cc.ctrlDownRightArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(3);
                    }
                    else if (gv.cc.ctrlUpLeftArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(7);
                    }
                    else if (gv.cc.ctrlDownLeftArrow.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        MoveTargetHighlight(1);
                    }
                    else if (btnSelect.getImpact(x, y))
                    {
                        TargetCastPressed(pc);
                        //Toast.makeText(gameContext, "Selected", Toast.LENGTH_SHORT).show();
                    }
                    else if (btnMove.getImpact(x, y))
                    {
                        if (canMove)
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            currentCombatMode = "move";
                            gv.screenType = "combat";
                            //Toast.makeText(gameContext, "Move Mode", Toast.LENGTH_SHORT).show();
                        }
                    }
                    else if (gv.cc.btnInventory.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        gv.screenType = "combatInventory";
                        gv.screenInventory.resetInventory();
                        //Toast.makeText(gameContext, "Inventory Button", Toast.LENGTH_SHORT).show();
                    }
                    else if (btnAttack.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        currentCombatMode = "attack";
                        gv.screenType = "combat";
                        setTargetHighlightStartLocation(pc);
                        //Toast.makeText(gameContext, "Attack Mode", Toast.LENGTH_SHORT).show();
                    }
                    else if (btnCast.getImpact(x, y))
                    {
                        if (pc.knownSpellsTags.Count > 0)
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            currentCombatMode = "castSelector";
                            gv.screenType = "combatCast";
                            gv.screenCastSelector.castingPlayerIndex = currentPlayerIndex;
                            spellSelectorIndex = 0;
                            setTargetHighlightStartLocation(pc);
                        }
                        else
                        {
                            //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                            //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                            //TODO Toast.makeText(gv.gameContext, "PC has no Spells", Toast.LENGTH_SHORT).show();
                        }
                    }
                    else if (btnSkipTurn.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        endPcTurn(false);
                    }
                    break;
            }
        }
        #endregion

        public void doUpdate(Player pc)
        {
            CalculateUpperLeft();
            if (moveCost == mod.diagonalMoveCost)
            {
                currentMoves += mod.diagonalMoveCost;
                moveCost = 1.0f;
            }
            else
            {
                currentMoves++;
            }
            float moveleft = pc.moveDistance - currentMoves;
            if (moveleft < 1) { moveleft = 0; }
        }
        public void MoveTargetHighlight(int numPadDirection)
        {
            switch (numPadDirection)
            {
                case 8: //up
                    if (targetHighlightCenterLocation.Y > 0) 
                    {
                        targetHighlightCenterLocation.Y--;
                        if (!IsInVisibleCombatWindow(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y))
                        {
                            targetHighlightCenterLocation.Y++;
                        }
                    }
                    break;
                case 2: //down
                    if (targetHighlightCenterLocation.Y < mod.currentEncounter.MapSizeY - 1)
                    {
                        targetHighlightCenterLocation.Y++;
                        if (!IsInVisibleCombatWindow(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y))
                        {
                            targetHighlightCenterLocation.Y--;
                        }
                    }
                    break;
                case 4: //left
                    if (targetHighlightCenterLocation.X > 0)
                    {
                        targetHighlightCenterLocation.X--;
                        if (!IsInVisibleCombatWindow(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y))
                        {
                            targetHighlightCenterLocation.X++;
                        }
                    }
                    break;
                case 6: //right
                    if (targetHighlightCenterLocation.X < mod.currentEncounter.MapSizeX - 1)
                    {
                        targetHighlightCenterLocation.X++;
                        if (!IsInVisibleCombatWindow(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y))
                        {
                            targetHighlightCenterLocation.X--;
                        }
                    }
                    break;
                case 9: //upright
                    if ((targetHighlightCenterLocation.X < mod.currentEncounter.MapSizeX - 1) && (targetHighlightCenterLocation.Y > 0))
                    {
                        targetHighlightCenterLocation.X++;
                        targetHighlightCenterLocation.Y--;
                        if (!IsInVisibleCombatWindow(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y))
                        {
                            targetHighlightCenterLocation.X--;
                            targetHighlightCenterLocation.Y++;
                        }
                    }
                    break;
                case 3: //downright
                    if ((targetHighlightCenterLocation.X < mod.currentEncounter.MapSizeX - 1) && (targetHighlightCenterLocation.Y < mod.currentEncounter.MapSizeY - 1))
                    {
                        targetHighlightCenterLocation.X++;
                        targetHighlightCenterLocation.Y++;
                        if (!IsInVisibleCombatWindow(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y))
                        {
                            targetHighlightCenterLocation.X--;
                            targetHighlightCenterLocation.Y--;
                        }
                    }
                    break;
                case 7: //upleft
                    if ((targetHighlightCenterLocation.X > 0) && (targetHighlightCenterLocation.Y > 0))
                    {
                        targetHighlightCenterLocation.X--;
                        targetHighlightCenterLocation.Y--;
                        if (!IsInVisibleCombatWindow(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y))
                        {
                            targetHighlightCenterLocation.X++;
                            targetHighlightCenterLocation.Y++;
                        }
                    }
                    break;
                case 1: //downleft
                    if ((targetHighlightCenterLocation.X > 0) && (targetHighlightCenterLocation.Y < mod.currentEncounter.MapSizeY - 1))
                    {
                        targetHighlightCenterLocation.X--;
                        targetHighlightCenterLocation.Y++;
                        if (!IsInVisibleCombatWindow(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y))
                        {
                            targetHighlightCenterLocation.X++;
                            targetHighlightCenterLocation.Y--;
                        }
                    }
                    break;
            }
            
        }
        public void MoveUp(Player pc)
        {
            if (pc.combatLocY > 0)
            {
                //check is walkable (blocked square or PC)
                if (isWalkable(pc.combatLocX, pc.combatLocY - 1))
                {
                    //check if creature -> do attack
                    Creature c = isBumpIntoCreature(pc.combatLocX, pc.combatLocY - 1);
                    if (c != null)
                    {
                        //attack creature
                        targetHighlightCenterLocation.X = pc.combatLocX;
                        targetHighlightCenterLocation.Y = pc.combatLocY - 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= 1.0f)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX, pc.combatLocY - 1);
                        doPlayerCombatFacing(pc, pc.combatLocX, pc.combatLocY - 1);
                        pc.combatLocY--;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveUpRight(Player pc)
        {
            if ((pc.combatLocX < mod.currentEncounter.MapSizeX - 1) && (pc.combatLocY > 0))
            {
                if (isWalkable(pc.combatLocX + 1, pc.combatLocY - 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX + 1, pc.combatLocY - 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX + 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY - 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= mod.diagonalMoveCost)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX + 1, pc.combatLocY - 1);
                        doPlayerCombatFacing(pc, pc.combatLocX + 1, pc.combatLocY - 1);
                        pc.combatLocX++;
                        pc.combatLocY--;
                        if (pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = false;
                        }
                        moveCost = mod.diagonalMoveCost;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveUpLeft(Player pc)
        {
            if ((pc.combatLocX > 0) && (pc.combatLocY > 0))
            {
                if (isWalkable(pc.combatLocX - 1, pc.combatLocY - 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX - 1, pc.combatLocY - 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX - 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY - 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= mod.diagonalMoveCost)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX - 1, pc.combatLocY - 1);
                        doPlayerCombatFacing(pc, pc.combatLocX - 1, pc.combatLocY - 1);
                        pc.combatLocX--;
                        pc.combatLocY--;
                        if (!pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = true;
                        }
                        moveCost = mod.diagonalMoveCost;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveDown(Player pc)
        {
            if (pc.combatLocY < mod.currentEncounter.MapSizeY - 1)
            {
                if (isWalkable(pc.combatLocX, pc.combatLocY + 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX, pc.combatLocY + 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX;
                        targetHighlightCenterLocation.Y = pc.combatLocY + 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= 1.0f)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX, pc.combatLocY + 1);
                        doPlayerCombatFacing(pc, pc.combatLocX, pc.combatLocY + 1);
                        pc.combatLocY++;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveDownRight(Player pc)
        {
            if ((pc.combatLocX < mod.currentEncounter.MapSizeX - 1) && (pc.combatLocY < mod.currentEncounter.MapSizeY - 1))
            {
                if (isWalkable(pc.combatLocX + 1, pc.combatLocY + 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX + 1, pc.combatLocY + 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX + 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY + 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= mod.diagonalMoveCost)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX + 1, pc.combatLocY + 1);
                        doPlayerCombatFacing(pc, pc.combatLocX + 1, pc.combatLocY + 1);
                        pc.combatLocX++;
                        pc.combatLocY++;
                        if (pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = false;
                        }
                        moveCost = mod.diagonalMoveCost;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveDownLeft(Player pc)
        {
            if ((pc.combatLocX > 0) && (pc.combatLocY < mod.currentEncounter.MapSizeY - 1))
            {
                if (isWalkable(pc.combatLocX - 1, pc.combatLocY + 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX - 1, pc.combatLocY + 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX - 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY + 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= mod.diagonalMoveCost)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX - 1, pc.combatLocY + 1);
                        doPlayerCombatFacing(pc, pc.combatLocX - 1, pc.combatLocY + 1);
                        pc.combatLocX--;
                        pc.combatLocY++;
                        if (!pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = true;
                        }
                        moveCost = mod.diagonalMoveCost;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveRight(Player pc)
        {
            if (pc.combatLocX < mod.currentEncounter.MapSizeX - 1)
            {
                if (isWalkable(pc.combatLocX + 1, pc.combatLocY))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX + 1, pc.combatLocY);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX + 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= 1.0f)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX + 1, pc.combatLocY);
                        doPlayerCombatFacing(pc, pc.combatLocX + 1, pc.combatLocY);
                        pc.combatLocX++;
                        if (pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = false;
                        }
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveLeft(Player pc)
        {
            if (pc.combatLocX > 0)
            {
                if (isWalkable(pc.combatLocX - 1, pc.combatLocY))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX - 1, pc.combatLocY);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX - 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= 1.0f)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX, pc.combatLocY);
                        doPlayerCombatFacing(pc, pc.combatLocX - 1, pc.combatLocY);
                        pc.combatLocX--;
                        if (!pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = true;
                        }
                        doUpdate(pc);
                    }                    
                }
            }
        }
        public void TargetAttackPressed(Player pc)
        {
            if (isValidAttackTarget(pc))
            {
                if ((targetHighlightCenterLocation.X < pc.combatLocX) && (!pc.combatFacingLeft)) //attack left
                {
                    pc.combatFacingLeft = true;
                }
                else if ((targetHighlightCenterLocation.X > pc.combatLocX) && (pc.combatFacingLeft)) //attack right
                {
                    pc.combatFacingLeft = false;
                }
                doPlayerCombatFacing(pc, targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y);
                gv.touchEnabled = false;
                creatureToAnimate = null;
                playerToAnimate = pc;
                gv.Render();
                if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                        || (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).name.Equals("none"))
                        || (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                {
                    animationState = AnimationState.PcMeleeAttackAnimation;
                }
                else
                {
                    //play attack sound for ranged
                    gv.PlaySound(mod.getItemByResRefForInfo(pc.MainHandRefs.resref).itemOnUseSound);
                    animationState = AnimationState.PcRangedAttackAnimation;
                    //do new animation system: attack animation, projectile animation, ending animation, hit/miss animation
                    launchProjectile(pc);
                }
                gv.postDelayed("doAnimation", (int) (5 * (0.5f) * mod.combatAnimationSpeed));
            }
        }
        public void TargetCastPressed(Player pc)
        {
            //Uses Map Pixel Locations
            int endX = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
            int endY = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);
            int startX = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
            int startY = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);

            if ((isValidCastTarget(pc)) && (isVisibleLineOfSight(new Coordinate(endX, endY), new Coordinate(startX, startY))))
            {
                if ((targetHighlightCenterLocation.X < pc.combatLocX) && (!pc.combatFacingLeft)) //attack left
                {
                    pc.combatFacingLeft = true;
                }
                else if ((targetHighlightCenterLocation.X > pc.combatLocX) && (pc.combatFacingLeft)) //attack right
                {
                    pc.combatFacingLeft = false;
                }
                doPlayerCombatFacing(pc, targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y);
                gv.touchEnabled = false;
                creatureToAnimate = null;
                playerToAnimate = pc;
                gv.Render();
                animationState = AnimationState.PcCastAttackAnimation;
                gv.postDelayed("doAnimation", (int) (5 * (0.5f) * mod.combatAnimationSpeed));
            }
        }
        public void launchProjectile(Player pc)
        {
            //load projectile image
            gv.cc.DisposeOfBitmap(ref projectile);
            projectile = gv.cc.LoadBitmap(mod.getItemByResRefForInfo(pc.AmmoRefs.resref).projectileSpriteFilename);
            int startX = getPixelLocX(pc.combatLocX);
            int startY = getPixelLocY(pc.combatLocY);
            int endX = getPixelLocX(targetHighlightCenterLocation.X);
            int endY = getPixelLocY(targetHighlightCenterLocation.Y);
            //calculate angle from start to end point
            float angle = AngleRad(new Point(startX, startY), new Point(endX, endY));
            //calculate distance in pixels
            float dist = (float)(Math.Sqrt((Math.Abs(startX - endX)) ^ 2 + (Math.Abs(startY - endY)) ^ 2));
            //calculate needed TTL based on a constant speed for projectiles
            float velX = (endX - startX);
            float velY = (endY - startY);
            float velocityX = 0;
            if (velX != 0)
            {
                velocityX = velX / 1000;
            }
            float velocityY = 0;
            if (velY != 0)
            {
                velocityY = velY / 1000;
            }
            int ttl = (int)(dist * 1000);
            Sprite spr = new Sprite(gv.cc.facing8, startX, startY, velocityX, velocityY, angle, 0, 1.0f, ttl);
            this.spriteList.Add(spr);
        }
        //const float Rad2Deg = (float)(180.0 / Math.PI);
        public float AngleRad(Point start, Point end)
        {
            return (float)(-1 * ((Math.Atan2(start.Y - end.Y, end.X - start.X)) - (Math.PI) / 2));
        }

        //Helper Methods
        public void CalculateUpperLeft()
        {
            Player pc = mod.playerList[currentPlayerIndex];
            int minX = pc.combatLocX - gv.playerOffset;
            if (minX < 0) { minX = 0; }
            int minY = pc.combatLocY - gv.playerOffset;
            if (minY < 0) { minY = 0; }

            /*mod.combatAnimationSpeed = gv.sf.GetGlobalInt("animationSpeed");
            if (mod.combatAnimationSpeed < 1)
            {
                mod.combatAnimationSpeed = 50;
            }*/

            if ((pc.combatLocX <= (UpperLeftSquare.X + 7)) && (pc.combatLocX >= UpperLeftSquare.X + 2) && (pc.combatLocY <= (UpperLeftSquare.Y + 7)) && (pc.combatLocY >= UpperLeftSquare.Y + 2))
            { 
                return; 
            }
            else
            { 
                UpperLeftSquare.X = minX; 
                UpperLeftSquare.Y = minY; 
            }
        }
        public void CalculateUpperLeftCreature()
        {
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
            //mod.combatAnimationSpeed = 10;
            int minX = crt.combatLocX - gv.playerOffset;
            if (minX < 0) { minX = 0; }
            int minY = crt.combatLocY - gv.playerOffset;
            if (minY < 0) { minY = 0; }

            //do not adjust view port if creature is on screen already and ends move at least one square away from border
            if (((crt.combatLocX + 2) <= (UpperLeftSquare.X + (gv.playerOffset * 2))) && ((crt.combatLocX - 2) >= (UpperLeftSquare.X)) && ((crt.combatLocY + 2) <= (UpperLeftSquare.Y + (gv.playerOffset * 2))) && ((crt.combatLocY - 2) >= (UpperLeftSquare.Y)))
            {
                return;
            }

            else
            {
                UpperLeftSquare.X = minX;
                UpperLeftSquare.Y = minY;
            }
        }
        public void CenterScreenOnPC()
        {
            Player pc = mod.playerList[currentPlayerIndex];
            int minX = pc.combatLocX - gv.playerOffset;
            if (minX < 0) { minX = 0; }
            int minY = pc.combatLocY - gv.playerOffset;
            if (minY < 0) { minY = 0; }

            UpperLeftSquare.X = minX;
            UpperLeftSquare.Y = minY; 
        }
        public bool IsInVisibleCombatWindow(int sqrX, int sqrY)
        {
            //all input coordinates are in Map Location, not Screen Location
            if ((sqrX < UpperLeftSquare.X) || (sqrY < UpperLeftSquare.Y))
            {
                return false;
            }
            if ((sqrX >= UpperLeftSquare.X + gv.playerOffset + gv.playerOffset + 1)
                || (sqrY >= UpperLeftSquare.Y + gv.playerOffset + gv.playerOffset + 1))
            {
                return false;
            }
            return true;
        }
        public bool IsInVisibleCombatWindow(int sqrX, int sqrY, int tileW, int tileH)
        {
            //all input coordinates are in Map Location, not Screen Location
            if ((sqrX < UpperLeftSquare.X) || (sqrY < UpperLeftSquare.Y))
            {
                return false;
            }
            if ((sqrX >= UpperLeftSquare.X + gv.playerOffset + gv.playerOffset + 1)
                || (sqrY >= UpperLeftSquare.Y + gv.playerOffset + gv.playerOffset + 1))
            {
                return false;
            }
            return true;
        }
        public int getPixelLocX(int sqrX)
        {
            return ((sqrX - UpperLeftSquare.X) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
        }
        public int getPixelLocY(int sqrY)
        {
            return (sqrY - UpperLeftSquare.Y) * gv.squareSize;
        }
        public int getViewportSquareLocX(int sqrX)
        {
            return sqrX - UpperLeftSquare.X;
        }
        public int getViewportSquareLocY(int sqrY)
        {
            return sqrY - UpperLeftSquare.Y;
        }
	    public void setTargetHighlightStartLocation(Player pc)
	    {
		    targetHighlightCenterLocation.X = pc.combatLocX;
		    targetHighlightCenterLocation.Y = pc.combatLocY;		
	    }
	    public bool isValidAttackTarget(Player pc)
	    {
		    if (isInRange(pc))
		    {
                Item it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                //if using ranged and have ammo, use ammo properties
                if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                        && (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                {
                    //ranged weapon with ammo
                    it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                }
                if (it == null)
                {
                    it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                }
                //check to see if is AoE or Point Target else needs a target PC or Creature
                if (it.AreaOfEffect > 0)
                {
                    return true;
                }

                //Uses the Map Pixel Locations
                int endX2 = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                int endY2 = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);
                int startX2 = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                int startY2 = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);

			    if ((isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX2, startY2)))
				    || (getDistance(new Coordinate(pc.combatLocX,pc.combatLocY), targetHighlightCenterLocation) == 1))
			    {
				    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
				    {
					    if ((crt.combatLocX == targetHighlightCenterLocation.X) && (crt.combatLocY == targetHighlightCenterLocation.Y))
					    {
						    return true;
					    }
				    }
			    }
		    }
		    return false;
	    }
	    public bool isValidCastTarget(Player pc)
	    {
		    if (isInRange(pc))
		    {
			    //check to see if is AoE or Point Target else needs a target PC or Creature
			    if ((gv.cc.currentSelectedSpell.aoeRadius > 0) || (gv.cc.currentSelectedSpell.spellTargetType.Equals("PointLocation")))
			    {
				    return true;
			    }			
			    //is not an AoE ranged attack, is a PC or Creature
			    else
			    {
				    //check to see if target is a friend or self
				    if ((gv.cc.currentSelectedSpell.spellTargetType.Equals("Friend")) || (gv.cc.currentSelectedSpell.spellTargetType.Equals("Self")))
				    {
					    foreach (Player p in mod.playerList)
					    {
						    if ((p.combatLocX == targetHighlightCenterLocation.X) && (p.combatLocY == targetHighlightCenterLocation.Y))
						    {
							    return true;
						    }
					    }
				    }
				    else //target is a creature
				    {
					    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
					    {
						    if ((crt.combatLocX == targetHighlightCenterLocation.X) && (crt.combatLocY == targetHighlightCenterLocation.Y))
						    {
							    return true;
						    }
					    }
				    }
			    }			
		    }
		    return false;
	    }
	    public object getCastTarget(Player pc)
	    {
		    if (isInRange(pc))
		    {
			    //check to see if is AoE or Point Target else needs a target PC or Creature
			    if ((gv.cc.currentSelectedSpell.aoeRadius > 0) || (gv.cc.currentSelectedSpell.spellTargetType.Equals("PointLocation")))
			    {
				    return new Coordinate(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y);
			    }			
			    //is not an AoE ranged attack, is a PC or Creature
			    else
			    {
				    //check to see if target is a friend or self
				    if ((gv.cc.currentSelectedSpell.spellTargetType.Equals("Friend")) || (gv.cc.currentSelectedSpell.spellTargetType.Equals("Self")))
				    {
					    foreach (Player p in mod.playerList)
					    {
						    if ((p.combatLocX == targetHighlightCenterLocation.X) && (p.combatLocY == targetHighlightCenterLocation.Y))
						    {
							    return p;
						    }
					    }
				    }
				    else //target is a creature
				    {
					    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
					    {
						    if ((crt.combatLocX == targetHighlightCenterLocation.X) && (crt.combatLocY == targetHighlightCenterLocation.Y))
						    {
							    return crt;
						    }
					    }
				    }
			    }			
		    }
		    return null;
	    }
	    public bool isInRange(Player pc)
	    {
		    if (currentCombatMode.Equals("attack"))
		    {
			    int range = 1;
			    if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")) 
	        		    && (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
	            {
				    //ranged weapon with no ammo
				    range = 1;
	            }
			    else 
			    {
				    range = mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attackRange;
			    }
			
			    if (getDistance(new Coordinate(pc.combatLocX,pc.combatLocY), targetHighlightCenterLocation) <= range)
			    {
				    return true;
			    }
		    }
		    else if (currentCombatMode.Equals("cast"))
		    {
			    if (getDistance(new Coordinate(pc.combatLocX,pc.combatLocY), targetHighlightCenterLocation) <= gv.cc.currentSelectedSpell.range)
			    {
				    return true;
			    }
		    }
		    return false;
	    }
	    public bool isAdjacentEnemy(Player pc)
	    {
		    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
		    {
			    if (getDistance(new Coordinate(pc.combatLocX,pc.combatLocY), new Coordinate(crt.combatLocX,crt.combatLocY)) == 1)
			    {
				    if (!crt.isHeld())
				    {
					    return true;
				    }
			    }
		    }
		    return false;
	    }
	    public bool isAdjacentPc(Creature crt)
	    {
		    foreach (Player pc in mod.playerList)
		    {
			    if (getDistance(new Coordinate(pc.combatLocX,pc.combatLocY), new Coordinate(crt.combatLocX,crt.combatLocY)) == 1)
			    {
				    return true;
			    }
		    }
		    return false;
	    }
	    public int getGridX(Coordinate nextPoint)
        {
            int gridx = ((nextPoint.X - mapStartLocXinPixels - gv.oXshift) / gv.squareSize) + UpperLeftSquare.X;
            if (gridx > mod.currentEncounter.MapSizeX - 1) { gridx = mod.currentEncounter.MapSizeX - 1; }
            if (gridx < 0) { gridx = 0; }
            return gridx;
        }
        public int getGridY(Coordinate nextPoint)
        {
            int gridy = ((nextPoint.Y - gv.oYshift) / gv.squareSize) + UpperLeftSquare.Y;
            if (gridy > mod.currentEncounter.MapSizeY - 1) { gridy = mod.currentEncounter.MapSizeY - 1; }
            if (gridy < 0) { gridy = 0; }
            return gridy;
        }
        public int getMapSquareX(Coordinate nextPoint)
        {
            int gridx = (nextPoint.X / gv.squareSize);
            if (gridx > mod.currentEncounter.MapSizeX - 1) { gridx = mod.currentEncounter.MapSizeX - 1; }
            if (gridx < 0) { gridx = 0; }
            return gridx;
        }
        public int getMapSquareY(Coordinate nextPoint)
        {
            int gridy = (nextPoint.Y / gv.squareSize);
            if (gridy > mod.currentEncounter.MapSizeY - 1) { gridy = mod.currentEncounter.MapSizeY - 1; }
            if (gridy < 0) { gridy = 0; }
            return gridy;
        }
        public bool isVisibleLineOfSight(Coordinate end, Coordinate start)
        {  
            //This Method Uses Map Pixel Locations Only

            int deltax = Math.Abs(end.X - start.X);
            int deltay = Math.Abs(end.Y - start.Y);
            int ystep = gv.squareSize / 50;
            int xstep = gv.squareSize / 50;
            if (ystep < 1) {ystep = 1;}
            if (xstep < 1) {xstep = 1;}

            if (deltax > deltay) //Low Angle line
            {
        	    Coordinate nextPoint = start;
                int error = deltax / 2;

                if (end.Y < start.Y) { ystep = -1 * ystep; } //down and right or left

                if (end.X > start.X) //down and right
                {
                    for (int x = start.X; x <= end.X; x += xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getMapSquareX(nextPoint);
                        int gridy = getMapSquareY(nextPoint);
                        if (mod.currentEncounter.encounterTiles[gridy * mod.currentEncounter.MapSizeX + gridx].LoSBlocked)
                        {
                    	    return false;
                        }                       
                    }
                }
                else //down and left
                {
                    for (int x = start.X; x >= end.X; x -= xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getMapSquareX(nextPoint);
                        int gridy = getMapSquareY(nextPoint);
                        if (mod.currentEncounter.encounterTiles[gridy * mod.currentEncounter.MapSizeX + gridx].LoSBlocked)
                        {
                    	    return false;
                        }                      
                    }
                }
            }

            else //Low Angle line
            {
        	    Coordinate nextPoint = start;
                int error = deltay / 2;

                if (end.X < start.X) { xstep = -1 * xstep; } //up and right or left

                if (end.Y > start.Y) //up and right
                {
                    for (int y = start.Y; y <= end.Y; y += ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getMapSquareX(nextPoint);
                        int gridy = getMapSquareY(nextPoint);
                        if (mod.currentEncounter.encounterTiles[gridy * mod.currentEncounter.MapSizeX + gridx].LoSBlocked)
                        {
                    	    return false;
                        }                      
                    }
                }
                else //up and right
                {
                    for (int y = start.Y; y >= end.Y; y -= ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getMapSquareX(nextPoint);
                        int gridy = getMapSquareY(nextPoint);
                        if (mod.currentEncounter.encounterTiles[gridy * mod.currentEncounter.MapSizeX + gridx].LoSBlocked)
                        {
                    	    return false;
                        }                       
                    }
                }
            }
        
            return true;
        }
	    public bool drawVisibleLineOfSightTrail(Coordinate end, Coordinate start, Color penColor, int penWidth)
        {
            // Bresenham Line algorithm
            // Creates a line from Begin to End starting at (x0,y0) and ending at (x1,y1)
            // where x0 less than x1 and y0 less than y1
            // AND line is less steep than it is wide (dx less than dy)    
        
            int deltax = Math.Abs(end.X - start.X);
            int deltay = Math.Abs(end.Y - start.Y);
            int ystep = gv.squareSize / 50;
            int xstep = gv.squareSize / 50;
            if (ystep < 1) {ystep = 1;}
            if (xstep < 1) {xstep = 1;}

            if (deltax > deltay) //Low Angle line
            {
        	    Coordinate nextPoint = start;
                int error = deltax / 2;

                if (end.Y < start.Y) { ystep = -1 * ystep; } //down and right or left

                if (end.X > start.X) //down and right
                {
            	    int lastX = start.X;
            	    int lastY = start.Y;
                    for (int x = start.X; x <= end.X; x += xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getGridX(nextPoint);
                        int gridy = getGridY(nextPoint);
                        gv.DrawLine(lastX + gv.oXshift, lastY, nextPoint.X + gv.oXshift, nextPoint.Y, penColor, penWidth);
                        if (mod.currentEncounter.encounterTiles[gridy * mod.currentEncounter.MapSizeX + gridx].LoSBlocked)
                        {
                    	    return false;
                        }
                        lastX = nextPoint.X;
                        lastY = nextPoint.Y;
                    }
                }
                else //down and left
                {
            	    int lastX = start.X;
            	    int lastY = start.Y;                
                    for (int x = start.X; x >= end.X; x -= xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getGridX(nextPoint);
                        int gridy = getGridY(nextPoint);
                        gv.DrawLine(lastX + gv.oXshift, lastY, nextPoint.X + gv.oXshift, nextPoint.Y, penColor, penWidth);
                        if (mod.currentEncounter.encounterTiles[gridy * mod.currentEncounter.MapSizeX + gridx].LoSBlocked)
                        {
                    	    return false;
                        }
                        lastX = nextPoint.X;
                        lastY = nextPoint.Y;
                    }
                }
            }

            else //Low Angle line
            {
        	    Coordinate nextPoint = start;
                int error = deltay / 2;

                if (end.X < start.X) { xstep = -1 * xstep; } //up and right or left

                if (end.Y > start.Y) //up and right
                {
            	    int lastX = start.X;
            	    int lastY = start.Y;                
                    for (int y = start.Y; y <= end.Y; y += ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getGridX(nextPoint);
                        int gridy = getGridY(nextPoint);
                        gv.DrawLine(lastX + gv.oXshift, lastY, nextPoint.X + gv.oXshift, nextPoint.Y, penColor, penWidth);
                        if (mod.currentEncounter.encounterTiles[gridy * mod.currentEncounter.MapSizeX + gridx].LoSBlocked)
                        {
                    	    return false;
                        }
                        lastX = nextPoint.X;
                        lastY = nextPoint.Y;
                    }
                }
                else //up and right
                {
            	    int lastX = start.X;
            	    int lastY = start.Y;                
                    for (int y = start.Y; y >= end.Y; y -= ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getGridX(nextPoint);
                        int gridy = getGridY(nextPoint);
                        gv.DrawLine(lastX + gv.oXshift, lastY, nextPoint.X + gv.oXshift, nextPoint.Y, penColor, penWidth);
                        if (mod.currentEncounter.encounterTiles[gridy * mod.currentEncounter.MapSizeX + gridx].LoSBlocked)
                        {
                    	    return false;
                        }   
                        lastX = nextPoint.X;
                        lastY = nextPoint.Y;
                    }
                }
            }
        
            return true;
        }
        public bool IsAttackFromBehind(Player pc, Creature crt)
        {
            if ((pc.combatLocX >  crt.combatLocX) && (pc.combatLocY >  crt.combatLocY) && (crt.combatFacing == 7)) { return true; }
            if ((pc.combatLocX == crt.combatLocX) && (pc.combatLocY >  crt.combatLocY) && (crt.combatFacing == 8)) { return true; }
            if ((pc.combatLocX <  crt.combatLocX) && (pc.combatLocY >  crt.combatLocY) && (crt.combatFacing == 9)) { return true; }
            if ((pc.combatLocX >  crt.combatLocX) && (pc.combatLocY == crt.combatLocY) && (crt.combatFacing == 4)) { return true; }
            if ((pc.combatLocX <  crt.combatLocX) && (pc.combatLocY == crt.combatLocY) && (crt.combatFacing == 6)) { return true; }
            if ((pc.combatLocX >  crt.combatLocX) && (pc.combatLocY <  crt.combatLocY) && (crt.combatFacing == 1)) { return true; }
            if ((pc.combatLocX == crt.combatLocX) && (pc.combatLocY <  crt.combatLocY) && (crt.combatFacing == 2)) { return true; }
            if ((pc.combatLocX <  crt.combatLocX) && (pc.combatLocY <  crt.combatLocY) && (crt.combatFacing == 3)) { return true; }
            return false;
        }
        public bool IsCreatureAttackFromBehind(Player pc, Creature crt)
        {
            if ((crt.combatLocX > pc.combatLocX) && (crt.combatLocY > pc.combatLocY) && (pc.combatFacing == 7)) { return true; }
            if ((crt.combatLocX == pc.combatLocX) && (crt.combatLocY > pc.combatLocY) && (pc.combatFacing == 8)) { return true; }
            if ((crt.combatLocX < pc.combatLocX) && (crt.combatLocY > pc.combatLocY) && (pc.combatFacing == 9)) { return true; }
            if ((crt.combatLocX > pc.combatLocX) && (crt.combatLocY == pc.combatLocY) && (pc.combatFacing == 4)) { return true; }
            if ((crt.combatLocX < pc.combatLocX) && (crt.combatLocY == pc.combatLocY) && (pc.combatFacing == 6)) { return true; }
            if ((crt.combatLocX > pc.combatLocX) && (crt.combatLocY < pc.combatLocY) && (pc.combatFacing == 1)) { return true; }
            if ((crt.combatLocX == pc.combatLocX) && (crt.combatLocY < pc.combatLocY) && (pc.combatFacing == 2)) { return true; }
            if ((crt.combatLocX < pc.combatLocX) && (crt.combatLocY < pc.combatLocY) && (pc.combatFacing == 3)) { return true; }
            return false;
        }
	    public int getDistance(Coordinate start, Coordinate end)
	    {
		    int dist = 0;
            int deltaX = (int)Math.Abs((start.X - end.X));
            int deltaY = (int)Math.Abs((start.Y - end.Y));
            if (deltaX > deltaY)
                dist = deltaX;
            else
                dist = deltaY;
            return dist;
	    }
	    public bool isWalkable(int x, int y)
	    {
            if (!mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x].Walkable)
            {
        	    return false;
            }
		    foreach (Player p in mod.playerList)
		    {
			    if ((p.combatLocX == x) && (p.combatLocY == y))
			    {
				    if ((!p.isDead()) && (p.hp > 0))
				    {
					    return false;
				    }
			    }
		    }
		    return true;			
	    }
        public Creature isBumpIntoCreature(int x, int y)
        {
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if ((crt.combatLocX == x) && (crt.combatLocY == y))
                {
            	    return crt;
                }
            }
            return null;
        }
        public void LeaveThreatenedCheck(Player pc, int futurePlayerLocationX, int futurePlayerLocationY)
        {
       
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if ((crt.hp > 0) && (!crt.isHeld()))
                {
                    //if started in distance = 1 and now distance = 2 then do attackOfOpportunity
                    //also do attackOfOpportunity if moving within controlled area around a creature, i.e. when distance to cerature after move is still one square
                    //the later makes it harder to circle around a cretaure or break through lines, fighters get more area control this way, allwoing them to protect other charcters with more ease
                    if ( ( (CalcDistance(crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY) == 1)
                        && (CalcDistance(crt.combatLocX, crt.combatLocY, futurePlayerLocationX, futurePlayerLocationY) == 2) ) 
                        || ( (currentMoves > 0) && (CalcDistance(crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY) == 1)
                        && (CalcDistance(crt.combatLocX, crt.combatLocY, futurePlayerLocationX, futurePlayerLocationY) == 1) ) )
                    {
                        if (pc.steathModeOn)
                        {
                            gv.cc.addLogText("<font color='lime'>Avoids Attack of Opportunity due to Stealth</font><BR>");
                        }
                        else
                        {
                            gv.cc.addLogText("<font color='blue'>Attack of Opportunity by: " + crt.cr_name + "</font><BR>");
                            doActualCreatureAttack(pc, crt, 1);
                            if (pc.hp <= 0)
                            {
                                currentMoves = 99;
                            }
                        }
                    }
                }
            }
        }
	            		
	    public Coordinate GetNextProjectileCoordinate(Coordinate startCoor, Coordinate target)
	    {
		    Coordinate nextPoint = new Coordinate();
		    float divider = ((float)animationFrameIndex) / 4.0f;
		    nextPoint.X = getViewportSquareLocX(startCoor.X) * gv.squareSize - (int)((getViewportSquareLocX(startCoor.X) * gv.squareSize - getViewportSquareLocX(target.X) * gv.squareSize) * divider);
            //shift to map viewport location
            nextPoint.X += gv.oXshift + mapStartLocXinPixels;
            if (startCoor.X < target.X)
		    {
                if (nextPoint.X > getPixelLocX(target.X)) { nextPoint.X = getPixelLocX(target.X); }
		    }
		    else
		    {
                if (nextPoint.X < getPixelLocX(target.X)) { nextPoint.X = getPixelLocX(target.X); }
		    }
            nextPoint.Y = getViewportSquareLocY(startCoor.Y) * gv.squareSize - (int)((getViewportSquareLocY(startCoor.Y) * gv.squareSize - getViewportSquareLocY(target.Y) * gv.squareSize) * divider); 
		    if (startCoor.Y < target.Y)
		    {
                if (nextPoint.Y > getPixelLocY(target.Y)) { nextPoint.Y = getPixelLocY(target.Y); }
		    }
		    else
		    {
                if (nextPoint.Y < getPixelLocY(target.Y)) { nextPoint.Y = getPixelLocY(target.Y); }
		    }
		    return nextPoint;
	    }

	    public int CalcPcAttackModifier(Player pc, Creature crt)
        {
            int modifier = 0;
            if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee")) 
        		    || (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).name.Equals("none"))
        		    || (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
            {
                modifier = (pc.strength - 10) / 2;
                //if has critical strike trait use dexterity for attack modifier in melee if greater than strength modifier
                if (pc.knownTraitsTags.Contains("criticalstrike"))
			    {
            	    int modifierDex = (pc.dexterity - 10) / 2;
            	    if (modifierDex > modifier)
            	    {
            		    modifier = (pc.dexterity - 10) / 2;
            	    }
			    }
                //if doing sneak attack, bonus to hit roll
                if (pc.steathModeOn)
                {
    	            if (pc.knownTraitsTags.Contains("sneakattack"))
    			    {
    	        	    //+1 for every 2 levels after level 1
    	        	    int adding = ((pc.classLevel - 1) / 2) + 1;
    	        	    modifier += adding;
    	        	    gv.cc.addLogText("<font color='lime'> sneak attack: +" + adding + " to hit</font><BR>");
    			    }		        
                }
                //all attacks of the PC from behind get a +2 bonus to hit            
                if (IsAttackFromBehind(pc, crt))
                {
                    modifier += mod.attackFromBehindToHitModifier;
                    if (mod.attackFromBehindToHitModifier > 0)
                    {
                        gv.cc.addLogText("<font color='lime'> Attack from behind: +" + mod.attackFromBehindToHitModifier.ToString() + " to hit." + "</font><BR>");
                    }
                }
            }
            else //ranged weapon used
            {
                modifier = (pc.dexterity - 10) / 2;
                //factor in penalty for adjacent enemies when using ranged weapon
                if (isAdjacentEnemy(pc))
                {
            	    if (gv.sf.hasTrait(pc, "pointblankshot"))
            	    {
            		    //has point blank shot trait, no penalty
            	    }
            	    else
            	    {
	            	    modifier -= 4;
	            	    gv.cc.addLogText("<font color='yellow'>" + "-4 ranged attack penalty" + "</font><BR>");
	            	    gv.cc.addLogText("<font color='yellow'>" + "with enemies in melee range" + "</font><BR>");
	            	    gv.cc.floatyTextOn = true;
	            	    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), "-4 att", "yellow");
	            	    gv.postDelayed("doFloatyText", 100);
            	    }
                }
                if (gv.sf.hasTrait(pc, "preciseshot2"))
        	    {
        		    modifier += 2;
        		    gv.cc.addLogText("<font color='lime'> PreciseShotL2: +2 to hit</font><BR>");
        	    }
                else if (gv.sf.hasTrait(pc, "preciseshot"))
        	    {
            	    modifier++;
            	    gv.cc.addLogText("<font color='lime'> PreciseShotL1: +1 to hit</font><BR>");
        	    }
            }
            if (gv.sf.hasTrait(pc, "hardtokill"))
            {
                modifier -= 2;
                gv.cc.addLogText("<font color='yellow'>" + "blinded by rage" + "</font><BR>");
                gv.cc.addLogText("<font color='yellow'>" + "-2 attack penalty" + "</font><BR>");
            }
            int attackMod = modifier + pc.baseAttBonus + mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attackBonus;        
            Item it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
            if (it != null)
            {
        	    attackMod += mod.getItemByResRefForInfo(pc.AmmoRefs.resref).attackBonus;
            }
            return attackMod;
        }
	    public int CalcCreatureDefense(Player pc, Creature crt)
        {
            int defense = crt.AC;
            if (crt.isHeld())
            {
        	    defense -= 4;
        	    gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), "+4 att", "green");
            }            
            return defense;
        }
	    public int CalcPcDamageToCreature(Player pc, Creature crt)
        {
            int damModifier = 0;
            int adder = 0;
            bool melee = false;
            if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee")) 
        		    || (pc.MainHandRefs.name.Equals("none"))
        		    || (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
            {
        	    melee = true;
        	    damModifier = (pc.strength - 10) / 2;
        	    //if has critical strike trait use dexterity for damage modifier in melee if greater than strength modifier
        	    if (gv.sf.hasTrait(pc, "criticalstrike"))
        	    {
        		    int damModifierDex = (pc.dexterity - 10) / 4;    
        		    if (damModifierDex > damModifier)
            	    {
        			    damModifier = (pc.dexterity - 10) / 2;
            	    }
        	    }
                
                if (IsAttackFromBehind(pc, crt))
                {
                    damModifier += mod.attackFromBehindDamageModifier;
                    if (mod.attackFromBehindDamageModifier > 0)
                    {
                        gv.cc.addLogText("<font color='lime'> Attack from behind: +" + mod.attackFromBehindDamageModifier.ToString() +  " damage." + "</font><BR>");
                    }
                }


            }
            else //ranged weapon used
            {
                damModifier = 0;  
                if (gv.sf.hasTrait(pc, "preciseshot2"))
	    	    {
	    		    damModifier += 2;
	    		    gv.cc.addLogText("<font color='lime'> PreciseShotL2: +2 damage</font><BR>");
	    	    }
                else if (gv.sf.hasTrait(pc, "preciseshot"))
	    	    {
            	    damModifier++;
            	    gv.cc.addLogText("<font color='lime'> PreciseShotL1: +1 damage</font><BR>");
	    	    }
	    	
            }            

            int dDam = mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageDie;
            float damage = (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageNumDice * gv.sf.RandInt(dDam)) + damModifier + adder + mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageAdder;
            if (damage < 0)
            {
                damage = 0;
            }
            Item it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
            if (it != null)
            {
        	    damage += mod.getItemByResRefForInfo(pc.AmmoRefs.resref).damageAdder;
            }

            float resist = 0;

            if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Acid"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueAcid / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Normal"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueNormal / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Cold"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueCold / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Electricity"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueElectricity / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Fire"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Magic"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueMagic / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Poison"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValuePoison / 100f));
            }        

            int totalDam = (int)(damage * resist);
            if (totalDam < 0)
            {
                totalDam = 0;
            }
            //if doing sneak attack, does extra damage
            if ((pc.steathModeOn) && (melee) && (IsAttackFromBehind(pc,crt)))
            {
	            if (pc.knownTraitsTags.Contains("sneakattack"))
			    {
	        	    //+1d6 for every 2 levels after level 1
	        	    int multiplier = ((pc.classLevel - 1) / 2) + 1;
	        	    int adding = 0;
	        	    for (int i = 0; i < multiplier; i++)
	        	    {
	        		    adding += gv.sf.RandInt(6);
	        	    }
				    totalDam += adding;
				    gv.cc.addLogText("<font color='lime'> sneak attack: +" + adding + " damage</font><BR>");
			    }		        
            }
            return totalDam;
        }
	    public int CalcCreatureAttackModifier(Creature crt, Player pc)
        {
            if ((crt.cr_category.Equals("Ranged")) && (isAdjacentPc(crt)))
		    {
			    gv.cc.addLogText("<font color='yellow'>" + "-4 ranged attack penalty" + "</font>" +
        			    "<BR>");
        	    gv.cc.addLogText("<font color='yellow'>" + "with enemies in melee range" + "</font>" +
        			    "<BR>");
        	    gv.cc.floatyTextOn = true;
        	    gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), "-4 att", "yellow");
        	    gv.postDelayed("doFloatyText", 100);
        	    return crt.cr_att - 4; 
            }
            else //melee weapon used
            {
                int modifier = 0;
                //all attacks of the Creature from behind get a +2 bonus to hit            
                if (IsCreatureAttackFromBehind(pc, crt))
                {
                    modifier += 2;
                    gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " attacks from behind: +2 att</font><BR>");
                }
        	    return crt.cr_att + modifier;            
            }
        }
	    public int CalcPcDefense(Player pc, Creature crt)
        {
            //pc.UpdateStats(this);
		    int defense = pc.AC;
            if (pc.isHeld())
            {
        	    defense -= 4;
        	    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), "+4 att", "yellow");
            }
            return defense;
        }
        public int CalcCreatureDamageToPc(Player pc, Creature crt)
        {
            int dDam = crt.cr_damageDie;
            float damage = (crt.cr_damageNumDice * gv.sf.RandInt(dDam)) + crt.cr_damageAdder;
            if (damage < 0)
            {
                damage = 0;
            }

            float resist = 0;

            if (crt.cr_typeOfDamage.Equals("Acid"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalAcid / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Normal"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalNormal / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Cold"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalCold / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Electricity"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalElectricity / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Fire"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Magic"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalMagic / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Poison"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalPoison / 100f));
            }  

            int totalDam = (int)(damage * resist);
            if (totalDam < 0)
            {
                totalDam = 0;
            }

            return totalDam;
        }

        public Player targetClosestPC(Creature crt)
        {
            Player pc = null;
            int farDist = 99;
            foreach (Player p in mod.playerList)
            {
                if ((!p.isDead()) && (p.hp >= 0) && (!p.steathModeOn))
                {
                    int dist = CalcDistance(crt.combatLocX, crt.combatLocY, p.combatLocX, p.combatLocY);
                    if (dist == farDist)
                    {
                	    //since at same distance, do a random check to see if switch or stay with current PC target
                	    if (gv.sf.RandInt(20) > 10)
                	    {
                		    //switch target
                		    pc = p;
                		    if (gv.mod.debugMode)
						    {
                			    gv.cc.addLogText("<font color='yellow'>target:" + pc.name + "</font><BR>");
						    }
                	    }                	
                    }
                    else if (dist < farDist)
                    {
                        farDist = dist;
                        pc = p;
                        if (gv.mod.debugMode)
					    {
            			    gv.cc.addLogText("<font color='yellow'>target:" + pc.name + "</font><BR>");
					    }
                    }
                }
            }
            return pc;
        }
        public Coordinate targetBestPointLocation(Creature crt)
        {
            Coordinate targetLoc = new Coordinate(-1,-1);
            //JamesManhattan Utility maximization function for the VERY INTELLIGENT CREATURE CASTER
            int utility = 0; //utility
            int optimalUtil = 0; //optimal utility, a storage of the highest achieved
            Coordinate selectedPoint = new Coordinate(crt.combatLocX, crt.combatLocY); //Initial Select Point is Creature itself, then loop through all squares within range!
            for (int y = gv.sf.SpellToCast.range; y > -gv.sf.SpellToCast.range; y--)  //start at far range and work backwards does a pretty good job of avoiding hitting allies.
            {
                for (int x = gv.sf.SpellToCast.range; x > -gv.sf.SpellToCast.range; x--)
                {
                    utility = 0; //reset utility for each point tested
                    selectedPoint = new Coordinate(crt.combatLocX + x, crt.combatLocY + y);
                
                    //check if selected point is a valid location on combat map
                    if ((selectedPoint.X < 0) || (selectedPoint.X > mod.currentEncounter.MapSizeX - 1) || (selectedPoint.Y < 0) || (selectedPoint.Y > mod.currentEncounter.MapSizeY - 1))
                    {
                	    continue;
                    }
                
                    //check if selected point is in LoS, if not skip this point
                    int endX = selectedPoint.X * gv.squareSize + (gv.squareSize / 2);
        		    int endY = selectedPoint.Y * gv.squareSize + (gv.squareSize / 2);
        		    int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
        		    int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
                    if (!isVisibleLineOfSight(new Coordinate(endX, endY), new Coordinate(startX, startY)))
                    {
                	    continue;
                    }
                
                    if (selectedPoint == new Coordinate(crt.combatLocX, crt.combatLocY))
                    {
                        utility -= 4; //the creature at least attempts to avoid hurting itself, but if surrounded might fireball itself!
                        if (crt.hp <= crt.hpMax / 4) //caster is wounded, definately avoids itself.
                        {
                            utility -= 4;
                        }
                    } 
                    foreach (Creature crtr in mod.currentEncounter.encounterCreatureList) //if its allies are in the burst subtract a point, or half depending on how evil it is.
                    {
                        if (this.CalcDistance(crtr.combatLocX, crtr.combatLocY, selectedPoint.X, selectedPoint.Y) <= gv.sf.SpellToCast.aoeRadius) //if friendly creatures are in the AOE burst, count how many, subtract 0.5 for each, evil is evil
                        {
                            utility -= 1;
                        }
                    }
                    foreach (Player tgt_pc in mod.playerList)
                    {
                        if ((this.CalcDistance(tgt_pc.combatLocX, tgt_pc.combatLocY, selectedPoint.X, selectedPoint.Y) <= gv.sf.SpellToCast.aoeRadius) && (tgt_pc.hp > 0)) //if players are in the AOE burst, count how many, total count is utility  //&& sf.GetLocalInt(tgt_pc.Tag, "StealthModeOn") != 1  <-throws an annoying message if not found!!
                        {
                            utility += 2;
                            if (utility > optimalUtil)
                            {
                                //optimal found, choose this point
                                optimalUtil = utility;
                                targetLoc = selectedPoint;
                            }
                        }
                    }
                    if (gv.mod.debugMode)
				    {
        			    gv.cc.addLogText("<font color='yellow'>(" + selectedPoint.X + "," + selectedPoint.Y + "):" + utility + "</font><BR>");
				    }
                }
            }

            return targetLoc;
        }
	    public int CalcDistance(int locCrX, int locCrY, int locPcX, int locPcY)
        {
            int dist = 0;
            int deltaX = (int)Math.Abs((locCrX - locPcX));
            int deltaY = (int)Math.Abs((locCrY - locPcY));
            if (deltaX > deltaY)
                dist = deltaX;
            else
                dist = deltaY;
            return dist;
        }
	    public Creature GetCreatureWithLowestHP()
        {
            int lowHP = 999;
            Creature returnCrt = null;
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if (crt.hp > 0)
                {
                    if (crt.hp < lowHP)
                    {
                        lowHP = crt.hp;
                        returnCrt = crt;
                    }
                }
            }
            return returnCrt;
        }
	    public Creature GetCreatureWithMostDamaged()
        {
            int damaged = 0;
            Creature returnCrt = null;
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {        	
                if (crt.hp > 0)
                {
            	    int dam = crt.hpMax - crt.hp;
                    if (dam > damaged)
                    {
                	    damaged = dam;
                        returnCrt = crt;
                    }
                }
            }
            return returnCrt;
        }
	    public Creature GetNextAdjacentCreature(Player pc)
        {
            foreach (Creature nextCrt in mod.currentEncounter.encounterCreatureList)
            {
                if ((CalcDistance(nextCrt.combatLocX, nextCrt.combatLocY, pc.combatLocX, pc.combatLocY) < 2) && (nextCrt.hp > 0))
                {
                    return nextCrt;
                }
            }
            return null;
        }
	    public Creature GetCreatureByTag(String tag)
        {
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if (crt.cr_tag.Equals(tag))
                {
                    return crt;
                }
            }
            return null;
        }
    }
}
