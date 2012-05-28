using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APNG
{
    public enum DisposeOps
    {
        APNGDisposeOpNone = 0,
        APNGDisposeOpBackground = 1,
        APNGDisposeOpPrevious = 2,
    }

    public enum BlendOps
    {
        APNGBlendOpSource = 0,
        APNGBlendOpOver = 1,
    }

    public class fcTLChunk : Chunk
    {
        /// <summary>
        /// Sequence number of the animation chunk, starting from 0
        /// </summary>
        public uint SequenceNumber { get; private set; }

        /// <summary>
        /// Width of the following frame
        /// </summary>
        public uint Width { get; private set; }

        /// <summary>
        /// Height of the following frame
        /// </summary>
        public uint Height { get; private set; }

        /// <summary>
        /// X position at which to render the following frame
        /// </summary>
        public uint XOffset { get; private set; }

        /// <summary>
        /// Y position at which to render the following frame
        /// </summary>
        public uint YOffset { get; private set; }

        /// <summary>
        /// Frame delay fraction numerator
        /// </summary>
        public ushort DelayNum { get; private set; }

        /// <summary>
        /// Frame delay fraction denominator
        /// </summary>
        public ushort DelayDen { get; private set; }

        /// <summary>
        /// Type of frame area disposal to be done after rendering this frame
        /// </summary>
        public DisposeOps DisposeOp { get; private set; }

        /// <summary>
        /// Type of frame area rendering for this frame
        /// </summary>
        public BlendOps BlendOp { get; private set; }

        public fcTLChunk(byte[] bytes)
            : base(bytes)
        {
        }

        public fcTLChunk(MemoryStreamEx ms)
            : base(ms)
        {
        }

        public fcTLChunk(Chunk chunk)
            : base(chunk)
        {
        }

        protected override void ParseData(MemoryStreamEx ms)
        {
            this.SequenceNumber = Helper.ConvertEndian(ms.ReadUInt32());
            this.Width = Helper.ConvertEndian(ms.ReadUInt32());
            this.Height = Helper.ConvertEndian(ms.ReadUInt32());
            this.XOffset = Helper.ConvertEndian(ms.ReadUInt32());
            this.YOffset = Helper.ConvertEndian(ms.ReadUInt32());
            this.DelayNum = Helper.ConvertEndian(ms.ReadUInt16());
            this.DelayDen = Helper.ConvertEndian(ms.ReadUInt16());
            this.DisposeOp = (DisposeOps)ms.ReadByte();
            this.BlendOp = (BlendOps)ms.ReadByte();
        }
    }
}