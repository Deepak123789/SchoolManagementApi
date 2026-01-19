
using Microsoft.Data.SqlClient;
using SchoolManagementApi.Models;
using System.Data;

namespace SchoolManagementApi.Repositories;

public class UserRepository
{
    private readonly string _conn;

    public UserRepository(IConfiguration config)
    {
        _conn = config.GetConnectionString("SqlServer");
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sp_GetUserByUsername", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Username", username);
        await con.OpenAsync();

        using var rd = await cmd.ExecuteReaderAsync();
        if (!rd.Read()) return null;

        return new User
        {
            Id = rd.GetInt32(0),
            Username = rd.GetString(1),
            PasswordHash = rd.GetString(2)
        };
    }

    public async Task CreateUserAsync(string username, string passwordHash, string ConfirmPassword, DateTime DateOfBirth, string Email, string FirstName, string LastName, string Gender, string MobileNumber, string Role, int SchoolId)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sp_CreateUser", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;
        cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = passwordHash;
        cmd.Parameters.Add("@ConfirmPassword", SqlDbType.NVarChar, 500).Value = ConfirmPassword;
        cmd.Parameters.Add("@DateOfBirth", SqlDbType.Date).Value = DateOfBirth;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = Email;
        cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 100).Value = FirstName;
        cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 100).Value = LastName;
        cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 20).Value = Gender;
        cmd.Parameters.Add("@MobileNumber", SqlDbType.NVarChar, 20).Value = MobileNumber;
        cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = Role;
        cmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = SchoolId;
        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
