using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Taller
{
    public class EvidenciaDTO
    {
        public int EvidenciaId { get; set; }
        public int OsId { get; set; }
        public int? RecepcionId { get; set; }
        public TipoEvidencia TipoEvidencia { get; set; }
        public string UrlArchivo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public DateTime FechaCaptura { get; set; }
        public string? UsuarioRegistroId { get; set; }

        // Extras para UI
        public string NumeroOs { get; set; } = null!;
        public string TipoEvidenciaNombre => TipoEvidencia.ToString();
        public bool EsFotoRecepcion { get; set; }
        public bool EsFotoDano { get; set; }
        public string NombreArchivo { get; set; } = null!;
        public string? Extension { get; set; }
        public bool EsImagen { get; set; }
    }
    public class CreateEvidenciaDTO
    {
        public int OsId { get; set; }
        public int? RecepcionId { get; set; }
        public TipoEvidencia TipoEvidencia { get; set; }
        public string UrlArchivo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
    public class CreateEvidenciaBase64DTO
    {
        public int OsId { get; set; }
        public int? RecepcionId { get; set; }
        public TipoEvidencia TipoEvidencia { get; set; }
        public string Base64Data { get; set; } = null!;
        public string NombreArchivo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
