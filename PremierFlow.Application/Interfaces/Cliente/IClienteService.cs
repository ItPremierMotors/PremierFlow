using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Cliente;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Cliente
{
    public interface IClienteService
    {
        Task<ApiResponse<ClienteDTO>> GetByIdAsync(int clienteId);
        Task<ApiResponse<List<ClienteDTO>>> GetAllAsync();
        Task<ApiResponse<List<ClienteDTO>>> SearchAsync(string term);
        Task<ApiResponse<ClienteDTO>> GetByDNIAsync(string dni);
        Task<ApiResponse<ClienteDTO>> GetByRTNAsync(string rtn);
        Task<ApiResponse<ClienteDTO>> GetByTelefonoAsync(string telefono);
        Task<ApiResponse<ClienteDTO>> CreateAsync(CreateClienteDTO dto, string usuarioId);
        Task<ApiResponse<ClienteDTO>> UpdateAsync(UpdateClienteDTO dto, string usuarioId);
        Task<ApiResponse<bool>> DeleteAsync(int clienteId, string usuarioId);
        Task<ApiResponse<bool>> IncrementarNoShowAsync(int clienteId);
    }
}
