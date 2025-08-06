namespace Application.DTOs.User;

public class LoginResponse
{
    /// <summary>
    /// Token Jwt para autorización.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6</example>
    public string Token { get; set; } = string.Empty;
}