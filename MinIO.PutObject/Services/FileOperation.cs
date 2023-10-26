using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;

namespace MinIO.PutObject.Services
{
    public class FileOperation : IFileOperation
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileOperation> _logger;
        private string MinioSecretkey => _configuration["MinioAccessInfo:SecretKey"];
        private string MinIoPassword => _configuration["MinioAccessInfo:Password"];
        private string MinIoEndPoint => _configuration["MinioAccessInfo:EndPoint"];
        private string BucketName => _configuration["MinioAccessInfo:BucketName"];
        private readonly AmazonS3Client _client;

        public FileOperation(IConfiguration configuration, ILogger<FileOperation> logger)
        {
            _configuration = configuration;
            _logger = logger;
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName("us-east-1"),
                ServiceURL = MinIoEndPoint,
                ForcePathStyle = true,
                SignatureVersion = "2"
            };
            _client = new AmazonS3Client(MinioSecretkey, MinIoPassword, config);
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            var key = String.Empty;
            try
            {
                key = Guid.NewGuid().ToString();
                var stream = file.OpenReadStream();
                var request = new PutObjectRequest()
                {
                    BucketName = BucketName,
                    InputStream = stream,
                    AutoCloseStream = true,
                    Key = key,
                    ContentType = file.ContentType
                };
                var encodedFilename = Uri.EscapeDataString(file.FileName);
                request.Metadata.Add("original-filename", encodedFilename);
                request.Headers.ContentDisposition = $"attachment; filename=\"{encodedFilename}\"";
                await _client.PutObjectAsync(request);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred in UploadFile", e);
            }
            return key;
        }
        public string GetFile(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            return _client.GetPreSignedURL(new GetPreSignedUrlRequest()
            {
                BucketName = BucketName,
                Key = key,
                Expires = DateTime.Now.AddMinutes(30)
            });
        }
    }
}