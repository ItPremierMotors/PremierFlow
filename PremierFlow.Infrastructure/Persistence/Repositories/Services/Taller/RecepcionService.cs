using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller
{
    public class RecepcionService : IRecepcionService
    {
        private readonly PremierFlowDbContext context;

        public RecepcionService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<RecepcionDTO>> GetByIdAsync(int recepcionId)
        {
            var recepcion = await context.Recepciones
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Marca)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Cliente)
                .Include(r => r.Evidencias.Where(e => e.Activo))
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RecepcionId == recepcionId && r.Activo);

            if (recepcion == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Recepción no encontrada.");

            return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción obtenida.");
        }

        public async Task<ApiResponse<RecepcionDTO>> GetByOsIdAsync(int osId)
        {
            var recepcion = await context.Recepciones
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Marca)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Cliente)
                .Include(r => r.Evidencias.Where(e => e.Activo))
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.OsId == osId && r.Activo);

            if (recepcion == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Recepción no encontrada para esta orden de servicio.");

            return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción obtenida.");
        }

        public async Task<ApiResponse<List<RecepcionDTO>>> GetPendientesFirmaAsync(int? sucursalId = null)
        {
            var query = context.Recepciones
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Marca)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Cliente)
                .Include(r => r.Evidencias.Where(e => e.Activo))
                .AsNoTracking()
                .Where(r => r.Activo &&
                           string.IsNullOrWhiteSpace(r.FirmaClienteBase64));

            if (sucursalId.HasValue)
                query = query.Where(r => r.OrdenServicio.SucursalId == sucursalId);

            var recepciones = await query
                .OrderBy(r => r.FechaHoraRecepcion)
                .ToListAsync();

            var dtos = recepciones.Select(MapToDto).ToList();

            return ApiResponse<List<RecepcionDTO>>.ok(dtos, "Recepciones pendientes de firma obtenidas.");
        }

        public async Task<ApiResponse<RecepcionDTO>> CreateAsync(CreateRecepcionDTO dto, string usuarioId)
        {
            // 1. Validar que existe la OS
            var os = await context.OrdenesServicio
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Cliente)
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Orden de servicio no encontrada.");

            if (!os.EstaAbierta)
                return ApiResponse<RecepcionDTO>.fail(400, null, "La orden de servicio no está abierta.");

            // 2. Validar que no exista recepción para esta OS
            var existeRecepcion = await context.Recepciones
                .AnyAsync(r => r.OsId == dto.OsId && r.Activo);

            if (existeRecepcion)
                return ApiResponse<RecepcionDTO>.fail(400, null, "Ya existe una recepción para esta orden de servicio.");

            // 3. Crear recepción
            var recepcion = new Recepcion
            {
                OsId = dto.OsId,
                FechaHoraRecepcion = TimeHelper.Now,
                RecibidoPorId = usuarioId,
                EntregadoPor = dto.EntregadoPor,
                EstadoCarroceria = dto.EstadoCarroceria,
                AccesoriosRecibidos = dto.AccesoriosRecibidos,
                LlantaRepuesto = dto.LlantaRepuesto,
                Gato = dto.Gato,
                Triangulos = dto.Triangulos,
                Extintor = dto.Extintor,
                Herramientas = dto.Herramientas,
                Radio = dto.Radio,
                Tapetes = dto.Tapetes,
                ObservacionesGenerales = dto.ObservacionesGenerales,
                ChecklistCompletado = false,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.Recepciones.Add(recepcion);
            await context.SaveChangesAsync();

            // 4. Cargar navegación para el DTO
            recepcion.OrdenServicio = os;

            return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción creada exitosamente.");
        }

        public async Task<ApiResponse<RecepcionDTO>> UpdateAsync(UpdateRecepcionDTO dto, string usuarioId)
        {
            var recepcion = await context.Recepciones
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Marca)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Cliente)
                .Include(r => r.Evidencias.Where(e => e.Activo))
                .FirstOrDefaultAsync(r => r.RecepcionId == dto.RecepcionId && r.Activo);

            if (recepcion == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Recepción no encontrada.");

            if (recepcion.EstaCompleta)
                return ApiResponse<RecepcionDTO>.fail(400, null, "No se puede modificar una recepción completada.");

            recepcion.EntregadoPor = dto.EntregadoPor;
            recepcion.EstadoCarroceria = dto.EstadoCarroceria;
            recepcion.AccesoriosRecibidos = dto.AccesoriosRecibidos;
            recepcion.LlantaRepuesto = dto.LlantaRepuesto;
            recepcion.Gato = dto.Gato;
            recepcion.Triangulos = dto.Triangulos;
            recepcion.Extintor = dto.Extintor;
            recepcion.Herramientas = dto.Herramientas;
            recepcion.Radio = dto.Radio;
            recepcion.Tapetes = dto.Tapetes;
            recepcion.ObservacionesGenerales = dto.ObservacionesGenerales;
            recepcion.UsuarioModificaId = usuarioId;
            recepcion.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción actualizada exitosamente.");
        }

        public async Task<ApiResponse<bool>> CompletarChecklistAsync(int recepcionId, string usuarioId)
        {
            var recepcion = await context.Recepciones
                .FirstOrDefaultAsync(r => r.RecepcionId == recepcionId && r.Activo);

            if (recepcion == null)
                return ApiResponse<bool>.fail(404, null, "Recepción no encontrada.");

            if (recepcion.ChecklistCompletado)
                return ApiResponse<bool>.fail(400, null, "El checklist ya está completado.");

            recepcion.CompletarChecklist();
            recepcion.UsuarioModificaId = usuarioId;
            recepcion.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Checklist completado exitosamente.");
        }

        public async Task<ApiResponse<bool>> RegistrarFirmaAsync(RegistrarFirmaDTO dto, string usuarioId)
        {
            var recepcion = await context.Recepciones
                .FirstOrDefaultAsync(r => r.RecepcionId == dto.RecepcionId && r.Activo);

            if (recepcion == null)
                return ApiResponse<bool>.fail(404, null, "Recepción no encontrada.");

            if (recepcion.TieneFirmaCliente)
                return ApiResponse<bool>.fail(400, null, "La recepción ya tiene firma del cliente.");

            if (string.IsNullOrWhiteSpace(dto.FirmaClienteBase64))
                return ApiResponse<bool>.fail(400, null, "La firma no puede estar vacía.");

            recepcion.RegistrarFirma(dto.FirmaClienteBase64);
            recepcion.UsuarioModificaId = usuarioId;
            recepcion.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Firma registrada exitosamente.");
        }

        public async Task<ApiResponse<DatosCitaWizardDTO>> GetDatosCitaAsync(int citaId)
        {
            var cita = await context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo).ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo).ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CitaId == citaId && c.Activo);

            if (cita == null)
                return ApiResponse<DatosCitaWizardDTO>.fail(404, null, "Cita no encontrada.");

            if (!cita.PuedeConvertirseEnOs)
                return ApiResponse<DatosCitaWizardDTO>.fail(400, null, "La cita no está en estado válido.");

            // Validar que no exista OS para esta cita
            var existeOs = await context.OrdenesServicio.AnyAsync(o => o.CitaId == citaId && o.Activo);
            if (existeOs)
                return ApiResponse<DatosCitaWizardDTO>.fail(400, null, "Ya existe una orden de servicio para esta cita.");

            var dto = new DatosCitaWizardDTO
            {
                CitaId = cita.CitaId,
                CodigoCita = cita.CodigoCita,
                ClienteNombre = cita.Cliente?.NombreCompleto ?? "",
                ClienteTelefono = cita.Cliente?.Telefono,
                VehiculoDescripcion = cita.Vehiculo?.DescripcionCompleta ?? "",
                VehiculoPlaca = cita.Vehiculo?.Placa,
                VehiculoVin = cita.Vehiculo?.Vin,
                KilometrajeActual = cita.Vehiculo?.KilometrajeActual ?? 0,
                SegmentoVehiculo = (int)(cita.Vehiculo?.Modelo?.Segmento ?? SegmentoVehiculo.Sedan),
                TipoServicioNombre = cita.TipoServicio?.Nombre ?? "",
                MotivoVisita = cita.MotivoVisita
            };

            return ApiResponse<DatosCitaWizardDTO>.ok(dto, "Datos de cita obtenidos.");
        }

        public async Task<ApiResponse<RecepcionDTO>> IniciarDesdeCitaAsync(IniciarRecepcionDTO dto, string usuarioId)
        {
            // 1. Cargar cita con navegaciones
            var cita = await context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo).ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo).ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .FirstOrDefaultAsync(c => c.CitaId == dto.CitaId && c.Activo);

            if (cita == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Cita no encontrada.");

            if (!cita.PuedeConvertirseEnOs)
                return ApiResponse<RecepcionDTO>.fail(400, null, "La cita no está en estado válido para iniciar atención.");

            // 2. Validar no exista OS para esta cita
            var existeOs = await context.OrdenesServicio.AnyAsync(o => o.CitaId == dto.CitaId && o.Activo);
            if (existeOs)
                return ApiResponse<RecepcionDTO>.fail(400, null, "Ya existe una orden de servicio para esta cita.");

            // 3. Validar nivel de combustible
            if (dto.NivelCombustiblePorcentaje < 0 || dto.NivelCombustiblePorcentaje > 100)
                return ApiResponse<RecepcionDTO>.fail(400, null, "El nivel de combustible debe estar entre 0 y 100.");

            // 4. Validar kilometraje
            if (dto.Kilometraje < cita.Vehiculo.KilometrajeActual)
                return ApiResponse<RecepcionDTO>.fail(400, null,
                    $"El kilometraje debe ser mayor o igual al actual ({cita.Vehiculo.KilometrajeActual} km).");

            // 4. Obtener estado ABIERTA
            var estadoAbierta = await context.EstadosOs
                .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.Abierta && e.Activo);

            if (estadoAbierta == null)
                return ApiResponse<RecepcionDTO>.fail(500, null, "Estado ABIERTA no configurado en el sistema.");

            // 5. Generar número de OS
            var numeroOs = await GenerarNumeroOsAsync();

            var strategy = context.Database.CreateExecutionStrategy();
            Recepcion recepcion = null!;
            OrdenServicio os = null!;
            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await context.Database.BeginTransactionAsync();

                    // 6. Crear OrdenServicio
                    os = new OrdenServicio
                    {
                        NumeroOs = numeroOs,
                        CitaId = dto.CitaId,
                        VehiculoId = cita.VehiculoId,
                        ClienteId = cita.ClienteId,
                        FechaApertura = TimeHelper.Now,
                        EstadoId = estadoAbierta.EstadoId,
                        KilometrajeIngreso = dto.Kilometraje,
                        NivelCombustible = dto.NivelCombustiblePorcentaje / 100m,
                        TipoIngreso = TipoIngreso.Cita,
                        EsGarantia = false,
                        ObservacionesApertura = dto.ObservacionesApertura,
                        SucursalId = cita.SucursalId,
                        Activo = true,
                        UsuarioCreaId = usuarioId,
                        FechaCreacion = TimeHelper.Now
                    };
                    context.OrdenesServicio.Add(os);

                    // 6b. Agregar servicio de la cita automaticamente
                    var servicioCita = new OsServicio
                    {
                        TipoServicioId = cita.TipoServicioId,
                        DescripcionTrabajo = cita.TipoServicio.Nombre,
                        Estado = EstadoServicioOS.Pendiente,
                        PrecioUnitario = cita.TipoServicio.PrecioBase,
                        Cantidad = 1,
                        Observaciones = cita.MotivoVisita,
                        Activo = true,
                        UsuarioCreaId = usuarioId,
                        FechaCreacion = TimeHelper.Now
                    };
                    servicioCita.CalcularSubtotal();
                    os.Servicios.Add(servicioCita);

                    // 7. Crear Recepción con todos los campos del wizard
                    recepcion = new Recepcion
                    {
                        OrdenServicio = os,
                        FechaHoraRecepcion = TimeHelper.Now,
                        RecibidoPorId = usuarioId,
                        EntregadoPor = dto.EntregadoPor,
                        EsPropietarioQuienEntrega = dto.EsPropietarioQuienEntrega,
                        RelacionEntregante = dto.RelacionEntregante,
                        TelefonoEntregante = dto.TelefonoEntregante,
                        DanosExteriorJson = dto.DanosExteriorJson,
                        LlantaRepuesto = dto.LlantaRepuesto,
                        Gato = dto.Gato,
                        Triangulos = dto.Triangulos,
                        Extintor = dto.Extintor,
                        Herramientas = dto.Herramientas,
                        Radio = dto.Radio,
                        Tapetes = dto.Tapetes,
                        Antena = dto.Antena,
                        EspejoIzquierdo = dto.EspejoIzquierdo,
                        EspejoDerecho = dto.EspejoDerecho,
                        Limpiaparabrisas = dto.Limpiaparabrisas,
                        PlacaDelantera = dto.PlacaDelantera,
                        PlacaTrasera = dto.PlacaTrasera,
                        TapaCombustible = dto.TapaCombustible,
                        ManualVehiculo = dto.ManualVehiculo,
                        SegundaLlave = dto.SegundaLlave,
                        InspeccionRuedasJson = dto.InspeccionRuedasJson,
                        NivelAceiteOk = dto.NivelAceiteOk,
                        NivelRefrigeranteOk = dto.NivelRefrigeranteOk,
                        NivelLiquidoFrenosOk = dto.NivelLiquidoFrenosOk,
                        BateriaOk = dto.BateriaOk,
                        ObservacionesGenerales = dto.ObservacionesGenerales,
                        FirmaClienteBase64 = dto.FirmaClienteBase64,
                        ChecklistCompletado = true,
                        Activo = true,
                        UsuarioCreaId = usuarioId,
                        FechaCreacion = TimeHelper.Now
                    };
                    context.Recepciones.Add(recepcion);

                    // 8. Actualizar vehículo y cita
                    cita.Vehiculo.ActualizarKilometraje(dto.Kilometraje);
                    cita.IniciarProceso();
                    cita.UsuarioModificaId = usuarioId;
                    cita.FechaModificacion = TimeHelper.Now;

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                });

                // 9. Cargar navegaciones para DTO
                recepcion.OrdenServicio = os;
                os.Vehiculo = cita.Vehiculo;
                os.Cliente = cita.Cliente;
                os.Estado = estadoAbierta;

                return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción y Orden de Servicio creadas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponse<RecepcionDTO>.fail(500, null, $"Error al iniciar recepción: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RecepcionDTO>> IniciarWalkInAsync(IniciarRecepcionWalkInDTO dto, string usuarioId)
        {
            // 1. Cargar cliente
            var cliente = await context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == dto.ClienteId && c.Activo);
            if (cliente == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Cliente no encontrado.");

            // 2. Cargar vehiculo con navegaciones
            var vehiculo = await context.Vehiculos
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .FirstOrDefaultAsync(v => v.VehiculoId == dto.VehiculoId && v.Activo);
            if (vehiculo == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Vehículo no encontrado.");

            // 3. Validar que el vehiculo pertenezca al cliente
            if (vehiculo.ClienteId != dto.ClienteId)
                return ApiResponse<RecepcionDTO>.fail(400, null, "El vehículo no pertenece al cliente seleccionado.");

            // 4. Cargar tipo de servicio
            var tipoServicio = await context.TiposServicio
                .FirstOrDefaultAsync(t => t.TipoServicioId == dto.TipoServicioId && t.Activo);
            if (tipoServicio == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Tipo de servicio no encontrado.");

            // 5. Validar que no tenga OS abierta para este vehiculo
            var osAbierta = await context.OrdenesServicio
                .Include(o => o.Estado)
                .AnyAsync(o => o.VehiculoId == dto.VehiculoId && o.Activo
                    && o.Estado.Codigo != EstadoOs.Estados.Cerrada
                    && o.Estado.Codigo != EstadoOs.Estados.Cancelada);
            if (osAbierta)
                return ApiResponse<RecepcionDTO>.fail(400, null, "El vehículo ya tiene una orden de servicio abierta.");

            // 6. Validar kilometraje
            if (dto.Kilometraje < vehiculo.KilometrajeActual)
                return ApiResponse<RecepcionDTO>.fail(400, null,
                    $"El kilometraje debe ser mayor o igual al actual ({vehiculo.KilometrajeActual} km).");

            // 7. Obtener estado ABIERTA
            var estadoAbierta = await context.EstadosOs
                .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.Abierta && e.Activo);
            if (estadoAbierta == null)
                return ApiResponse<RecepcionDTO>.fail(500, null, "Estado ABIERTA no configurado en el sistema.");

            // 8. Generar número de OS
            var numeroOs = await GenerarNumeroOsAsync();

            var strategy = context.Database.CreateExecutionStrategy();
            Recepcion recepcion = null!;
            OrdenServicio os = null!;
            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await context.Database.BeginTransactionAsync();

                    // 9. Crear OrdenServicio (sin CitaId, TipoIngreso = WalkIn)
                    os = new OrdenServicio
                    {
                        NumeroOs = numeroOs,
                        CitaId = null,
                        VehiculoId = dto.VehiculoId,
                        ClienteId = dto.ClienteId,
                        FechaApertura = TimeHelper.Now,
                        EstadoId = estadoAbierta.EstadoId,
                        KilometrajeIngreso = dto.Kilometraje,
                        NivelCombustible = dto.NivelCombustiblePorcentaje / 100m,
                        TipoIngreso = TipoIngreso.WalkIn,
                        EsGarantia = false,
                        ObservacionesApertura = dto.ObservacionesApertura,
                        SucursalId = dto.SucursalId,
                        Activo = true,
                        UsuarioCreaId = usuarioId,
                        FechaCreacion = TimeHelper.Now
                    };
                    context.OrdenesServicio.Add(os);

                    // 10. Agregar servicio seleccionado
                    var servicioOs = new OsServicio
                    {
                        TipoServicioId = dto.TipoServicioId,
                        DescripcionTrabajo = tipoServicio.Nombre,
                        Estado = EstadoServicioOS.Pendiente,
                        PrecioUnitario = tipoServicio.PrecioBase,
                        Cantidad = 1,
                        Observaciones = dto.MotivoVisita,
                        Activo = true,
                        UsuarioCreaId = usuarioId,
                        FechaCreacion = TimeHelper.Now
                    };
                    servicioOs.CalcularSubtotal();
                    os.Servicios.Add(servicioOs);

                    // 11. Crear Recepción con todos los campos del wizard
                    recepcion = new Recepcion
                    {
                        OrdenServicio = os,
                        FechaHoraRecepcion = TimeHelper.Now,
                        RecibidoPorId = usuarioId,
                        EntregadoPor = dto.EntregadoPor,
                        EsPropietarioQuienEntrega = dto.EsPropietarioQuienEntrega,
                        RelacionEntregante = dto.RelacionEntregante,
                        TelefonoEntregante = dto.TelefonoEntregante,
                        DanosExteriorJson = dto.DanosExteriorJson,
                        LlantaRepuesto = dto.LlantaRepuesto,
                        Gato = dto.Gato,
                        Triangulos = dto.Triangulos,
                        Extintor = dto.Extintor,
                        Herramientas = dto.Herramientas,
                        Radio = dto.Radio,
                        Tapetes = dto.Tapetes,
                        Antena = dto.Antena,
                        EspejoIzquierdo = dto.EspejoIzquierdo,
                        EspejoDerecho = dto.EspejoDerecho,
                        Limpiaparabrisas = dto.Limpiaparabrisas,
                        PlacaDelantera = dto.PlacaDelantera,
                        PlacaTrasera = dto.PlacaTrasera,
                        TapaCombustible = dto.TapaCombustible,
                        ManualVehiculo = dto.ManualVehiculo,
                        SegundaLlave = dto.SegundaLlave,
                        InspeccionRuedasJson = dto.InspeccionRuedasJson,
                        NivelAceiteOk = dto.NivelAceiteOk,
                        NivelRefrigeranteOk = dto.NivelRefrigeranteOk,
                        NivelLiquidoFrenosOk = dto.NivelLiquidoFrenosOk,
                        BateriaOk = dto.BateriaOk,
                        ObservacionesGenerales = dto.ObservacionesGenerales,
                        FirmaClienteBase64 = dto.FirmaClienteBase64,
                        ChecklistCompletado = true,
                        Activo = true,
                        UsuarioCreaId = usuarioId,
                        FechaCreacion = TimeHelper.Now
                    };
                    context.Recepciones.Add(recepcion);

                    // 12. Actualizar kilometraje del vehiculo
                    vehiculo.ActualizarKilometraje(dto.Kilometraje);

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                });

                // 13. Cargar navegaciones para DTO
                recepcion.OrdenServicio = os;
                os.Vehiculo = vehiculo;
                os.Cliente = cliente;
                os.Estado = estadoAbierta;

                return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción Walk-In y Orden de Servicio creadas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponse<RecepcionDTO>.fail(500, null, $"Error al iniciar recepción Walk-In: {ex.Message}");
            }
        }

        #region Helpers

        private async Task<string> GenerarNumeroOsAsync()
        {
            var fecha = TimeHelper.Now;
            var prefijo = $"OS-{fecha:yyyyMMdd}-";

            var ultimaOs = await context.OrdenesServicio
                .Where(o => o.NumeroOs.StartsWith(prefijo))
                .OrderByDescending(o => o.NumeroOs)
                .FirstOrDefaultAsync();

            int siguiente = 1;
            if (ultimaOs != null)
            {
                var ultimoNumero = ultimaOs.NumeroOs.Replace(prefijo, "");
                if (int.TryParse(ultimoNumero, out int numero))
                    siguiente = numero + 1;
            }

            return $"{prefijo}{siguiente:D4}";
        }

        private static RecepcionDTO MapToDto(Recepcion r)
        {
            return new RecepcionDTO
            {
                RecepcionId = r.RecepcionId,
                OsId = r.OsId,
                FechaHoraRecepcion = r.FechaHoraRecepcion,
                RecibidoPorId = r.RecibidoPorId,
                EntregadoPor = r.EntregadoPor,
                EsPropietarioQuienEntrega = r.EsPropietarioQuienEntrega,
                RelacionEntregante = r.RelacionEntregante,
                TelefonoEntregante = r.TelefonoEntregante,
                EstadoCarroceria = r.EstadoCarroceria,
                AccesoriosRecibidos = r.AccesoriosRecibidos,
                DanosExteriorJson = r.DanosExteriorJson,
                LlantaRepuesto = r.LlantaRepuesto,
                Gato = r.Gato,
                Triangulos = r.Triangulos,
                Extintor = r.Extintor,
                Herramientas = r.Herramientas,
                Radio = r.Radio,
                Tapetes = r.Tapetes,
                Antena = r.Antena,
                EspejoIzquierdo = r.EspejoIzquierdo,
                EspejoDerecho = r.EspejoDerecho,
                Limpiaparabrisas = r.Limpiaparabrisas,
                PlacaDelantera = r.PlacaDelantera,
                PlacaTrasera = r.PlacaTrasera,
                TapaCombustible = r.TapaCombustible,
                ManualVehiculo = r.ManualVehiculo,
                SegundaLlave = r.SegundaLlave,
                InspeccionRuedasJson = r.InspeccionRuedasJson,
                NivelAceiteOk = r.NivelAceiteOk,
                NivelRefrigeranteOk = r.NivelRefrigeranteOk,
                NivelLiquidoFrenosOk = r.NivelLiquidoFrenosOk,
                BateriaOk = r.BateriaOk,
                ObservacionesGenerales = r.ObservacionesGenerales,
                FirmaClienteBase64 = r.FirmaClienteBase64,
                ChecklistCompletado = r.ChecklistCompletado,
                NumeroOs = r.OrdenServicio?.NumeroOs ?? "",
                VehiculoDescripcion = r.OrdenServicio?.Vehiculo?.DescripcionCompleta,
                VehiculoPlaca = r.OrdenServicio?.Vehiculo?.Placa,
                ClienteNombre = r.OrdenServicio?.Cliente?.NombreCompleto,
                TieneFirmaCliente = r.TieneFirmaCliente,
                CantidadAccesoriosVerificados = r.CantidadAccesoriosVerificados,
                EstaCompleta = r.EstaCompleta,
                CantidadEvidencias = r.Evidencias?.Count ?? 0
            };
        }

        #endregion
    }
}
