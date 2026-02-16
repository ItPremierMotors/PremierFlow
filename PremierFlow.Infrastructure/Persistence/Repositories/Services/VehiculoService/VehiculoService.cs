using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Vehiculos;
using PremierFlow.Application.Interfaces.Vehiculo;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using PremierFlow.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.VehiculoService
{
    public class VehiculoService : IVehiculoService
    {
        private readonly PremierFlowDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public VehiculoService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
           
        public async Task<ApiResponse<bool>> ActualizarKilometrajeAsync(int vehiculoId, int nuevoKm, string usuarioId)
        {
            var vehiculo = await context.Vehiculos
          .FirstOrDefaultAsync(v => v.VehiculoId == vehiculoId && v.Activo);

            if (vehiculo == null)
                return ApiResponse<bool>.fail(404, null, "Vehículo no encontrado.");

            if (nuevoKm <= vehiculo.KilometrajeActual)
                return ApiResponse<bool>.fail(400, null, "El nuevo kilometraje debe ser mayor al actual.");

            vehiculo.ActualizarKilometraje(nuevoKm);
            vehiculo.UsuarioModificaId = usuarioId;
            vehiculo.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, $"Kilometraje actualizado a {nuevoKm} km.");
        }

//        ### Transiciones válidas:
//```
//EnTransito → EnAduana → EnBodega → EnExhibicion → Reservado → Vendido → Entregado
//                ↓            ↓           ↓
//              EnBodega EnBodega    EnExhibicion(si cancela reserva)
        public async Task<ApiResponse<bool>> CambiarEstadoAsync(int vehiculoId, EstadoVehiculo nuevoEstado, string usuarioId, int? clienteId = null, string? vendedorId = null)
        {
            var vehiculo = await context.Vehiculos
          .FirstOrDefaultAsync(v => v.VehiculoId == vehiculoId && v.Activo);

            if (vehiculo == null)
                return ApiResponse<bool>.fail(404, null, "Vehículo no encontrado.");

            // Validar transición
            if (!EsTransicionValida(vehiculo.Estado, nuevoEstado))
                return ApiResponse<bool>.fail(400, null,
                    $"No se puede cambiar de {vehiculo.Estado} a {nuevoEstado}.");

            // Validar datos requeridos según el estado destino
            var validacion = ValidarDatosParaTransicion(vehiculo, nuevoEstado);
            if (validacion != null)
                return ApiResponse<bool>.fail(400, null, validacion);

            // Entrando a Reservado: establecer campos de reserva
            if (nuevoEstado == EstadoVehiculo.Reservado)
            {
                vehiculo.ReservadoPorId = usuarioId;
                vehiculo.FechaReserva = DateTime.UtcNow;
                vehiculo.FechaLimiteReserva = DateTime.UtcNow.AddDays(10);
                if (!string.IsNullOrEmpty(vendedorId))
                    vehiculo.VendedorId = vendedorId;
                if (clienteId.HasValue)
                {
                    var clienteExiste = await context.Clientes.AnyAsync(c => c.ClienteId == clienteId && c.Activo);
                    if (!clienteExiste)
                        return ApiResponse<bool>.fail(400, null, "El cliente no existe.");
                    vehiculo.ClienteId = clienteId;
                }
            }

            // Saliendo de Reservado a EnExhibicion (cancelar): limpiar todo
            if (vehiculo.Estado == EstadoVehiculo.Reservado && nuevoEstado == EstadoVehiculo.EnExhibicion)
            {
                vehiculo.ReservadoPorId = null;
                vehiculo.FechaReserva = null;
                vehiculo.FechaLimiteReserva = null;
                vehiculo.ClienteId = null;
                vehiculo.VendedorId = null;
            }

            // Saliendo de Reservado a Vendido: limpiar reserva (cliente se mantiene para venta)
            if (vehiculo.Estado == EstadoVehiculo.Reservado && nuevoEstado == EstadoVehiculo.Vendido)
            {
                vehiculo.ReservadoPorId = null;
                vehiculo.FechaReserva = null;
                vehiculo.FechaLimiteReserva = null;
            }

            vehiculo.Estado = nuevoEstado;
            vehiculo.UsuarioModificaId = usuarioId;
            vehiculo.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, $"Estado cambiado a {nuevoEstado}.");
        }

        public async Task<ApiResponse<VehiculoDTO>> CreateAsync(CreateVehiculoDTO dto, string usuarioId)
        {
            //1. validar vin duplicado
            if(await VinExiste(dto.Vin))
            {
                return ApiResponse<VehiculoDTO>.fail(400, null, "El VIN ya existe.");
            }
            //2. validar placa duplicada si aplica
            if(!string.IsNullOrEmpty(dto.Placa))
            {
                if (await PlacaExiste(dto.Placa))
                {
                    return ApiResponse<VehiculoDTO>.fail(400, null, "La placa ya existe.");
                }
            }
            //2. validar si la marca, modelo, version existen
            var marcaExiste = await context.Marcas.AnyAsync(m => m.MarcaId == dto.MarcaId && m.Activo);
            if (!marcaExiste)
            {
                return ApiResponse<VehiculoDTO>.fail(400, null, "La marca no existe.");
            }
            var modeloExiste = await context.Modelos.AnyAsync(m => m.ModeloId == dto.ModeloId && m.Activo);
            if (!modeloExiste)
            {
                return ApiResponse<VehiculoDTO>.fail(400, null, "El modelo no existe.");
            }
            if(dto.VersionId.HasValue)
            {
                var versionExiste = await context.Versiones.AnyAsync(v => v.VersionId == dto.VersionId && v.Activo);
                if (!versionExiste)
                {
                    return ApiResponse<VehiculoDTO>.fail(400, null, "La versión no existe.");
                }
            }
            //3. validar si cliente existe si aplica
            if(dto.ClienteId.HasValue)
            {
                var clienteExiste = await context.Clientes.AnyAsync(c => c.ClienteId == dto.ClienteId && c.Activo);
                if (!clienteExiste)
                {
                    return ApiResponse<VehiculoDTO>.fail(400, null, "El cliente no existe.");
                }
            }
            //4. crear entidad
            var vehiculo = new Vehiculo
            {
                Vin = dto.Vin,
                Placa = dto.Placa,
                NumeroMotor = dto.NumeroMotor,
                NumeroChasis = dto.NumeroChasis,
                MarcaId = dto.MarcaId,
                ModeloId = dto.ModeloId,
                VersionId = dto.VersionId,
                Anio = dto.Anio,
                Color = dto.Color,
                TipoCombustible = dto.TipoCombustible,
                Transmision = dto.Transmision,
                Estado = dto.Estado,
                UbicacionId = dto.UbicacionId,
                SucursalId = dto.SucursalId,
                Procedencia = dto.Procedencia,
                NumeroImportacion = dto.NumeroImportacion,
                NumeroPoliza = dto.NumeroPoliza,
                FechaIngresoPais = dto.FechaIngresoPais,
                FechaRecepcion = dto.FechaRecepcion,
                CostoImportacion = dto.CostoImportacion,
                ClienteId = dto.ClienteId,
                PrecioLista = dto.PrecioLista,
                KilometrajeActual = dto.KilometrajeActual,
                FechaPrimeraMatricula = dto.FechaPrimeraMatricula,
                GarantiaHasta = dto.GarantiaHasta,
                Observaciones = dto.Observaciones,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };
            context.Vehiculos.Add(vehiculo);
            await context.SaveChangesAsync();
           
            // Cargar navegaciones para el DTO
            await context.Entry(vehiculo).Reference(v => v.Marca).LoadAsync();
            await context.Entry(vehiculo).Reference(v => v.Modelo).LoadAsync();
            if (vehiculo.ClienteId.HasValue)
                await context.Entry(vehiculo).Reference(v => v.Cliente).LoadAsync();
            if (vehiculo.VersionId.HasValue)
                await context.Entry(vehiculo).Reference(v => v.Version).LoadAsync();
            if (vehiculo.SucursalId.HasValue)
                await context.Entry(vehiculo).Reference(v => v.Sucursal).LoadAsync();
            if (vehiculo.UbicacionId.HasValue)
                await context.Entry(vehiculo).Reference(v => v.Ubicacion).LoadAsync();
            return ApiResponse<VehiculoDTO>.ok(MapToDto(vehiculo), "Vehículo creado exitosamente.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int vehiculoId, string usuarioId)
        {
            //1. buscar y validar
            //2. eliminar lógico solo si no tiene ordenes de trabajo abiertas
            var vehiculo = await context.Vehiculos
            .FirstOrDefaultAsync(v => v.VehiculoId == vehiculoId && v.Activo);

            if (vehiculo == null)
                return ApiResponse<bool>.fail(404, null, "Vehículo no encontrado."); 

            // Validar que no tenga OS abiertas
            var tieneOsAbiertas = await context.OrdenesServicio
                .AnyAsync(os => os.VehiculoId == vehiculoId &&
                          os.Estado.Codigo != EstadoOs.Estados.Cerrada &&
                          os.Estado.Codigo != EstadoOs.Estados.Cancelada);

            if (tieneOsAbiertas)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar, el vehículo tiene órdenes de servicio abiertas.");
            // 2. No eliminar si está vendido o entregado
            if (vehiculo.EstaVendido)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar un vehículo vendido o entregado.");

            // 3. No eliminar si tiene citas pendientes
            var tieneCitasPendientes = await context.Citas
                .AnyAsync(c => c.VehiculoId == vehiculoId &&
                              (c.Estado == EstadoCita.Agendada || c.Estado == EstadoCita.Confirmada));

            if (tieneCitasPendientes)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar, tiene citas pendientes.");

            vehiculo.Activo = false;
            vehiculo.UsuarioModificaId = usuarioId;
            vehiculo.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Vehículo eliminado exitosamente.");
        }

        public async Task<ApiResponse<List<VehiculoDTO>>> GetAllAsync()
        {
            var vehiculos = await context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .Include(v => v.Version)
                .Include(v => v.Sucursal)
                .Include(v => v.Ubicacion)
                .AsNoTracking()
                .Where(v => v.Activo)
                .ToListAsync();
            var vehiculoDtos = vehiculos.Select(MapToDto).ToList();
            await ResolverNombresVendedor(vehiculoDtos);
            return ApiResponse<List<VehiculoDTO>>.ok(vehiculoDtos, "Vehículos obtenidos.");

        }

        public async Task<ApiResponse<List<VehiculoDTO>>> GetByClienteAsync(int clienteId)
        {
            var vehiculos = await context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .Include(v => v.Version)
                .Include(v => v.Sucursal)
                .Include(v => v.Ubicacion)
                .AsNoTracking()
                .Where(v => v.ClienteId==clienteId && v.Activo)
                .ToListAsync();
            var dtos = vehiculos.Select(MapToDto).ToList();
            await ResolverNombresVendedor(dtos);

            return ApiResponse<List<VehiculoDTO>>.ok(dtos, "Vehículos obtenidos.");
        }

        public async Task<ApiResponse<List<VehiculoDTO>>> GetByEstadoAsync(EstadoVehiculo estado)
        {
            var vehiculos = await context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .Include(v => v.Version)
                .Include(v => v.Sucursal)
                .Include(v => v.Ubicacion)
                .AsNoTracking()
                .Where(v => v.Estado == estado && v.Activo)
                .ToListAsync();
            var dtos = vehiculos.Select(MapToDto).ToList();
            await ResolverNombresVendedor(dtos);

            return ApiResponse<List<VehiculoDTO>>.ok(dtos, "Vehículos obtenidos.");
        }

        public async Task<ApiResponse<VehiculoDTO>> GetByIdAsync(int vehiculoId)
        {
            var vehiculo = await context.Vehiculos
            .Include(v => v.Cliente)
            .Include(v => v.Marca)
            .Include(v => v.Modelo)
            .Include(v => v.Version)
            .Include(v => v.Sucursal)
            .Include(v => v.Ubicacion)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VehiculoId == vehiculoId && v.Activo);

            if (vehiculo == null)
                return ApiResponse<VehiculoDTO>.fail(404, null, "Vehículo no encontrado.");

            return ApiResponse<VehiculoDTO>.ok(MapToDto(vehiculo), "Vehículo obtenido.");
        }

        public async Task<ApiResponse<VehiculoDTO>> GetByPlacaAsync(string placa)
        {
            var vehiculo = await context.Vehiculos
           .Include(v => v.Cliente)
           .Include(v => v.Marca)
           .Include(v => v.Modelo)
           .Include(v => v.Version)
           .Include(v => v.Sucursal)
           .Include(v => v.Ubicacion)
           .AsNoTracking()
           .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                return ApiResponse<VehiculoDTO>.fail(404, null, "Vehículo no encontrado.");

            return ApiResponse<VehiculoDTO>.ok(MapToDto(vehiculo), "Vehículo obtenido.");
        }

        public async Task<ApiResponse<List<VehiculoDTO>>> GetBySucursalAsync(int sucursalId)
        {
            var vehiculos = await context.Vehiculos
            .Include(v => v.Cliente)
            .Include(v => v.Marca)
            .Include(v => v.Modelo)
            .Include(v => v.Version)
            .Include(v => v.Sucursal)
            .Include(v => v.Ubicacion)
            .AsNoTracking()
            .Where(v => v.SucursalId == sucursalId && v.Activo)
            .ToListAsync();

            var dtos = vehiculos.Select(MapToDto).ToList();
            await ResolverNombresVendedor(dtos);

            return ApiResponse<List<VehiculoDTO>>.ok(dtos, "Vehículos obtenidos.");
        }

        public async Task<ApiResponse<VehiculoDTO>> GetByVinAsync(string vin)
        {
            var vehiculo = await context.Vehiculos
            .Include(v => v.Cliente)
            .Include(v => v.Marca)
            .Include(v => v.Modelo)
            .Include(v => v.Version)
            .Include(v => v.Sucursal)
            .Include(v => v.Ubicacion)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Vin == vin && v.Activo);

            if (vehiculo == null)
                return ApiResponse<VehiculoDTO>.fail(404, null, "Vehículo no encontrado.");

            return ApiResponse<VehiculoDTO>.ok(MapToDto(vehiculo), "Vehículo obtenido.");
        }

        public async Task<ApiResponse<VehiculoDetalleDTO>> GetDetalleByIdAsync(int vehiculoId)
        {
            var vehiculo = await context.Vehiculos
            .Include(v => v.Cliente)
            .Include(v => v.Marca)
            .Include(v => v.Modelo)
            .Include(v => v.Version)
            .Include(v => v.Ubicacion)
            .Include(v => v.Sucursal)
            .Include(v => v.HistorialAts.Where(h => h.Activo))
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VehiculoId == vehiculoId && v.Activo);

            if (vehiculo == null)
                return ApiResponse<VehiculoDetalleDTO>.fail(404, null, "Vehículo no encontrado.");

            var dto = MapToDetalleDto(vehiculo);

            if (!string.IsNullOrEmpty(dto.ReservadoPorId))
            {
                var user = await userManager.FindByIdAsync(dto.ReservadoPorId);
                dto.ReservadoPorNombre = user?.NombreCompleto ?? "Usuario desconocido";
            }

            if (!string.IsNullOrEmpty(dto.VendedorId))
            {
                var vendedor = await userManager.FindByIdAsync(dto.VendedorId);
                dto.VendedorNombre = vendedor?.NombreCompleto ?? "Usuario desconocido";
            }

            return ApiResponse<VehiculoDetalleDTO>.ok(dto, "Vehículo obtenido.");
        }
           
  

        public async Task<ApiResponse<List<VehiculoDTO>>> GetDisponiblesParaVentaAsync()
        {
            var vehiculos = await context.Vehiculos
           .Include(v => v.Marca)
           .Include(v => v.Modelo)
           .Include(v => v.Version)
           .AsNoTracking()
           .Where(v => v.Activo &&
                      (v.Estado == EstadoVehiculo.EnBodega || v.Estado == EstadoVehiculo.EnExhibicion))
           .ToListAsync();

            var dtos = vehiculos.Select(MapToDto).ToList();
            await ResolverNombresVendedor(dtos);

            return ApiResponse<List<VehiculoDTO>>.ok(dtos, "Vehículos disponibles obtenidos.");
        }

        public async Task<ApiResponse<List<VehiculoDTO>>> SearchAsync(string term)
        {
            var termLower = term.ToLower().Trim();

            var vehiculos = await context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .Include(v => v.Version)
                .Include(v => v.Sucursal)
                .Include(v => v.Ubicacion)
                .AsNoTracking()
                .Where(v => v.Activo &&
                           (v.Vin.ToLower().Contains(termLower) ||
                            (v.Placa != null && v.Placa.ToLower().Contains(termLower)) ||
                            v.Marca.Nombre.ToLower().Contains(termLower) ||
                            v.Modelo.Nombre.ToLower().Contains(termLower) ||
                            (v.Cliente != null && v.Cliente.Nombre.ToLower().Contains(termLower))))
                .Take(20)
                .ToListAsync();

            var dtos = vehiculos.Select(MapToDto).ToList();
            await ResolverNombresVendedor(dtos);

            return ApiResponse<List<VehiculoDTO>>.ok(dtos, "Búsqueda completada.");
        }

        public async Task<ApiResponse<VehiculoDTO>> UpdateAsync(UpdateVehiculoDTO dto, string usuarioId)
        {
            //1. buscar
            var vehiculo = await context.Vehiculos
                .Include(v=> v.Cliente)
                .Include(v=> v.Marca)
                .Include(v=> v.Modelo)
                .Include(v=> v.Version)
                .Include(v => v.Sucursal)
                .Include(v => v.Ubicacion)
                .FirstOrDefaultAsync(v => v.VehiculoId == dto.VehiculoId && v.Activo);
            if (vehiculo == null)
            {
                return ApiResponse<VehiculoDTO>.fail(404, null, "Vehículo no encontrado.");
            }
            // 2. Validar VIN duplicado
            if (await VinExiste(dto.Vin, dto.VehiculoId))
                return ApiResponse<VehiculoDTO>.fail(400, null, "Ya existe un vehículo con ese VIN.");

            // 3. Validar Placa duplicada
            if (!string.IsNullOrEmpty(dto.Placa))
            {
                if (await PlacaExiste(dto.Placa, dto.VehiculoId))
                    return ApiResponse<VehiculoDTO>.fail(400, null, "Ya existe un vehículo con esa placa.");
            }

            // 4. Validar marca (solo si cambió)
            if (vehiculo.MarcaId != dto.MarcaId)
            {
                var marcaExiste = await context.Marcas.AnyAsync(m => m.MarcaId == dto.MarcaId && m.Activo);
                if (!marcaExiste)
                    return ApiResponse<VehiculoDTO>.fail(400, null, "La marca no existe o está inactiva.");
            }

            // 5. Validar modelo (solo si cambió marca o modelo)
            if (vehiculo.ModeloId != dto.ModeloId || vehiculo.MarcaId != dto.MarcaId)
            {
                var modeloExiste = await context.Modelos
                    .AnyAsync(m => m.ModeloId == dto.ModeloId && m.MarcaId == dto.MarcaId && m.Activo);
                if (!modeloExiste)
                    return ApiResponse<VehiculoDTO>.fail(400, null, "El modelo no existe o no pertenece a la marca.");
            }

            // 6. Validar versión (solo si cambió)
            if (dto.VersionId.HasValue && dto.VersionId != vehiculo.VersionId)
            {
                var versionExiste = await context.Versiones
                    .AnyAsync(v => v.VersionId == dto.VersionId && v.ModeloId == dto.ModeloId && v.Activo);
                if (!versionExiste)
                    return ApiResponse<VehiculoDTO>.fail(400, null, "La versión no existe o no pertenece al modelo.");
            }

            // 7. Actualizar
            vehiculo.Vin = dto.Vin;
            vehiculo.Placa = dto.Placa;
            vehiculo.NumeroMotor = dto.NumeroMotor;
            vehiculo.NumeroChasis = dto.NumeroChasis;
            vehiculo.MarcaId = dto.MarcaId;
            vehiculo.ModeloId = dto.ModeloId;
            vehiculo.VersionId = dto.VersionId;
            vehiculo.Anio = dto.Anio;
            vehiculo.Color = dto.Color;
            vehiculo.TipoCombustible = dto.TipoCombustible;
            vehiculo.Transmision = dto.Transmision;
            vehiculo.Estado = dto.Estado;
            vehiculo.UbicacionId = dto.UbicacionId;
            vehiculo.SucursalId = dto.SucursalId;
            vehiculo.Procedencia = dto.Procedencia;
            vehiculo.NumeroImportacion = dto.NumeroImportacion;
            vehiculo.NumeroPoliza = dto.NumeroPoliza;
            vehiculo.FechaIngresoPais = dto.FechaIngresoPais;
            vehiculo.FechaRecepcion = dto.FechaRecepcion;
            vehiculo.CostoImportacion = dto.CostoImportacion;
            vehiculo.ClienteId = dto.ClienteId;
            vehiculo.PrecioLista = dto.PrecioLista;
            vehiculo.PrecioVenta = dto.PrecioVenta;
            vehiculo.FechaVenta = dto.FechaVenta;
            vehiculo.FechaEntrega = dto.FechaEntrega;
            vehiculo.VendedorId = dto.VendedorId;
            vehiculo.KilometrajeActual = dto.KilometrajeActual;
            vehiculo.FechaPrimeraMatricula = dto.FechaPrimeraMatricula;
            vehiculo.GarantiaHasta = dto.GarantiaHasta;
            vehiculo.Observaciones = dto.Observaciones;
            vehiculo.UsuarioModificaId = usuarioId;
            vehiculo.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // Recargar navegaciones
            await context.Entry(vehiculo).Reference(v => v.Marca).LoadAsync();
            await context.Entry(vehiculo).Reference(v => v.Modelo).LoadAsync();
            if (vehiculo.SucursalId.HasValue)
                await context.Entry(vehiculo).Reference(v => v.Sucursal).LoadAsync();
            if (vehiculo.UbicacionId.HasValue)
                await context.Entry(vehiculo).Reference(v => v.Ubicacion).LoadAsync();

            return ApiResponse<VehiculoDTO>.ok(MapToDto(vehiculo), "Vehículo actualizado exitosamente.");
        }

        public async Task<ApiResponse<int>> CancelarReservasVencidasAsync(string usuarioId)
        {
            var ahora = DateTime.UtcNow;
            var vencidas = await context.Vehiculos
                .Where(v => v.Activo
                          && v.Estado == EstadoVehiculo.Reservado
                          && v.FechaLimiteReserva.HasValue
                          && v.FechaLimiteReserva.Value < ahora)
                .ToListAsync();

            if (vencidas.Count == 0)
                return ApiResponse<int>.ok(0, "No hay reservas vencidas.");

            foreach (var v in vencidas)
            {
                v.Estado = EstadoVehiculo.EnExhibicion;
                v.ClienteId = null;
                v.ReservadoPorId = null;
                v.FechaReserva = null;
                v.FechaLimiteReserva = null;
                v.UsuarioModificaId = usuarioId;
                v.FechaModificacion = ahora;
            }

            await context.SaveChangesAsync();
            return ApiResponse<int>.ok(vencidas.Count, $"{vencidas.Count} reserva(s) vencida(s) cancelada(s).");
        }

        #region Helpers

        private async Task ResolverNombresVendedor(List<VehiculoDTO> dtos)
        {
            var vendedorIds = dtos
                .Where(d => !string.IsNullOrEmpty(d.VendedorId))
                .Select(d => d.VendedorId!)
                .Distinct()
                .ToList();

            if (vendedorIds.Count == 0) return;

            var vendedores = await userManager.Users
                .AsNoTracking()
                .Where(u => vendedorIds.Contains(u.Id))
                .Select(u => new { u.Id, u.NombreCompleto })
                .ToListAsync();

            var dict = vendedores.ToDictionary(v => v.Id, v => v.NombreCompleto);

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.VendedorId) && dict.TryGetValue(dto.VendedorId, out var nombre))
                    dto.VendedorNombre = nombre;
            }
        }

        private async Task<bool> VinExiste(string vin, int? excludeId = null)
        {
            return await context.Vehiculos
                .AnyAsync(v => v.Vin == vin &&
                              (excludeId == null || v.VehiculoId != excludeId) &&
                              v.Activo);
        }

        private async Task<bool> PlacaExiste(string placa, int? excludeId = null)
        {
            return await context.Vehiculos
                .AnyAsync(v => v.Placa == placa &&
                              (excludeId == null || v.VehiculoId != excludeId) &&
                              v.Activo);
        }

        private static VehiculoDTO MapToDto(Vehiculo v)
        {
            return new VehiculoDTO
            {
                VehiculoId = v.VehiculoId,
                Vin = v.Vin,
                Placa = v.Placa,
                MarcaId = v.MarcaId,
                ModeloId = v.ModeloId,
                VersionId = v.VersionId,
                Anio = v.Anio,
                Color = v.Color,
                TipoCombustible = v.TipoCombustible,
                Transmision = v.Transmision,
                Estado = v.Estado,
                ClienteId = v.ClienteId,
                PrecioLista = v.PrecioLista,
                PrecioVenta = v.PrecioVenta,
                FechaVenta = v.FechaVenta,
                FechaEntrega = v.FechaEntrega,
                VendedorId = v.VendedorId,
                KilometrajeActual = v.KilometrajeActual,
                GarantiaHasta = v.GarantiaHasta,
                ClienteNombre = v.Cliente?.NombreCompleto,
                MarcaNombre = v.Marca?.Nombre ?? "",
                ModeloNombre = v.Modelo?.Nombre ?? "",
                VersionNombre = v.Version?.Nombre,
                Identificador = v.Identificador,
                DescripcionCompleta = v.DescripcionCompleta,
                EnGarantia = v.EnGarantia,
                EstaVendido = v.EstaVendido,
                DisponibleParaVenta = v.DisponibleParaVenta,
                SucursalId = v.SucursalId,
                UbicacionId = v.UbicacionId,
                SucursalNombre = v.Sucursal?.Nombre,
                UbicacionNombre = v.Ubicacion?.Nombre,
                FechaLimiteReserva = v.FechaLimiteReserva
            };
        }

        private static VehiculoDetalleDTO MapToDetalleDto(Vehiculo v)
        {
            return new VehiculoDetalleDTO
            {
                VehiculoId = v.VehiculoId,
                Vin = v.Vin,
                Placa = v.Placa,
                NumeroMotor = v.NumeroMotor,
                NumeroChasis = v.NumeroChasis,
                MarcaId = v.MarcaId,
                ModeloId = v.ModeloId,
                VersionId = v.VersionId,
                Anio = v.Anio,
                Color = v.Color,
                TipoCombustible = v.TipoCombustible,
                Transmision = v.Transmision,
                Estado = v.Estado,
                UbicacionId = v.UbicacionId,
                SucursalId = v.SucursalId,
                Procedencia = v.Procedencia,
                NumeroImportacion = v.NumeroImportacion,
                NumeroPoliza = v.NumeroPoliza,
                FechaIngresoPais = v.FechaIngresoPais,
                FechaRecepcion = v.FechaRecepcion,
                CostoImportacion = v.CostoImportacion,
                ClienteId = v.ClienteId,
                PrecioLista = v.PrecioLista,
                PrecioVenta = v.PrecioVenta,
                FechaVenta = v.FechaVenta,
                FechaEntrega = v.FechaEntrega,
                VendedorId = v.VendedorId,
                KilometrajeActual = v.KilometrajeActual,
                FechaPrimeraMatricula = v.FechaPrimeraMatricula,
                GarantiaHasta = v.GarantiaHasta,
                Observaciones = v.Observaciones,
                FechaRegistro = v.FechaRegistro,
                ClienteNombre = v.Cliente?.NombreCompleto,
                MarcaNombre = v.Marca?.Nombre ?? "",
                ModeloNombre = v.Modelo?.Nombre ?? "",
                VersionNombre = v.Version?.Nombre,
                UbicacionNombre = v.Ubicacion?.Nombre,
                SucursalNombre = v.Sucursal?.Nombre,
                Identificador = v.Identificador,
                DescripcionCompleta = v.DescripcionCompleta,
                EnGarantia = v.EnGarantia,
                EstaVendido = v.EstaVendido,
                DisponibleParaVenta = v.DisponibleParaVenta,
                CantidadServicios = v.HistorialAts?.Count ?? 0,
                ReservadoPorId = v.ReservadoPorId,
                FechaReserva = v.FechaReserva,
                FechaLimiteReserva = v.FechaLimiteReserva
            };
        }
        private static string? ValidarDatosParaTransicion(Vehiculo vehiculo, EstadoVehiculo nuevoEstado)
        {
            var faltantes = new List<string>();

            // Saliendo de EnAduana: validar datos de importación completos
            if (vehiculo.Estado == EstadoVehiculo.EnAduana)
            {
                if (string.IsNullOrEmpty(vehiculo.NumeroImportacion)) faltantes.Add("Número de importación");
                if (string.IsNullOrEmpty(vehiculo.NumeroPoliza)) faltantes.Add("Número de póliza");
                if (!vehiculo.CostoImportacion.HasValue || vehiculo.CostoImportacion <= 0) faltantes.Add("Costo de importación");
                if (!vehiculo.FechaIngresoPais.HasValue) faltantes.Add("Fecha de ingreso al país");
                if (!vehiculo.FechaRecepcion.HasValue) faltantes.Add("Fecha de recepción");
                if (!vehiculo.PrecioLista.HasValue || vehiculo.PrecioLista <= 0) faltantes.Add("Precio de lista");
                if (!vehiculo.SucursalId.HasValue) faltantes.Add("Sucursal");
            }

            switch (nuevoEstado)
            {
                case EstadoVehiculo.EnBodega:
                    if (vehiculo.Estado != EstadoVehiculo.EnAduana)
                    {
                        if (!vehiculo.SucursalId.HasValue) faltantes.Add("Sucursal");
                        if (!vehiculo.FechaRecepcion.HasValue) faltantes.Add("Fecha de recepción");
                        if (!vehiculo.PrecioLista.HasValue || vehiculo.PrecioLista <= 0) faltantes.Add("Precio de lista");
                    }
                    break;

                case EstadoVehiculo.EnExhibicion:
                    if (!vehiculo.SucursalId.HasValue) faltantes.Add("Sucursal");
                    if (!vehiculo.PrecioLista.HasValue || vehiculo.PrecioLista <= 0) faltantes.Add("Precio de lista");
                    if (!vehiculo.GarantiaHasta.HasValue) faltantes.Add("Garantía hasta");
                    if (string.IsNullOrEmpty(vehiculo.Color)) faltantes.Add("Color");
                    if (!vehiculo.Transmision.HasValue) faltantes.Add("Transmisión");
                    break;

                case EstadoVehiculo.Vendido:
                    if (!vehiculo.ClienteId.HasValue) faltantes.Add("Cliente");
                    if (!vehiculo.PrecioVenta.HasValue || vehiculo.PrecioVenta <= 0) faltantes.Add("Precio de venta");
                    if (!vehiculo.FechaVenta.HasValue) faltantes.Add("Fecha de venta");
                    if (string.IsNullOrEmpty(vehiculo.VendedorId)) faltantes.Add("Vendedor");
                    break;

                case EstadoVehiculo.Entregado:
                    if (!vehiculo.FechaEntrega.HasValue) faltantes.Add("Fecha de entrega");
                    if (string.IsNullOrEmpty(vehiculo.Placa)) faltantes.Add("Placa");
                    if (vehiculo.KilometrajeActual <= 0) faltantes.Add("Kilometraje actual");
                    if (!vehiculo.FechaPrimeraMatricula.HasValue) faltantes.Add("Fecha de primera matrícula");
                    break;
            }

            if (faltantes.Count == 0)
                return null;

            return $"No se puede cambiar a {nuevoEstado}. Faltan datos requeridos: {string.Join(", ", faltantes)}.";
        }

        private static bool EsTransicionValida(EstadoVehiculo actual, EstadoVehiculo nuevo)
        {
            var transicionesValidas = new Dictionary<EstadoVehiculo, EstadoVehiculo[]>
            {
                { EstadoVehiculo.EnTransito, new[] { EstadoVehiculo.EnAduana, EstadoVehiculo.EnBodega } },
                { EstadoVehiculo.EnAduana, new[] { EstadoVehiculo.EnBodega } },
                { EstadoVehiculo.EnBodega, new[] { EstadoVehiculo.EnExhibicion, EstadoVehiculo.Reservado } },
                { EstadoVehiculo.EnExhibicion, new[] { EstadoVehiculo.Reservado, EstadoVehiculo.EnBodega } },
                { EstadoVehiculo.Reservado, new[] { EstadoVehiculo.Vendido, EstadoVehiculo.EnExhibicion } },
                { EstadoVehiculo.Vendido, new[] { EstadoVehiculo.Entregado } },
                { EstadoVehiculo.Entregado, Array.Empty<EstadoVehiculo>() }  // Estado final
            };

            if (!transicionesValidas.TryGetValue(actual, out var permitidos))
                return false;

            return permitidos.Contains(nuevo);
        }

        #endregion
    }


}
