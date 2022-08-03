using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Terri_Fried.DesktopGL
{
    // This class contains all extra functionality for compatibility with Terri-Fried (pressed and released bools, mouse position, DrawLineEx())
    public class Extra
    {
        public RaylibMouseState gameRBMouseState; // Stores the game's mouse state and substitutes non-existent Raylib mouse functions (see function comments for description)
        public RaylibMouseState rbMouseState; // Stores the actual mouse state and substitutes non-existent Raylib mouse functions (see function comments for description)
        public MouseState mouseState; // Stores the mouse state (X and Y, which is pressed and released, etc.)
        public KeyboardState keyState; // Stores the keyboard state (keys pressed and released)
        public PauseState pauseState; // Stores pressed and released bools for pause button 'p'
        Texture2D pixel; // Stores the texture used to create a line through the DrawLineEx() function

        public Extra(GraphicsDevice _graphicsDevice)
        {
            pixel = new Texture2D(_graphicsDevice, 1, 1);
            pixel.SetData(new Color[1] { new Color(new Vector4(.906f, .847f, .788f, 1.0f)) }); // Only colour used for lines (constantly creating textures is very slow)

            gameRBMouseState = new RaylibMouseState();
            rbMouseState = new RaylibMouseState();
            pauseState = new PauseState();
        }

        // Updates all mouse states (pressed, released mouse position)
        public void Update(bool paused)
        {
            mouseState = Mouse.GetState(); // Updates the Raylib-like mouse functions
            keyState = Keyboard.GetState();

            rbMouseState.Update();
            rbMouseState.buttonState = mouseState.LeftButton;
            if (!paused)
            {
                gameRBMouseState.Update();
                gameRBMouseState.buttonState = mouseState.LeftButton;
            }
            pauseState.Update();
            pauseState.keyState = keyState.IsKeyDown(Keys.P);
        }

        public void Draw(GraphicsDevice _graphicsDevice, SpriteBatch _spriteBatch, RenderTarget2D rt, RenderTarget2D pauseMenu, bool paused)
        {
            _graphicsDevice.SetRenderTarget(null); // Renders all sprites in rt to screen
            _graphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp); // Prevents anti-aliasing
            _spriteBatch.Draw(rt, Vector2.Zero, Color.White); // Draws the rt to fit to the screen
            if (paused) { _spriteBatch.Draw(pauseMenu, Vector2.Zero, Color.White); } // Adds fade effect to screen
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