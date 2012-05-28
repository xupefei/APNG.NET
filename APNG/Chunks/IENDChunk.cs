using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APNG
{
    public class IENDChunk : Chunk
    {
        public IENDChunk(byte[] bytes)
            : base(bytes)
        {
        }

        public IENDChunk(MemoryStreamEx ms)
            : base(ms)
        {
        }

        public IENDChunk(Chunk chunk)
            : base(chunk)
        {
        }
    }
}