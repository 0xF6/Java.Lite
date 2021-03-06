﻿/*
 * Copyright 2000-2007 Sun Microsystems, Inc.  All Rights Reserved.
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
    /*
     *
     * @author Mark Reinhold
     * @author JSR-51 Expert Group
     * @since 1.4
     */
    public abstract class Buffer
    {
        // Invariants: mark <= position <= limit <= capacity
        private int mark = -1;
        private int position = 0;
        private int limit;
        private int capacity;

        // Creates a new buffer with the given mark, position, limit, and capacity,
        // after checking invariants.
        //
        protected Buffer(int mark, int pos, int lim, int cap)
        {
            if (cap < 0)
                throw new Exception("IllegalArgumentException: Negative capacity: " + cap);
            this.capacity = cap;
            Limit(lim);
            Position(pos);
            if (mark >= 0)
            {
                if (mark > pos)
                    throw new Exception("IllegalArgumentException: mark > position: (" + mark + " > " + pos + ")");
                this.mark = mark;
            }
        }
        /**
        * Returns this buffer's capacity. </p>
        *
        * @return  The capacity of this buffer
        */
        public int Capacity()
        {
            return capacity;
        }
        protected void setCapacity(int cap)
        {
            capacity = cap;
        }
        /**
        * Returns this buffer's position. </p>
        *
        * @return  The position of this buffer
        */
        public int Position()
        {
            return position;
        }
        /**
         * Sets this buffer's position.  If the mark is defined and larger than the
         * new position then it is discarded. </p>
         *
         * @param  newPosition
         *         The new position value; must be non-negative
         *         and no larger than the current limit
         *
         * @return  This buffer
         *
         * @throws  IllegalArgumentException
         *          If the preconditions on <tt>newPosition</tt> do not hold
         */
        public Buffer Position(int newPosition)
        {
            if ((newPosition > limit) || (newPosition < 0))
                throw new Exception("IllegalArgumentException");
            position = newPosition;
            if (mark > position) mark = -1;
            return this;
        }

        /**
         * Returns this buffer's limit. </p>
         *
         * @return  The limit of this buffer
         */
        public int Limit()
        {
            return limit;
        }
        /**
         * Sets this buffer's limit.  If the position is larger than the new limit
         * then it is set to the new limit.  If the mark is defined and larger than
         * the new limit then it is discarded. </p>
         *
         * @param  newLimit
         *         The new limit value; must be non-negative
         *         and no larger than this buffer's capacity
         *
         * @return  This buffer
         *
         * @throws  IllegalArgumentException
         *          If the preconditions on <tt>newLimit</tt> do not hold
         */
        public Buffer Limit(int newLimit)
        {
            //if ((newLimit > capacity) || (newLimit < 0))
            //    throw new Exception("IllegalArgumentException. newLimit:" + newLimit + ". cap:" + capacity);
            limit = newLimit;
            if (position > limit) position = limit;
            if (mark > limit) mark = -1;
            return this;
        }

        /**
         * Sets this buffer's mark at its position. </p>
         *
         * @return  This buffer
         */
        public Buffer Mark()
        {
            mark = position;
            return this;
        }

        /**
         * Resets this buffer's position to the previously-marked position.
         *
         * <p> Invoking this method neither changes nor discards the mark's
         * value. </p>
         *
         * @return  This buffer
         *
         * @throws  InvalidMarkException
         *          If the mark has not been set
         */
        public Buffer reset()
        {
            int m = mark;
            if (m < 0)
                throw new Exception("InvalidMarkException");
            position = m;
            return this;
        }

        /**
         * Clears this buffer.  The position is set to zero, the limit is set to
         * the capacity, and the mark is discarded.
         *
         * <p> Invoke this method before using a sequence of channel-read or
         * <i>put</i> operations to fill this buffer.  For example:
         *
         * <blockquote><pre>
         * buf.clear();     // Prepare buffer for reading
         * in.read(buf);    // Read data</pre></blockquote>
         *
         * <p> This method does not actually erase the data in the buffer, but it
         * is named as if it did because it will most often be used in situations
         * in which that might as well be the case. </p>
         *
         * @return  This buffer
         */
        public Buffer Clear()
        {
            position = 0;
            limit = capacity;
            mark = -1;
            return this;
        }

        /**
         * Flips this buffer.  The limit is set to the current position and then
         * the position is set to zero.  If the mark is defined then it is
         * discarded.
         *
         * <p> After a sequence of channel-read or <i>put</i> operations, invoke
         * this method to prepare for a sequence of channel-write or relative
         * <i>get</i> operations.  For example:
         *
         * <blockquote><pre>
         * buf.put(magic);    // Prepend header
         * in.read(buf);      // Read data into rest of buffer
         * buf.flip();        // Flip buffer
         * out.write(buf);    // Write header + data to channel</pre></blockquote>
         *
         * <p> This method is often used in conjunction with the {@link
         * java.nio.ByteBuffer#compact compact} method when transferring data from
         * one place to another.  </p>
         *
         * @return  This buffer
         */
        public Buffer Flip()
        {
            limit = position;
            position = 0;
            mark = -1;
            return this;
        }

        /**
         * Rewinds this buffer.  The position is set to zero and the mark is
         * discarded.
         *
         * <p> Invoke this method before a sequence of channel-write or <i>get</i>
         * operations, assuming that the limit has already been set
         * appropriately.  For example:
         *
         * <blockquote><pre>
         * out.write(buf);    // Write remaining data
         * buf.rewind();      // Rewind buffer
         * buf.get(array);    // Copy data into array</pre></blockquote>
         *
         * @return  This buffer
         */
        public Buffer Rewind()
        {
            position = 0;
            mark = -1;
            return this;
        }

        /**
         * Returns the number of elements between the current position and the
         * limit. </p>
         *
         * @return  The number of elements remaining in this buffer
         */
        public int Remaining()
        {
            return limit - position;
        }

        /**
         * Tells whether there are any elements between the current position and
         * the limit. </p>
         *
         * @return  <tt>true</tt> if, and only if, there is at least one element
         *          remaining in this buffer
         */
        public bool hasRemaining()
        {
            return position < limit;
        }

        /**
         * Tells whether or not this buffer is read-only. </p>
         *
         * @return  <tt>true</tt> if, and only if, this buffer is read-only
         */
        public abstract bool isReadOnly();

        /**
         * Tells whether or not this buffer is backed by an accessible
         * array.
         *
         * <p> If this method returns <tt>true</tt> then the {@link #array() array}
         * and {@link #arrayOffset() arrayOffset} methods may safely be invoked.
         * </p>
         *
         * @return  <tt>true</tt> if, and only if, this buffer
         *          is backed by an array and is not read-only
         *
         * @since 1.6
         */
        public abstract bool hasArray();

        /**
         * Returns the array that backs this
         * buffer&nbsp;&nbsp;<i>(optional operation)</i>.
         *
         * <p> This method is intended to allow array-backed buffers to be
         * passed to native code more efficiently. Concrete subclasses
         * provide more strongly-typed return values for this method.
         *
         * <p> Modifications to this buffer's content will cause the returned
         * array's content to be modified, and vice versa.
         *
         * <p> Invoke the {@link #hasArray hasArray} method before invoking this
         * method in order to ensure that this buffer has an accessible backing
         * array.  </p>
         *
         * @return  The array that backs this buffer
         *
         * @throws  ReadOnlyBufferException
         *          If this buffer is backed by an array but is read-only
         *
         * @throws  UnsupportedOperationException
         *          If this buffer is not backed by an accessible array
         *
         * @since 1.6
         */
        public abstract object array();

        /**
         * Returns the offset within this buffer's backing array of the first
         * element of the buffer&nbsp;&nbsp;<i>(optional operation)</i>.
         *
         * <p> If this buffer is backed by an array then buffer position <i>p</i>
         * corresponds to array index <i>p</i>&nbsp;+&nbsp;<tt>arrayOffset()</tt>.
         *
         * <p> Invoke the {@link #hasArray hasArray} method before invoking this
         * method in order to ensure that this buffer has an accessible backing
         * array.  </p>
         *
         * @return  The offset within this buffer's array
         *          of the first element of the buffer
         *
         * @throws  ReadOnlyBufferException
         *          If this buffer is backed by an array but is read-only
         *
         * @throws  UnsupportedOperationException
         *          If this buffer is not backed by an accessible array
         *
         * @since 1.6
         */
        public abstract int arrayOffset();

        /**
         * Tells whether or not this buffer is
         * <a href="ByteBuffer.html#direct"><i>direct</i></a>. </p>
         *
         * @return  <tt>true</tt> if, and only if, this buffer is direct
         *
         * @since 1.6
         */
        public abstract bool isDirect();


        // -- Package-private methods for bounds checking, etc. --

        /**
         * Checks the current position against the limit, throwing a {@link
         * BufferUnderflowException} if it is not smaller than the limit, and then
         * increments the position. </p>
         *
         * @return  The current position value, before it is incremented
         */
        protected int nextGetIndex()
        {
            if (position >= limit)
                throw new Exception("BufferUnderflowException");
            return position++;
        }

        protected int nextGetIndex(int nb)
        {
            if (limit - position < nb)
                throw new Exception("BufferUnderflowException");
            int p = position;
            position += nb;
            return p;
        }

        /**
         * Checks the current position against the limit, throwing a {@link
         * BufferOverflowException} if it is not smaller than the limit, and then
         * increments the position. </p>
         *
         * @return  The current position value, before it is incremented
         */
        protected int nextPutIndex()
        {
            if (position >= limit)
                throw new Exception("BufferOverflowException");
            return position++;
        }

        protected int nextPutIndex(int nb)
        {
            if (limit - position < nb)
                throw new Exception("BufferOverflowException");
            int p = position;
            position += nb;
            return p;
        }

        /**
         * Checks the given index against the limit, throwing an {@link
         * IndexOutOfBoundsException} if it is not smaller than the limit
         * or is smaller than zero.
         */
        protected int checkIndex(int i)
        {
            if ((i < 0) || (i >= limit))
                throw new Exception("IndexOutOfBoundsException");
            return i;
        }

        protected int checkIndex(int i, int nb)
        {
            if ((i < 0) || (nb > limit - i))
                throw new Exception("IndexOutOfBoundsException");
            return i;
        }

        protected int markValue()
        {
            return mark;
        }

        public static void checkBounds(int off, int len, int size)
        {
            if ((off | len | (off + len) | (size - (off + len))) < 0)
                throw new Exception("IndexOutOfBoundsException");
        }
    }
}