using Application;
using Infrastructure;   

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// --- AQUELA LINHA ÚNICA PARA CADA CAMADA ---
builder.Services.AddInfrastructure();
builder.Services.AddApplication();   // Registra tudo da Application
// -------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();