using GZipTest.Data;

namespace GZipTest.Abstractions
{
    public interface IQueueWriter
    {
        void EnqueueForWriting(ByteBlock block);
        ByteBlock Dequeue();
        void Stop();
    }
}
