using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace LibAPNG.XNA.APNGPipelineExtension
{
    [ContentProcessor(DisplayName = "APNG Processor")]
    public class APNGContentProcessor : ContentProcessor<APNGFrameList, APNGFrameList>
    {
        public APNGContentProcessor()
        {
            ColorKeyColor = Color.Magenta;
            ColorKeyEnabled = true;
            GenerateMipmaps = false;
            PremultiplyAlpha = true;
            ResizeToPowerOfTwo = false;
        }

        [DefaultValue(typeof (Color), "255, 0, 255, 255")]
        public Color ColorKeyColor { get; set; }

        [DefaultValue(true)]
        public bool ColorKeyEnabled { get; set; }

        [DefaultValue(false)]
        public bool GenerateMipmaps { get; set; }

        [DefaultValue(true)]
        public bool PremultiplyAlpha { get; set; }

        [DefaultValue(false)]
        public bool ResizeToPowerOfTwo { get; set; }

        [DefaultValue(TextureProcessorOutputFormat.Color)]
        public TextureProcessorOutputFormat TextureFormat { get; set; }

        public override APNGFrameList Process(APNGFrameList input, ContentProcessorContext context)
        {
            for (int i = 0; i < input.Frames.Length; i++)
            {
                ProcessSingleFrame(input.Frames[i].Content);
            }

            return input;
        }

        internal void ProcessSingleFrame(TextureContent input)
        {
            bool colorKeyEnabled = ColorKeyEnabled;
            bool premultiplyAlpha = PremultiplyAlpha;
            Type bitmapType = null;
            if (colorKeyEnabled || premultiplyAlpha)
            {
                Type type2;
                input.Validate(null);
                bitmapType = input.Faces[0][0].GetType();
                if (TextureProcessorUtils.IsHighPrecisionFormat(bitmapType))
                {
                    type2 = typeof (PixelBitmapContent<Vector4>);
                }
                else
                {
                    type2 = typeof (PixelBitmapContent<Color>);
                }
                input.ConvertBitmapType(type2);
            }
            if (colorKeyEnabled)
            {
                TextureProcessorUtils.ColorKey(input, ColorKeyColor);
            }
            if (premultiplyAlpha)
            {
                TextureProcessorUtils.PremultiplyAlpha(input);
            }
            if (ResizeToPowerOfTwo)
            {
                TextureProcessorUtils.ResizeToPowerOfTwo(input);
            }
            if (GenerateMipmaps)
            {
                input.GenerateMipmaps(false);
            }
            TextureProcessorUtils.ChangeTextureToRequestedFormat(input, bitmapType, TextureFormat);
        }
    }
}