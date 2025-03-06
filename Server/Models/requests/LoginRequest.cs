﻿using System.ComponentModel.DataAnnotations;

namespace chatApp.Server.Models.requests
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Required]
        [StringLength(100,MinimumLength =6)]
        public string Password { get; set; } = string.Empty;


    }
}
