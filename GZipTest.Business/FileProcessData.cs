namespace GZipTest.Business
{
    /// <summary>
    /// Данные необходимые для статуса процесса.
    /// </summary>
    public class FileProcessData
    {
        private static FileProcessData _uniqueStatusData;
        public bool Cancelled { get; set; }
        public int LastBlockId { get; set; }

        private FileProcessData()
        {

        }
        private static readonly object _locker = new object();

        public static FileProcessData Instance
        {
            get
            {
                if (_uniqueStatusData == null)
                {
                    lock (_locker)
                    {
                        if (_uniqueStatusData == null)
                        {
                            _uniqueStatusData = new FileProcessData();
                        }
                    }
                }
                return _uniqueStatusData;
            }
        }
    }
}
