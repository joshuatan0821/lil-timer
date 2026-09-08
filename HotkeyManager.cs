using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LilTimer;

public sealed class HotkeyManager : IDisposable
{
    private const uint ModAlt = 0x0001;
    private const uint ModControl = 0x0002;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private readonly IntPtr _hWnd;
    private readonly int _toggleId;
    private readonly int _resetId;
    private bool _registered;

    public HotkeyManager(IntPtr hWnd, int toggleId, int resetId)
    {
        _hWnd = hWnd;
        _toggleId = toggleId;
        _resetId = resetId;
    }

    public void Register()
    {
        if (_registered)
        {
            return;
        }

        if (!RegisterHotKey(_hWnd, _toggleId, ModControl | ModAlt, (uint)Keys.T))
        {
            throw new InvalidOperationException(
                "Could not register Ctrl+Alt+T. The shortcut may already be in use.");
        }

        if (!RegisterHotKey(_hWnd, _resetId, ModControl | ModAlt, (uint)Keys.R))
        {
            UnregisterHotKey(_hWnd, _toggleId);
            throw new InvalidOperationException(
                "Could not register Ctrl+Alt+R. The shortcut may already be in use.");
        }

        _registered = true;
    }

    public void Unregister()
    {
        if (!_registered)
        {
            return;
        }

        UnregisterHotKey(_hWnd, _toggleId);
        UnregisterHotKey(_hWnd, _resetId);
        _registered = false;
    }

    public void Dispose() => Unregister();
}