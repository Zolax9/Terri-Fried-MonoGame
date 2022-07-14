using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Terri_Fried.DesktopGL
{
    // THis class contains all extra functionality for compatibility with Terri-Fried (pressed and released bools, mouse position, DrawLineEx())
    public class Extra
    {
        public RaylibMouseState raylibMouseState; // Stores the mouse state and substitutes non-existent Raylib mouse functions (see function comments for description)
        public MouseState mouseState; // Stores the mouse state (X and Y, which is pressed and released, etc.)
        Texture2D pixel; // Stores the texture used to create a line through the DrawLineEx() function

        public Extra(GraphicsDevice _graphicsDevice)
        {
            pixel = new Texture2D(_graphicsDevice, 1, 1);
            pixel.SetData(new Color[1] { new Color(new Vector4(.906f, .847f, .788f, 1.0f)) }); // Only colour used for lines (constantly creating textures is very slow)

            raylibMouseState = new RaylibMouseState();
        }

        // Updates all mouse states (pressed, released mouse position)
        public void Update()
        {
            raylibMouseState.Update();
            raylibMouseState.buttonState = mouseState.LeftButton;
            mouseState = Mouse.GetState();
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