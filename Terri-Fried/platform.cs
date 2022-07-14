using System;

namespace Terri_Fried
{
    public class Platform
    {
        // Changed from constant to allow for different aspect ratios (important for Android)
        int screenWidth;
        int screenHeight;

        double x;
        double y;
        int width;
        int height;
        bool hasCoin;
        int coinX;
        int coinY;

        // Added variables
        Random rand = new Random();

        public Platform(int index, int screenWidth = 800, int screenHeight = 450)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            width = 100;
            height = 32;
            x = rand.Next(20, screenWidth - 120);
            y = 0 - height - (index * 100);
            int coinInt = rand.Next(0, 4);
            if (coinInt == 0 || index == 0)
            {
                hasCoin = false;
            } else {
                hasCoin = true;
            }
            coinX = (int)x + width/2 - 24/2;
            coinY = (int)y - 24 - 5;
            
        }

        public double getX()
        {
            return x;
        }

        public double getY()
        {
            return y;
        }

        public int getWidth()
        {
            return width;
        }

        public int getHeight()
        {
            return height;
        }

        public bool getHasCoin()
        {
            return hasCoin;
        }
        public void setHasCoin(bool value)
        {
            hasCoin = value;
        }
        public int getCoinX()
        {
            return coinX;
        }
        public int getCoinY()
        {
            return coinY;
        }

        public void updatePosition()
        {
            y+=1;
            coinX = (int)x + width/2 - 24/2;
            coinY = (int)y - 24 - 5;
            if (y > screenHeight)
            {
                x = rand.Next(20, screenWidth - 120);
                y = 0 - height;
                int coinInt = rand.Next(0, 4);
                if (coinInt == 0)
                {
                    hasCoin = false;
                } else {
                    hasCoin = true;
                }
            }
        }
    }
}