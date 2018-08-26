namespace GZipTest.Abstractions
{
    public interface IFileProcessor
    {
        /// <summary>
        /// Compress/Decompress
        /// </summary>
        /// <param name="doneEvent"></param>
        void Process(object doneEvent);
    }
}
