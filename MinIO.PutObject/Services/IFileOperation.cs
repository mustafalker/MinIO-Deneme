namespace MinIO.PutObject.Services
{
    public interface IFileOperation
    {
        Task<string> UploadFile(IFormFile file);
        string GetFile(string key);
    }
}
