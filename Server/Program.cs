using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using chatApp.Server.Data.Context;
using chatApp.Server.Domain.Repositories.Bases;
using chatApp.Server.Domain.Repositories.Interfaces;
using chatApp.Server.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ========================== CONFIGURANDO BANCO DE DADOS ========================== //

// Configura o DbContext com SQLite (Banco de Desenvolvimento)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=chatApp.db"));

//Adiciono por scoped ,  ServiceProvider, que contém todas as dependências registradas.
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());


// ========================== CONFIGURANDO JWT ========================== //
// Recupera as configurações do JWT do appsettings.json ou User Secrets

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not found!");

// Configura o middleware de autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida o emissor do token
        ValidateAudience = true, // Valida o público-alvo do token
        ValidateLifetime = true, // Garante que o token não tenha expirado
        ValidateIssuerSigningKey = true, // Verifica a assinatura do token

        ValidIssuer = jwtSettings["Issuer"], // Define o emissor válido
        ValidAudience = jwtSettings["Audience"], // Define a audiência válida
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey) // Converte a chave secreta para um formato válido
        )
    };
});

// ========================== CONFIGURANDO CORS ========================== //
// Configuração para permitir requisições do frontend vindo de outro dominio
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://192.168.1.162:5001") // URL do frontend
              .AllowCredentials() // Permite envio de cookies
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ======================================================= //

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // Porta 5000 disponível na rede
});


// ========================== CONFIGURANDO SERVIÇOS ========================== //

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();


//================= Repositories =============================//

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));


var app = builder.Build();

// ========================== CONFIGURANDO O PIPELINE ========================== //
app.UseCors("AllowAll"); // Ativa a política de CORS
app.UseAuthentication(); // Importante: deve vir antes de Authorization!
app.UseAuthorization();
app.MapControllers(); // Mapeia os controllers automaticamente


//==========   Configura o SignalR no pipeline de middleware =============================//

app.MapHub<ChatHub>("/chatHub");

// ========================== INICIANDO A APLICAÇÃO ========================== //

app.MapGet("/", () => "App rodando!");

app.Run();
