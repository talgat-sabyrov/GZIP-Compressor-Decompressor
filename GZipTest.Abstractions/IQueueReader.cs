using GZipTest.Data;

namespace GZipTest.Abstractions
{
    public interface IQueueReader
    {
        void EnqueueForCompressing(byte[] buffer);
        void EnqueueForDecompressing(ByteBlock block);
        ByteBlock Dequeue();
        void Stop();
    }
}
