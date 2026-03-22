using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.User;
using PremierFlow.Application.Interfaces.Auth;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Infrastructure.Identity;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.AuthServices
{
    public class VendedorSucursalService : IVendedorSucursalService
{
    private readonly PremierFlowDbContext context;
    private readonly UserManager<ApplicationUser> userManager;

    public VendedorSucursalService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    public async Task<ApiResponse<VendedorSucursalDTO>> AsignarAsync(AsignarVendedorSucursalDTO dto, string usuarioId)
    {
        var vendedor = await userManager.FindByIdAsync(dto.VendedorId);
        if (vendedor == null)
            return ApiResponse<VendedorSucursalDTO>.fail(404, null, "Vendedor no encontrado");

        var sucursal = await context.Sucursales.FirstOrDefaultAsync(s => s.Id == dto.SucursalId && s.Activa);
        if (sucursal == null)
            return ApiResponse<VendedorSucursalDTO>.fail(404, null, "Sucursal no encontrada o inactiva");

        var existente = await context.VendedorSucursales
            .FirstOrDefaultAsync(vs => vs.VendedorId == dto.VendedorId && vs.SucursalId == dto.SucursalId && vs.Activo);

        if (existente != null)
        {
            if (existente.EstaActivo)
                return ApiResponse<VendedorSucursalDTO>.fail(409, null, "El vendedor ya está asignado a esta sucursal");

            existente.EstaActivo = true;
            existente.UsuarioModificaId = usuarioId;
            existente.FechaModificacion = TimeHelper.Now;
            await context.SaveChangesAsync();

            return ApiResponse<VendedorSucursalDTO>.ok(MapToDto(existente, vendedor.NombreCompleto, sucursal.Nombre), "Vendedor reactivado en sucursal");
        }

        var nuevo = new VendedorSucursal
        {
            VendedorId = dto.VendedorId,
            SucursalId = dto.SucursalId,
            EstaActivo = true,
            UsuarioCreaId = usuarioId,
            FechaCreacion = TimeHelper.Now
        };

        context.VendedorSucursales.Add(nuevo);
        await context.SaveChangesAsync();

        return ApiResponse<VendedorSucursalDTO>.ok(MapToDto(nuevo, vendedor.NombreCompleto, sucursal.Nombre), "Vendedor asignado a sucursal exitosamente");
    }

    public async Task<ApiResponse<bool>> DesactivarAsync(int vendedorSucursalId, string usuarioId)
    {
        var registro = await context.VendedorSucursales.FindAsync(vendedorSucursalId);
        if (registro == null)
            return ApiResponse<bool>.fail(404, null, "Registro no encontrado");

        if (!registro.EstaActivo)
            return ApiResponse<bool>.fail(400, null, "El vendedor ya está desactivado en esta sucursal");

        registro.EstaActivo = false;
        registro.UsuarioModificaId = usuarioId;
        registro.FechaModificacion = TimeHelper.Now;
        await context.SaveChangesAsync();

        return ApiResponse<bool>.ok(true, "Vendedor desactivado de sucursal");
    }

    public async Task<ApiResponse<bool>> ActivarAsync(int vendedorSucursalId, string usuarioId)
    {
        var registro = await context.VendedorSucursales.FindAsync(vendedorSucursalId);
        if (registro == null)
            return ApiResponse<bool>.fail(404, null, "Registro no encontrado");

        if (registro.EstaActivo && registro.Activo)
            return ApiResponse<bool>.fail(400, null, "El vendedor ya está activo en esta sucursal");

        registro.EstaActivo = true;
        registro.UsuarioModificaId = usuarioId;
        registro.FechaModificacion = TimeHelper.Now;
        await context.SaveChangesAsync();

        return ApiResponse<bool>.ok(true, "Vendedor activado en sucursal");
    }

    public async Task<ApiResponse<List<VendedorSucursalDTO>>> GetBySucursalAsync(int sucursalId)
    {
        var registros = await context.VendedorSucursales
            .Include(vs => vs.Sucursal)
            .Where(vs => vs.SucursalId == sucursalId && vs.EstaActivo && vs.Activo)
            .ToListAsync();

        var dtos = new List<VendedorSucursalDTO>();
        foreach (var r in registros)
        {
            var vendedor = await userManager.FindByIdAsync(r.VendedorId);
            dtos.Add(MapToDto(r, vendedor?.NombreCompleto, r.Sucursal?.Nombre));
        }

        return ApiResponse<List<VendedorSucursalDTO>>.ok(dtos, "Vendedores por sucursal");
    }

    public async Task<ApiResponse<List<VendedorSucursalDTO>>> GetByVendedorAsync(string vendedorId)
    {
        var vendedor = await userManager.FindByIdAsync(vendedorId);
        if (vendedor == null)
            return ApiResponse<List<VendedorSucursalDTO>>.fail(404, null, "Vendedor no encontrado");

        var registros = await context.VendedorSucursales
            .Include(vs => vs.Sucursal)
            .Where(vs => vs.VendedorId == vendedorId && vs.EstaActivo && vs.Activo)
            .ToListAsync();

        var dtos = registros.Select(r => MapToDto(r, vendedor.NombreCompleto, r.Sucursal?.Nombre)).ToList();

        return ApiResponse<List<VendedorSucursalDTO>>.ok(dtos, "Sucursales del vendedor");
    }

    private VendedorSucursalDTO MapToDto(VendedorSucursal vs, string? vendedorNombre, string? sucursalNombre)
    {
        return new VendedorSucursalDTO
        {
            VendedorSucursalId = vs.VendedorSucursalId,
            VendedorId = vs.VendedorId,
            VendedorNombre = vendedorNombre,
            SucursalId = vs.SucursalId,
            SucursalNombre = sucursalNombre,
            EstaActivo = vs.EstaActivo
        };
    }
}

}