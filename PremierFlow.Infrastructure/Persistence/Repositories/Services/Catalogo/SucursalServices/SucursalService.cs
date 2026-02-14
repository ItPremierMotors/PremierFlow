using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces.Catalogo;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.SucursalServices
{
    public class SucursalService : ISucursalService
    {
        private readonly PremierFlowDbContext context;

        public SucursalService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<List<SucursalDTO>>> GetAllAsync()
        {
            var sucursales = await context.Sucursales
                .AsNoTracking()
                .Where(s => s.Activa)
                .Select(s => new SucursalDTO
                {
                    Id = s.Id,
                    Codigo = s.Codigo,
                    Nombre = s.Nombre,
                    Ciudad = s.Ciudad,
                    Direccion = s.Direccion,
                    Activa = s.Activa
                })
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            return ApiResponse<List<SucursalDTO>>.ok(sucursales, "Sucursales obtenidas.");
        }

        public async Task<ApiResponse<SucursalDTO>> GetByIdAsync(int sucursalId)
        {
            var sucursal = await context.Sucursales
                .AsNoTracking()
                .Where(s => s.Id == sucursalId && s.Activa)
                .Select(s => new SucursalDTO
                {
                    Id = s.Id,
                    Codigo = s.Codigo,
                    Nombre = s.Nombre,
                    Ciudad = s.Ciudad,
                    Direccion = s.Direccion,
                    Activa = s.Activa
                })
                .FirstOrDefaultAsync();

            if (sucursal == null)
                return ApiResponse<SucursalDTO>.fail(404, null, "Sucursal no encontrada.");

            return ApiResponse<SucursalDTO>.ok(sucursal, "Sucursal obtenida.");
        }
    }
}
