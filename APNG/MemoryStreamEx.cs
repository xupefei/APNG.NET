using System;

namespace APNG
{
    public class MemoryStreamEx : System.IO.MemoryStream
    {
        #region Constructor

        public MemoryStreamEx()
            : base()
        {
        }

        public MemoryStreamEx(byte[] buffer)
            : base(buffer)
        {
        }

        public MemoryStreamEx(byte[] buffer, bool writable)
            : base(buffer, writable)
        {
        }

        public MemoryStreamEx(byte[] buffer, int index, int count)
            : base(buffer, index, count)
        {
        }

        public MemoryStreamEx(byte[] buffer, int index, int count, bool writable)
            : base(buffer, index, count, writable)
        {
        }

        public MemoryStreamEx(byte[] buffer, int index, int count, bool writable, bool publiclyVisible)
            : base(buffer, index, count, writable, publiclyVisible)
        {
        }

        public MemoryStreamEx(int capacity)
            : base(capacity)
        {
        }

        #endregion Constructor

        #region Read

        public byte[] ReadBytes(int count)
        {
            if (count == 0) return new byte[] { };

            var buffer = new byte[count];

            if (this.Read(buffer, 0, count) != count)
                throw new Exception("End reached.");

            return buffer;
        }

        public Int16 ReadInt16()
        {
            return BitConverter.ToInt16(this.ReadBytes(2), 0);
        }

        public Int32 ReadInt32()
        {
            return BitConverter.ToInt32(this.ReadBytes(4), 0);
        }

        public Int64 ReadInt64()
        {
            return BitConverter.ToInt64(this.ReadBytes(8), 0);
        }

        public UInt16 ReadUInt16()
        {
            return BitConverter.ToUInt16(this.ReadBytes(2), 0);
        }

        public UInt32 ReadUInt32()
        {
            return BitConverter.ToUInt32(this.ReadBytes(4), 0);
        }

        public UInt64 ReadUInt64()
        {
            return BitConverter.ToUInt64(this.ReadBytes(8), 0);
        }

        public char ReadChar()
        {
            return BitConverter.ToChar(this.ReadBytes(2), 0);
        }

        #endregion Read

        #region Peek

        public byte[] PeekBytes(int position, int count)
        {
            long prevPosition = this.Position;

            this.Position = position;
            var buffer = this.ReadBytes(count);
            this.Position = prevPosition;

            return buffer;
        }

        public Int16 PeekInt16()
        {
            return this.PeekInt16((int)this.Position);
        }

        public Int32 PeekInt32()
        {
            return this.PeekInt32((int)this.Position);
        }

        public Int64 PeekInt64()
        {
            return this.PeekInt64((int)this.Position);
        }

        public UInt16 PeekUInt16()
        {
            return this.PeekUInt16((int)this.Position);
        }

        public UInt32 PeekUInt32()
        {
            return this.PeekUInt32((int)this.Position);
        }

        public UInt64 PeekUInt64()
        {
            return this.PeekUInt64((int)this.Position);
        }

        public char PeekChar()
        {
            return this.PeekChar((int)this.Position);
        }

        public Int16 PeekInt16(int position)
        {
            return BitConverter.ToInt16(this.PeekBytes(position, 2), 0);
        }

        public Int32 PeekInt32(int position)
        {
            return BitConverter.ToInt32(this.PeekBytes(position, 4), 0);
        }

        public Int64 PeekInt64(int position)
        {
            return BitConverter.ToInt64(this.PeekBytes(position, 8), 0);
        }

        public UInt16 PeekUInt16(int position)
        {
            return BitConverter.ToUInt16(this.PeekBytes(position, 2), 0);
        }

        public UInt32 PeekUInt32(int position)
        {
            return BitConverter.ToUInt32(this.PeekBytes(position, 4), 0);
        }

        public UInt64 PeekUInt64(int position)
        {
            return BitConverter.ToUInt64(this.PeekBytes(position, 8), 0);
        }

        public char PeekChar(int position)
        {
            return BitConverter.ToChar(this.PeekBytes(position, 2), 0);
        }

        #endregion Peek

        #region Write

        public void WriteByte(int position, byte value)
        {
            long prevPosition = this.Position;

            this.Position = position;
            this.WriteByte(value);
            this.Position = prevPosition;
        }

        public void WriteBytes(byte[] value)
        {
            this.Write(value, 0, value.Length);
        }

        public void WriteBytes(int position, byte[] value)
        {
            long prevPosition = this.Position;

            this.Position = position;
            this.Write(value, 0, value.Length);
            this.Position = prevPosition;
        }

        public void WriteInt16(Int16 value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public void WriteInt32(Int32 value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void WriteInt64(Int64 value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public void WriteUInt16(UInt16 value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public void WriteUInt32(UInt32 value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void WriteUInt64(UInt64 value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public void WriteInt16(int position, Int16 value)
        {
            this.WriteBytes(position, BitConverter.GetBytes(value));
        }

        public void WriteInt32(int position, Int32 value)
        {
            this.WriteBytes(position, BitConverter.GetBytes(value));
        }

        public void WriteInt64(int position, Int64 value)
        {
            this.WriteBytes(position, BitConverter.GetBytes(value));
        }

        public void WriteUInt16(int position, UInt16 value)
        {
            this.WriteBytes(position, BitConverter.GetBytes(value));
        }

        public void WriteUInt32(int position, UInt32 value)
        {
            this.WriteBytes(position, BitConverter.GetBytes(value));
        }

        public void WriteUInt64(int position, UInt64 value)
        {
            this.WriteBytes(position, BitConverter.GetBytes(value));
        }

        #endregion Write
    }
}