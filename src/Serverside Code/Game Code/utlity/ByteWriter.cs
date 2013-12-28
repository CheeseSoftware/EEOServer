using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MushroomsUnity3DExample.utlity
{
    /// <summary>
    /// ByteWriter is child of Queue< byte >, but cointains PushShort, PushInt, PushLong...
    /// Add new push-methods if you need so.
    /// </summary>
    public class ByteWriter : Queue<byte>
    {
        /// <summary>
        /// Writes a byte...
        /// </summary>
        /// <param name="value"></param>
        public void WriteByte(byte value)
        {
            this.Enqueue(value);
        }

        /// <summary>
        /// Pushes a short into the stack.
        /// </summary>
        /// <param name="value">short / Int64 / 2 bytes</param>
        public void WriteShort(short value)
        {
            this.Enqueue((byte)((value >> 8)&0xFF));
            this.Enqueue((byte)(value & 0xFF));

            //output.Enqueue((byte)(value >> 8));
            //output.Enqueue((byte)(value & 0xFF));
        }

        /// <summary>
        /// Pushes an int into the stack.
        /// </summary>
        /// <param name="value">int / Int32 / 4 bytes</param>
        public void WriteInt(short value)
        {
            this.Enqueue((byte)(value >> 24));
            this.Enqueue((byte)(value >> 16 & 0xFF));
            this.Enqueue((byte)(value >> 8 & 0xFF));
            this.Enqueue((byte)(value & 0xFF));
        }

        /// <summary>
        /// Pushes a long into the stack.
        /// </summary>
        /// <param name="value">long / Int64 / 8 bytes</param>
        public void WriteLong(short value)
        {
            this.Enqueue((byte)(value >> 56));
            this.Enqueue((byte)(value >> 48 & 0xFF));
            this.Enqueue((byte)(value >> 40 & 0xFF));
            this.Enqueue((byte)(value >> 32 & 0xFF));
            this.Enqueue((byte)(value >> 24 & 0xFF));
            this.Enqueue((byte)(value >> 16 & 0xFF));
            this.Enqueue((byte)(value >> 8 & 0xFF));
            this.Enqueue((byte)(value & 0xFF));
        }
    }
}
