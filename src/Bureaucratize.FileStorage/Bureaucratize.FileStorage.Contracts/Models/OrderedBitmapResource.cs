namespace Bureaucratize.FileStorage.Contracts.Models
{
    public class OrderedBitmapResource
    {
        public int Order { get; set; }
        public byte[] FileData { get; set; }
        public BitmapFiletype Filetype { get; set; }
    }
}
