using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PremierFlow.Application.Dtos.Auth
{
    public sealed class LoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("password")]
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
