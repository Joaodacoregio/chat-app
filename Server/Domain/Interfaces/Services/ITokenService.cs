﻿namespace chatApp.Server.Services.Interfaces
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using chatApp.Server.Domain.Models;

    public interface ITokenService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token, out JwtSecurityToken? jwtToken);
        ClaimsPrincipal DecodeJwtToken(string token);
    }
}
