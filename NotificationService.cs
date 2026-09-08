using System.Media;
using System.Windows.Forms;

namespace LilTimer;

public static class NotificationService
{
    private static readonly SoundPlayer Player = new(RetroSound.GetStream());

    public static void PlayRetroChime()
    {
        try
        {
            Player.Play();
        }
        catch
        {
            // Sound is best-effort; never crash the background app.
        }
    }

    public static void Notify(NotifyIcon tray, string title, string message)
    {
        tray.ShowBalloonTip(5000, title, message, ToolTipIcon.Info);
    }
}