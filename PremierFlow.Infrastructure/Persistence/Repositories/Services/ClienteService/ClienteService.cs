using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Cliente;
using PremierFlow.Application.Interfaces.Cliente;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Cliente
{
    public class ClienteService : IClienteService
    {
        private readonly PremierFlowDbContext context;
        public ClienteService(PremierFlowDbContext context)
        {
                this.context = context;
        }
        public async Task<ApiResponse<ClienteDTO>> CreateAsync(CreateClienteDTO dto, string usuarioId)
        {
            // 1. Validar DNI duplicado
            if (!string.IsNullOrEmpty(dto.DNI))
            {
                if (await DNIExiste(dto.DNI))
                    return ApiResponse<ClienteDTO>.fail(400, null, "Ya existe un cliente con ese DNI.");
            }
            //1.2 validar nombre y apelllido que no venga vacio
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return ApiResponse<ClienteDTO>.fail(400, null, "El nombre es obligatorio.");


            // 2. Validar RTN duplicado
            if (!string.IsNullOrEmpty(dto.RTN))
            {
                if (await RTNExiste(dto.RTN))
                    return ApiResponse<ClienteDTO>.fail(400, null, "Ya existe un cliente con ese RTN.");
            }

            // 3. Validar teléfono duplicado
            if (await TelefonoExiste(dto.Telefono))
                return ApiResponse<ClienteDTO>.fail(400, null, "Ya existe un cliente con ese teléfono.");

            // 4. Crear
            var cliente = new PremierFlow.Domain.Entities.Cliente
            {
                TipoCliente = dto.TipoCliente,
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                DNI = dto.DNI,
                RTN = dto.RTN,
                Telefono = dto.Telefono,
                TelefonoSecundario = dto.TelefonoSecundario,
                Email = dto.Email,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                FechaRegistro = TimeHelper.Now,
                NoShowCount = 0,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            return ApiResponse<ClienteDTO>.ok(MapToDto(cliente), "Cliente creado exitosamente.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int clienteId, string usuarioId)
        {
            // 1. Buscar
            var cliente = await context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId && c.Activo);

            if (cliente == null)
                return ApiResponse<bool>.fail(404, null, "Cliente no encontrado.");

            // 2. Validar que no tenga vehículos activos
            var tieneVehiculos = await context.Vehiculos
                .AnyAsync(v => v.ClienteId == clienteId && v.Activo);

            if (tieneVehiculos)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar, el cliente tiene vehículos asociados.");

            // 3. Soft delete
            cliente.Activo = false;
            cliente.UsuarioModificaId = usuarioId;
            cliente.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Cliente eliminado exitosamente.");
        }

        public async Task<ApiResponse<List<ClienteDTO>>> GetAllAsync()
        {
            var clientes = await context.Clientes
                .Include(c => c.Vehiculos.Where(v => v.Activo)) //traera solo clientes con vehiculos activos
                .AsNoTracking()
                .Where(c=>c.Activo)
                .ToListAsync();
            var dtos = clientes.Select(MapToDto).ToList();
            return ApiResponse<List<ClienteDTO>>.ok(dtos, "Clientes obtenidos.");
        }

        public async Task<ApiResponse<ClienteDTO>> GetByDNIAsync(string dni)
        {
            var cliente = await context.Clientes
           .Include(c => c.Vehiculos.Where(v => v.Activo))
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.DNI == dni && c.Activo);

            if (cliente == null)
                return ApiResponse<ClienteDTO>.fail(404, null, "Cliente no encontrado.");

            return ApiResponse<ClienteDTO>.ok(MapToDto(cliente), "Cliente obtenido.");
        }

        public async Task<ApiResponse<ClienteDTO>> GetByIdAsync(int clienteId)
        {
            var cliente=await context.Clientes
                .Include(c=>c.Vehiculos.Where(v=>v.Activo))
                .AsNoTracking()
                .FirstOrDefaultAsync(c=>c.ClienteId==clienteId && c.Activo);
            if (cliente == null)
                return ApiResponse<ClienteDTO>.fail(400, null, "Cliente no encontrado");
            return ApiResponse<ClienteDTO>.ok(MapToDto(cliente), "cliente Obtenido"); 
        }

        public async Task<ApiResponse<ClienteDTO>> GetByRTNAsync(string rtn)
        {
            var cliente = await context.Clientes
             .Include(c => c.Vehiculos.Where(v => v.Activo))
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.RTN == rtn && c.Activo);

            if (cliente == null)
                return ApiResponse<ClienteDTO>.fail(404, null, "Cliente no encontrado.");

            return ApiResponse<ClienteDTO>.ok(MapToDto(cliente), "Cliente obtenido.");
        }

        public async Task<ApiResponse<ClienteDTO>> GetByTelefonoAsync(string telefono)
        {
            var cliente = await context.Clientes
            .Include(c => c.Vehiculos.Where(v => v.Activo))
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Telefono == telefono && c.Activo);

            if (cliente == null)
                return ApiResponse<ClienteDTO>.fail(404, null, "Cliente no encontrado.");

            return ApiResponse<ClienteDTO>.ok(MapToDto(cliente), "Cliente obtenido.");
        }


        public async Task<ApiResponse<bool>> IncrementarNoShowAsync(int clienteId)
        {
            var cliente = await context.Clientes
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId && c.Activo);

            if (cliente == null)
                return ApiResponse<bool>.fail(404, null, "Cliente no encontrado.");

            cliente.IncrementarNoShow();
            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, $"No-show registrado. Total: {cliente.NoShowCount}");
        }

        public async Task<ApiResponse<List<ClienteDTO>>> SearchAsync(string term)
        {
            var termLower= term.ToLower().Trim();
            var clientes= await context.Clientes
                .Include(c=>c.Vehiculos.Where(v=>v.Activo))
                .AsNoTracking()
                .Where(c => c.Activo &&
                       (c.Nombre.ToLower().Contains(termLower) ||
                        (c.Apellidos != null && c.Apellidos.ToLower().Contains(termLower)) ||
                        c.Telefono.Contains(termLower) ||
                        (c.DNI != null && c.DNI.Contains(termLower)) ||
                        (c.RTN != null && c.RTN.Contains(termLower)) ||
                        (c.Email != null && c.Email.ToLower().Contains(termLower))))
            .Take(20)
            .ToListAsync();

            var dtos = clientes.Select(MapToDto).ToList();
            return ApiResponse <List<ClienteDTO>>.ok(dtos, "Busqueda completada");
        }

        public async Task<ApiResponse<ClienteDTO>> UpdateAsync(UpdateClienteDTO dto, string usuarioId)
        {
            // 1. Buscar
            var cliente = await context.Clientes
                .Include(c => c.Vehiculos.Where(v => v.Activo))
                .FirstOrDefaultAsync(c => c.ClienteId == dto.ClienteId && c.Activo);

            if (cliente == null)
                return ApiResponse<ClienteDTO>.fail(404, null, "Cliente no encontrado.");

            // 2. Validar DNI duplicado
            if (!string.IsNullOrEmpty(dto.DNI))
            {
                if (await DNIExiste(dto.DNI, dto.ClienteId))
                    return ApiResponse<ClienteDTO>.fail(400, null, "Ya existe un cliente con ese DNI.");
            }

            // 3. Validar RTN duplicado
            if (!string.IsNullOrEmpty(dto.RTN))
            {
                if (await RTNExiste(dto.RTN, dto.ClienteId))
                    return ApiResponse<ClienteDTO>.fail(400, null, "Ya existe un cliente con ese RTN.");
            }

            // 4. Validar teléfono duplicado
            if (await TelefonoExiste(dto.Telefono, dto.ClienteId))
                return ApiResponse<ClienteDTO>.fail(400, null, "Ya existe un cliente con ese teléfono.");

            // 5. Actualizar
            cliente.TipoCliente = dto.TipoCliente;
            cliente.Nombre = dto.Nombre;
            cliente.Apellidos = dto.Apellidos;
            cliente.DNI = dto.DNI;
            cliente.RTN = dto.RTN;
            cliente.Telefono = dto.Telefono;
            cliente.TelefonoSecundario = dto.TelefonoSecundario;
            cliente.Email = dto.Email;
            cliente.Direccion = dto.Direccion;
            cliente.Ciudad = dto.Ciudad;
            cliente.UsuarioModificaId = usuarioId;
            cliente.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<ClienteDTO>.ok(MapToDto(cliente), "Cliente actualizado exitosamente.");
        }
        #region Helpers

        private async Task<bool> DNIExiste(string dni, int? excludeId = null)
        {
            return await context.Clientes
                .AnyAsync(c => c.DNI == dni &&
                              (excludeId == null || c.ClienteId != excludeId) &&
                              c.Activo);
        }

        private async Task<bool> RTNExiste(string rtn, int? excludeId = null)
        {
            return await context.Clientes
                .AnyAsync(c => c.RTN == rtn &&
                              (excludeId == null || c.ClienteId != excludeId) &&
                              c.Activo);
        }

        private async Task<bool> TelefonoExiste(string telefono, int? excludeId = null)
        {
            return await context.Clientes
                .AnyAsync(c => c.Telefono == telefono &&
                              (excludeId == null || c.ClienteId != excludeId) &&
                              c.Activo);
        }

        private static ClienteDTO MapToDto(PremierFlow.Domain.Entities.Cliente c)
        {
            return new ClienteDTO
            {
                ClienteId = c.ClienteId,
                TipoCliente = c.TipoCliente,
                Nombre = c.Nombre,
                Apellidos = c.Apellidos,
                DNI = c.DNI,
                RTN = c.RTN,
                Telefono = c.Telefono,
                TelefonoSecundario = c.TelefonoSecundario,
                Email = c.Email,
                Direccion = c.Direccion,
                Ciudad = c.Ciudad,
                FechaRegistro = c.FechaRegistro,
                NoShowCount = c.NoShowCount,
                NombreCompleto = c.NombreCompleto,
                TieneAlertaNoShow = c.TieneAlertaNoShow,
                CantidadVehiculos = c.Vehiculos?.Count(v => v.Activo && v.EstaVendido) ?? 0
            };
        }

        #endregion
    }

}
