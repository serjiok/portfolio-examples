using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public sealed class FileController : ControllerBase
{
    [HttpGet(nameof(Download))]
    public IActionResult Download(string filename, CancellationToken cancellationToken = default)
    {
        filename = Path.Combine(AppContext.BaseDirectory, "upload", filename);
        if (!System.IO.File.Exists(filename)) return NotFound();
        var fileStream = new FileStream(filename, FileMode.Open, 
            FileAccess.Read, 
            FileShare.Read, 4096, 
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        return new FileStreamResult(fileStream, "application/octet-stream")
        {
            FileDownloadName = fileStream.Name,
            EnableRangeProcessing = true
        }; 
    }

    [HttpPut(nameof(Upload))]
    public async Task<IActionResult> Upload(CancellationToken cancellationToken = default)
    {
        string fileName = 
        Request.Headers.Any(x => x.Key == "filename") ? 
        Request.Headers["filename"]!:
        Path.Combine(AppContext.BaseDirectory, "upload", Path.GetRandomFileName());
        
        Directory.CreateDirectory(Path.GetDirectoryName(fileName)!);
        using Stream fileStream = new FileStream(fileName, FileMode.Create); 
        await Request.Body.CopyToAsync(fileStream, cancellationToken);
        return Ok();
    }
}
