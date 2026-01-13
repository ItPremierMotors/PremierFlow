//using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PremierFlow.Infrastructure.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


namespace PremierFlow.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {

        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        public JwtTokenService(IConfiguration _configuration, UserManager<ApplicationUser> _userManager)
        {
            configuration=_configuration;
            userManager=_userManager;
        }
        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            //claims son el corazon del token es la informacion
            var authClaims = new List<Claim>
           {
               new Claim(JwtRegisteredClaimNames.Sub, user.Id), //INDENTIFICADOR UNICO DEL USUARIO
               new Claim(ClaimTypes.NameIdentifier, user.Id),//INDENTIFICADOR UNICO DEL USUARIO usado por aspnetcore
               new Claim(ClaimTypes.Name, user.UserName ?? ""), //NOMBRE DEL USUARIO usado por aspnetcore
               new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""), //EMAIL DEL USUARIO
               new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""), //NOMBRE DEL USUARIO
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //IDENTIFICADOR UNICO DEL TOKEN
           };
            var roles = await userManager.GetRolesAsync(user); //OBTENER LOS ROLES DEL USUARIO

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role)); //AGREGAR LOS ROLES A LOS CLAIMS
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]!)); //LLAVE SECRETA PARA FIRMAR EL TOKEN

            var creds= new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //CREDENCIALES DE FIRMA

            //var expiresMinutes= Convert.ToDouble(configuration["Jwt:expiresInMinutes"]); //TIEMPO DE EXPIRACION DEL TOKEN
            var expiresMinutes=double.Parse(configuration["Jwt:expiresInMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:issuer"], //EMISOR DEL TOKEN
                audience: configuration["Jwt:audience"], //AUDIENCIA DEL TOKEN
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes), //FECHA DE EXPIRACION
                claims: authClaims, //CLAIMS DEL TOKEN
                signingCredentials: creds //CREDENCIALES DE FIRMA
            );

            return new JwtSecurityTokenHandler().WriteToken(token); //RETORNAR EL TOKEN COMO CADENA
        }
    }
}
