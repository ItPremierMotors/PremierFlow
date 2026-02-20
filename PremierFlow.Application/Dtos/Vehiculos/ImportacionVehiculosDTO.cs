namespace PremierFlow.Application.Dtos.Vehiculos
{
    public class FilaImportacionResultDTO
    {
        public int NumeroFila { get; set; }
        public bool Exitoso { get; set; }
        public string? Vin { get; set; }
        public string? Error { get; set; }
        public int? VehiculoId { get; set; }
    }

    public class ResultadoImportacionDTO
    {
        public int TotalFilas { get; set; }
        public int Exitosos { get; set; }
        public int Fallidos { get; set; }
        public List<FilaImportacionResultDTO> Detalle { get; set; } = new();
    }
}
