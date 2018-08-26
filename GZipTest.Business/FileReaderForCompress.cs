using System;
using System.IO;
using GZipTest.Abstractions;

namespace GZipTest.Business
{
    public class FileReaderForCompress: IFileReader
    {
        private readonly IQueueReader _queueReader;
        private readonly IGZipProgress _gZipProgress;

        private readonly FileProcessData _fileProcessData;

        public FileReaderForCompress(IQueueReader queueReader, IGZipProgress gZipProgress)
        {
            _queueReader = queueReader;
            _gZipProgress = gZipProgress;
            _fileProcessData = FileProcessData.Instance;
        }

        public void Read(object sourceFile)
        {
            try
            {
                using (FileStream fileToBeCompressed = new FileStream(sourceFile.ToString(), FileMode.Open))
                {
                    _fileProcessData.LastBlockId = Convert.ToInt32(fileToBeCompressed.Length / Constants.BlockSize);
                    while (fileToBeCompressed.Position < fileToBeCompressed.Length && !_fileProcessData.Cancelled)
                    {
                        int bytesToRead;
                        long readingFileBlock = fileToBeCompressed.Length - fileToBeCompressed.Position;
                        if (readingFileBlock <= Constants.BlockSize)
                        {
                            bytesToRead = (int)(readingFileBlock);
                        }
                        else
                        {
                            bytesToRead = Constants.BlockSize;
                        }
                        
                        var lastBuffer = new byte[bytesToRead];
                        fileToBeCompressed.Read(lastBuffer, 0, bytesToRead);
                        _queueReader.EnqueueForCompressing(lastBuffer);
                        _gZipProgress.ProcessProgress(fileToBeCompressed.Position, fileToBeCompressed.Length);
                    }
                    _queueReader.Stop();
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
