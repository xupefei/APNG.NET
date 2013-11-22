using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace LibAPNG.XNA.APNGPipelineExtension
{
    // The code below is from Microsoft.Xna.Framework.Content.Pipeline.Processors.TextureProcessorUtils.
    internal static class TextureProcessorUtils
    {
        private static void BestGuessCompress(TextureContent texture)
        {
            texture.ConvertBitmapType(typeof (PixelBitmapContent<Color>));
            if (!(texture is Texture3DContent))
            {
                texture.ConvertBitmapType(HasFractionalAlpha(texture)
                                              ? typeof (Dxt5BitmapContent)
                                              : typeof (Dxt1BitmapContent));
            }
        }

        internal static void ChangeTextureToRequestedFormat(TextureContent texture,
                                                            Type originalType,
                                                            TextureProcessorOutputFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureProcessorOutputFormat.NoChange:
                    if (originalType == null)
                    {
                        break;
                    }
                    texture.ConvertBitmapType(originalType);
                    return;

                case TextureProcessorOutputFormat.Color:
                    texture.ConvertBitmapType(typeof (PixelBitmapContent<Color>));
                    return;

                case TextureProcessorOutputFormat.DxtCompressed:
                    BestGuessCompress(texture);
                    break;

                default:
                    return;
            }
        }

        public static void ColorKey(TextureContent texture, Color colorKey)
        {
            foreach (MipmapChain chain in texture.Faces)
            {
                foreach (BitmapContent content in chain)
                {
                    var content3 = content as PixelBitmapContent<Color>;
                    if (content3 == null)
                    {
                        var content2 = content as PixelBitmapContent<Vector4>;
                        if (content2 == null)
                        {
                            throw new NotSupportedException();
                        }
                        content2.ReplaceColor(colorKey.ToVector4(), Vector4.Zero);
                    }
                    else
                    {
                        content3.ReplaceColor(colorKey, Color.Transparent);
                    }
                }
            }
        }

        private static bool HasFractionalAlpha(TextureContent texture)
        {
            foreach (MipmapChain chain in texture.Faces)
            {
                foreach (PixelBitmapContent<Color> content in chain)
                {
                    for (int i = 0; i < content.Height; i++)
                    {
                        if (content.GetRow(i).Select(color => color.A).Any(a => (a != 0xff) && (a != 0)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsHighPrecisionFormat(Type bitmapType)
        {
            if ((((!typeof (PixelBitmapContent<float>).IsAssignableFrom(bitmapType)
                   && !typeof (PixelBitmapContent<Vector2>).IsAssignableFrom(bitmapType))
                  && (!typeof (PixelBitmapContent<Vector4>).IsAssignableFrom(bitmapType)
                      && !typeof (PixelBitmapContent<HalfSingle>).IsAssignableFrom(bitmapType)))
                 && ((!typeof (PixelBitmapContent<HalfVector2>).IsAssignableFrom(bitmapType)
                      && !typeof (PixelBitmapContent<HalfVector4>).IsAssignableFrom(bitmapType))
                     && (!typeof (PixelBitmapContent<NormalizedByte2>).IsAssignableFrom(bitmapType)
                         && !typeof (PixelBitmapContent<NormalizedByte4>).IsAssignableFrom(bitmapType))))
                && (!typeof (PixelBitmapContent<Rgba1010102>).IsAssignableFrom(bitmapType)
                    && !typeof (PixelBitmapContent<Rg32>).IsAssignableFrom(bitmapType)))
            {
                return typeof (PixelBitmapContent<Rgba64>).IsAssignableFrom(bitmapType);
            }
            return true;
        }

        public static void PremultiplyAlpha(TextureContent texture)
        {
            foreach (MipmapChain chain in texture.Faces)
            {
                foreach (BitmapContent content3 in chain)
                {
                    var content2 = content3 as PixelBitmapContent<Color>;
                    if (content2 != null)
                    {
                        for (int i = 0; i < content2.Height; i++)
                        {
                            Color[] row = content2.GetRow(i);
                            for (int j = 0; j < row.Length; j++)
                            {
                                Color color = row[j];
                                if (color.A < 0xff)
                                {
                                    row[j] = Color.FromNonPremultiplied(color.R, color.G, color.B, color.A);
                                }
                            }
                        }
                    }
                    else
                    {
                        var content = content3 as PixelBitmapContent<Vector4>;
                        if (content == null)
                        {
                            throw new NotSupportedException();
                        }
                        for (int k = 0; k < content.Height; k++)
                        {
                            Vector4[] vectorArray = content.GetRow(k);
                            for (int m = 0; m < vectorArray.Length; m++)
                            {
                                Vector4 vector = vectorArray[m];
                                if (vector.W < 1f)
                                {
                                    vector.X *= vector.W;
                                    vector.Y *= vector.W;
                                    vector.Z *= vector.W;
                                    vectorArray[m] = vector;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ResizeToPowerOfTwo(TextureContent texture)
        {
            foreach (MipmapChain chain in texture.Faces)
            {
                for (int i = 0; i < chain.Count; i++)
                {
                    BitmapContent source = chain[i];
                    int width = RoundUpToPowerOfTwo(source.Width);
                    int height = RoundUpToPowerOfTwo(source.Height);
                    if ((width != source.Width) || (height != source.Height))
                    {
                        chain[i] = ConvertBitmap(source, source.GetType(), width, height);
                    }
                }
            }
        }

        private static BitmapContent ConvertBitmap(BitmapContent source, Type newType, int width, int height)
        {
            BitmapContent content;
            try
            {
                content = (BitmapContent)Activator.CreateInstance(newType, new object[] {width, height});
            }
            catch (TargetInvocationException exception)
            {
                throw new Exception(exception.InnerException.Message);
            }
            BitmapContent.Copy(source, content);
            return content;
        }

        private static int RoundUpToPowerOfTwo(int value)
        {
            int num = 1;
            while (num < value)
            {
                num = num << 1;
            }
            return num;
        }
    }
}