namespace GZipTest.Data
{
    public class ByteBlock
    {
        public int Id { get; set; }
        public byte[] Buffer { get; set; }
        public byte[] CompressedBuffer { get; set; }

        public ByteBlock()
        {
            CompressedBuffer = new byte[0];
        }
    }
}
