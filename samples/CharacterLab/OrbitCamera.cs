using System.Numerics;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace DXSharp.CharacterLab;

sealed class OrbitCamera
{
    float _yaw = 0.35f;
    float _pitch = 0.12f;
    float _distance = 4.0f;
    bool _wasDragging;
    POINT _lastCursor;
    readonly Dictionary<int, bool> _lastKeyState = [];

    public Vector3 Eye
    {
        get
        {
            float cp = MathF.Cos(_pitch);
            return new Vector3(
                MathF.Sin(_yaw) * cp * _distance,
                0.92f + MathF.Sin(_pitch) * _distance,
                MathF.Cos(_yaw) * cp * _distance);
        }
    }

    public void Update(float elapsedSeconds)
    {
        if (PInvoke.GetActiveWindow().Value == 0)
        {
            _wasDragging = false;
            return;
        }

        bool dragging = IsDown(0x01);
        if (dragging && PInvoke.GetCursorPos(out POINT cursor))
        {
            if (_wasDragging)
            {
                int dx = cursor.X - _lastCursor.X;
                int dy = cursor.Y - _lastCursor.Y;
                _yaw -= dx * 0.006f;
                _pitch = Math.Clamp(_pitch + dy * 0.006f, -0.65f, 1.15f);
            }
            _lastCursor = cursor;
        }
        _wasDragging = dragging;

        if (IsDown(0x6B) || IsDown(0xBB)) _distance = Math.Max(2.0f, _distance - 3.6f * elapsedSeconds);
        if (IsDown(0x6D) || IsDown(0xBD)) _distance = Math.Min(9.0f, _distance + 3.6f * elapsedSeconds);
    }

    public bool WasPressed(int virtualKey)
    {
        bool down = IsDown(virtualKey);
        _lastKeyState.TryGetValue(virtualKey, out bool wasDown);
        _lastKeyState[virtualKey] = down;
        return down && !wasDown;
    }

    static bool IsDown(int virtualKey) =>
        PInvoke.GetActiveWindow().Value != 0 && (PInvoke.GetAsyncKeyState(virtualKey) & 0x8000) != 0;

}
