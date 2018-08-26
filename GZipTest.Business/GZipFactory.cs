using GZipTest.Abstractions;

namespace GZipTest.Business
{
    public static class GZipFactory
    {
        public static GZipStrategy CompressorFactory(IQueueReader queueReader, IQueueWriter queueWriter, IGZipProgress gZipProgress)
        {
            var fileCompressor = new FileCompressor(queueReader, queueWriter);
            var fileReader = new FileReaderForCompress(queueReader, gZipProgress);
            var fileWriter = new FileWriterForCompress(queueWriter, gZipProgress);
            return new GZipStrategy(fileCompressor, fileReader, fileWriter);
        }

        public static GZipStrategy DecompressorFactory(IQueueReader queueReader, IQueueWriter queueWriter, IGZipProgress gZipProgress)
        {
            var fileCompressor = new FileDecompressor(queueReader, queueWriter);
            var fileReader = new FileReaderForDecompress(queueReader, gZipProgress);
            var fileWriter = new FileWriterForDecompress(queueWriter, gZipProgress);
            return new GZipStrategy(fileCompressor, fileReader, fileWriter);
        }
    }
}
