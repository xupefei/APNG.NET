using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LibAPNG.XNA.APNGTextureProvider
{
    public class APNGContentTypeReader : ContentTypeReader<APNGTexture>
    {
        protected override APNGTexture Read(ContentReader input, APNGTexture existingInstance)
        {
            var list = new APNGTexture(input.ReadUInt32());

            int frameCount = input.ReadInt32();

            for (int i = 0; i < frameCount; ++i)
            {
                list.AddFrame(new TimeSpan(input.ReadInt64()), input.ReadObject<Texture2D>());
            }

            return list;
        }
    }
}