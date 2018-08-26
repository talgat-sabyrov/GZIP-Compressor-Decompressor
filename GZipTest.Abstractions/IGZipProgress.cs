namespace GZipTest.Abstractions
{
    public delegate void ProcessProgress(long current, long total);

    public delegate void ProcessFinished();
    public interface IGZipProgress
    {
        event ProcessProgress ProcessProgressedEvent;
        void ProcessProgress(long current, long total);
        event ProcessFinished ProcessFinished;
        void ProcessFinish();
    }
}
