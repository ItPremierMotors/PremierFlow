using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Interfaces.Dashboard;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardCrmController : ControllerBase
    {
        private readonly IDashboardCrmService service;
        public DashboardCrmController(IDashboardCrmService service)
        {
            this.service = service;
        }
        /// <summary>
        /// KPIs de leads: nuevos, tasa de conversión, tiempo de respuesta, sin contactar, por origen.
        /// </summary>
        [HttpGet("leads")]
        public async Task<IActionResult> GetLeads(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var result = await service.GetLeadsKpisAsync(desde, hasta, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        /// <summary>
        /// KPIs de pipeline: oportunidades por etapa, valor del pipeline, win rate, valor promedio por oportunidad ganada.
        /// </summary>
        [HttpGet("Pipeline")]
        public async Task<IActionResult> GetPipeline(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var result = await service.GetPipelineKpisAsync(desde, hasta, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        /// <summary>
        /// KPIs de equipo: leads asignados, oportunidades ganadas, valor vendido y carga actual por vendedor.
        /// </summary>
        [HttpGet("Equipo")]
        public async Task<IActionResult> GetEquipo(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var result = await service.GetEquipoKpisAsync(desde, hasta, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        /// <summary>
        /// KPIs de retención: leads descartados, oportunidades perdidas/canceladas, motivos de descarte y pérdida.
        /// </summary>
        [HttpGet("Retencion")]
        public async Task<IActionResult> GetRetencion(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var result = await service.GetRetencionKpisAsync(desde, hasta, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        /// <summary>
        /// KPIs de actividad: total, completadas, vencidas, tasa de cumplimiento, por tipo y por vendedor.
        /// </summary>
        [HttpGet("Actividad")]
        public async Task<IActionResult> GetActividad(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var result = await service.GetActividadKpisAsync(desde, hasta, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        /// <summary>
        /// KPIs por origen/canal: leads por origen, tasa de conversión y win rate por canal de captación.
        /// </summary>
        [HttpGet("Origen")]
        public async Task<IActionResult> GetOrigen(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var result = await service.GetOrigenKpisAsync(desde, hasta, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        // <summary>
        /// KPIs de tiempo: tiempo promedio de primera respuesta (horas) y ciclo de venta promedio (días).
        /// </summary>
        [HttpGet("Tiempo")]
        public async Task<IActionResult> GetTiempo(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var result = await service.GetTiempoKpisAsync(desde, hasta, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
    }

}