using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinIO.PutObject.Services;

namespace MinIO.PutObject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinIo : ControllerBase
    {
        private readonly IFileOperation fileOperation1;
        public MinIo(IFileOperation fileOperation)
        {
            fileOperation1 = fileOperation;
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> FileUpload(IFormFile file)
        {
            try
            {
                var key = await fileOperation1.UploadFile(file);
                return Ok(key);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during file upload." + ex);
            }
        }
    }
}
