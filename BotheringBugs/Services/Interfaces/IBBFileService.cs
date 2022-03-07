namespace BotheringBugs.Services.Interfaces
{
    public interface IBBFileService
    {
        public Task<byte[]> ConvertFileToByteArraySync(IFormFile file);
        public string ConvertByteArrayToFile(byte[] fileData, string extension);
        public string GetFileIcon(string file);
        public string FormatFileSize(long bytes);
    }
}
