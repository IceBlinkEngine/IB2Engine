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
        public float scale = 1.0f;                    // The scale of the sprite
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

        //more ideas for later: isShadowCaster, extend vector and position by z-coordinate, hardness (simulate shatter on impact as well as speed loss on collision due energy going into deformation)

        //mostly internal to this class only
        public int currentFrameIndex = 0;
        public int numberOfFrames = 1;
        public int frameHeight = 0;
        public int totalElapsedTime = 0;

        //overloaded constructor: complexSprite 
        public Sprite(GameView gv, string bitmap, float positionX, float positionY, float velocityX, float velocityY, float angle, float angularVelocity, float scale, int timeToLiveInMilliseconds, bool permanent, int msPerFrame, float opacity, float mass, string movementMethod, bool movesIndependentlyFromPlayerPosition)
        {
            this.bitmap = bitmap;
            this.position = new Vector2(positionX, positionY);
            this.velocity = new Vector2(velocityX, velocityY);
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.scale = scale;
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
            this.scale = scale;
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
                position += velocity * elapsed;
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
                position += velocity * elapsed;
                opacity = gv.mod.fullScreenEffectOpacityWeather;
            }

            int x = totalElapsedTime % (numberOfFrames * millisecondsPerFrame);
            currentFrameIndex = x / millisecondsPerFrame;            
        }

        public void Draw(GameView gv)
        {            
            IbRect src = new IbRect(currentFrameIndex * frameHeight, 0, frameHeight, frameHeight);
            IbRect dst = new IbRect((int)this.position.X, (int)this.position.Y, (int)(gv.squareSize * this.scale), (int)(gv.squareSize*this.scale));
            gv.DrawBitmap(gv.cc.GetFromBitmapList(bitmap), src, dst, angle, false, this.opacity);            
        }
    }    
}
