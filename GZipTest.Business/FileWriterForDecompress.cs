using System;
using System.IO;
using GZipTest.Abstractions;
using GZipTest.Data;

namespace GZipTest.Business
{
    public class FileWriterForDecompress : IFileWriter
    {
        private readonly IQueueWriter _queueWriter;
        private readonly IGZipProgress _gZipProgress;

        private readonly FileProcessData _fileProcessData;

        public FileWriterForDecompress(IQueueWriter queueWriter, IGZipProgress gZipProgress)
        {
            _queueWriter = queueWriter;
            _gZipProgress = gZipProgress;
            _fileProcessData = FileProcessData.Instance;
        }

        public void Write(object destinationFile)
        {
            try
            {
                string destinationFileString = destinationFile.ToString();
                using (FileStream decompressedFile = new FileStream(destinationFileString, FileMode.OpenOrCreate))
                {
                    while (!_fileProcessData.Cancelled)
                    {
                        ByteBlock block = _queueWriter.Dequeue();

                        if (_fileProcessData.LastBlockId > 0 && _fileProcessData.LastBlockId == block?.Id)
                        {
                            _queueWriter.Stop();
                            _gZipProgress.ProcessFinish();
                        }

                        if (block == null)
                            return;

                        decompressedFile.Write(block.Buffer, 0, block.Buffer.Length);
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
