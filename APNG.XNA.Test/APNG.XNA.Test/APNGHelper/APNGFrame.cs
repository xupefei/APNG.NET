using System;
using System.IO;
using APNG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace APNGTest.APNGHelper
{
    internal struct APNGFrame
    {
        internal int X;
        internal int Y;
        internal int Width;
        internal int Height;
        internal BlendOps BlendOp;
        internal DisposeOps DisposeOp;
        internal TimeSpan DelayTime;
        internal Texture2D FrameTexture;

        internal APNGFrame(Game game, Frame frame)
        {
            this.X = (int)frame.fcTLChunk.XOffset;
            this.Y = (int)frame.fcTLChunk.YOffset;
            this.Width = (int)frame.fcTLChunk.Width;
            this.Height = (int)frame.fcTLChunk.Height;
            this.BlendOp = frame.fcTLChunk.BlendOp;
            this.DisposeOp = frame.fcTLChunk.DisposeOp;
            this.DelayTime = new TimeSpan(
                TimeSpan.TicksPerSecond * frame.fcTLChunk.DelayNum / frame.fcTLChunk.DelayDen);

            // frame.GetStream() is not seekable, so we build a new MemoryStream.
            this.FrameTexture = Texture2D.FromStream(
                game.GraphicsDevice, new MemoryStream(frame.GetStream().ToArray()));
            MultiplyAlpha(this.FrameTexture);
        }

        private static void MultiplyAlpha(Texture2D ret)
        {
            var data = new Byte4[ret.Width * ret.Height];

            ret.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                Vector4 vec = data[i].ToVector4();

                float alpha = vec.W / 255.0f;
                var a = (int)(vec.W);
                var r = (int)(alpha * vec.X);
                var g = (int)(alpha * vec.Y);
                var b = (int)(alpha * vec.Z);
                var packed = (uint)((a << 24) + (b << 16) + (g << 8) + r);

                data[i].PackedValue = packed;
            }

            ret.SetData(data);
        }
    }
}