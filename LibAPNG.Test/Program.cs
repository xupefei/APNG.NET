using System;
using System.IO;

namespace LibAPNG.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Write("Usage: APNG.Test.exe filename.png");
                Console.ReadKey();

                return;
            }
            var apng = new APNG(args[0]);

            if (!apng.DefaultImageIsAnimated)
                File.WriteAllBytes("0.png", apng.DefaultImage.GetStream().ToArray());

            foreach (Frame frame in apng.Frames)
            {
                File.WriteAllBytes(
                                   frame.fcTLChunk.SequenceNumber + ".png",
                                   frame.GetStream().ToArray());
            }
        }
    }
}