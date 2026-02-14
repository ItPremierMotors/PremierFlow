using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface IEvidenciaService
    {
        Task<ApiResponse<EvidenciaDTO>> GetByIdAsync(int evidenciaId);
        Task<ApiResponse<List<EvidenciaDTO>>> GetByOsIdAsync(int osId);
        Task<ApiResponse<List<EvidenciaDTO>>> GetByRecepcionIdAsync(int recepcionId);
        Task<ApiResponse<List<EvidenciaDTO>>> GetByTipoAsync(int osId, TipoEvidencia tipo);
        Task<ApiResponse<List<EvidenciaDTO>>> GetFotosDanoAsync(int osId);
        Task<ApiResponse<EvidenciaDTO>> AgregarAsync(CreateEvidenciaDTO dto, string usuarioId);
        Task<ApiResponse<EvidenciaDTO>> AgregarBase64Async(CreateEvidenciaBase64DTO dto, string usuarioId);
        Task<ApiResponse<bool>> EliminarAsync(int evidenciaId, string usuarioId);
    }
}
