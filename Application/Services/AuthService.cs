using Application.DTOs.User;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || user.PasswordHash != request.Password)
            throw new UnauthorizedAccessException("Usuario o contraseña inválidos");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("DocumentId", user.DocumentId)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new LoginResponse { Token = new JwtSecurityTokenHandler().WriteToken(token) };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
            throw new InvalidOperationException("El nombre de usuario ya está en uso");

        var existingDocumentId = await _userRepository.GetByDocumentIdAsync(request.Username);
        if (existingUser != null)
            throw new InvalidOperationException("El documento de identidad ya está en uso");

        var user = new User
        {
            Username = request.Username,
            PasswordHash = request.Password,
            DocumentId = request.DocumentId,
            CreatedAt = DateTime.UtcNow.AddHours(-5)
        };

        await _userRepository.AddAsync(user);

        return await LoginAsync(new LoginRequest
        {
            Username = request.Username,
            Password = request.Password
        });
    }
}
