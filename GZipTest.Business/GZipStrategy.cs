using System;
using System.Threading;
using GZipTest.Abstractions;

namespace GZipTest.Business
{
    public class GZipStrategy
    {
        private static readonly int ThreadsCount = Environment.ProcessorCount;
        private readonly ManualResetEvent[] _doneEvents = new ManualResetEvent[ThreadsCount];
        
        /// <summary>
        /// Compress/Decompress
        /// </summary>
        private readonly IFileProcessor _fileProcessor;
        private readonly IFileReader _fileReader;
        private readonly IFileWriter _fileWriter;

        public GZipStrategy(IFileProcessor fileProcessor, IFileReader fileReader, IFileWriter fileWriter)
        {
            _fileProcessor = fileProcessor;
            _fileReader = fileReader;
            _fileWriter = fileWriter;
        }

        //Template method
        public void LaunchProcess(string sourceFile, string destinationFile)
        {
            Thread reader = new Thread(_fileReader.Read);
            reader.Start(sourceFile);

            for (int i = 0; i < ThreadsCount; i++)
            {
                _doneEvents[i] = new ManualResetEvent(false);
                Thread gzipProsessor = new Thread(_fileProcessor.Process);
                gzipProsessor.IsBackground = true;
                gzipProsessor.Start(_doneEvents[i]);
            }

            Thread writer = new Thread(_fileWriter.Write);
            writer.Start(destinationFile);
            
            WaitHandle.WaitAll(_doneEvents);
        }
    }
}
