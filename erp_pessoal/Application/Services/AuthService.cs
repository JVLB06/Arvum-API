using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

public class AuthService : IAuthService
{
    private readonly IAuthReader _reader;

    private readonly IAuthWriter _writer;

    public AuthService(
        IAuthReader reader,
        IAuthWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task RegisterAsync(UserDTO newUser)
    {
        var exists = await _reader.GetUserByEmailAsync(newUser.Email);

        if (exists is not null)
            throw new Exception("Email já cadastrado");

        var user = new UserEntity(
            newUser.UserName,
            newUser.Email,
            newUser.Password,
            newUser.BirthDate
        );

        await _writer.CreateUserAsync(user);
    }

    public async Task<LoginEntity> LoginAsync(LoginDTO login)
    {
        var connect = await _reader.GetLoginAsync(login);

        if (BCrypt.Net.BCrypt.Verify(connect.Password, login.Password))
        {
            var acces = new LoginEntity(
                login.Id,
                login.Email,
                login.Password);

            return acces;
        }

        throw new Exception("Email ou senha inválidos");
    }
}