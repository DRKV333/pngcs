namespace Hjg.Pngcs {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// stream that outputs to memory and allows to flush fragments every 'size'
    /// bytes to some other destination
    /// </summary>
    ///
    abstract internal class ProgressiveOutputStream : Stream {
        private long countFlushed = 0;
        private readonly byte[] buffer;
        private int buffered = 0;

        public override bool CanWrite => true;
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override long Length => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public ProgressiveOutputStream(int size_0) {
            if (size_0 < 8) throw new PngjException("bad size for ProgressiveOutputStream: " + size_0);
            buffer = new byte[size_0];
        }

        public override void Flush() {
            DoFlush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Flush();
                base.Dispose(disposing);
            }
        }

        public override void Write(byte[] b, int off, int len) {
            int end = off + len;
            for (int i = off; i < end; i++)
            {
                buffer[buffered++] = b[i];
                if (buffered == buffer.Length)
                    DoFlush();
            }
        }

        public void Write(byte[] b) {
            Write(b, 0, b.Length);
        }

        private void DoFlush()
        {
            FlushBuffer(buffer, buffered);
            countFlushed += buffered;
            buffered = 0;
        }

        protected abstract void FlushBuffer(byte[] b, int n);

        public long GetCountFlushed() {
            return countFlushed;
        }
    }
}
