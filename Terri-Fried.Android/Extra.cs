using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace Terri_Fried.Android
{
    // This class contains all extra functionality for compatibility with Terri-Fried (pressed and released bools (touchscreen), touch position, code to scale to screen, DrawLineEx())
    public class Extra
    {
        public RaylibMouseState gameRBMouseState; // Stores the game's mouse state and substitutes non-existent Raylib mouse functions (see function comments for description)
        public RaylibMouseState rbMouseState; // Stores the actual mouse state and substitutes non-existent Raylib mouse functions (see function comments for description)

        Texture2D pixel; // Stores the texture used to create a line through the DrawLineEx() function
        public float aspectMultiplier; // Contains the multiplier needed to fit the actual game window onto the screen
        int displayWidth; // Width of the actual screen
        int displayHeight; // Height of the actual screen
        int screenWidth; // Width of Terri-Fried
        int screenHeight; // Height of Terri-Fried

        public Extra(GraphicsDevice _graphicsDevice, int displayWidth, int displayHeight, int screenWidth, ref int screenHeight)
        {
            this.displayWidth = displayWidth;
            this.displayHeight = displayHeight;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            this.screenHeight = (int)(screenHeight * ((float)displayHeight / screenHeight) / ((float)displayWidth / screenWidth)); // Changes screen height to match aspect ratio
            screenHeight = this.screenHeight;
            aspectMultiplier = (float)displayWidth / this.screenWidth; // Declares the multiplier needed to fit the Terri-Fried instance to the screen
            
            pixel = new Texture2D(_graphicsDevice, 1, 1);
            pixel.SetData(new Color[1] { new Color(new Vector4(.906f, .847f, .788f, 1.0f)) }); // Only colour used for lines (constantly creating textures is very slow)

            gameRBMouseState = new RaylibMouseState((float)screenWidth / displayWidth, (float)screenHeight / displayHeight);
            rbMouseState = new RaylibMouseState((float)screenWidth / displayWidth, (float)screenHeight / displayHeight);
        }

        public void Update(bool paused)
        {
            rbMouseState.Update();
            rbMouseState.touchCollState = TouchPanel.GetState();
            if (!paused)
            {
                gameRBMouseState.Update();
                gameRBMouseState.touchCollState = TouchPanel.GetState();
            }
        }

        public void Draw(GraphicsDevice _graphicsDevice, SpriteBatch _spriteBatch, RenderTarget2D rt, RenderTarget2D pauseMenu, bool paused)
        {
            _graphicsDevice.SetRenderTarget(null); // Renders all sprites in rt to screen
            _graphicsDevice.Clear(Color.Gray); // Any gaps in the screen show a grey border

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp); // Prevents anti-aliasing
            _spriteBatch.Draw(rt, new Rectangle((displayWidth - (int)(screenWidth * aspectMultiplier)) / 2, (displayHeight - (int)(screenHeight * aspectMultiplier)) / 2, (int)(screenWidth * aspectMultiplier), (int)(screenHeight * aspectMultiplier)), new Rectangle(0, 0, screenWidth, screenHeight), Color.White); // Draws the rt to fit to the screen
            if (paused) { _spriteBatch.Draw(pauseMenu, new Rectangle(0, 0, (int)(screenWidth * aspectMultiplier), (int)(screenHeight * aspectMultiplier)), Color.White); } // Adds fade effect to screen
            _spriteBatch.End();
        }

        // Represents the DrawLineEx() function
        public void DrawLineEx(SpriteBatch _spriteBatch, Vector2 begin, Vector2 end, int width, Color color) // Copied from Cryal's answer @ https://stackoverflow.com/questions/16403809/drawing-lines-in-c-sharp-with-xna
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = 2 * (float)Math.PI - angle;
            _spriteBatch.Draw(pixel, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}