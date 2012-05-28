using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APNG
{
    internal class fdATChunk : Chunk
    {
        public uint SequenceNumber { get; private set; }

        public byte[] FrameData { get; private set; }

        public fdATChunk(byte[] bytes)
            : base(bytes)
        {
        }

        public fdATChunk(MemoryStreamEx ms)
            : base(ms)
        {
        }

        public fdATChunk(Chunk chunk)
            : base(chunk)
        {
        }

        protected override void ParseData(MemoryStreamEx ms)
        {
            this.SequenceNumber = Helper.ConvertEndian(ms.ReadUInt32());
            this.FrameData = ms.ReadBytes((int)Length - 4);
        }

        public IDATChunk ToIDATChunk()
        {
            uint newCrc;
            using (var msCrc = new MemoryStreamEx())
            {
                msCrc.WriteBytes(new[] { (byte)'I', (byte)'D', (byte)'A', (byte)'T' });
                msCrc.WriteBytes(this.FrameData);

                newCrc = CrcHelper.Calculate(msCrc.ToArray());
            }

            using (var ms = new MemoryStreamEx())
            {
                ms.WriteUInt32(Helper.ConvertEndian(this.Length - 4));
                ms.WriteBytes(new[] { (byte)'I', (byte)'D', (byte)'A', (byte)'T' });
                ms.WriteBytes(this.FrameData);
                ms.WriteUInt32(Helper.ConvertEndian(newCrc));
                ms.Position = 0;

                return new IDATChunk(ms);
            }
        }
    }
}