using GZipTest.Data;

namespace GZipTest.Abstractions
{
    public interface IValidator
    {
        void CommandIsValid(InputCommand input);
    }
}
