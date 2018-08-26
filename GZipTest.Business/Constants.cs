namespace GZipTest.Business
{
    public static class Constants
    {
        public const string CompressCommand = "compress";
        public const string DecompressCommand = "decompress";

        public const int FirstByteCountToDecompress = 4096;
        public const int BlockSize = 10000000;
        public const int QueueCountLimit = 25;

        public const int WordsCount = 3;
        public const int CommandIndex = 0;
        public const int SourceIndex = 1;
        public const int DestinationIndex = 2;
        public const string GzipExtention = ".gz";
    }
}
