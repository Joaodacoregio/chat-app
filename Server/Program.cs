using Microsoft.EntityFrameworkCore;
using chatApp.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar o DbContext com a string de conexão
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=chatApp.db"));

// Adicionar outros serviços (CORS, Swagger, etc.)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline de requisições
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
