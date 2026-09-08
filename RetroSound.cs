using System;
using System.IO;

namespace LilTimer;

public static class RetroSound
{
    private const int SampleRate = 22050;

    private static readonly double[] Arpeggio = { 523.25, 659.25, 783.99, 1046.50 }; // C5 E5 G5 C6
    private static readonly MemoryStream Stream = Build();

    public static Stream GetStream() => Stream;

    private static MemoryStream Build()
    {
        var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true);

        int sampleCount = (int)(SampleRate * 0.9);
        var samples = new byte[sampleCount];

        int cursor = 0;
        foreach (var freq in Arpeggio)
        {
            int noteSamples = SampleRate / 8;
            for (int i = 0; i < noteSamples && cursor < sampleCount; i++)
            {
                double t = (double)i / SampleRate;
                double envelope = Math.Exp(-6.0 * t);
                double square = Math.Sign(Math.Sin(2 * Math.PI * freq * t));
                samples[cursor++] = (byte)(128 + 110 * envelope * square);
            }
        }

        for (int i = 0; i < SampleRate / 10 && cursor < sampleCount; i++)
        {
            double t = (double)i / SampleRate;
            double envelope = Math.Exp(-30.0 * t);
            double square = Math.Sign(Math.Sin(2 * Math.PI * 130.81 * t)); // C3 thump
            samples[cursor++] = (byte)(128 + 90 * envelope * square);
        }

        WriteHeader(writer, sampleCount);
        writer.Write(samples, 0, sampleCount);
        writer.Flush();
        ms.Position = 0;
        return ms;
    }

    private static void WriteHeader(BinaryWriter writer, int dataSize)
    {
        writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + dataSize);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);   // PCM
        writer.Write((short)1);   // mono
        writer.Write(SampleRate);
        writer.Write(SampleRate * 1 * 16 / 8);
        writer.Write((short)(1 * 16 / 8));
        writer.Write((short)16);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        writer.Write(dataSize);
    }
}