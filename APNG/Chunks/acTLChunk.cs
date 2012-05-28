using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APNG
{
    public class acTLChunk : Chunk
    {
        public uint NumFrames { get; private set; }

        public uint NumPlays { get; private set; }

        public acTLChunk(byte[] bytes)
            : base(bytes)
        {
        }

        public acTLChunk(MemoryStreamEx ms)
            : base(ms)
        {
        }

        public acTLChunk(Chunk chunk)
            : base(chunk)
        {
        }

        protected override void ParseData(MemoryStreamEx ms)
        {
            this.NumFrames = Helper.ConvertEndian(ms.ReadUInt32());
            this.NumPlays = Helper.ConvertEndian(ms.ReadUInt32());
        }
    }
}