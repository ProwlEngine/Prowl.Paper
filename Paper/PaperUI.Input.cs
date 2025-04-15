﻿using System.Numerics;

namespace Prowl.PaperUI
{
    public static partial class Paper
    {
        #region Fields & Properties

        // Enums
        public static readonly PaperKey[] KeyValues = Enum.GetValues<PaperKey>();
        public static readonly PaperMouseBtn[] MouseValues = Enum.GetValues<PaperMouseBtn>();

        // Events
        public static event Action<Vector2> OnPointerPosSet;
        public static event Action<bool> OnCursorVisibilitySet;

        #region Keyboard State

        // Keyboard state tracking
        private static bool[] _keyCurState;
        private static bool[] _keyPrevState;
        private static float[] _keyPressedTime;
        public static PaperKey LastKeyPressed { get; private set; } = PaperKey.Unknown;

        #endregion

        #region Mouse State

        // Mouse state tracking
        private static bool[] _pointerCurState;
        private static bool[] _pointerPrevState;
        private static float[] _pointerPressedTime;
        private static Vector2[] _pointerClickPos;
        public static PaperMouseBtn LastButtonPressed { get; private set; } = PaperMouseBtn.Unknown;
        public static Vector2 PreviousPointerPos { get; private set; } = Vector2.Zero;

        // Current pointer position
        private static Vector2 _pointerPos;
        public static Vector2 PointerPos {
            get => _pointerPos;
            set {
                _pointerPos = value;
                OnPointerPosSet?.Invoke(_pointerPos / _frameBufferScale);
            }
        }

        // Mouse wheel
        public static float PointerWheel { get; private set; } = 0;

        // Derived properties
        public static Vector2 PointerDelta => PointerPos - PreviousPointerPos;
        public static bool IsPointerMoving => PointerDelta.LengthSquared() > 0;

        // Double-click tracking
        private static float[] _pointerLastClickTime;
        private static Vector2[] _pointerLastClickPos;
        private const float MaxDoubleClickTime = 0.25f;

        #endregion

        #region Text Input

        // Text input
        public static readonly Queue<char> InputString = new Queue<char>();

        #endregion

        #region Timing & Scaling

        // Frame timing
        private static float _deltaTime = 0.016f; // Default to 60 FPS
        private static float _time = 0f;
        public static float DeltaTime => _deltaTime;
        public static float Time => _time;

        // Scaling
        private static Vector2 _frameBufferScale = Vector2.One;

        #endregion

        // Clipboard handling
        private static IClipboardHandler _clipboardHandler;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the input system.
        /// </summary>
        private static void InitializeInput()
        {
            // Initialize keyboard arrays
            _keyCurState = new bool[KeyValues.Length];
            _keyPrevState = new bool[KeyValues.Length];
            _keyPressedTime = new float[KeyValues.Length];

            // Initialize mouse arrays
            _pointerCurState = new bool[MouseValues.Length];
            _pointerPrevState = new bool[MouseValues.Length];
            _pointerPressedTime = new float[MouseValues.Length];
            _pointerClickPos = new Vector2[MouseValues.Length];
            _pointerLastClickTime = new float[MouseValues.Length];
            _pointerLastClickPos = new Vector2[MouseValues.Length];

            // Initialize clipboard handler
            _clipboardHandler = null;

            _time = 0;
        }

        #endregion

        #region Clipboard Handling

