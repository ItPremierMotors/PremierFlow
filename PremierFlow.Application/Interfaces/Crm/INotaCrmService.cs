using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;

namespace PremierFlow.Application.Interfaces.Crm
{
    public interface INotaCrmService
    {
        Task<ApiResponse<List<NotaCrmDTO>>> GetByLeadAsync(int leadId); //obtenemos las notas relacionadas a un lead específico
        Task<ApiResponse<List<NotaCrmDTO>>> GetByOportunidadAsync(int oportunidadId); //obtenemos las notas relacionadas a una oportunidad específica
        Task<ApiResponse<NotaCrmDTO>> CreateAsync(CreateNotaCrmDTO dto, string usuarioId); //creamos una nueva nota de CRM a partir de un DTO con la información de la nota y el ID del usuario que la crea
        Task<ApiResponse<bool>> DeleteAsync(int notaCrmId, string usuarioId); //eliminamos una nota de CRM a partir del ID de la nota y el ID del usuario que la elimina
    }
}
