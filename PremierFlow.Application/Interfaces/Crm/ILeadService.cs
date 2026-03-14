using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Interfaces.Crm
{
    public interface ILeadService
    {
        Task<ApiResponse<LeadDTO>> GetByIdAsync(int leadId); //obtenemos un lead por su ID
        Task<ApiResponse<List<LeadDTO>>> GetAllAsync(int? sucursalId); //obtenemos todos los leads, opcionalmente filtrados por sucursal
        Task<ApiResponse<List<LeadDTO>>> GetByVendedorAsync(string vendedorId); //obtenemos los leads asignados a un vendedor específico
        Task<ApiResponse<List<LeadDTO>>> GetByEstadoAsync(EstadoLead estado, int? sucursalId); //obtenemos los leads filtrados por su estado (Nuevo, Contactado, Calificado, Descartado) y opcionalmente por sucursal
        Task<ApiResponse<List<LeadDTO>>> GetSinAsignarAsync(int? sucursalId); //obtenemos los leads que no tienen un vendedor asignado, opcionalmente filtrados por sucursal
        Task<ApiResponse<List<LeadDTO>>> SearchAsync(string termino); //buscamos leads por término (nombre, correo, teléfono, etc.)
        Task<ApiResponse<LeadDTO>> CreateAsync(CreateLeadDTO dto, string usuarioId); //creamos un nuevo lead a partir de un DTO con la información del lead y el ID del usuario que lo crea
        Task<ApiResponse<LeadDTO>> UpdateAsync(UpdateLeadDTO dto, string usuarioId); //actualizamos un lead existente a partir de un DTO con la información actualizada del lead y el ID del usuario que lo actualiza
        Task<ApiResponse<bool>> AsignarVendedorAsync(AsignarVendedorLeadDTO dto, string usuarioId); //asignamos un vendedor a un lead a partir de un DTO con el ID del lead, el ID del vendedor y el ID del usuario que realiza la asignación
        Task<ApiResponse<bool>> CalificarAsync(CalificarLeadDTO dto, string usuarioId);//calificamos un lead a partir de un DTO con el ID del lead, la calificación (1-5) y el ID del usuario que realiza la calificación
        Task<ApiResponse<OportunidadDTO>> ConvertirAOportunidadAsync(int leadId, CreateOportunidadDTO dto, string usuarioId); //convertimos un lead a oportunidad a partir del ID del lead, un DTO con la información de la oportunidad a crear y el ID del usuario que realiza la conversión
        Task<ApiResponse<bool>> DescartarAsync(DescartarLeadDTO dto, string usuarioId);//descartamos un lead a partir de un DTO con el ID del lead, el motivo de descarte y el ID del usuario que realiza el descarte
    }
}
