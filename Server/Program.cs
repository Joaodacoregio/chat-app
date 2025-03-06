using Microsoft.EntityFrameworkCore;
using chatApp.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========================== CONFIGURANDO BANCO DE DADOS ========================== //
// Configura o DbContext com SQLite (Banco de Desenvolvimento)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=chatApp.db"));

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

    // Evento para capturar o token do cookie e depura��o
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Obt�m o token do cookie, se existir
            var token = context.Request.Cookies["authToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
                Console.WriteLine($"[AUTH] Token recebido do cookie: {token.Substring(0, Math.Min(token.Length, 10))}..."); // Log para debug
            }
            else
            {
                Console.WriteLine("[AUTH] Nenhum token encontrado no cookie.");
            }
            return Task.CompletedTask;
        }
    };
});

// ========================== CONFIGURANDO CORS ========================== //
// Configura��o para permitir requisi��es do frontend vindo de outro dominio
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5127") // URL do frontend
              .AllowCredentials() // Permite envio de cookies
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ========================== CONFIGURANDO SERVI�OS ========================== //
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ========================== CONFIGURANDO O PIPELINE ========================== //
app.UseCors("AllowAll"); // Ativa a pol�tica de CORS
app.UseAuthentication(); // Importante: deve vir antes de Authorization!
app.UseAuthorization();

app.MapControllers(); // Mapeia os controllers automaticamente

// ========================== SWAGGER (Documenta��o da API) ========================== //
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ========================== INICIANDO A APLICA��O ========================== //
app.Run();
