using Microsoft.AspNetCore.Authorization;

namespace PremierFlow.Infrastructure.Security
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        //ESTA CLASE SE ENCARGA DE PROPORCIONAR LAS POLITICAS DE AUTORIZACION BASADAS EN LOS PERMISOS DEFINIDOS EN LOS CLAIMS

        //GetPolicyAsync SE LLAMA CUANDO SE SOLICITA UNA POLITICA DE AUTORIZACION Y RECIBE EL NOMBRE DE LA POLITICA SOLICITADA
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy=new AuthorizationPolicyBuilder()
             .RequireClaim("Permission", policyName) //REQUIERE UN CLAIM DE PERMISO CON EL NOMBRE DEL PERMISO COMO VALOR
             .Build(); //CONSTRUIR LA POLITICA DE AUTORIZACION  
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }
      
      //GetDefaultPolicyAsync SE LLAMA CUANDO SE SOLICITA LA POLITICA DE AUTORIZACION POR DEFECTO Y EN ESTE CASO NO SE PROPORCIONA NINGUNA POLITICA POR DEFECTO
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(new AuthorizationPolicyBuilder() 
                .RequireAuthenticatedUser() //REQUIERE UN USUARIO AUTENTICADO
                .Build()
            );
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy?>(null); //NO SE PROPORCIONA NINGUNA POLITICA DE RESPALDO
        }
    }
}