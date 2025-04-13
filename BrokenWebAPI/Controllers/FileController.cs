using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using BrokenWebAPI.Models;

namespace BrokenWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly IWebHostEnvironment _env;

    public FileController(ILogger<FileController> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }


    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });
        }

        var filePath = Path.Combine(_env.ContentRootPath, "Uploads", file.FileName);
        
        // Ensure directory exists
        Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, "Uploads"));
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new ApiResponse 
        { 
            Success = true, 
            Message = "File uploaded successfully",
            Data = new { FileName = file.FileName, Size = file.Length }
        });
    }

    [HttpGet("ping")]
    public IActionResult PingHost(string host)
    {
        if (string.IsNullOrEmpty(host))
        {
            return BadRequest(new ApiResponse { Success = false, Message = "Host parameter is required" });
        }

        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"ping -c 4 {host}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrEmpty(error))
            {
                return Ok(new ApiResponse { Success = true, Data = output });
            }
            else
            {
                return BadRequest(new ApiResponse { Success = false, Message = error });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse { Success = false, Message = ex.Message });
        }
    }

    [HttpGet("get-file")]
    public IActionResult GetFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest(new ApiResponse { Success = false, Message = "fileName parameter is required" });
        }

        try
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Uploads", fileName);
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new ApiResponse { Success = false, Message = "File not found" });
            }

            var fileContent = System.IO.File.ReadAllText(filePath);
            return Ok(new ApiResponse { Success = true, Data = fileContent });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse { Success = false, Message = ex.Message });
        }
    }
}
