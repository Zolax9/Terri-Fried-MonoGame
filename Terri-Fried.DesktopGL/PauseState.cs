using Microsoft.Xna.Framework.Input;

namespace Terri_Fried.DesktopGL
{
    // This class represents the IsKeyPressed() and IsKeyReleased() functions of Raylib
    public class PauseState
    {
        public bool _keyState;
        public bool keyState
        {
            get => _keyState;
            set
            {
                if (_keyState != value)
                {
                    switch (value)
                    {
                        case true:
                            Pressed = true;

                            Up = false;
                            Down = true;
                            break;

                        case false:
                            Released = true;

                            Up = true;
                            Down = false;
                            break;
                    }
                }
                _keyState = value;
            }
        }

        public bool Pressed;
        public bool Released;

        public bool Up;
        public bool Down;

        public void Update()
        {
            Pressed = false;
            Released = false;
        }
    }
}