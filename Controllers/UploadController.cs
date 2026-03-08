using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using REPS_backend.Services;

namespace REPS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public UploadController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            try
            {
                var url = await _cloudinaryService.UploadImageAsync(file);
                if (url == null)
                    return BadRequest("Upload failed.");
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