        /// <summary>
        /// Sets the clipboard handler for text operations.
        /// </summary>
        /// <param name="handler">The clipboard handler implementation</param>
        public static void SetClipboardHandler(IClipboardHandler handler)
        {
            _clipboardHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Gets the current clipboard text.
        /// </summary>
        /// <returns>The text from the clipboard</returns>
        public static string GetClipboard()
        {
            if (_clipboardHandler == null)
            {
                Console.WriteLine("Warning: Clipboard handler not initialized.");
                return "";
            }

            return _clipboardHandler.GetClipboardText();
        }

        /// <summary>
        /// Sets the clipboard text.
        /// </summary>
        /// <param name="text">The text to set</param>
        public static void SetClipboard(string text)
        {
            if (_clipboardHandler == null)
            {
                Console.WriteLine("Warning: Clipboard handler not initialized.");
                return;
            }

            _clipboardHandler.SetClipboardText(text);
        }

        #endregion

        #region Text Input Handling

        /// <summary>
        /// Adds a character to the input queue.
        /// </summary>
        /// <param name="character">The character to add</param>
        public static void PushInputText(char character) => InputString.Enqueue(character);

        #endregion

        #region Frame Management

        /// <summary>
        /// Updates the timing information.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        public static void SetTime(float deltaTime)
        {
            _time += deltaTime;
            _deltaTime = deltaTime;
        }

        /// <summary>
        /// Begins the input processing for a new frame.
        /// </summary>
        /// <param name="frameBufferScale">The framebuffer scale factor</param>
        private static void StartInputFrame(Vector2 frameBufferScale)
        {
            _frameBufferScale = frameBufferScale;

            // Update key pressed times
            for (var i = 0; i < _keyPressedTime.Length; ++i)
                if (_keyCurState[i])
                    _keyPressedTime[i] += _deltaTime;

            // Update pointer pressed times
            for (var i = 0; i < _pointerPressedTime.Length; ++i)
                if (_pointerCurState[i])
                    _pointerPressedTime[i] += _deltaTime;
        }

        /// <summary>
        /// Finalizes the input processing for the current frame.
        /// </summary>
        private static void EndInputFrame()
        {
            // Update keyboard state
            for (var i = 0; i < _keyCurState.Length; ++i)
            {
                _keyPrevState[i] = _keyCurState[i];

                if (!_keyCurState[i])
                    _keyPressedTime[i] = 0.0f;
            }

            // Update mouse state
            for (var i = 0; i < _pointerCurState.Length; ++i)
            {
                if (_pointerPrevState[i] && !_pointerCurState[i]) // Just released
                {
                    _pointerLastClickTime[i] = _time + MaxDoubleClickTime;
                    _pointerLastClickPos[i] = PointerPos;
                }

                _pointerPrevState[i] = _pointerCurState[i];

                if (!_pointerCurState[i])
                    _pointerPressedTime[i] = 0.0f;
            }

            // Reset transient values
            PointerWheel = 0;
            PreviousPointerPos = PointerPos;
            InputString.Clear();
        }

        /// <summary>
        /// Clears all input state.
        /// </summary>
        public static void ClearInput()
        {
            // Clear keyboard state
            for (var i = 0; i < _keyCurState.Length; ++i)
            {
                _keyCurState[i] = false;
                _keyPrevState[i] = false;
                _keyPressedTime[i] = 0;
            }

            LastKeyPressed = PaperKey.Unknown;

            // Clear mouse state
            for (var i = 0; i < _pointerCurState.Length; ++i)
            {
                _pointerCurState[i] = false;
                _pointerPrevState[i] = false;
                _pointerPressedTime[i] = 0;
                _pointerClickPos[i] = Vector2.Zero;
            }

            LastButtonPressed = PaperMouseBtn.Unknown;
            PreviousPointerPos = _pointerPos;
            _pointerPos = Vector2.Zero;
            PointerWheel = 0;
        }

        #endregion

        #region Input State Management

        /// <summary>
        /// Sets the state of a keyboard key.
        /// </summary>
        /// <param name="key">The key to update</param>
        /// <param name="isKeyDown">Whether the key is pressed</param>
        public static void SetKeyState(PaperKey key, bool isKeyDown)
        {
            var index = (int)key;
            _keyPrevState[index] = _keyCurState[index];
            _keyCurState[index] = isKeyDown;

            if (isKeyDown)
                LastKeyPressed = key;
        }

        /// <summary>
        /// Sets the state of a mouse button or updates pointer position.
        /// </summary>
        /// <param name="btn">The mouse button</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="isPointerBtnDown">Whether the button is pressed</param>
        /// <param name="isPointerMove">Whether this is a movement event</param>
        public static void SetPointerState(PaperMouseBtn btn, float x, float y, bool isPointerBtnDown, bool isPointerMove)
        {
            var index = (int)btn;
            LastButtonPressed = btn;

            x *= _frameBufferScale.X;
            y *= _frameBufferScale.Y;

            if (!isPointerMove)
            {
                _pointerPrevState[index] = _pointerCurState[index];
                _pointerCurState[index] = isPointerBtnDown;
                _pointerClickPos[index] = new Vector2(x, y);
            }
            else
            {
                _pointerPos = new Vector2(x, y);
            }
        }

        /// <summary>
        /// Sets the mouse wheel value.
        /// </summary>
        /// <param name="wheel">The wheel delta</param>
        public static void SetPointerWheel(float wheel)
        {
            PointerWheel = wheel;
        }

        /// <summary>
        /// Adds text input characters.
        /// </summary>
        /// <param name="text">The text to add</param>
        public static void AddInputCharacter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (text.Length == 1)
            {
                var c = text[0];
                if (c == '\r')
                    c = '\n';

                InputString.Enqueue(c);
            }
            else
            {
                foreach (var c in text)
                {
                    if (c == '\r')
                        InputString.Enqueue('\n');
                    else
                        InputString.Enqueue(c);
                }
            }
        }

        #endregion

        #region Input State Queries

        #region Keyboard Queries

        /// <summary>
        /// Checks if a key is currently down.
        /// </summary>
        public static bool IsKeyDown(PaperKey key) => _keyCurState[(int)key];

        /// <summary>
        /// Checks if a key is currently up.
        /// </summary>
        public static bool IsKeyUp(PaperKey key) => !_keyCurState[(int)key];

