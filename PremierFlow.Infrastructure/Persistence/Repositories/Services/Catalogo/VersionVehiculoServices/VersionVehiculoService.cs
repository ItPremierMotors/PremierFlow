using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces.Catalogo;
using PremierFlow.Domain.Entities;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.VersionVehiculoServices
{
    public class VersionVehiculoService : IVersionVehiculoService
    {
        private readonly PremierFlowDbContext context;
        public VersionVehiculoService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<VersionVehiculoDTO>> CreateAsync(CreateVersionVehiculoDTO dto, string usuarioId)
        {
            // 1. Validar que el modelo existe y está activo
            var modelo = await context.Modelos
                .Include(m => m.Marca)
                .FirstOrDefaultAsync(m => m.ModeloId == dto.ModeloId && m.Activo);

            if (modelo == null)
                return ApiResponse<VersionVehiculoDTO>.fail(400, null, "El modelo no existe o está inactivo.");

            // 2. Validar código duplicado
            var codigoExiste = await context.Versiones
                .AnyAsync(v => v.Codigo == dto.Codigo && v.Activo);

            if (codigoExiste)
                return ApiResponse<VersionVehiculoDTO>.fail(400, null, "Ya existe una versión con ese código.");

            // 3. Crear la versión
            var version = new VersionVehiculo
            {
                ModeloId = dto.ModeloId,
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Motor = dto.Motor,
                Transmision = dto.Transmision,
                Traccion = dto.Traccion,
                NumPuertas = dto.NumPuertas,
                NumPasajeros = dto.NumPasajeros,
                TipoCombustible = dto.TipoCombustible,
                Cilindraje = dto.Cilindraje,
                PotenciaHp = dto.PotenciaHp,
                TorqueNm = dto.TorqueNm,
                PrecioBase = dto.PrecioBase,
                AnioVersion = dto.AnioVersion,
                CaracteristicasPrincipales = dto.CaracteristicasPrincipales,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            // 4. Guardar
            context.Versiones.Add(version);
            await context.SaveChangesAsync();

            // 5. Asignar navegación para el MapToDto
            version.Modelo = modelo;

            return ApiResponse<VersionVehiculoDTO>.ok(MapToDto(version), "Versión creada exitosamente.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int versionId, string usuarioId)
        {
            // 1. Buscar la versión
            var version = await context.Versiones
                .FirstOrDefaultAsync(v => v.VersionId == versionId && v.Activo);

            if (version == null)
                return ApiResponse<bool>.fail(404, null, "Versión no encontrada.");

            // 2. Validar que no tenga vehículos activos
            var tieneVehiculos = await context.Vehiculos
                .AnyAsync(v => v.VersionId == versionId && v.Activo);

            if (tieneVehiculos)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar, la versión tiene vehículos asociados.");

            // 3. Soft delete
            version.Activo = false;
            version.UsuarioModificaId = usuarioId;
            version.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Versión eliminada exitosamente.");
        }

        public async Task<ApiResponse<List<VersionVehiculoDTO>>> GetAllAsync()
        {
            var versionVehiculo = await context.Versiones
                .Include(v => v.Modelo)
                .Include(v => v.Modelo.Marca)
                .AsNoTracking()
                .Where(v => v.Activo && v.Modelo.Activo)
                .Select(v => new VersionVehiculoDTO
                {
                    VersionId = v.VersionId,
                    ModeloId = v.ModeloId,
                    Codigo = v.Codigo,
                    Nombre = v.Nombre,
                    Motor = v.Motor,
                    Transmision = v.Transmision,
                    Traccion = v.Traccion,
                    NumPuertas = v.NumPuertas,
                    NumPasajeros = v.NumPasajeros,
                    TipoCombustible = v.TipoCombustible,
                    Cilindraje = v.Cilindraje,
                    PotenciaHp = v.PotenciaHp,
                    TorqueNm = v.TorqueNm,
                    PrecioBase = v.PrecioBase,
                    AnioVersion = v.AnioVersion,
                    CaracteristicasPrincipales = v.CaracteristicasPrincipales,
                    DescripcionCompleta = v.DescripcionCompleta,
                    ModeloNombre = v.Modelo.Nombre,
                    MarcaNombre = v.Modelo.Marca.Nombre
                }).ToListAsync();

            return ApiResponse<List<VersionVehiculoDTO>>.ok(versionVehiculo,
                versionVehiculo.Count == 0 ? "No se encontraron versiones de vehículos." : "Versiones de vehículos obtenidas exitosamente."
                );

        }

        public async Task<ApiResponse<VersionVehiculoDTO>> GetByIdAsync(int versionId)
        {
            var versionVehiculo=await context.Versiones
                .AsNoTracking()
                .Where(v => v.VersionId == versionId && v.Activo)
                 .Select(v => new VersionVehiculoDTO
                 {
                     VersionId = v.VersionId,
                     ModeloId = v.ModeloId,
                     Codigo = v.Codigo,
                     Nombre = v.Nombre,
                     Motor = v.Motor,
                     Transmision = v.Transmision,
                     Traccion = v.Traccion,
                     NumPuertas = v.NumPuertas,
                     NumPasajeros = v.NumPasajeros,
                     TipoCombustible = v.TipoCombustible,
                     Cilindraje = v.Cilindraje,
                     PotenciaHp = v.PotenciaHp,
                     TorqueNm = v.TorqueNm,
                     PrecioBase = v.PrecioBase,
                     AnioVersion = v.AnioVersion,
                     CaracteristicasPrincipales = v.CaracteristicasPrincipales,
                     DescripcionCompleta = v.DescripcionCompleta,
                     ModeloNombre = v.Modelo.Nombre,
                     MarcaNombre = v.Modelo.Marca.Nombre
                 }).FirstOrDefaultAsync();
            if (versionVehiculo == null)
                return ApiResponse<VersionVehiculoDTO>.fail(404, null, "La versión de vehículo no fue encontrada.");

            return ApiResponse<VersionVehiculoDTO>.ok(versionVehiculo, "Versión de vehículo obtenida exitosamente.");

        }

        public async Task<ApiResponse<List<VersionVehiculoDTO>>> GetByModeloAsync(int modeloId)
        {
            var versiones = await context.Versiones
                .Include(v => v.Modelo)
                    .ThenInclude(m => m.Marca)
                .AsNoTracking()
                .Where(v => v.ModeloId == modeloId && v.Activo)
                .Select(v => new VersionVehiculoDTO
                {
                    VersionId = v.VersionId,
                    ModeloId = v.ModeloId,
                    Codigo = v.Codigo,
                    Nombre = v.Nombre,
                    Motor = v.Motor,
                    Transmision = v.Transmision,
                    Traccion = v.Traccion,
                    NumPuertas = v.NumPuertas,
                    NumPasajeros = v.NumPasajeros,
                    TipoCombustible = v.TipoCombustible,
                    Cilindraje = v.Cilindraje,
                    PotenciaHp = v.PotenciaHp,
                    TorqueNm = v.TorqueNm,
                    PrecioBase = v.PrecioBase,
                    AnioVersion = v.AnioVersion,
                    CaracteristicasPrincipales = v.CaracteristicasPrincipales,
                    DescripcionCompleta = $"{v.Nombre} - {v.Motor ?? "N/A"} {v.Transmision ?? ""}".Trim(),
                    ModeloNombre = v.Modelo.Nombre,
                    MarcaNombre = v.Modelo.Marca.Nombre
                }).ToListAsync();

            if (versiones.Count == 0)
                return ApiResponse<List<VersionVehiculoDTO>>.fail(404, null, "No se encontraron versiones.");

            return ApiResponse<List<VersionVehiculoDTO>>.ok(versiones, "Versiones obtenidas exitosamente.");
        }

        public async Task<ApiResponse<VersionVehiculoDTO>> UpdateAsync(UpdateVersionVehiculoDTO dto, string usuarioId)
        {
            // 1. Buscar la versión
            var version = await context.Versiones
                .Include(v => v.Modelo)
                    .ThenInclude(m => m.Marca)
                .FirstOrDefaultAsync(v => v.VersionId == dto.VersionId && v.Activo);

            if (version == null)
                return ApiResponse<VersionVehiculoDTO>.fail(404, null, "Versión no encontrada.");

            // 2. Validar que el modelo existe y está activo
            var modeloExiste = await context.Modelos
                .AnyAsync(m => m.ModeloId == dto.ModeloId && m.Activo);

            if (!modeloExiste)
                return ApiResponse<VersionVehiculoDTO>.fail(400, null, "El modelo no existe o está inactivo.");

            // 3. Validar código duplicado
            var codigoExiste = await context.Versiones
                .AnyAsync(v => v.Codigo == dto.Codigo && v.VersionId != dto.VersionId && v.Activo);

            if (codigoExiste)
                return ApiResponse<VersionVehiculoDTO>.fail(400, null, "Ya existe una versión con ese código.");

            // 4. Actualizar campos
            version.ModeloId = dto.ModeloId;
            version.Codigo = dto.Codigo;
            version.Nombre = dto.Nombre;
            version.Motor = dto.Motor;
            version.Transmision = dto.Transmision;
            version.Traccion = dto.Traccion;
            version.NumPuertas = dto.NumPuertas;
            version.NumPasajeros = dto.NumPasajeros;
            version.TipoCombustible = dto.TipoCombustible;
            version.Cilindraje = dto.Cilindraje;
            version.PotenciaHp = dto.PotenciaHp;
            version.TorqueNm = dto.TorqueNm;
            version.PrecioBase = dto.PrecioBase;
            version.AnioVersion = dto.AnioVersion;
            version.CaracteristicasPrincipales = dto.CaracteristicasPrincipales;
            version.UsuarioModificaId = usuarioId;
            version.FechaModificacion = DateTime.UtcNow;

            // 5. Guardar
            await context.SaveChangesAsync();

            return ApiResponse<VersionVehiculoDTO>.ok(MapToDto(version), "Versión actualizada exitosamente.");
        }

        private VersionVehiculoDTO MapToDto(VersionVehiculo version)
        {
            return new VersionVehiculoDTO
            {
                VersionId = version.VersionId,
                ModeloId = version.ModeloId,
                Codigo = version.Codigo,
                Nombre = version.Nombre,
                Motor = version.Motor,
                Transmision = version.Transmision,
                Traccion = version.Traccion,
                NumPuertas = version.NumPuertas,
                NumPasajeros = version.NumPasajeros,
                TipoCombustible = version.TipoCombustible,
                Cilindraje = version.Cilindraje,
                PotenciaHp = version.PotenciaHp,
                TorqueNm = version.TorqueNm,
                PrecioBase = version.PrecioBase,
                AnioVersion = version.AnioVersion,
                CaracteristicasPrincipales = version.CaracteristicasPrincipales,
                ModeloNombre = version.Modelo?.Nombre,
                MarcaNombre = version.Modelo?.Marca?.Nombre,
                DescripcionCompleta = $"{version.Nombre} - {version.Motor ?? "N/A"} {version.Transmision ?? ""}".Trim()
            };
        }
    }
}
