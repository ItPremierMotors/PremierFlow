using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.BlobAzure;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;


namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller
{
    public class EvidenciaService : IEvidenciaService
    {
        private readonly PremierFlowDbContext context;
        private readonly IBlobStoragesServices _blobStorageService;
    

        public EvidenciaService(PremierFlowDbContext context, IBlobStoragesServices blobStorageService)
        {
            this.context = context;
            this._blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Determina la subcarpeta según el tipo de evidencia.
        /// Estructura: {NumeroOs}/entrada/ o {NumeroOs}/salida/
        /// </summary>
        private static string ObtenerSubcarpeta(TipoEvidencia tipo, int? recepcionId)
        {
            // Fotos de salida (FotoSalidaFrontal, FotoSalidaTrasera, etc.)
            if (tipo.ToString().StartsWith("FotoSalida"))
                return "salida";

            // Fotos con recepción asociada = entrada
            if (recepcionId.HasValue)
                return "entrada";

            // Otros (daños adicionales, fotos durante servicio, etc.)
            return "otros";
        }

        public async Task<ApiResponse<EvidenciaDTO>> GetByIdAsync(int evidenciaId)
        {
            var evidencia = await context.Evidencias
                .Include(e => e.OrdenServicio)
                .Include(e => e.Recepcion)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EvidenciaId == evidenciaId && e.Activo);

            if (evidencia == null)
                return ApiResponse<EvidenciaDTO>.fail(404, null, "Evidencia no encontrada.");

            return ApiResponse<EvidenciaDTO>.ok(MapToDto(evidencia), "Evidencia obtenida.");
        }

        public async Task<ApiResponse<List<EvidenciaDTO>>> GetByOsIdAsync(int osId)
        {
            var evidencias = await context.Evidencias
                .Include(e => e.OrdenServicio)
                .Include(e => e.Recepcion)
                .AsNoTracking()
                .Where(e => e.OsId == osId && e.Activo)
                .OrderBy(e => e.FechaCaptura)
                .ToListAsync();

            var dtos = evidencias.Select(MapToDto).ToList();

            return ApiResponse<List<EvidenciaDTO>>.ok(dtos, "Evidencias obtenidas.");
        }

        public async Task<ApiResponse<List<EvidenciaDTO>>> GetByRecepcionIdAsync(int recepcionId)
        {
            var evidencias = await context.Evidencias
                .Include(e => e.OrdenServicio)
                .Include(e => e.Recepcion)
                .AsNoTracking()
                .Where(e => e.RecepcionId == recepcionId && e.Activo)
                .OrderBy(e => e.FechaCaptura)
                .ToListAsync();

            var dtos = evidencias.Select(MapToDto).ToList();

            return ApiResponse<List<EvidenciaDTO>>.ok(dtos, "Evidencias de recepción obtenidas.");
        }

        public async Task<ApiResponse<List<EvidenciaDTO>>> GetByTipoAsync(int osId, TipoEvidencia tipo)
        {
            var evidencias = await context.Evidencias
                .Include(e => e.OrdenServicio)
                .Include(e => e.Recepcion)
                .AsNoTracking()
                .Where(e => e.OsId == osId && e.TipoEvidencia == tipo && e.Activo)
                .OrderBy(e => e.FechaCaptura)
                .ToListAsync();

            var dtos = evidencias.Select(MapToDto).ToList();

            return ApiResponse<List<EvidenciaDTO>>.ok(dtos, "Evidencias obtenidas.");
        }

        public async Task<ApiResponse<List<EvidenciaDTO>>> GetFotosDanoAsync(int osId)
        {
            var evidencias = await context.Evidencias
                .Include(e => e.OrdenServicio)
                .Include(e => e.Recepcion)
                .AsNoTracking()
                .Where(e => e.OsId == osId && e.TipoEvidencia == TipoEvidencia.FotoDano && e.Activo)
                .OrderBy(e => e.FechaCaptura)
                .ToListAsync();

            var dtos = evidencias.Select(MapToDto).ToList();

            return ApiResponse<List<EvidenciaDTO>>.ok(dtos, "Fotos de daño obtenidas.");
        }

        public async Task<ApiResponse<EvidenciaDTO>> AgregarAsync(CreateEvidenciaDTO dto, string usuarioId)
        {
            // 1. Validar que existe la OS
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<EvidenciaDTO>.fail(404, null, "Orden de servicio no encontrada.");

            // 2. Validar recepción si se proporciona
            if (dto.RecepcionId.HasValue)
            {
                var recepcionExiste = await context.Recepciones
                    .AnyAsync(r => r.RecepcionId == dto.RecepcionId && r.OsId == dto.OsId && r.Activo);

                if (!recepcionExiste)
                    return ApiResponse<EvidenciaDTO>.fail(404, null, "Recepción no encontrada para esta OS.");
            }

            // 3. Validar URL
            if (string.IsNullOrWhiteSpace(dto.UrlArchivo))
                return ApiResponse<EvidenciaDTO>.fail(400, null, "La URL del archivo es requerida.");

            // 4. Crear evidencia
            var evidencia = new Evidencia
            {
                OsId = dto.OsId,
                RecepcionId = dto.RecepcionId,
                TipoEvidencia = dto.TipoEvidencia,
                UrlArchivo = dto.UrlArchivo,
                Descripcion = dto.Descripcion,
                FechaCaptura = TimeHelper.Now,
                UsuarioRegistroId = usuarioId,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.Evidencias.Add(evidencia);
            await context.SaveChangesAsync();

            // Cargar navegación
            evidencia.OrdenServicio = os;

            return ApiResponse<EvidenciaDTO>.ok(MapToDto(evidencia), "Evidencia agregada exitosamente.");
        }

        public async Task<ApiResponse<EvidenciaDTO>> AgregarBase64Async(CreateEvidenciaBase64DTO dto, string usuarioId)
        {
            // 1. Validar que existe la OS
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<EvidenciaDTO>.fail(404, null, "Orden de servicio no encontrada.");

            // 2. Validar recepción si se proporciona
            if (dto.RecepcionId.HasValue)
            {
                var recepcionExiste = await context.Recepciones
                    .AnyAsync(r => r.RecepcionId == dto.RecepcionId && r.OsId == dto.OsId && r.Activo);

                if (!recepcionExiste)
                    return ApiResponse<EvidenciaDTO>.fail(404, null, "Recepción no encontrada para esta OS.");
            }

            // 3. Validar y procesar Base64
            if (string.IsNullOrWhiteSpace(dto.Base64Data))
                return ApiResponse<EvidenciaDTO>.fail(400, null, "Los datos Base64 son requeridos.");

            // Remover prefijo data:image si existe
            var base64Data = dto.Base64Data;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Split(',')[1];
            }

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                return ApiResponse<EvidenciaDTO>.fail(400, null, "Formato Base64 inválido.");
            }

            // 4. Construir ruta organizada: {NumeroOs}/{subcarpeta}/{tipo}_{timestamp}_{guid}.ext
            var extension = Path.GetExtension(dto.NombreArchivo);
            if (string.IsNullOrEmpty(extension))
                extension = ".jpg";

            var subcarpeta = ObtenerSubcarpeta(dto.TipoEvidencia, dto.RecepcionId);
            var carpetaOs = os.NumeroOs; // Ej: OS-20260223-0001
            var nombreArchivo = $"{dto.TipoEvidencia}_{TimeHelper.Now:HHmmss}_{Guid.NewGuid().ToString("N")[..6]}{extension}";

            // 5. Subir archivo a Azure Blob Storage
            var blobName = $"{carpetaOs}/{subcarpeta}/{nombreArchivo}";
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            string urlArchivo;
            try
            {
                urlArchivo = await _blobStorageService.UploadAsync(fileBytes, blobName, contentType);
            }
            catch (Exception ex)
            {
                return ApiResponse<EvidenciaDTO>.fail(500, null, $"Error al guardar el archivo: {ex.Message}");
            }

            // 7. Crear evidencia
            var evidencia = new Evidencia
            {
                OsId = dto.OsId,
                RecepcionId = dto.RecepcionId,
                TipoEvidencia = dto.TipoEvidencia,
                UrlArchivo = urlArchivo,
                Descripcion = dto.Descripcion,
                FechaCaptura = TimeHelper.Now,
                UsuarioRegistroId = usuarioId,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.Evidencias.Add(evidencia);
            await context.SaveChangesAsync();

            // Cargar navegación
            evidencia.OrdenServicio = os;

            return ApiResponse<EvidenciaDTO>.ok(MapToDto(evidencia), "Evidencia agregada exitosamente.");
        }

        public async Task<ApiResponse<bool>> EliminarAsync(int evidenciaId, string usuarioId)
        {
            var evidencia = await context.Evidencias
                .FirstOrDefaultAsync(e => e.EvidenciaId == evidenciaId && e.Activo);

            if (evidencia == null)
                return ApiResponse<bool>.fail(404, null, "Evidencia no encontrada.");

            // Soft delete (no eliminar archivo físico por auditoría)
            evidencia.Activo = false;
            evidencia.UsuarioModificaId = usuarioId;
            evidencia.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Evidencia eliminada exitosamente.");
        }

        #region Helper

        private static EvidenciaDTO MapToDto(Evidencia e)
        {
            return new EvidenciaDTO
            {
                EvidenciaId = e.EvidenciaId,
                OsId = e.OsId,
                RecepcionId = e.RecepcionId,
                TipoEvidencia = e.TipoEvidencia,
                UrlArchivo = e.UrlArchivo,
                Descripcion = e.Descripcion,
                FechaCaptura = e.FechaCaptura,
                UsuarioRegistroId = e.UsuarioRegistroId,
                NumeroOs = e.OrdenServicio?.NumeroOs ?? "",
                EsFotoRecepcion = e.EsFotoRecepcion,
                EsFotoDano = e.EsFotoDano,
                NombreArchivo = e.NombreArchivo,
                Extension = e.Extension,
                EsImagen = e.EsImagen
            };
        }

        #endregion
    }
}
//## Flujo de uso:
//```
//1.Cliente llega → Se crea OS y Recepción
//   ↓
//2. Recepcionista toma fotos con tablet
//   ↓
//3. Cada foto se envía como Base64 (AgregarBase64Async)
//   - Foto frontal
//   - Foto trasera
//   - Foto lateral izq/der
//   - Fotos de daños existentes
//   ↓
//4. Archivos se guardan en /wwwroot/uploads/evidencias/
//   ↓
//5. URL se guarda en BD: / uploads / evidencias / 123_FotoFrontal_20260128....jpg