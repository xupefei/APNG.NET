using System;
using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace LibAPNG.XNA.APNGPipelineExtension
{
    [ContentImporter(".png", DisplayName = "APNG Importer", DefaultProcessor = "APNG Processor")]
    public class APNGContentImporter : ContentImporter<APNGFrameList>
    {
        public override APNGFrameList Import(string filename, ContentImporterContext context)
        {
            var apng = new APNG(filename);

            APNGFrameList frameList;

            if (apng.IsSimplePNG)
            {
                frameList = new APNGFrameList(0);
                frameList.AddFrame(new TimeSpan(), BuildTextureContent(apng.DefaultImage.GetStream().ToArray()));
            }
            else
            {
                frameList = new APNGFrameList(apng.acTLChunk.NumPlays);

                if (!apng.DefaultImageIsAnimeated)
                    frameList.AddFrame(
                                       new TimeSpan(TimeSpan.TicksPerSecond * apng.DefaultImage.fcTLChunk.DelayNum /
                                                    apng.DefaultImage.fcTLChunk.DelayDen),
                                       BuildTextureContent(apng.DefaultImage.GetStream().ToArray()));

                foreach (LibAPNG.Frame frame in apng.Frames)
                {
                    frameList.AddFrame(
                                       new TimeSpan(TimeSpan.TicksPerSecond * frame.fcTLChunk.DelayNum
                                                    / frame.fcTLChunk.DelayDen),
                                       BuildTextureContent(frame.GetStream().ToArray()));
                }
            }

            return frameList;
        }

        private Texture2DContent BuildTextureContent(byte[] buffer)
        {
            var bitmap = new Bitmap(new MemoryStream(buffer));

            var bitmapContent = new PixelBitmapContent<Color>(bitmap.Width, bitmap.Height);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color from = bitmap.GetPixel(i, j);

                    //System.Drawing.Color to Microsoft.Xna.Framework.Color
                    var to = new Color(from.R, from.G, from.B, from.A);
                    bitmapContent.SetPixel(i, j, to);
                }
            }

            return new Texture2DContent {Mipmaps = new MipmapChain(bitmapContent)};
        }
    }
}