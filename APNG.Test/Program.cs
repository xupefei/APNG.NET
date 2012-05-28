using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace APNG.Test
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

            if (!apng.DefaultImageIsAnimeated)
                File.WriteAllBytes("0.png", apng.DefaultImage.GetStream().ToArray());

            foreach (var frame in apng.Frames)
            {
                File.WriteAllBytes(
                    frame.fcTLChunk.SequenceNumber.ToString() + ".png", frame.GetStream().ToArray());
            }
        }
    }
}