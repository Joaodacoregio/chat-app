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

//Adiciono por scoped ,  ServiceProvider, que cont�m todas as depend�ncias registradas.
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());


// ========================== CONFIGURANDO JWT ========================== //
// Recupera as configura��es do JWT do appsettings.json ou User Secrets

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not found!");

// Configura o middleware de autentica��o JWT
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
        ValidateAudience = true, // Valida o p�blico-alvo do token
        ValidateLifetime = true, // Garante que o token n�o tenha expirado
        ValidateIssuerSigningKey = true, // Verifica a assinatura do token

        ValidIssuer = jwtSettings["Issuer"], // Define o emissor v�lido
        ValidAudience = jwtSettings["Audience"], // Define a audi�ncia v�lida
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey) // Converte a chave secreta para um formato v�lido
        )
    };
});

// ========================== CONFIGURANDO CORS ========================== //
// Configura��o para permitir requisi��es do frontend vindo de outro dominio
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
    options.ListenAnyIP(5000); // Porta 5000 dispon�vel na rede
});


// ========================== CONFIGURANDO SERVI�OS ========================== //

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();


//================= Repositories =============================//

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));


var app = builder.Build();

// ========================== CONFIGURANDO O PIPELINE ========================== //
app.UseCors("AllowAll"); // Ativa a pol�tica de CORS
app.UseAuthentication(); // Importante: deve vir antes de Authorization!
app.UseAuthorization();
app.MapControllers(); // Mapeia os controllers automaticamente


//==========   Configura o SignalR no pipeline de middleware =============================//

app.MapHub<ChatHub>("/chatHub");

// ========================== INICIANDO A APLICA��O ========================== //

app.MapGet("/", () => "App rodando!");

app.Run();
