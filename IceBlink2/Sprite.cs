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

        //mostly internal to this class only
        public int currentFrameIndex = 0;
        public int numberOfFrames = 1;
        public int frameHeight = 0;
        public int totalElapsedTime = 0;
        
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

            if (millisecondsPerFrame == 0) { millisecondsPerFrame = 100; }
            frameHeight = gv.cc.GetFromBitmapList(bitmap).PixelSize.Height;
            numberOfFrames = gv.cc.GetFromBitmapList(bitmap).PixelSize.Width / frameHeight;
        }
        
        public void Update(int elapsed)
        {
            timeToLiveInMilliseconds -= elapsed;
            totalElapsedTime += elapsed;
            position += velocity * elapsed;
            angle += angularVelocity * elapsed;
            int x = totalElapsedTime % (numberOfFrames * millisecondsPerFrame);
            currentFrameIndex = x / millisecondsPerFrame;            
        }

        public void Draw(GameView gv)
        {            
            IbRect src = new IbRect(currentFrameIndex * frameHeight, 0, frameHeight, frameHeight);
            IbRect dst = new IbRect((int)this.position.X, (int)this.position.Y, gv.squareSize, gv.squareSize);
            gv.DrawBitmap(gv.cc.GetFromBitmapList(bitmap), src, dst, angle, false);            
        }
    }    
}
