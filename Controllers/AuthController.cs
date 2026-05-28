using Microsoft.AspNetCore.Mvc;
using SendSmsApi.Models;
using SendSmsApi.Services;

namespace SendSmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return Unauthorized();

        if (request.Username != "admin" || request.Password != "admin123")
            return Unauthorized();

        var token = _tokenService.GenerateToken(request.Username);
        return Ok(new { token });
    }
}
