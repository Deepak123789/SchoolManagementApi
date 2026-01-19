
using Microsoft.IdentityModel.Tokens;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models;
using SchoolManagementApi.Repositories;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolManagementApi.Services;

public class AuthService
{
    private readonly UserRepository _repo;
    private readonly IConfiguration _config;
    private readonly IDatabase _redis;

    public AuthService(UserRepository repo, IConfiguration config, IConnectionMultiplexer redis)
    {
        _repo = repo;
        _config = config;
        _redis = redis.GetDatabase();
    }

    public async Task Register(RegisterRequest req)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);
        var hashs = BCrypt.Net.BCrypt.HashPassword(req.ConfirmPassword);
        await _repo.CreateUserAsync(req.Username, hash, hashs, req.DateOfBirth, req.Email, req.FirstName, req.LastName, req.Gender, req.MobileNumber, req.Role, req.SchoolId);
    }

    public async Task<LoginResponse> Login(LoginRequest req)
    {
        if (req == null)
            throw new ArgumentNullException(nameof(req));

        var user = await _repo.GetByUsernameAsync(req.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // Generate JWT Access Token
        var accessToken = GenerateJwt(user);

        // Generate Refresh Token
        var refreshToken = Guid.NewGuid().ToString("N");

        // Store refresh token in Redis (7 days)
        await _redis.StringSetAsync(
            $"refresh:{user.Id}",
            refreshToken,
            TimeSpan.FromDays(7)
        );

        return new LoginResponse
        {
            Username = user.Username,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private string GenerateJwt(User user)
    {
        // 1. Claims
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };

        // 2. Generate key from appsettings
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Create token
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        // 4. Return token string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
