using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using GZipTest.Abstractions;
using GZipTest.Data;

namespace GZipTest.Business
{
    public class FileCompressor : IFileProcessor
    {
        private readonly IQueueReader _queueReader;
        private readonly IQueueWriter _queueWriter;

        private readonly FileProcessData _fileProcessData;

        public FileCompressor(IQueueReader queueReader, IQueueWriter queueWriter)
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
                    ByteBlock block = _queueReader.Dequeue();

                    if (block == null)
                        return;

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var gzs = new GZipStream(memoryStream, CompressionMode.Compress))
                        {
                            gzs.Write(block.Buffer, 0, block.Buffer.Length);
                        }

                        byte[] compressedData = memoryStream.ToArray();
                        var bb = new ByteBlock
                        {
                            Id = block.Id,
                            Buffer = compressedData
                        };

                        _queueWriter.EnqueueForWriting(bb);
                    }
                    ((ManualResetEvent)doneEvent).Set();
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
