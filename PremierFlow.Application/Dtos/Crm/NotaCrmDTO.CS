namespace PremierFlow.Application.Dtos.Crm
{
    public class NotaCrmDTO
    {
        public int NotaCrmId { get; set; }
        public string Contenido { get; set; } = null!;
        public bool EsPrivada { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Datos relacionados
        public int? LeadId { get; set; }
        public int? OportunidadId { get; set; }
        public string AutorId { get; set; } = null!;
        public string AutorNombre { get; set; } = null!;
    }

    public class CreateNotaCrmDTO
    {
        public int? LeadId { get; set; }
        public int? OportunidadId { get; set; }
        public string Contenido { get; set; } = null!;
        public bool EsPrivada { get; set; }
    }
}
