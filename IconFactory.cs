using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace LilTimer;

public static class IconFactory
{
    private static readonly Dictionary<(bool IsWork, bool Running), Icon> Cache = new();

    public static Icon Create(bool isWork = true, bool running = false)
    {
        var key = (isWork, running);
        if (Cache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        using var bitmap = new Bitmap(32, 32);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            var body = running
                ? isWork
                    ? Color.FromArgb(0xff, 0xff, 0x3b, 0x30) // danger red — focus
                    : Color.FromArgb(0xff, 0x30, 0xd1, 0x58) // success green — break
                : Color.FromArgb(0xff, 0x9a, 0x98, 0x98);    // ash — idle/paused

            using (var bodyBrush = new SolidBrush(body))
            {
                g.FillEllipse(bodyBrush, 3, 7, 26, 22);
            }

            using (var leafBrush = new SolidBrush(Color.FromArgb(0xff, 0x30, 0xd1, 0x58)))
            {
                g.FillEllipse(leafBrush, 10, 2, 12, 9);
            }

            using (var highlight = new SolidBrush(Color.FromArgb(0xff, 0xfd, 0xfc, 0xfc)))
            {
                g.FillEllipse(highlight, 9, 12, 6, 4);
            }
        }

        var hicon = bitmap.GetHicon();
        try
        {
            var icon = Icon.FromHandle(hicon);
            Cache[key] = icon;
            return icon;
        }
        catch
        {
            DestroyIcon(hicon);
            throw;
        }
    }

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr handle);
}