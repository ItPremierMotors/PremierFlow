using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces.Catalogo;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.UbicacionServices
{
    public class UbicacionService : IUbicacionService
    {
        private readonly PremierFlowDbContext context;

        public UbicacionService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<List<UbicacionDTO>>> GetAllAsync()
        {
            var ubicaciones = await context.Ubicaciones
                .Include(u => u.Sucursal)
                .AsNoTracking()
                .Select(u => new UbicacionDTO
                {
                    Id = u.id,
                    Nombre = u.Nombre,
                    Tipo = u.Tipo.ToString(),
                    SucursalId = u.SucursalID,
                    SucursalNombre = u.Sucursal != null ? u.Sucursal.Nombre : null,
                    Activa = u.Activa
                })
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return ApiResponse<List<UbicacionDTO>>.ok(ubicaciones, "Ubicaciones obtenidas.");
        }

        public async Task<ApiResponse<List<UbicacionDTO>>> GetActivasAsync()
        {
            var ubicaciones = await context.Ubicaciones
                .Include(u => u.Sucursal)
                .AsNoTracking()
                .Where(u => u.Activa)
                .Select(u => new UbicacionDTO
                {
                    Id = u.id,
                    Nombre = u.Nombre,
                    Tipo = u.Tipo.ToString(),
                    SucursalId = u.SucursalID,
                    SucursalNombre = u.Sucursal != null ? u.Sucursal.Nombre : null,
                    Activa = u.Activa
                })
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return ApiResponse<List<UbicacionDTO>>.ok(ubicaciones, "Ubicaciones activas obtenidas.");
        }
    }
}
