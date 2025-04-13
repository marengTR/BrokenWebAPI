using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrokenWebAPI.Models;
using BrokenWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BrokenWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly DatabaseService _databaseService;
    private readonly IConfiguration _configuration;

    public AuthController(
        ILogger<AuthController> logger,
        DatabaseService databaseService,
        IConfiguration configuration)
    {
        _logger = logger;
        _databaseService = databaseService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new ApiResponse { Success = false, Message = "Username and password are required" });
        }

        var isValid = await _databaseService.ValidateUser(request.Username, request.Password);
        
        if (isValid)
        {
            var user = await _databaseService.GetUserByUsername(request.Username);
            
            if (user != null)
            {
                // Generate JWT token
                var token = GenerateJwtToken(user);
                
                return Ok(new ApiResponse 
                { 
                    Success = true, 
                    Message = "Login successful",
                    Data = new { Token = token, Username = user.Username, Role = user.Role }
                });
            }
        }
        
        return Unauthorized(new ApiResponse { Success = false, Message = "Invalid username or password" });
    }

    [HttpGet("admin/users")]
    public IActionResult GetAllUsers()
    {
        
        var users = new List<object>
        {
            new { Id = 1, Username = "admin", Email = "admin@example.com", Role = "Admin" },
            new { Id = 2, Username = "user1", Email = "user1@example.com", Role = "User" },
            new { Id = 3, Username = "user2", Email = "user2@example.com", Role = "User" }
        };
        
        return Ok(new ApiResponse { Success = true, Data = users });
    }

    [HttpPost("process-data")]
    public async Task<IActionResult> ProcessUserData([FromBody] string serializedData)
    {
        try
        {
            var result = await _databaseService.GetUserData(serializedData);
            return Ok(new ApiResponse { Success = true, Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes("ThisIsMyDamnSecretKey");
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
