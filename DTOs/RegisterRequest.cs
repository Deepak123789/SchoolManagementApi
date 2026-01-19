
namespace SchoolManagementApi.DTOs;
public class RegisterRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? Role { get; set; }   // Admin, Teacher, Student, Parent
    public int SchoolId { get; set; } = 0;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;
    public string? Gender { get; set; }
}

