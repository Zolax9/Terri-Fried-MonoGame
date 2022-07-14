using Microsoft.Xna.Framework;

namespace Terri_Fried
{
    public class Player
    {
        const double pi = 3.1415926535897;
        const int gravity = 1;
        // Changed from constant to allow for different aspect ratios (important for Android)
        int screenWidth;
        int screenHeight;

        double x;
        double y;
        int width;
        int height;
        bool onPlatform;
        Vector2 velocity;

        public Player(double x, double y, int width, int height, int screenWidth = 800, int screenHeight = 450)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            onPlatform = false;
        }

        public double getX()
        {
            return x;
        }

        public double getY()
        {
            return y;
        }

        public void setX(double x) // Changed from an int to a double
        {
            this.x = x;
        }

        public void setY(double y) // Changed from an int to a double
        {
            this.y = y;
        }

        public int getWidth()
        {
            return width;
        }

        public int getHeight()
        {
            return height;
        }

        public bool isOnGround()
        {
            return onPlatform;
        }
        public bool isOnPlatform()
        {
            return onPlatform;
        }

        public void setOnPlatform(bool result)
        {
            onPlatform = result;
        }

        public void setVelocity(double x, double y)
        {
            velocity = new Vector2((float)x, (float)y);
        }

        public Vector2 getVelocity()
        {
            return velocity;
        }

        public void updatePosition()
        {
            x += velocity.X;
            y += velocity.Y;
            if (!isOnGround())
            {
                velocity.Y += gravity;
            } else 
            {
                velocity = new Vector2(0, 0);
            }
            if (x < 0)
            {
                velocity.X *= -1;
            }
            if (x + width > screenWidth)
            {
                velocity.X *= -1;
            }
        }
    }
}