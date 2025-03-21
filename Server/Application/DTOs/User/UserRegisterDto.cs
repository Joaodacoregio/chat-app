﻿using System.ComponentModel.DataAnnotations;

namespace chatApp.Server.Application.DTOs 
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string Nickname { get; set; } = null!;

        public string? Img { get; set; }
    }
}
