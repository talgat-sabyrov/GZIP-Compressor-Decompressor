using System;
using System.IO;
using GZipTest.Abstractions;
using GZipTest.Data;

namespace GZipTest.Business
{
    public class FileWriterForCompress: IFileWriter
    {
        private readonly IQueueWriter _queueWriter;
        private readonly IGZipProgress _gZipProgress;

        private readonly FileProcessData _fileProcessData;

        public FileWriterForCompress(IQueueWriter queueWriter, IGZipProgress gZipProgress)
        {
            _queueWriter = queueWriter;
            _gZipProgress = gZipProgress;
            _fileProcessData = FileProcessData.Instance;
        }

        public void Write(object destinationFile)
        {
            try
            {
                using (FileStream fileCompressed = new FileStream(destinationFile + Constants.GzipExtention, FileMode.OpenOrCreate))
                {
                    while (!_fileProcessData.Cancelled)
                    {
                        ByteBlock block = _queueWriter.Dequeue();

                        if (_fileProcessData.LastBlockId == block?.Id)
                        {
                            _queueWriter.Stop();
                            _gZipProgress.ProcessFinish();
                        }

                        if (block == null)
                            return;

                        BitConverter.GetBytes(block.Buffer.Length).CopyTo(block.Buffer, 4);
                        fileCompressed.Write(block.Buffer, 0, block.Buffer.Length);
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
