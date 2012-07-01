using System;
using System.IO;
using LibAPNG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace LibAPNGTest.APNGHelper
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
                X = (int) frame.fcTLChunk.XOffset;
                Y = (int) frame.fcTLChunk.YOffset;
                Width = (int) frame.fcTLChunk.Width;
                Height = (int) frame.fcTLChunk.Height;
                BlendOp = frame.fcTLChunk.BlendOp;
                DisposeOp = frame.fcTLChunk.DisposeOp;
                DelayTime = new TimeSpan(
                    TimeSpan.TicksPerSecond*frame.fcTLChunk.DelayNum/frame.fcTLChunk.DelayDen);
            }
            else
            {
                X = 0;
                Y = 0;
                Width = frame.IHDRChunk.Width;
                Height = frame.IHDRChunk.Height;
                BlendOp = BlendOps.APNGBlendOpSource;
                DisposeOp = DisposeOps.APNGDisposeOpNone;
                DelayTime = TimeSpan.Zero;
            }

            // frame.GetStream() is not seekable, so we build a new MemoryStream.
            FrameTexture = Texture2D.FromStream(
                game.GraphicsDevice, new MemoryStream(frame.GetStream().ToArray()));
            MultiplyAlpha(FrameTexture);
        }

        #endregion Constructors and Destructors

        #region Methods

        private static void MultiplyAlpha(Texture2D ret)
        {
            var data = new Byte4[ret.Width*ret.Height];

            ret.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                Vector4 vec = data[i].ToVector4();

                float alpha = vec.W/255.0f;
                var a = (int) (vec.W);
                var r = (int) (alpha*vec.X);
                var g = (int) (alpha*vec.Y);
                var b = (int) (alpha*vec.Z);
                var packed = (uint) ((a << 24) + (b << 16) + (g << 8) + r);

                data[i].PackedValue = packed;
            }

            ret.SetData(data);
        }

        #endregion Methods
    }
}