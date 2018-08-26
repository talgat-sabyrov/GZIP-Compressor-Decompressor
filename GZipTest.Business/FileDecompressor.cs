using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using GZipTest.Abstractions;
using GZipTest.Data;

namespace GZipTest.Business
{
    public class FileDecompressor : IFileProcessor
    {
        private readonly IQueueReader _queueReader;
        private readonly IQueueWriter _queueWriter;

        private readonly FileProcessData _fileProcessData;

        public FileDecompressor(IQueueReader queueReader, IQueueWriter queueWriter)
        {
            _queueReader = queueReader;
            _queueWriter = queueWriter;

            _fileProcessData = FileProcessData.Instance;
        }

        public void Process(object doneEvent)
        {
            if (!(doneEvent is ManualResetEvent))
                throw new ArgumentException();

            try
            {
                while (!_fileProcessData.Cancelled)
                {
                    ByteBlock byteBlock = _queueReader.Dequeue();
                    if (byteBlock == null)
                        return;

                    using (MemoryStream ms = new MemoryStream(byteBlock.CompressedBuffer))
                    {
                        using (GZipStream gzs = new GZipStream(ms, CompressionMode.Decompress))
                        {
                            gzs.Read(byteBlock.Buffer, 0, byteBlock.Buffer.Length);
                            byte[] decompressedData = byteBlock.Buffer.ToArray();
                            ByteBlock block = new ByteBlock
                            {
                                Id = byteBlock.Id,
                                Buffer = decompressedData
                            };
                            _queueWriter.EnqueueForWriting(block);
                        }
                        ((ManualResetEvent)doneEvent).Set();
                    }
                }
            }
            catch (Exception ex)
            {
                _fileProcessData.Cancelled = true;
                throw new Exception(ex.Message);
            }
        }
    }
}
