using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using erp_pessoal.Controllers;

var builder = WebApplication.CreateBuilder(args);

// 1. Adiciona suporte a Controllers (MVC)
builder.Services.AddControllers();

// 2. Configuração do JWT
var chaveJwt = Essentials._jwtSecret; 
var key = Encoding.ASCII.GetBytes(chaveJwt);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// 3. Middlewares da aplicação
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 4. Mapeia controllers
app.MapControllers();
app.Run();
