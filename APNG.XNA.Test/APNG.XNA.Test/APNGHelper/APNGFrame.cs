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
        #region Constants and Fields

        internal BlendOps BlendOp;
        internal TimeSpan DelayTime;
        internal DisposeOps DisposeOp;
        internal Texture2D FrameTexture;
        internal int Height;
        internal int Width;
        internal int X;
        internal int Y;

        #endregion Constants and Fields

        #region Constructors and Destructors

        internal APNGFrame(Game game, Frame frame)
        {
            if (frame.fcTLChunk != null)
            {
                this.X = (int)frame.fcTLChunk.XOffset;
                this.Y = (int)frame.fcTLChunk.YOffset;
                this.Width = (int)frame.fcTLChunk.Width;
                this.Height = (int)frame.fcTLChunk.Height;
                this.BlendOp = frame.fcTLChunk.BlendOp;
                this.DisposeOp = frame.fcTLChunk.DisposeOp;
                this.DelayTime = new TimeSpan(
                    TimeSpan.TicksPerSecond * frame.fcTLChunk.DelayNum / frame.fcTLChunk.DelayDen);
            }
            else
            {
                this.X = 0;
                this.Y = 0;
                this.Width = frame.IHDRChunk.Width;
                this.Height = frame.IHDRChunk.Height;
                this.BlendOp = BlendOps.APNGBlendOpSource;
                this.DisposeOp = DisposeOps.APNGDisposeOpNone;
                this.DelayTime = TimeSpan.Zero;
            }

            // frame.GetStream() is not seekable, so we build a new MemoryStream.
            this.FrameTexture = Texture2D.FromStream(
                game.GraphicsDevice, new MemoryStream(frame.GetStream().ToArray()));
            MultiplyAlpha(this.FrameTexture);
        }

        #endregion Constructors and Destructors

        #region Methods

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

        #endregion Methods
    }
}