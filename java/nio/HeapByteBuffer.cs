/*
 * Copyright 2000-2002 Sun Microsystems, Inc.  All Rights Reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Sun designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Sun in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Sun Microsystems, Inc., 4150 Network Circle, Santa Clara,
 * CA 95054 USA or visit www.sun.com if you need additional information or
 * have any questions.
 */

using System;

namespace java.nio
{
    public class HeapByteBuffer : ByteBuffer
    {
        // For speed these fields are actually declared in X-Buffer;
        // these declarations are here as documentation
        /*

        protected final byte[] hb;
        protected final int offset;

        */

        public HeapByteBuffer(int cap, int lim) : base(-1, 0, lim, cap, new byte[cap], 0)
        {
        }
        public HeapByteBuffer(byte[] buf, int off, int len) : base(-1, off, off + len, buf.Length, buf, 0)
        {
        }

        protected HeapByteBuffer(byte[] buf, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap, buf, off)
        {
        }

        public override ByteBuffer slice()
        {
            return new HeapByteBuffer(hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + offset);
        }

        public override ByteBuffer duplicate()
        {
            return new HeapByteBuffer(hb, this.markValue(), this.Position(), this.Limit(), this.Capacity(), offset);
        }

        public override ByteBuffer asReadOnlyBuffer()
        {
            throw new NotImplementedException();
        }



        protected int ix(int i)
        {
            return i + offset;
        }

        public override byte get()
        {
            return hb[ix(nextGetIndex())];
        }
        public override byte get(int i)
        {
            return hb[ix(checkIndex(i))];
        }
        public override ByteBuffer get(byte[] dst, int offset, int length)
        {
            checkBounds(offset, length, dst.Length);
            if (length > Remaining())
                throw new Exception("BufferUnderflowException");
            Array.Copy(hb, ix(Position()), dst, offset, length);
            Position(Position() + length);
            return this;
        }
        public override bool isDirect()
        {
            return false;
        }
        public override bool isReadOnly()
        {
            return false;
        }
        public override ByteBuffer put(byte x)
        {
            hb[ix(nextPutIndex())] = x;
            return this;
        }
        public override ByteBuffer put(int i, byte x)
        {
            hb[ix(checkIndex(i))] = x;
            return this;
        }
        public ByteBuffer put(byte[] src, int offset, int length)
        {

            checkBounds(offset, length, src.Length);
            if (length > Remaining())
                throw new Exception("BufferOverflowException");
            Array.Copy(src, offset, hb, ix(Position()), length);
            Position(Position() + length);
            return this;



        }

        public ByteBuffer put(ByteBuffer src)
        {

            if (src is HeapByteBuffer)
            {
                if (src == this)
                    throw new Exception("IllegalArgumentException");
                HeapByteBuffer sb = (HeapByteBuffer)src;
                int n = sb.Remaining();
                if (n > Remaining())
                    throw new Exception("BufferOverflowException");
                Array.Copy(sb.hb, sb.ix(sb.Position()),
                                 hb, ix(Position()), n);
                sb.Position(sb.Position() + n);
                Position(Position() + n);
            } else if (src.isDirect())
            {
                int n = src.Remaining();
                if (n > Remaining())
                    throw new Exception("BufferOverflowException");
                src.get(hb, ix(Position()), n);
                Position(Position() + n);
            }
            else
            {
                base.put(src);
            }
            return this;
        }
        public override ByteBuffer compact()
        {
            Array.Copy(hb, ix(Position()), hb, ix(0), Remaining());
            Position(Remaining());
            Limit(Capacity());
            //clearMark();
            return this;
        }
        protected override byte _get(int i)
        {
            return hb[i];
        }
        protected override void _put(int i, byte b)
        {
            hb[i] = b;
        }
        public override char getChar()
        {
            return BitConverter.ToChar(this.hb, ix(nextGetIndex(2)));
        }

        public override char getChar(int i)
        {
            return BitConverter.ToChar(this.hb, ix(checkIndex(i, 2)));
        }



        public override ByteBuffer putChar(char x)
        {
            foreach (var b in BitConverter.GetBytes(x))
            {
                put(b);
            }
            return this;
        }

        public override ByteBuffer putChar(int i, char x)
        {
            foreach (var b in BitConverter.GetBytes(x))
            {
                put(ix(checkIndex(i, 2)), b);
            }
            return this;
        }
        // short



        public override short getShort()
        {
            return BitConverter.ToInt16(this.hb, ix(nextGetIndex(2)));
        }
        public override short getShort(int i)
        {
            return BitConverter.ToInt16(this.hb, ix(checkIndex(i, 2)));
        }
        public override ByteBuffer putShort(short x)
        {
            foreach (var b in BitConverter.GetBytes(x))
                put(ix(nextGetIndex(2)), b);
            return this;
        }

        public override ByteBuffer putShort(int i, short x)
        {
            foreach (var b in BitConverter.GetBytes(x))
                put(ix(checkIndex(i, 2)), b);
            return this;
        }
        // int
        public override int getInt()
        {
            return BitConverter.ToInt32(this.hb, ix(nextGetIndex(4)));
        }
        public override int getInt(int i)
        {
            return BitConverter.ToInt32(this.hb, ix(checkIndex(i, 4)));
        }
        public override ByteBuffer putInt(int x)
        {
            foreach (var b in BitConverter.GetBytes(x))
                put(ix(nextGetIndex(4)), b);
            return this;
        }
        public override ByteBuffer putInt(int i, int x)
        {
            foreach (var b in BitConverter.GetBytes(x))
                put(ix(checkIndex(i, 4)), b);
            return this;
        }
        // long
        public override long getLong()
        {
            return BitConverter.ToInt64(this.hb, ix(nextGetIndex(8)));
        }

        public override long getLong(int i)
        {
            return BitConverter.ToInt64(this.hb, ix(checkIndex(i, 8)));
        }
        public override ByteBuffer putLong(long x)
        {
            foreach (var b in BitConverter.GetBytes(x))
                put(ix(nextGetIndex(8)), b);
            return this;
        }
        public override ByteBuffer putLong(int i, long x)
        {
            foreach (var b in BitConverter.GetBytes(x))
                put(ix(checkIndex(i, 8)), b);
            return this;
        }
        // float
        public override float getFloat()
        {
            return BitConverter.ToSingle(this.hb, ix(nextGetIndex(4)));
        }

        public override float getFloat(int i)
        {
            return BitConverter.ToSingle(this.hb, ix(checkIndex(i, 4)));
        }
        public override ByteBuffer putFloat(float x)
        {
            foreach (var b in BitConverter.GetBytes(x))
                put(ix(nextGetIndex(4)), b);
            return this;
        }
        public override ByteBuffer putFloat(int i, float x)
        {
            foreach (var b in BitConverter.GetBytes(x))
                put(ix(checkIndex(i, 4)), b);
            return this;
        }
        // double
    }
}