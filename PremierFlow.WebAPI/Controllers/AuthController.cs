using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Auth;
using PremierFlow.Application.Dtos.User;
using PremierFlow.Application.Interfaces.Auth;
using PremierFlow.Infrastructure.Identity;
using PremierFlow.Infrastructure.Security;
using System.Security.Claims;


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
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {

                var response = await auth.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {

                return Unauthorized(new { Message = ex.Message });
            }
        }

        //crear usuario
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(CreateUserRequest reques)
        {
            try
            {
                var response = await userService.CreateAsync(reques);
                return Ok(response);

            }
            catch (Exception ex)
            {

                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        }

        //cambio de contraseña
        [Authorize]
        [HttpPut("{userId}/change-password")]
        public async Task<IActionResult> ChangePassword([FromRoute] string userId, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId != userId)
                {
                    return Unauthorized(new { Message = "No autorizado para cambiar la contraseña de este usuario." });
                }
                var response = await userService.ChangePasswordAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "AdminTI")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                //verificar que sea el rol adecuado
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == null)
                {
                    return Unauthorized(new { Message = "No autorizado para ver los usuarios." });
                }
                var Usuarios = await userService.GetAllAsync();
                return Ok(Usuarios);

            }
            catch (Exception)
            {

                //retornar error generico 
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al obtener los usuarios." });
            }
        }
        [Authorize(Roles = "AdminTI")]
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            try
            {
                //verificar que sea el rol adecuado
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == null)
                {
                    return Unauthorized(new { Message = "No autorizado para ver los usuarios." });
                }
                var Usuarios = await userService.GetByIdAsync(id);
                return Ok(Usuarios);

            }
            catch (Exception)
            {

                //retornar error generico 
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al obtener los usuarios." });
            }
        }

        //update
        [Authorize]
        [HttpPut("Update-User/{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request, [FromRoute] string id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == null)
                {
                    return Unauthorized(new { Message = "No autorizado para ver los usuarios." });
                }
                var resultado = await userService.UpdateAsync(id, request);
                return StatusCode(StatusCodes.Status200OK, new { message = "Usuario Modificado" });

            }
            catch (Exception ex)
            {


                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error al obtener los usuarios. {ex.Message}" });


            }

        }

        [Authorize(Roles = "AdminTI")]
        [HttpPost("Reset-Password/{id}")]
        public async Task<IActionResult> ResetPassword(
        [FromRoute] string id,
        [FromBody] ResetPasswordRequest request)
        {
            try
            {
                // Aquí ya no necesitas validar el rol manualmente porque
                // [Authorize(Roles = "AdminTI")] ya bloquea a quien no lo tenga.

                var result = await userService.ResetPasswordAsync(id, request);

                if (!result) // si falla es false
                {
                    return StatusCode(
                        StatusCodes.Status400BadRequest,
                        new { Message = "Error al cambiar la contraseña." }
                    );
                }

                return StatusCode(
                    StatusCodes.Status200OK,
                    new { Message = "Contraseña cambiada correctamente." }
                );
            }
            catch (Exception ex)
            {
                // Idealmente aquí loggearías el error con ILogger
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = $"Error interno al cambiar la contraseña.{ex.Message}" }
                );
            }
        }


    }
}
