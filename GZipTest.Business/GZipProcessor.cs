using GZipTest.Abstractions;
using GZipTest.Data;

namespace GZipTest.Business
{
    public class GZipProcessor : IGZipProcessor
    {
        private readonly IQueueWriter _queueWriter;
        private readonly IQueueReader _queueReader;
        private readonly IValidator _validator;
        private readonly IGZipProgress _gZipProgress;

        public GZipProcessor(IQueueReader queueReader, IQueueWriter queueWriter, IGZipProgress gZipProgress, IValidator validator)
        {
            _queueReader = queueReader;
            _queueWriter = queueWriter;
            _gZipProgress = gZipProgress;
            _validator = validator;
        }

        private GZipStrategy _gZipStrategy;
        public void Process(InputCommand input)
        {
            _validator.CommandIsValid(input);

            switch (input.CommandName)
            {
                case Constants.CompressCommand:
                    _gZipStrategy = GZipFactory.CompressorFactory(_queueReader, _queueWriter, _gZipProgress);
                    break;
                case Constants.DecompressCommand:
                    _gZipStrategy = GZipFactory.DecompressorFactory(_queueReader, _queueWriter, _gZipProgress);
                    break;
            }

            _gZipStrategy.LaunchProcess(input.SourceFile, input.DestinationFile);
        }
    }
}
