using System;
using System.IO;
using GZipTest.Abstractions;
using GZipTest.Data;

namespace GZipTest.Business
{
    public class FileReaderForDecompress: IFileReader
    {
        private readonly IQueueReader _queueReader;
        private readonly IGZipProgress _gZipProgress;

        private readonly FileProcessData _fileProcessData;

        public FileReaderForDecompress(IQueueReader queueReader, IGZipProgress gZipProgress)
        {
            _queueReader = queueReader;
            _gZipProgress = gZipProgress;

            _fileProcessData = FileProcessData.Instance;
        }


        public void Read(object sourceFile)
        {
            try
            {
                int blockId = 0;
                using (FileStream compressedFile = new FileStream(sourceFile.ToString(), FileMode.Open))
                {
                    while (compressedFile.Position < compressedFile.Length)
                    {
                        var lengthBuffer = new byte[Constants.FirstByteCountToDecompress];
                        compressedFile.Read(lengthBuffer, 0, lengthBuffer.Length);
                        int blockLength = BitConverter.ToInt32(lengthBuffer, 4);
                        var compressedData = new byte[blockLength];
                        lengthBuffer.CopyTo(compressedData, 0);

                        compressedFile.Read(compressedData, Constants.FirstByteCountToDecompress, blockLength - Constants.FirstByteCountToDecompress);
                        int dataSize = BitConverter.ToInt32(compressedData, blockLength - 4);
                        byte[] lastBuffer = new byte[dataSize];
                        ByteBlock block = new ByteBlock
                        {
                            Id = blockId,
                            Buffer = lastBuffer,
                            CompressedBuffer = compressedData
                        };
                        _queueReader.EnqueueForDecompressing(block);
                        if ((compressedFile.Length - compressedFile.Position) / Constants.BlockSize == 0)
                            _fileProcessData.LastBlockId = blockId;
                        
                        blockId++;
                        _gZipProgress.ProcessProgress(compressedFile.Position, compressedFile.Length);
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
