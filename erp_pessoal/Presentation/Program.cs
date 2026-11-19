using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtChaveSecreta = Domain.Helpers.AuthHelper._jwtSecret;
var key = Encoding.ASCII.GetBytes(jwtChaveSecreta);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Configuração do CORS lendo a URL do Frontend via variável de ambiente
builder.Services.AddCors(options =>
{
    options.AddPolicy("NetlifyPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration["FRONTEND_URL"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(url => url.TrimEnd('/'))
            .ToArray() ?? Array.Empty<string>();

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// --- AQUELA LINHA ÚNICA PARA CADA CAMADA ---
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();   // Registra tudo da Application
// -------------------------------------------
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS deve ficar antes da autenticação e autorização
app.UseCors("NetlifyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
