using Microsoft.Xna.Framework.Input.Touch;

namespace Terri_Fried.Android
{
    // This class represents the IsMouseButtonPressed() and IsMouseButtonReleased() functions of Raylib (and for Android, IsMouseButtonUp() and IsMouseButtonDown())
    public class RaylibMouseState
    {
        public TouchCollection _touchCollState;
        public TouchCollection touchCollState
        {
            get => _touchCollState;
            set
            {
                bool valueDown;

                switch (value.Count)
                {
                    case 0:
                        valueDown = false;

                        // Keeps the touch location the same if the screen isn't being touched
                        break;

                    default:
                        valueDown = true;

                        // Changes the touch location to the first touch point (first index and finger) if there are any touch points
                        X = (int)value[0].Position.X;
                        Y = (int)value[0].Position.Y;
                        break;
                }

                if (Down != valueDown)
                {
                    switch (valueDown)
                    {
                        case true:
                            Pressed = true;

                            Up = false;
                            Down = true;
                            break;

                        default:
                            Released = true;

                            Up = true;
                            Down = false;
                            break;
                    }
                }
            }
        }

        public bool Pressed;
        public bool Released;

        public bool Up;
        public bool Down;

        public int X;
        public int Y;

        public void Update()
        {
            Pressed = false;
            Released = false;
        }
    }
}