        /// <summary>
        /// Checks if a key was just pressed this frame.
        /// </summary>
        public static bool IsKeyPressed(PaperKey key) => !_keyPrevState[(int)key] && _keyCurState[(int)key];

        /// <summary>
        /// Checks if a key was just released this frame.
        /// </summary>
        public static bool IsKeyReleased(PaperKey key) => _keyPrevState[(int)key] && !_keyCurState[(int)key];

        /// <summary>
        /// Checks if a key has been held down for a while.
        /// </summary>
        public static bool IsKeyHeld(PaperKey key) => IsKeyDown(key) && _keyPressedTime[(int)key] >= 0.5f;

        #endregion

        #region Mouse Queries

        /// <summary>
        /// Checks if a mouse button is currently down.
        /// </summary>
        public static bool IsPointerDown(PaperMouseBtn btn) => _pointerCurState[(int)btn];

        /// <summary>
        /// Checks if a mouse button is currently up.
        /// </summary>
        public static bool IsPointerUp(PaperMouseBtn btn) => !_pointerCurState[(int)btn];

        /// <summary>
        /// Checks if a mouse button was just pressed this frame.
        /// </summary>
        public static bool IsPointerPressed(PaperMouseBtn btn) => !_pointerPrevState[(int)btn] && _pointerCurState[(int)btn];

        /// <summary>
        /// Checks if a mouse button was just released this frame.
        /// </summary>
        public static bool IsPointerReleased(PaperMouseBtn btn) => _pointerPrevState[(int)btn] && !_pointerCurState[(int)btn];

        /// <summary>
        /// Checks if a mouse button has been held down for a while.
        /// </summary>
        public static bool IsPointerHeld(PaperMouseBtn btn) => IsPointerDown(btn) && _pointerPressedTime[(int)btn] >= 0.5f;

        /// <summary>
        /// Checks if a mouse button was double-clicked.
        /// </summary>
        public static bool IsPointerDoubleClick(PaperMouseBtn btn) =>
            IsPointerPressed(btn) && _time < _pointerLastClickTime[(int)btn] &&
            (PointerPos - _pointerLastClickPos[(int)btn]).LengthSquared() < 2; // 5^2 = 25

        /// <summary>
        /// Gets the position where a mouse button was clicked.
        /// </summary>
        public static Vector2 GetPointerClickPos(PaperMouseBtn btn) => _pointerClickPos[(int)btn];

        /// <summary>
        /// Checks if the pointer is over a specified rectangle.
        /// </summary>
        public static bool IsPointerOverRect(float x, float y, float width, float height)
        {
            return _pointerPos.X >= x && _pointerPos.X <= x + width &&
                   _pointerPos.Y >= y && _pointerPos.Y <= y + height;
        }

        #endregion

        #endregion

        #region UI Control

        /// <summary>
        /// Sets the visibility of the cursor.
        /// </summary>
        /// <param name="visible">Whether the cursor should be visible</param>
        public static void SetCursorVisibility(bool visible) => OnCursorVisibilitySet?.Invoke(visible);

        #endregion
    }

    /// <summary>
    /// Interface for clipboard handling operations.
    /// </summary>
    public interface IClipboardHandler
    {
        /// <summary>
        /// Gets text from the clipboard.
        /// </summary>
        string GetClipboardText();

        /// <summary>
        /// Sets text to the clipboard.
        /// </summary>
        void SetClipboardText(string text);
    }

    public enum PaperKey
    {
        Unknown = 0,

        // Alphanumeric keys
        A, B, C, D, E, F, G, H, I, J, K, L, M,
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

        Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9, Num0,

        // Function keys
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

        // Special keys
        Return, Escape, Backspace, Tab, Space,
        Minus, Equals, LeftBracket, RightBracket, Backslash,
        Semicolon, Apostrophe, Grave, Comma, Period, Slash,

        CapsLock, PrintScreen, ScrollLock, Pause,
        Insert, Home, PageUp, Delete, End, PageDown,
        Right, Left, Down, Up,

        // Keypad
        NumLock, KeypadDivide, KeypadMultiply, KeypadMinus, KeypadPlus, KeypadEnter, KeypadEquals,
        Keypad1, Keypad2, Keypad3, Keypad4, Keypad5, Keypad6, Keypad7, Keypad8, Keypad9, Keypad0,
        KeypadPeriod,

        // Modifier keys
        LeftControl, LeftShift, LeftAlt, LeftSuper,
        RightControl, RightShift, RightAlt, RightSuper,

        // Media keys
        AudioNext, AudioPrevious, AudioStop, AudioPlay, AudioMute,

        // Application control keys
        Application, Menu, Select, Help
    }

    /// <summary>
    /// Enumeration of mouse buttons.
    /// </summary>
    public enum PaperMouseBtn
    {
        Unknown = 0,
        Left,
        Middle,
        Right,
        Button4,
        Button5,
        Button6,
        Button7,
        Button8
    }
}