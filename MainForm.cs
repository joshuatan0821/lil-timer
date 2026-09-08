using System;
using System.Windows.Forms;

namespace LilTimer;

public sealed class MainForm : Form
{
    private const int HotkeyToggle = 1;
    private const int HotkeyReset = 2;
    private const int WmHotkey = 0x0312;

    private readonly PomodoroTimer _pomodoro = new();
    private readonly NotifyIcon _tray;
    private readonly ContextMenuStrip _menu;
    private readonly StatusForm _status;
    private readonly System.Windows.Forms.Timer _clock;
    private HotkeyManager? _hotkeys;
    private ToolStripMenuItem? _toggleItem;

    public MainForm()
    {
        ShowInTaskbar = false;
        FormBorderStyle = FormBorderStyle.None;

        _clock = new System.Windows.Forms.Timer { Interval = 1000 };
        _clock.Tick += (_, _) => _pomodoro.Tick();
        _clock.Start();

        _pomodoro.StateChanged += (_, _) => UpdateStatus();
        _pomodoro.SessionCompleted += (_, e) => OnSessionCompleted(e);

        _menu = BuildMenu();
        _tray = new NotifyIcon
        {
            Icon = IconFactory.Create(),
            Text = "lil timer",
            Visible = true,
            ContextMenuStrip = _menu,
        };
        _tray.DoubleClick += (_, _) => ShowStatus();

        _status = new StatusForm(_pomodoro);

        Application.ApplicationExit += (_, _) =>
        {
            _clock.Stop();
            _tray.Visible = false;
            _tray.Dispose();
            _menu.Dispose();
            _status.Dispose();
            _hotkeys?.Dispose();
        };

        UpdateStatus();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        try
        {
            _hotkeys = new HotkeyManager(Handle, HotkeyToggle, HotkeyReset);
            _hotkeys.Register();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "lil timer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        _hotkeys?.Dispose();
        _hotkeys = null;
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WmHotkey)
        {
            switch (m.WParam.ToInt32())
            {
                case HotkeyToggle:
                    _pomodoro.Toggle();
                    break;
                case HotkeyReset:
                    _pomodoro.Reset();
                    break;
            }
        }
        base.WndProc(ref m);
    }

    private ContextMenuStrip BuildMenu()
    {
        var menu = new ContextMenuStrip();

        _toggleItem = new ToolStripMenuItem("[>] start");
        _toggleItem.Click += (_, _) => _pomodoro.Toggle();

        var resetItem = new ToolStripMenuItem("[x] reset");
        resetItem.Click += (_, _) => _pomodoro.Reset();

        var statusItem = new ToolStripMenuItem("[s] status");
        statusItem.Click += (_, _) => ShowStatus();

        var quitItem = new ToolStripMenuItem("[q] quit");
        quitItem.Click += (_, _) => Application.Exit();

        menu.Items.AddRange(new ToolStripItem[] { _toggleItem, resetItem, new ToolStripSeparator(), statusItem, quitItem });
        return menu;
    }

    private void ShowStatus()
    {
        _status.UpdateDisplay();
        _status.Show();
        _status.Activate();
    }

    private void UpdateStatus()
    {
        var snapshot = _pomodoro.GetSnapshot();
        _tray.Text = $"lil timer — {(snapshot.IsWork ? "focus" : "break")} {Format(snapshot.Remaining)}";
        _tray.Icon = IconFactory.Create(snapshot.IsWork, snapshot.IsRunning);
        _toggleItem!.Text = snapshot.IsRunning ? "[||] pause" : "[>] start";
        _status.UpdateDisplay();
    }

    private void OnSessionCompleted(SessionCompletedEventArgs e)
    {
        NotificationService.PlayRetroChime();
        if (e.WasWork)
        {
            NotificationService.Notify(_tray, "focus complete", "[+] take a break — 5:00");
        }
        else
        {
            NotificationService.Notify(_tray, "break over", "[>] back to focus — 25:00");
        }
    }

    private static string Format(TimeSpan t) => $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
}