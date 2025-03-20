using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;
using chatApp.Server.Data.Context;
using chatApp.Server.Presentation.Hubs;
using chatApp.Server.Domain.Interfaces.Bases;
using chatApp.Server.Application.Bases;
using chatApp.Server.Services.Interfaces;
using chatApp.Server.Services;
using chatApp.Server.Domain.Interfaces.Services;
using chatApp.Server.Application.Repositories;
using chatApp.Server.Domain.Interfaces.Repository;
using chatApp.Server.Application.Services;
using chatApp.Server.Domain.Interfaces.UoW;
using chatApp.Server.Application.UoW;
using chatApp.Server.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

#region Configurando Banco de Dados

string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

#endregion

#region Configurando Identity

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

#endregion

#region Configurando JWT

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not found!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

#endregion

#region Configurando CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                  "http://192.168.1.162:5001")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

#endregion

#region Configurando Kestrel

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

#endregion

#region Configurando Serviços Adicionais

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#endregion

#region Injetar Dependências

builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenKeeper, CookieTokenSave>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserConnectionHubService, UserConnectionHubService>();

#endregion

#region Configurando Pipeline

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapGet("/", () => "App rodando!");

#endregion

#region Iniciando Aplicação

app.Run();

#endregion
