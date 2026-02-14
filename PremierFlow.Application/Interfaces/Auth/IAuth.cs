using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Auth
{
    public interface IAuth
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
