using PremierFlow.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces
{
    public interface IAuth
    {
       Task<LoginResponse> LoginAsync(LoginRequest login);
    }
}
