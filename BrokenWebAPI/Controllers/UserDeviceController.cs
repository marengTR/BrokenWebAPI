using Microsoft.AspNetCore.Mvc;

namespace BrokenWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserDeviceController : ControllerBase
{
    private readonly IList<string> _people;
    private readonly IList<string> _devices;
    private readonly ILogger _logger;
    
    public UserDeviceController(ILogger<UserDeviceController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _people = configuration.GetSection("People").Get<IList<string>>();
        _devices = configuration.GetSection("Devices").Get<IList<string>>();
    }

    [HttpGet]
    public IActionResult Get()
    {
        var random = new Random();
        var nextPeople = _people[random.Next(0, _people.Count - 1)];
        var nextDevice = _devices[random.Next(0, _devices.Count - 1)];
        return Ok(new { Result = $"{nextPeople} has {nextDevice}" });
    }
}