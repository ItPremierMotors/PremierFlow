using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Auth;
using PremierFlow.Application.Dtos.User;
using PremierFlow.Application.Interfaces.Auth;
using PremierFlow.Infrastructure.Identity;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.AuthServices;
using PremierFlow.Infrastructure.Security;
using System.Globalization;
using System.Security.Claims;
using System.Text;


namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth auth;
        private readonly IUserService userService;


        public AuthController(IAuth auth, IUserService userService)
        {
            this.auth = auth;
            this.userService = userService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            //try
            //{

            //    var response = await auth.LoginAsync(loginRequest);
            //    return Ok(response);
            //}
            //catch (UnauthorizedAccessException ex)
            //{

            //    return Unauthorized(new { Message = ex.Message });
            //}

            var result = await auth.LoginAsync(loginRequest);

            return StatusCode(result.StatusCode, result);
        }

        //crear usuario
        [HttpPost("create-user")]
        [AllowAnonymous]
       
        public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] CreateUserRequest request)
        {
           if (string.IsNullOrWhiteSpace(request.NombreCompleto))
            {
                return BadRequest(new 
                {
                    Success = false,
                    Message = "El nombre completo es obligatorio"
                });
            }

              request.UserName = GenerarUserName(request.NombreCompleto);

              var result = await userService.CreateAsync(request);
              return StatusCode(result.StatusCode, result);
        }

        //cambio de contraseña
        [Authorize]
        [HttpPut("change-password/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(string userId, [FromBody] ChangePasswordRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != userId)
            {
                return Unauthorized(new { Message = "No autorizado para cambiar la contraseña de este usuario." });
            }
            var result = await userService.ChangePasswordAsync(userId, request);
            return StatusCode(result.StatusCode, result);
        }


        [Authorize(Roles = "AdminTI")]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<ApiResponse<List<UsersDTO>>>> GetAll()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized(new { Message = "No autorizado para ver los usuarios." });
            }
            var result = await userService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<ApiResponse<UsersDTO?>>> GetById(string id)
        {
            var result = await userService.GetByIdAsync(id);

            if (result == null)
            {
                return StatusCode(500, new ApiResponse<UsersDTO?>
                {
                    Success = false,
                    message = "Error al obtener el usuario",
                    StatusCode = 500
                });
            }

            return StatusCode(result.StatusCode, result);
        }

        //update
        [Authorize]
        [HttpPut("Update-User/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(string id, [FromBody] UpdateUserRequest request)
        {
            var result = await userService.UpdateAsync(id, request);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "AdminTI")]
        [HttpPost("Reset-Password/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword(string id, [FromBody] ResetPasswordRequest request)
        {
            var result = await userService.ResetPasswordAsync(id, request);
            return StatusCode(result.StatusCode, result);
        }

        public static string GenerarUserName(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                return string.Empty;

            // Separar por espacios y limpiar
            var partes = nombreCompleto
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length < 2)
                return partes[0].Substring(0, 1).ToUpper();

            var inicialNombre = partes[0].Substring(0, 1);
            var apellido = partes.Length >= 3 ? partes[2] : partes[1];

            var username = inicialNombre + apellido;

            return QuitarAcentos(username).ToUpper();
        }
        private static string QuitarAcentos(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
