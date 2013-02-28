using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ErMyGerdMernsters
{
    public static class ControlManager
    {
        # if WINDOWS
            static ControlManager()
            {
                oldScrollValue = new MouseState().ScrollWheelValue;
                changeControlMethod(ControlMethod.KeyboardMouse);
            }
        #endif

        #if XBOX
            static ControlManager()
            {
                oldScrollValue = new MouseState().ScrollWheelValue;
                changeControlMethod(ControlMethod.Xbox);
            }  
        #endif

        private static ControlMethod _controlType;
        public static ControlMethod ControlType
        {
            get { return _controlType; }
            set { _controlType = value; }
        }

        public static Vector2 mouseVector;
        public static int DELTA_SCROLL;
        private static int oldScrollValue;
        public static PlayerIndex playerIndex = PlayerIndex.One;

        public static Button FIRE;
        public static Button ALT_FIRE;

        public static Button PAUSE;

        //Arrow Keys
        public static Button UP;
        public static Button LEFT;
        public static Button RIGHT;
        public static Button DOWN;

        public static Button WEAPON_SWITCH_UP;
        public static Button WEAPON_SWITCH_DOWN;

        public static Button MENU_SELECT;
        public static Button MENU_RETURN;

        //Num Keys
        public static Button NUM_1;
        public static Button NUM_2;
        public static Button NUM_3; 
        public static Button NUM_4; 
        public static Button NUM_5;
        public static Button NUM_6;
        public static Button NUM_7;
        public static Button NUM_8;
        public static Button NUM_9;
        public static Button NUM_0;
        
        //Xbox Specific Buttons
        private static Button RIGHT_THUMBSTICK;

        public enum ControlMethod
        {
            KeyboardMouse, Xbox
        }

        private enum ToggleState
        {
            ON_PRESSED, ON_UNPRESSED, OFF_PRESSED, OFF_UNPRESSED
        }

        private enum ButtonLoc
        {
            LEFT, RIGHT
        }

        private enum XboxButtonLoc
        {
            LEFT_BUMPER, RIGHT_BUMPER, DPAD_UP, DPAD_DOWN, DPAD_LEFT, DPAD_RIGHT, X, Y, A, B, BACK, START, XBOX_BUTTON, LEFT_STICK, RIGHT_STICK
        }

        public abstract class Button
        {
            public bool Pressed;
            private bool Tap;
            public bool Bumped;
            public bool Toggle;
            public bool DoubleTap;
            private ToggleState ToggleState;

            private const int doubleTapInterval = 2500;
            private int time_Double_Tap = 0;
            private int taps;

            public Button()
            {
                Tap = false;
                Bumped = false;
                Pressed = false;
                Toggle = false;
                ToggleState = ToggleState.OFF_UNPRESSED;
            }

            public abstract void check();

            public void Update(GameTime gt)
            {
                check();
                time_Double_Tap += gt.ElapsedGameTime.Milliseconds;
                if (time_Double_Tap > doubleTapInterval)
                {
                    time_Double_Tap = 0;
                    taps = 0;
                }
                if (Tap)
                {
                    if (Pressed)
                    {
                        Bumped = false;
                    }
                    else
                    {
                        Tap = false;
                    }
                }
                else
                {
                    if (Pressed)
                    {
                        Tap = true;
                        Bumped = true;
                        taps++;
                    }
                    else
                    {
                        Tap = false;
                        Bumped = false;
                    }
                }
                DoubleTap = taps >= 2;
                switch (ToggleState)
                {
                    case ToggleState.ON_PRESSED:
                        if (!Pressed)
                        {
                            ToggleState = ToggleState.ON_UNPRESSED;
                        }
                        break;
                    case ToggleState.ON_UNPRESSED:
                        if (Pressed)
                        {
                            ToggleState = ToggleState.OFF_PRESSED;
                            Toggle = false;
                        }
                        break;
                    case ToggleState.OFF_PRESSED:
                        if (!Pressed)
                        {
                            ToggleState = ToggleState.OFF_UNPRESSED;
                        }
                        break;
                    case ToggleState.OFF_UNPRESSED:
                        if (Pressed)
                        {
                            ToggleState = ToggleState.ON_PRESSED;
                        }
                        break;
                }
            }
        }

        private class NullButton : Button
        {
            public override void check()
            {
                Pressed = false;
            }
        }

        private class KeyboardButton : Button
        {
            public Keys Key;

            public KeyboardButton(Keys key) : base()
            {
                Key = key;
            }

            public void changeKey(Keys key)
            {
                Key = key;
            }

            public override void check()
            {
                KeyboardState keyState = Keyboard.GetState();
                Pressed = keyState.IsKeyDown(Key);
            }
        }

        private class MouseButton : Button
        {
            public ButtonLoc bl;

            public MouseButton(ButtonLoc buttonLoc)
                : base()
            {
                bl = buttonLoc;
            }

            public override void check()
            {
                MouseState mouseState = Mouse.GetState();
                if(bl == ButtonLoc.LEFT)
                    Pressed = mouseState.LeftButton.Equals(ButtonState.Pressed);
                if (bl == ButtonLoc.RIGHT)
                    Pressed = mouseState.RightButton.Equals(ButtonState.Pressed);
            }
        }

        private class TriggerButton : Button
        {
            public ButtonLoc bl;

            public TriggerButton(ButtonLoc buttonLoc)
                : base()
            {
                bl = buttonLoc;
            }

            public override void check()
            {
                if (bl == ButtonLoc.LEFT)
                {
                    Pressed = GamePad.GetState(playerIndex).Triggers.Left != 0;
                }
                if (bl == ButtonLoc.RIGHT)
                {
                    Pressed = GamePad.GetState(playerIndex).Triggers.Right != 0;
                }
            }
        }

        private class JoystickButton : Button
        {
            private ButtonLoc bl;
            private float[] bottomRange;
            private float[] topRange;
            private Vector2 joystickLoc;
            public float joystickAngle;

            public JoystickButton(ButtonLoc buttonLoc, params float[] ranges)
            {
                bl = buttonLoc;
                List<float> br = new List<float>();
                List<float> tr = new List<float>();
                for (int i = 0; i < ranges.Length; i++)
                {
                    if(i == 0)
                        tr.Add(ranges[i]);
                    else if (i % 2 == 0)
                        tr.Add(ranges[i]);
                    else if (i % 2 == 1)
                        br.Add(ranges[i]);
                }
                bottomRange = br.ToArray();
                topRange = tr.ToArray();
            }

            public override void check()
            {
                if (bl == ButtonLoc.LEFT)
                    joystickLoc = GamePad.GetState(playerIndex).ThumbSticks.Left;
                if (bl == ButtonLoc.RIGHT)
                    joystickLoc = GamePad.GetState(playerIndex).ThumbSticks.Right;
                if (joystickLoc != Vector2.Zero)
                {
                    joystickAngle = (float)(Math.Atan2(joystickLoc.Y, joystickLoc.X));
                    bool check = false;
                    for (int i = 0; i < topRange.Length; i++)
                    {
                        check |= joystickAngle < topRange[i] && joystickAngle > bottomRange[i];
                    }
                    Pressed = check;
                }
                else
                {
                    Pressed = false;
                }
            }
        }

        private class XboxButton : Button
        {
            private XboxButtonLoc xbb;

            public XboxButton(XboxButtonLoc x)
            {
                xbb = x;
            }

            public override void check()
            {
                GamePadButtons buttons = GamePad.GetState(playerIndex).Buttons;
                GamePadDPad dpad = GamePad.GetState(playerIndex).DPad;
                switch (xbb)
                {
                    case XboxButtonLoc.LEFT_BUMPER:
                        Pressed = buttons.LeftShoulder.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.RIGHT_BUMPER:
                        Pressed = buttons.RightShoulder.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.DPAD_UP:
                        Pressed = dpad.Up.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.DPAD_DOWN:
                        Pressed = dpad.Down.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.DPAD_LEFT:
                        Pressed = dpad.Left.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.DPAD_RIGHT:
                        Pressed = dpad.Right.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.X:
                        Pressed = buttons.X.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.Y:
                        Pressed = buttons.Y.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.A:
                        Pressed = buttons.A.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.B:
                        Pressed = buttons.B.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.BACK:
                        Pressed = buttons.Back.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.START:
                        Pressed = buttons.Start.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.XBOX_BUTTON:
                        Pressed = buttons.BigButton.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.LEFT_STICK:
                        Pressed = buttons.LeftStick.Equals(ButtonState.Pressed);
                        break;
                    case XboxButtonLoc.RIGHT_STICK:
                        Pressed = buttons.RightStick.Equals(ButtonState.Pressed);
                        break;
                }
            }
        }

        public static void changeControlMethod(ControlMethod newCM)
        {
            _controlType = newCM;
            switch(_controlType)
            {
                case ControlMethod.KeyboardMouse:
                    FIRE = new MouseButton(ButtonLoc.LEFT);
                    ALT_FIRE = new MouseButton(ButtonLoc.RIGHT);

                    PAUSE = new KeyboardButton(Keys.Escape);

                    UP = new KeyboardButton(Keys.W);
                    LEFT = new KeyboardButton(Keys.A);
                    RIGHT = new KeyboardButton(Keys.D);
                    DOWN = new KeyboardButton(Keys.S);

                    NUM_1 = new KeyboardButton(Keys.D1);
                    NUM_2 = new KeyboardButton(Keys.D2);
                    NUM_3 = new KeyboardButton(Keys.D3);
                    NUM_4 = new KeyboardButton(Keys.D4);
                    NUM_5 = new KeyboardButton(Keys.D5);
                    NUM_6 = new KeyboardButton(Keys.D6);
                    NUM_7 = new KeyboardButton(Keys.D7);
                    NUM_8 = new KeyboardButton(Keys.D8);
                    NUM_9 = new KeyboardButton(Keys.D9);
                    NUM_0 = new KeyboardButton(Keys.D0);

                    WEAPON_SWITCH_UP = new KeyboardButton(Keys.E);
                    WEAPON_SWITCH_DOWN = new KeyboardButton(Keys.Q);

                    MENU_SELECT = new KeyboardButton(Keys.Enter);
                    MENU_RETURN = new KeyboardButton(Keys.Back);

                    RIGHT_THUMBSTICK = new NullButton();
                    break;
                case ControlMethod.Xbox:
                    FIRE = new TriggerButton(ButtonLoc.LEFT);
                    ALT_FIRE = new TriggerButton(ButtonLoc.RIGHT);

                    PAUSE = new XboxButton(XboxButtonLoc.START);

                    UP = new JoystickButton(ButtonLoc.LEFT,  (float)(Math.PI * (7.0 / 8.0)), (float)(Math.PI * (1.0 / 8.0)));
                    RIGHT = new JoystickButton(ButtonLoc.LEFT,  (float)(Math.PI * (3.0/ 8.0)), (float)(Math.PI * (-3.0 / 8.0)));
                    LEFT = new JoystickButton(ButtonLoc.LEFT, (float)Math.PI, (float)(Math.PI * (5.0 / 8.0)), 
                                                              (float)(Math.PI * (-5.0 / 8.0)), -(float)Math.PI);
                    DOWN = new JoystickButton(ButtonLoc.LEFT, (float)(Math.PI * (-1.0 / 8.0)), (float)(Math.PI * (-7.0 / 8.0)));

                    NUM_1 = new NullButton();
                    NUM_2 = new NullButton();
                    NUM_3 = new NullButton();
                    NUM_4 = new NullButton();
                    NUM_5 = new NullButton();
                    NUM_6 = new NullButton();
                    NUM_7 = new NullButton();
                    NUM_8 = new NullButton();
                    NUM_9 = new NullButton();
                    NUM_0 = new NullButton();

                    WEAPON_SWITCH_UP = new XboxButton(XboxButtonLoc.RIGHT_BUMPER);
                    WEAPON_SWITCH_DOWN = new XboxButton(XboxButtonLoc.LEFT_BUMPER);

                    MENU_SELECT = new XboxButton(XboxButtonLoc.A);
                    MENU_RETURN = new XboxButton(XboxButtonLoc.B);

                    RIGHT_THUMBSTICK = new JoystickButton(ButtonLoc.RIGHT);
                    break;
            }
        }

        public static void Update(GameTime gt)
        {
            if (!GamePad.GetState(playerIndex).IsConnected)
            {
                Array values = Enum.GetValues(typeof(PlayerIndex));
                foreach (PlayerIndex p in values)
                    if (GamePad.GetState(p).IsConnected)
                    {
                        playerIndex = p;
                        if(_controlType != ControlMethod.Xbox)
                            changeControlMethod(ControlMethod.Xbox);
                        break;
                    }
            }
            else
            {
                if (_controlType != ControlMethod.KeyboardMouse)
                    changeControlMethod(ControlMethod.KeyboardMouse);
            }
            MouseState mouseState = Mouse.GetState();

            FIRE.Update(gt);
            ALT_FIRE.Update(gt);

            PAUSE.Update(gt);

            //Arrow Keys
            UP.Update(gt); //UP
            LEFT.Update(gt); //LEFT
            RIGHT.Update(gt); //RIGHT
            DOWN.Update(gt); //DOWN

            //Num Keys
            NUM_1.Update(gt);
            NUM_2.Update(gt);
            NUM_3.Update(gt);
            NUM_4.Update(gt);
            NUM_5.Update(gt);
            NUM_6.Update(gt);
            NUM_7.Update(gt);
            NUM_8.Update(gt);
            NUM_9.Update(gt);
            NUM_0.Update(gt);

            WEAPON_SWITCH_UP.Update(gt);
            WEAPON_SWITCH_DOWN.Update(gt);

            MENU_RETURN.Update(gt);
            MENU_SELECT.Update(gt);

            RIGHT_THUMBSTICK.Update(gt);


            if (_controlType == ControlMethod.KeyboardMouse)
            {
                DELTA_SCROLL = (mouseState.ScrollWheelValue - oldScrollValue) / 120;
                oldScrollValue = mouseState.ScrollWheelValue;
                mouseVector = new Vector2(mouseState.X, mouseState.Y);
                if (DELTA_SCROLL == 0)
                {
                    if (WEAPON_SWITCH_UP.Bumped)
                        DELTA_SCROLL = 1;
                    else if (WEAPON_SWITCH_DOWN.Bumped)
                        DELTA_SCROLL = -1;
                    else
                        DELTA_SCROLL = 0;
                }
            }
            else
            {
                if (WEAPON_SWITCH_UP.Bumped)
                    DELTA_SCROLL = 1;
                else if (WEAPON_SWITCH_DOWN.Bumped)
                    DELTA_SCROLL = -1;
                else
                    DELTA_SCROLL = 0;
            }
        }

        public static float getCursorAngleFrom(Vector2 pos)
        {
            if(ControlType == ControlMethod.KeyboardMouse)
            {
                Vector2 calcVector = pos - mouseVector;
                return (float)Math.Atan2(calcVector.Y, calcVector.X);
            }
            else if (ControlType == ControlMethod.Xbox)
            {
                return -((JoystickButton)RIGHT_THUMBSTICK).joystickAngle;
            }
            return 0;
        }
    }
}

