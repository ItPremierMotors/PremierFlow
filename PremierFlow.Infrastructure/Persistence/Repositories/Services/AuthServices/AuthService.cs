using Microsoft.AspNetCore.Identity;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Auth;
using PremierFlow.Application.Interfaces.Auth;
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

        //public async Task<LoginResponse> LoginAsync(LoginRequest request)
        //{
        //    var user = await userManager.FindByEmailAsync(request.Email);
        //    if (user == null || !user.Activo || user.Email == null)
        //    {
        //        throw new UnauthorizedAccessException("Invalid username or password");
        //    }
        //    var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        //    if(result.IsLockedOut)
        //    {
        //        throw new UnauthorizedAccessException("User account is locked out");
        //    }

        //    if (!result.Succeeded)
        //    {
        //        throw new UnauthorizedAccessException("Invalid username or password");
        //    }
        //    var token = await jwtTokenService.GenerateTokenAsync(user);
        //    return new LoginResponse
        //    {
        //        Token = token,
        //        User = new UserInfo
        //        {
        //            Id = user.Id,
        //            UserName = user.UserName,
        //            Email = user.Email
        //        }
        //    };
        //}
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            // Validar request
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return ApiResponse<LoginResponse>.fail(400,
                    new[] { "Email y contraseña son requeridos" },
                    "Datos inválidos");
            }

            // Buscar usuario
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<LoginResponse>.fail(401,
                    new[] { "Credenciales inválidas" },
                    "Usuario o contraseña incorrectos");
            }

            // Verificar si está activo
            if (!user.Activo)
            {
                return ApiResponse<LoginResponse>.fail(401,
                    new[] { "Usuario inactivo" },
                    "Tu cuenta está desactivada. Contacta al administrador");
            }

            // Verificar contraseña
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return ApiResponse<LoginResponse>.fail(423,
                    new[] { "Cuenta bloqueada" },
                    "Tu cuenta ha sido bloqueada por múltiples intentos fallidos. Intenta más tarde");
            }

            //if (result.IsNotAllowed)
            //{
            //    return ApiResponse<LoginResponse>.fail(403,
            //        new[] { "Acceso no permitido" },
            //        "Debes confirmar tu email antes de iniciar sesión");
            //}

            if (!result.Succeeded)
            {
                return ApiResponse<LoginResponse>.fail(401,
                    new[] { "Credenciales inválidas" },
                    "Usuario o contraseña incorrectos");
            }

            // Generar token
            var token = await jwtTokenService.GenerateTokenAsync(user);

            var response = new LoginResponse
            {
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email!
                }
            };

            return ApiResponse<LoginResponse>.ok(response, "Inicio de sesión exitoso");
        }
    }
}
