namespace PremierFlow.Application.Dtos.Catalogo
{
    public class UbicacionDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public int? SucursalId { get; set; }
        public string? SucursalNombre { get; set; }
        public bool Activa { get; set; }
    }
}
