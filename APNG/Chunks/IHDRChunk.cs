using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APNG
{
    public class IHDRChunk : Chunk
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public byte BitDepth { get; private set; }

        public byte ColorType { get; private set; }

        public byte CompressionMethod { get; private set; }

        public byte FilterMethod { get; private set; }

        public byte InterlaceMethod { get; private set; }

        public IHDRChunk(byte[] chunkBytes)
            : base(chunkBytes)
        {
        }

        public IHDRChunk(MemoryStreamEx ms)
            : base(ms)
        {
        }

        public IHDRChunk(Chunk chunk)
            : base(chunk)
        {
        }

        protected override void ParseData(MemoryStreamEx ms)
        {
            this.Width = Helper.ConvertEndian(ms.ReadInt32());
            this.Height = Helper.ConvertEndian(ms.ReadInt32());
            this.BitDepth = Convert.ToByte(ms.ReadByte());
            this.ColorType = Convert.ToByte(ms.ReadByte());
            this.CompressionMethod = Convert.ToByte(ms.ReadByte());
            this.FilterMethod = Convert.ToByte(ms.ReadByte());
            this.InterlaceMethod = Convert.ToByte(ms.ReadByte());
        }
    }
}