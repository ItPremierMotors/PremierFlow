using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Auth
{
    public sealed class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public sealed class LoginResponse
    {
        public string Token { get; set; } = null!;
        public UserInfo User { get; set; } = new();
    }
        public sealed class UserInfo
        {
            public string Id { get; set; } = null!;
            public string? UserName { get; set; }
            public string Email { get; set; } = null!;
        }
 }
