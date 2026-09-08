using System;
using System.Threading;
using System.Windows.Forms;

namespace LilTimer;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var mutex = new Mutex(true, "LilTimer.SingleInstance", out var createdNew);
        if (!createdNew)
        {
            MessageBox.Show("lil timer is already running.", "lil timer",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.Run(new MainForm());
    }
}