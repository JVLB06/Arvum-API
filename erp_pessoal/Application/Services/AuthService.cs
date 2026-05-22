using Application.DTOs;
using Application.Interfaces;

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
        // verifica se já existe
        var exists = await _reader.GetUsersAsync();

        if (exists)
        {
            throw new Exception("Email já cadastrado");
        }

        // processa entidade
        var user = new UserDTO(
            newUser.UserName,
            newUser.Email,
            newUser.PasswordHash
        );

        // salva
        await _writer.CreateAsync(user);
    }
}