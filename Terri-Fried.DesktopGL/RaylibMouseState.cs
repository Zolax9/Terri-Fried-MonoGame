using Microsoft.Xna.Framework.Input;

namespace Terri_Fried.DesktopGL
{
    // This class represents the IsMouseButtonPressed() and IsMouseButtonReleased() functions of Raylib
    public class RaylibMouseState
    {
        public ButtonState _buttonState;
        public ButtonState buttonState
        {
            get => _buttonState;
            set
            {
                if (_buttonState != value)
                {
                    switch (value)
                    {
                        case ButtonState.Pressed:
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
                _buttonState = value;
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