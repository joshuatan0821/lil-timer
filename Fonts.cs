using System.Drawing;

namespace LilTimer;

public static class Fonts
{
    private static readonly string[] Stack =
    {
        "Berkeley Mono",
        "JetBrains Mono",
        "IBM Plex Mono",
        "Consolas",
        "Courier New",
    };

    public static Font Mono(float size, FontStyle style = FontStyle.Regular)
    {
        foreach (var name in Stack)
        {
            try
            {
                using var probe = new Font(name, size, style);
                if (probe.Name == name)
                {
                    return new Font(name, size, style);
                }
            }
            catch
            {
                // Family missing — try the next one in the stack.
            }
        }

        return new Font(FontFamily.GenericMonospace, size, style);
    }
}