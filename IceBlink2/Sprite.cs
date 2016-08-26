using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class Sprite
    {
        public string bitmap = "blank";               // filename of bitmap, do NOT include filename extension
        public Vector2 position = new Vector2(0, 0);  // The current position of the sprite        
        public Vector2 velocity = new Vector2(0, 0);  // The speed of the sprite at the current instance
        public float angle = 0;                       // The current angle of rotation of the sprite
        public float angularVelocity = 0;             // The speed that the angle is changing
        public float scaleX = 1.0f;                    // The X-scale of the sprite
        public float scaleY = 1.0f;                    // The Y-scale of the sprite
        public int timeToLiveInMilliseconds = 1000;   // The 'time to live' of the sprite in milliseconds after the startTimeInMilliseconds
        public int millisecondsPerFrame = 100;        // The amount of time (ms) before switching to next frame  
        public bool permanent = false;
        public bool movesIndependentlyFromPlayerPosition = false; //when party moves sprite position is adjusted in Common Code's doUpdate function to make it move independently

        //new stuff yet to add into the calculations
        public float opacity = 1.0f;                  //The transparency of the sprite, at 1.0f it's totally solid, at 0 it's invisible
        public string movementMethod = "linear";      //They way the sprite is moved across screen (i.e. how the velocities are used to determine new position)
        public float mass = 0;                        //Might be used later for determining the effects of collission
        public string spriteType = "normalSprite";    //to make different types of srpites identifiable for the calculations in update(elapsed)
        public float screenWidth = 0;
        public float screenHeight = 0;
        public float xShift = 0;
        public bool reverseXShift = false;
        //This is an alterntive frame counter for animations stitched together from several separate bitmaps (vs. a horizintal sprite sheet)
        //a value of 0 inidcates that no sprite chnaging animations is called for
        //a value of 1 or more is the number of the current frame starting with 1, the number is always added to the end of the bitmap(s) name
        //like flame1.bmp, flame2.bmp, flame3.bmp and so forth 
        public int numberOFFramesForAnimationsMadeFromSeveralBitmaps = 0;

        //more ideas for later: isShadowCaster, extend vector and position by z-coordinate, hardness (simulate shatter on impact as well as speed loss on collision due energy going into deformation)

        //mostly internal to this class only
        public int currentFrameIndex = 0;
        public int numberOfFrames = 1;
        public int frameHeight = 0;
        public int totalElapsedTime = 0;

        //overloaded constructor: complexSprite 
        public Sprite(GameView gv, string bitmap, float positionX, float positionY, float velocityX, float velocityY, float angle, float angularVelocity, float scaleX, float scaleY, int timeToLiveInMilliseconds, bool permanent, int msPerFrame, float opacity, float mass, string movementMethod, bool movesIndependentlyFromPlayerPosition, int numberOFFramesForAnimationsMadeFromSeveralBitmaps)
        {
            this.bitmap = bitmap;
            this.position = new Vector2(positionX, positionY);
            this.velocity = new Vector2(velocityX, velocityY);
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.timeToLiveInMilliseconds = timeToLiveInMilliseconds;
            this.millisecondsPerFrame = msPerFrame;
            this.permanent = permanent;
            this.opacity = opacity;
            this.mass = mass;
            this.movementMethod = movementMethod;
            this.spriteType = "complexSprite";
            this.screenHeight = gv.screenHeight;
            this.screenWidth = gv.screenWidth;
            this.movesIndependentlyFromPlayerPosition = movesIndependentlyFromPlayerPosition;
            this.xShift = xShift;
            this.reverseXShift = reverseXShift;
            this.numberOFFramesForAnimationsMadeFromSeveralBitmaps = numberOFFramesForAnimationsMadeFromSeveralBitmaps;

            if (millisecondsPerFrame == 0) { millisecondsPerFrame = 100; }
            frameHeight = gv.cc.GetFromBitmapList(bitmap).PixelSize.Height;
            numberOfFrames = gv.cc.GetFromBitmapList(bitmap).PixelSize.Width / frameHeight;
        }

        //lean constructor: normal Sprite
        public Sprite(GameView gv, string bitmap, float positionX, float positionY, float velocityX, float velocityY, float angle, float angularVelocity, float scale, int timeToLiveInMilliseconds, bool permanent, int msPerFrame)
        {
            this.bitmap = bitmap;
            this.position = new Vector2(positionX, positionY);
            this.velocity = new Vector2(velocityX, velocityY);
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.scaleX = scale;
            this.scaleY = scale;
            this.timeToLiveInMilliseconds = timeToLiveInMilliseconds;
            this.millisecondsPerFrame = msPerFrame;
            this.permanent = permanent;
            this.spriteType = "normalSprite";

            if (millisecondsPerFrame == 0) { millisecondsPerFrame = 100; }
            frameHeight = gv.cc.GetFromBitmapList(bitmap).PixelSize.Height;
            numberOfFrames = gv.cc.GetFromBitmapList(bitmap).PixelSize.Width / frameHeight;
        }

        public void Update(int elapsed, GameView gv)
        {
            timeToLiveInMilliseconds -= elapsed;
            totalElapsedTime += elapsed;
            if (movementMethod == "linear")
            {
                position += velocity * elapsed;
                angle += angularVelocity * elapsed;
            }
            else if (movementMethod == "clouds")
            {
                position += velocity * elapsed * 0.9f;
                gv.cc.transformSpritePixelPositionOnContactWithVisibleMainMapBorders(this, 1, true, false, 0);
                opacity = gv.mod.fullScreenEffectOpacityWeather;

            }
            else if (movementMethod == "fog")
            {
                position += velocity * elapsed;
                gv.cc.transformSpritePixelPositionOnContactWithVisibleMainMapBorders(this, 0.5f, false, true, 0);
                opacity = gv.mod.fullScreenEffectOpacityWeather;
           
            }
            else if (movementMethod == "rain")
            {
                position += velocity * elapsed * 1.275f;
                opacity = gv.mod.fullScreenEffectOpacityWeather;
            }
            else if (movementMethod == "snow")
            {
                position += velocity * elapsed * 1.1f;
                float shiftAdder = gv.sf.RandInt(300);
                float limitAdder = gv.sf.RandInt(300);

                if (!reverseXShift)
                {
                    xShift = xShift + 0.02f - 0.005f + (shiftAdder/10000);
                }
                if (xShift >= (0.85 + (limitAdder / 1000)))
                {
                    reverseXShift = true;
                }
                if (reverseXShift)
                {
                    xShift = xShift - 0.02f + 0.005f - (shiftAdder / 10000);
                }
                if (xShift <= (-0.85 - (limitAdder / 1000)))
                {
                    reverseXShift = false;
                }

                position.X += (xShift*0.75f);
                //old approach with sin, doing it via customized values like above for now
                //position.X += (float)Math.Sin(position.Y);
                opacity = gv.mod.fullScreenEffectOpacityWeather;
            }
            else if (movementMethod == "sandstorm")
            {
                position += velocity * elapsed * 1.4f;
                opacity = gv.mod.fullScreenEffectOpacityWeather;
            }

            if (this.numberOFFramesForAnimationsMadeFromSeveralBitmaps > 0)
            {
                numberOfFrames = this.numberOFFramesForAnimationsMadeFromSeveralBitmaps;
            }

            if (numberOFFramesForAnimationsMadeFromSeveralBitmaps == 0)
            {
                int x = totalElapsedTime % (numberOfFrames * millisecondsPerFrame);
                currentFrameIndex = x / millisecondsPerFrame;
            }
            else
            {
                int x = (totalElapsedTime % (numberOfFrames * millisecondsPerFrame)) ;
                currentFrameIndex = (x / millisecondsPerFrame) + 1;
            }
            //if ((this.movementMethod == "lightning") && (currentFrameIndex == 1))
                    
                    //{
                //int stop = 0;
                    //}
        }

        public void Draw(GameView gv)
        {
            IbRect src = new IbRect(currentFrameIndex * frameHeight, 0, frameHeight, frameHeight);
            IbRect dst = new IbRect(0, 0, 0, 0);
            //assumes frames of equal proportions
            if (numberOFFramesForAnimationsMadeFromSeveralBitmaps != 0)
            {
                src = new IbRect(0, 0, 150, 150);
            }
            /*
            if (gv.screenType.Equals("combat"))
            {
                int randXInt = gv.sf.RandInt(500);
                float randX = randXInt / 10000f;
                int decider = gv.sf.RandInt(2);
                if (decider == 1)
                {
                    randX = -1 * randX;
                }

                int randYInt = gv.sf.RandInt(500);
                float randY = randXInt / 10000f;
                decider = gv.sf.RandInt(2);
                if (decider == 1)
                {
                    randY = -1 * randY;
                }

                dst = new IbRect((int)this.position.X, (int)(this.position.Y + randY), (int)((gv.squareSize * this.scaleX) + randX), (int)(gv.squareSize * this.scaleY));
            }
            else
            {
            */
                dst = new IbRect((int)this.position.X, (int)this.position.Y, (int)(gv.squareSize * this.scaleX), (int)(gv.squareSize * this.scaleY));
            //}
            float opacityMulti = 1;
            if (this.movementMethod.Contains("fog") || this.movementMethod.Contains("clouds"))
            {
                opacityMulti = 0.64f;
            }

                if (numberOFFramesForAnimationsMadeFromSeveralBitmaps == 0)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(bitmap), src, dst, angle, false, this.opacity * opacityMulti);
            }
            else
            {
                //gv.cc.addLogText("red", currentFrameIndex.ToString());

                gv.DrawBitmap(gv.cc.GetFromBitmapList(bitmap + currentFrameIndex.ToString()), src, dst, angle, false, this.opacity * opacityMulti);

            }   
        }
    }    
}
