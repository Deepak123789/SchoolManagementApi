
namespace SchoolManagementApi.DTOs;
public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; } = "";
}
public class LoginResponse
{
    public string? Username { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? Role { get; set; } = "Admin";
}
