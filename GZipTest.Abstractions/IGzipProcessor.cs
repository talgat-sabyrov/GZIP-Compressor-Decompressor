using GZipTest.Data;

namespace GZipTest.Abstractions
{
    public interface IGZipProcessor
    {
        void Process(InputCommand input);
    }
}
