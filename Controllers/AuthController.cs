
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Services;

namespace SchoolManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(RegisterRequest req)
    {
        await _service.Register(req);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var tokens = await _service.Login(req);

        return Ok(new
        {
            tokens.Username,
            accessToken = tokens.AccessToken,
            refreshToken = tokens.RefreshToken
        });
    }

}
