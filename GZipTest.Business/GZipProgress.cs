using GZipTest.Abstractions;

namespace GZipTest.Business
{
    public class GZipProgress: IGZipProgress
    {
        public event ProcessProgress ProcessProgressedEvent;
        public void ProcessProgress(long current, long total)
        {
            ProcessProgressedEvent?.Invoke(current, total);
        }
        public event ProcessFinished ProcessFinished;
        public void ProcessFinish()
        {
            ProcessFinished?.Invoke();
        }
    }
}
