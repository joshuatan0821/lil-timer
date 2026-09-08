using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LilTimer;

public sealed class StatusForm : Form
{
    private static readonly Color Canvas = Color.FromArgb(0xff, 0xfd, 0xfc, 0xfc); // {colors.canvas}
    private static readonly Color Ink = Color.FromArgb(0xff, 0x20, 0x1d, 0x1d);     // {colors.ink}
    private static readonly Color Body = Color.FromArgb(0xff, 0x42, 0x42, 0x45);    // {colors.body}
    private static readonly Color Mute = Color.FromArgb(0xff, 0x64, 0x62, 0x62);    // {colors.mute}
    private static readonly Color HairlineStrong = Color.FromArgb(0xff, 0x64, 0x62, 0x62);
    private static readonly Color Success = Color.FromArgb(0xff, 0x30, 0xd1, 0x58); // {colors.success}

    private readonly PomodoroTimer _pomodoro;
    private readonly Label _timerLabel;
    private readonly Label _stateLabel;
    private readonly Button _toggleButton;

    public StatusForm(PomodoroTimer pomodoro)
    {
        _pomodoro = pomodoro;

        Text = "lil timer";
        BackColor = Canvas;
        ForeColor = Ink;
        Font = Fonts.Mono(16f);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(400, 232);

        var title = new Label
        {
            Text = "LIL TIMER",
            Font = Fonts.Mono(16f, FontStyle.Bold),
            ForeColor = Ink,
            AutoSize = true,
            Location = new Point(16, 16),
        };

        var caption = new Label
        {
            Text = "ctrl+alt+t start/pause   ctrl+alt+r reset",
            Font = Fonts.Mono(14f),
            ForeColor = Mute,
            AutoSize = true,
            Location = new Point(16, 42),
        };

        _timerLabel = new Label
        {
            Font = Fonts.Mono(38f, FontStyle.Bold),
            ForeColor = Ink,
            AutoSize = true,
            Location = new Point(16, 74),
        };

        _stateLabel = new Label
        {
            Font = Fonts.Mono(16f),
            ForeColor = Body,
            AutoSize = true,
            Location = new Point(16, 132),
        };

        _toggleButton = MakeButton("[>] start", primary: true);
        var resetButton = MakeButton("[x] reset", primary: false);
        var quitButton = MakeButton("[q] quit", primary: false);

        _toggleButton.Location = new Point(16, 176);
        resetButton.Location = new Point(144, 176);
        quitButton.Location = new Point(272, 176);

        _toggleButton.Click += (_, _) => _pomodoro.Toggle();
        resetButton.Click += (_, _) => _pomodoro.Reset();
        quitButton.Click += (_, _) => Application.Exit();

        Controls.AddRange(new Control[] { title, caption, _timerLabel, _stateLabel, _toggleButton, resetButton, quitButton });

        UpdateDisplay();
    }

    private static Button MakeButton(string text, bool primary)
    {
        var button = new Button
        {
            Text = text,
            Font = Fonts.Mono(16f, FontStyle.Bold),
            Size = new Size(112, 36),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
        };
        button.FlatAppearance.BorderSize = 1;

        if (primary)
        {
            button.BackColor = Ink;
            button.ForeColor = Canvas;
            button.FlatAppearance.BorderColor = Ink;
        }
        else
        {
            button.BackColor = Canvas;
            button.ForeColor = Ink;
            button.FlatAppearance.BorderColor = HairlineStrong;
        }

        RoundCorners(button, 4); // {rounded.sm}
        return button;
    }

    private static void RoundCorners(Control control, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(0, 0, d, d, 180, 90);
        path.AddArc(control.Width - d, 0, d, d, 270, 90);
        path.AddArc(control.Width - d, control.Height - d, d, d, 0, 90);
        path.AddArc(0, control.Height - d, d, d, 90, 90);
        path.CloseFigure();
        control.Region = new Region(path);
    }

    public void UpdateDisplay()
    {
        var snapshot = _pomodoro.GetSnapshot();
        _timerLabel.Text = Format(snapshot.Remaining);
        _stateLabel.Text = snapshot.IsRunning
            ? snapshot.IsWork ? "[+] focus · running" : "[-] break · running"
            : snapshot.IsWork ? "[+] focus · paused" : "[-] break · paused";
        _stateLabel.ForeColor = snapshot.IsRunning && !snapshot.IsWork ? Success : Body;
        _toggleButton.Text = snapshot.IsRunning ? "[||] pause" : "[>] start";
        _toggleButton.Invalidate();
    }

    private static string Format(TimeSpan t) => $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
}