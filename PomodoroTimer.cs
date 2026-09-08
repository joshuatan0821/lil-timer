using System;

namespace LilTimer;

public enum TimerState
{
    Idle,
    Running,
    Paused,
}

public readonly record struct TimerSnapshot(bool IsWork, bool IsRunning, TimeSpan Remaining);

public sealed class PomodoroTimer
{
    public static readonly TimeSpan WorkDuration = TimeSpan.FromMinutes(25);
    public static readonly TimeSpan BreakDuration = TimeSpan.FromMinutes(5);

    private bool _isWork = true;
    private TimerState _state = TimerState.Idle;
    private TimeSpan _remaining = WorkDuration;

    public event EventHandler? StateChanged;
    public event EventHandler<SessionCompletedEventArgs>? SessionCompleted;

    public TimerSnapshot GetSnapshot() => new(_isWork, _state == TimerState.Running, _remaining);

    public void Toggle()
    {
        switch (_state)
        {
            case TimerState.Idle:
                _isWork = true;
                _remaining = WorkDuration;
                _state = TimerState.Running;
                break;
            case TimerState.Running:
                _state = TimerState.Paused;
                break;
            case TimerState.Paused:
                _state = TimerState.Running;
                break;
        }
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Reset()
    {
        _isWork = true;
        _remaining = WorkDuration;
        _state = TimerState.Idle;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Tick()
    {
        if (_state != TimerState.Running)
        {
            return;
        }

        _remaining -= TimeSpan.FromSeconds(1);
        if (_remaining <= TimeSpan.Zero)
        {
            var completedWork = _isWork;
            _isWork = !_isWork;
            _remaining = _isWork ? WorkDuration : BreakDuration;
            _state = TimerState.Running;
            SessionCompleted?.Invoke(this, new SessionCompletedEventArgs(completedWork));
        }
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}

public sealed class SessionCompletedEventArgs : EventArgs
{
    public bool WasWork { get; }

    public SessionCompletedEventArgs(bool wasWork) => WasWork = wasWork;
}