using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APNG
{
    public class IDATChunk : Chunk
    {
        public IDATChunk(byte[] bytes)
            : base(bytes)
        {
        }

        public IDATChunk(MemoryStreamEx ms)
            : base(ms)
        {
        }

        public IDATChunk(Chunk chunk)
            : base(chunk)
        {
        }
    }
}