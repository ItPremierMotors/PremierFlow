using Microsoft.AspNetCore.Identity;
using PremierFlow.Application.Dtos.Auth;
using PremierFlow.Application.Interfaces;
using PremierFlow.Infrastructure.Identity;
using PremierFlow.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Text;
using static PremierFlow.Application.Dtos.Auth.LoginResponse;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.AuthServices
{
    public class AuthService:IAuth
    {

        private readonly  UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IJwtTokenService jwtTokenService;


        public AuthService(
         UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager,
         IJwtTokenService jwtTokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.Activo || user.Email == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
               
            if(result.IsLockedOut)
            {
                throw new UnauthorizedAccessException("User account is locked out");
            }

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }
            var token = await jwtTokenService.GenerateTokenAsync(user);
            return new LoginResponse
            {
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                }
            };
        }
    }
}
