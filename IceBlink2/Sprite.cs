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
        public Bitmap Img = null;
        public Vector2 position = new Vector2(0, 0);  // The current position of the sprite        
        public Vector2 velocity = new Vector2(0, 0);  // The speed of the sprite at the current instance
        public float angle = 0;                       // The current angle of rotation of the sprite
        public float angularVelocity = 0;             // The speed that the angle is changing
        public float scale = 1.0f;                    // The scale of the sprite
        public int timeToLiveInMilliseconds = 1000;   // The 'time to live' of the sprite in milliseconds
        
        public Sprite(string bitmap, float positionX, float positionY, float velocityX, float velocityY, float angle, float angularVelocity, float scale, int timeToLiveInMilliseconds)
        {
            this.bitmap = bitmap;
            this.position = new Vector2(positionX, positionY);
            this.velocity = new Vector2(velocityX, velocityY);
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.scale = scale;
            this.timeToLiveInMilliseconds = timeToLiveInMilliseconds;
        }
        public Sprite(Bitmap bitmap, float positionX, float positionY, float velocityX, float velocityY, float angle, float angularVelocity, float scale, int timeToLiveInMilliseconds)
        {
            this.Img = bitmap;
            this.position = new Vector2(positionX, positionY);
            this.velocity = new Vector2(velocityX, velocityY);
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.scale = scale;
            this.timeToLiveInMilliseconds = timeToLiveInMilliseconds;
        }
        public Sprite(string bitmap, Vector2 position, Vector2 velocity, float angle, float angularVelocity, float scale, int timeToLiveInMilliseconds)
        {
            this.bitmap = bitmap;
            this.position = position;
            this.velocity = velocity;
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.scale = scale;
            this.timeToLiveInMilliseconds = timeToLiveInMilliseconds;
        }

        public void Update(int elapsed)
        {
            timeToLiveInMilliseconds -= elapsed;
            position += velocity * elapsed;
            angle += angularVelocity * elapsed;
        }

        public void Draw(GameView gv)
        {
            int pH = (int)((float)gv.screenHeight / 200.0f);
            int pW = (int)((float)gv.screenHeight / 200.0f);

            IbRect src = new IbRect(0, 0, this.Img.PixelSize.Width, this.Img.PixelSize.Height);            
            IbRect dst = new IbRect((int)this.position.X, (int)this.position.Y, (int)((float)this.Img.PixelSize.Width * gv.screenDensity), (int)((float)this.Img.PixelSize.Height * gv.screenDensity));
                        
            gv.DrawBitmap(this.Img, src, dst, angle, false, false);
            //spriteBatch.Draw(Texture, Position, sourceRectangle, Color, Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }    
}
