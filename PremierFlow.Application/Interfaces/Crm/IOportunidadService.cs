using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Interfaces.Crm
{
    public interface IOportunidadService
    {
        Task<ApiResponse<OportunidadDTO>> GetByIdAsync(int oportunidadId); //obtenemos una oportunidad por su ID
        Task<ApiResponse<OportunidadDetalleDTO>> GetDetalleAsync(int oportunidadId); //obtenemos el detalle completo de una oportunidad por su ID, incluyendo información del lead, actividades, notas, etc.
        Task<ApiResponse<List<OportunidadDTO>>> GetAllAsync(int? sucursalId, string? VendedorId); //obtenemos todas las oportunidades, opcionalmente filtradas por sucursal y vendedor
        Task<ApiResponse<List<OportunidadDTO>>> GetByVendedorAsync(string vendedorId); //obtenemos las oportunidades asignadas a un vendedor específico
        Task<ApiResponse<List<OportunidadDTO>>> GetByEtapaAsync(EtapaOportunidad etapa, int? sucursalId, string? VendedorId); //obtenemos las oportunidades filtradas por su etapa y opcionalmente por sucursal
        Task<ApiResponse<List<PipelineResumenDTO>>> GetPipelineAsync(int? sucursalId); //obtenemos el pipeline de oportunidades, opcionalmente filtrado por sucursal
        Task<ApiResponse<List<OportunidadDTO>>> GetVencidasAsync(int? sucursalId, int diasSinActividad = 7); //obtenemos las oportunidades vencidas, opcionalmente filtradas por sucursal
        Task<ApiResponse<OportunidadDTO>> CreateAsync(CreateOportunidadDTO dto, string usuarioId); //creamos una nueva oportunidad a partir de un DTO con la información de la oportunidad a crear y el ID del usuario que la crea
        Task<ApiResponse<OportunidadDTO>> UpdateAsync(UpdateOportunidadDTO dto, string usuarioId); //actualizamos una oportunidad existente a partir de un DTO con la información actualizada y el ID del usuario que la actualiza
       
        Task<ApiResponse<bool>> CerrarGanadaAsync(CerrarGanadaDTO dto, string usuarioId); //cerramos una oportunidad como ganada
        Task<ApiResponse<bool>> CerrarPerdidaAsync(CerrarOportunidadDTO dto, string usuarioId); //cerramos una oportunidad como perdida
        Task<ApiResponse<bool>> CancelarAsync(CerrarOportunidadDTO dto, string usuarioId); //cancelamos una oportunidad
        Task<ApiResponse<bool>> CambiarEtapaAsync(CambiarEtapaDTO dto, string usuarioId); //cambiamos la etapa de una oportunidad validando transiciones permitidas
    }
}
