using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace PlatformerTest
{
    public static class Input
    {
        private static KeyboardState _prevKState;
        private static KeyboardState _kState;

        private static MouseState _prevMouseStates;
        private static MouseState _mouseState;

        public static MouseState CurrentMouseState => _mouseState;

        public static Dictionary<string, Keys[]> KeyMap = new Dictionary<string, Keys[]>
        {
            { "quit", new Keys[] { Keys.Escape, } },
            { "restart", new Keys[] { Keys.Enter, } },
            { "player1_move_left", new Keys[] { Keys.Left, Keys.A, } },
            { "player1_move_right", new Keys[] { Keys.Right, Keys.D, } },
            { "player1_jump", new Keys[] { Keys.Space, Keys.W, Keys.Up, } },
            /*{ "player1_move_up", new Keys[] { Keys.Up, Keys.W, } },
            { "player1_move_down", new Keys[] { Keys.Down, Keys.S, } },*/
        };

        public static void UpdateState()
        {
            _prevKState = _kState;
            _kState = Keyboard.GetState();

            _prevMouseStates = _mouseState;
            _mouseState = Mouse.GetState();
        }

        public static bool IsKeyDown(Keys key)
        {
            if (_kState.IsKeyDown(key))
                return true;
            return false;
        }

        public static bool IsKeyUp(Keys key)
        {
            if (_kState.IsKeyUp(key))
                return true;
            return false;
        }

        public static bool IsKeyPressed(Keys key)
        {
            if (_kState.IsKeyDown(key) && !_prevKState.IsKeyDown(key))
                return true;
            return false;
        }

        public static bool IsKeyReleased(Keys key)
        {
            if (!_kState.IsKeyDown(key) && _prevKState.IsKeyDown(key))
                return true;
            return false;
        }

        public static bool IsKeyDown(Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (_kState.IsKeyDown(key))
                    return true;
            }
            return false;
        }

        public static bool IsKeyUp(Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (_kState.IsKeyUp(key))
                    return true;
            }
            return false;
        }

        public static bool IsKeyPressed(Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (_kState.IsKeyDown(key) && !_prevKState.IsKeyDown(key))
                    return true;
            }
            return false;
        }

        public static bool IsKeyReleased(Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (!_kState.IsKeyDown(key) && _prevKState.IsKeyDown(key))
                    return true;
            }
            return false;
        }

        public static bool IsLeftMousePressed() => _mouseState.LeftButton == ButtonState.Pressed && _prevMouseStates.LeftButton != ButtonState.Pressed;

        public static bool IsLeftMouseReleased() => _mouseState.LeftButton == ButtonState.Released && _prevMouseStates.LeftButton == ButtonState.Pressed;

        public static bool IsRightMousePressed() => _mouseState.RightButton == ButtonState.Pressed && _prevMouseStates.RightButton != ButtonState.Pressed;

        public static bool IsRightMouseReleased() => _mouseState.RightButton == ButtonState.Released && _prevMouseStates.RightButton == ButtonState.Pressed;

        public static bool IsLeftMouseDown() => _mouseState.LeftButton == ButtonState.Pressed;

        public static bool IsLeftMouseUp() => _mouseState.LeftButton == ButtonState.Released;

        public static bool IsRightMouseDown() => _mouseState.RightButton == ButtonState.Pressed;

        public static bool IsRightMouseUp() => _mouseState.RightButton == ButtonState.Released;

        public static bool IsMiddleMousePressed() => _mouseState.MiddleButton == ButtonState.Pressed && _prevMouseStates.MiddleButton != ButtonState.Pressed;

        public static bool IsMiddleMouseReleased() => _mouseState.MiddleButton == ButtonState.Released && _prevMouseStates.MiddleButton != ButtonState.Released;

        public static bool IsMiddleMouseDown() => _mouseState.MiddleButton == ButtonState.Pressed;

        public static bool IsMiddleMouseUp() => _mouseState.MiddleButton == ButtonState.Released;
    }
}