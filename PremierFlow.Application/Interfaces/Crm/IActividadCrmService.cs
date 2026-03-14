using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;

namespace PremierFlow.Application.Interfaces.Crm
{
    public interface IActividadCrmService
    {
        Task<ApiResponse<List<ActividadCrmDTO>>> GetByLeadAsync(int leadId); //obtenemos las actividades relacionadas a un lead específico
        Task<ApiResponse<List<ActividadCrmDTO>>> GetByOportunidadAsync(int oportunidadId); //obtenemos las actividades relacionadas a una oportunidad específica
        Task<ApiResponse<List<ActividadCrmDTO>>> GetPendientesVendedorAsync(string vendedorId); //obtenemos las actividades pendientes asignadas a un vendedor específico
        Task<ApiResponse<List<ActividadCrmDTO>>> GetVencidasAsync(int? sucursalId); //obtenemos las actividades vencidas, opcionalmente filtradas por sucursal
        Task<ApiResponse<List<ActividadCrmDTO>>> GetProximosContactosAsync(string vendedorId, DateTime fecha); //obtenemos los próximos contactos asignados a un vendedor específico
        Task<ApiResponse<ActividadCrmDTO>> CreateAsync(CreateActividadCrmDTO dto, string usuarioId); //creamos una nueva actividad de CRM a partir de un DTO con la información de la actividad y el ID del usuario que la crea
        Task<ApiResponse<bool>> CompletarAsync(CompletarActividadDTO dto, string usuarioId); //completamos una actividad de CRM a partir de un DTO con la información de la actividad y el ID del usuario que la completa
        Task<ApiResponse<bool>> CancelarAsync(int actividadCrmId, string usuarioId); //cancelamos una actividad de CRM a partir del ID de la actividad y el ID del usuario que la cancela
    }
}
