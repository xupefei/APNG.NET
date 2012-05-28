using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace APNG
{
    public class Chunk
    {
        public uint Length { get; set; }

        public string ChunkType { get; set; }

        public byte[] ChunkData { get; set; }

        public uint Crc { get; set; }

        /// <summary>
        /// Get raw data of the chunk
        /// </summary>
        public byte[] RawData
        {
            get
            {
                var ms = new MemoryStreamEx();
                ms.WriteUInt32(Helper.ConvertEndian(this.Length));
                ms.WriteBytes(Encoding.ASCII.GetBytes(this.ChunkType));
                ms.WriteBytes(this.ChunkData);
                ms.WriteUInt32(Helper.ConvertEndian(this.Crc));

                return ms.ToArray();
            }
        }

        internal Chunk()
        {
            this.Length = 0;
            this.ChunkType = String.Empty;
            this.ChunkData = null;
            this.Crc = 0;
        }

        internal Chunk(byte[] bytes)
        {
            var ms = new MemoryStreamEx(bytes);
            this.Length = Helper.ConvertEndian(ms.ReadUInt32());
            this.ChunkType = Encoding.ASCII.GetString(ms.ReadBytes(4));
            this.ChunkData = ms.ReadBytes((int)this.Length);
            this.Crc = Helper.ConvertEndian(ms.ReadUInt32());

            if (ms.Position != ms.Length)
                throw new Exception("Chunk length not correct.");
            if (this.Length != this.ChunkData.Length)
                throw new Exception("Chunk data length not correct.");

            ParseData(new MemoryStreamEx(this.ChunkData));
        }

        internal Chunk(MemoryStreamEx ms)
        {
            this.Length = Helper.ConvertEndian(ms.ReadUInt32());
            this.ChunkType = Encoding.ASCII.GetString(ms.ReadBytes(4));
            this.ChunkData = ms.ReadBytes((int)this.Length);
            this.Crc = Helper.ConvertEndian(ms.ReadUInt32());

            ParseData(new MemoryStreamEx(this.ChunkData));
        }

        internal Chunk(Chunk chunk)
        {
            this.Length = chunk.Length;
            this.ChunkType = chunk.ChunkType;
            this.ChunkData = chunk.ChunkData;
            this.Crc = chunk.Crc;

            ParseData(new MemoryStreamEx(this.ChunkData));
        }

        /// <summary>
        /// Modify the ChunkData part.
        /// </summary>
        public void ModifyChunkData(int postion, byte[] newData)
        {
            Array.Copy(newData, 0, ChunkData, postion, newData.Length);

            using (var msCrc = new MemoryStreamEx())
            {
                msCrc.WriteBytes(Encoding.ASCII.GetBytes(this.ChunkType));
                msCrc.WriteBytes(this.ChunkData);

                this.Crc = CrcHelper.Calculate(msCrc.ToArray());
            }
        }

        /// <summary>
        /// Modify the ChunkData part.
        /// </summary>
        public void ModifyChunkData(int postion, uint newData)
        {
            ModifyChunkData(postion, BitConverter.GetBytes(newData));
        }

        protected virtual void ParseData(MemoryStreamEx ms)
        {
        }
    }
}