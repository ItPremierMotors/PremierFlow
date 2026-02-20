using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface IRecepcionService
    {
        Task<ApiResponse<RecepcionDTO>> GetByIdAsync(int recepcionId);
        Task<ApiResponse<RecepcionDTO>> GetByOsIdAsync(int osId);
        Task<ApiResponse<List<RecepcionDTO>>> GetPendientesFirmaAsync(int? sucursalId = null);
        Task<ApiResponse<RecepcionDTO>> CreateAsync(CreateRecepcionDTO dto, string usuarioId);
        Task<ApiResponse<RecepcionDTO>> UpdateAsync(UpdateRecepcionDTO dto, string usuarioId);
        Task<ApiResponse<bool>> CompletarChecklistAsync(int recepcionId, string usuarioId);
        Task<ApiResponse<bool>> RegistrarFirmaAsync(RegistrarFirmaDTO dto, string usuarioId);
        Task<ApiResponse<RecepcionDTO>> IniciarDesdeCitaAsync(IniciarRecepcionDTO dto, string usuarioId);
        Task<ApiResponse<DatosCitaWizardDTO>> GetDatosCitaAsync(int citaId);
    }
}
