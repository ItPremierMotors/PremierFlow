using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Auth
{
    public interface IVendedorSucursalService
    {
        Task<ApiResponse<VendedorSucursalDTO>> AsignarAsync(AsignarVendedorSucursalDTO dto, string usuarioId);
        Task<ApiResponse<bool>> DesactivarAsync(int vendedorSucursalId, string usuarioId);
        Task<ApiResponse<bool>> ActivarAsync(int vendedorSucursalId, string usuarioId);
        Task<ApiResponse<List<VendedorSucursalDTO>>> GetBySucursalAsync(int sucursalId);
        Task<ApiResponse<List<VendedorSucursalDTO>>> GetByVendedorAsync(string vendedorId);

    }
}
