using PremierFlow.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Security
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);

    }
}
