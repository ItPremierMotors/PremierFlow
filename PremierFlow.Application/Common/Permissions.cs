namespace PremierFlow.Application.Common
{
    public static class Permissions
    {
        // Órdenes de Servicio
        public const string OrdenServicioVer = "OrdenServicio.Ver";
        public const string OrdenServicioCrear = "OrdenServicio.Crear";
        public const string OrdenServicioEditar = "OrdenServicio.Editar";
        public const string OrdenServicioEliminar = "OrdenServicio.Eliminar";

        // Inventario / Vehículos
        public const string InventarioVer = "Inventario.Ver";
        public const string InventarioCrear = "Inventario.Crear";
        public const string InventarioEditar = "Inventario.Editar";
        public const string InventarioEliminar = "Inventario.Eliminar";

        // Catalogo / marca
        public const string CatalogoMarcaVer = "Catalogo.Marca.Ver";
        public const string CatalogoMarcaCrear = "Catalogo.Marca.Crear";
        public const string CatalogoMarcaEditar = "Catalogo.Marca.Editar";
        public const string CatalogoMarcaEliminar = "Catalogo.Marca.Eliminar";
        // Catalogo / modelo
        public const string CatalogoModeloVer = "Catalogo.Modelo.Ver";
        public const string CatalogoModeloCrear = "Catalogo.Modelo.Crear";
        public const string CatalogoModeloEditar = "Catalogo.Modelo.Editar";
        public const string CatalogoModeloEliminar = "Catalogo.Modelo.Eliminar";
        //catalogo /versión
        public const string CatalogoVersionVer = "Catalogo.Version.Ver";
        public const string CatalogoVersionCrear = "Catalogo.Version.Crear";
        public const string CatalogoVersionEditar = "Catalogo.Version.Editar";
        public const string CatalogoVersionEliminar = "Catalogo.Version.Eliminar";
        // Catalogo / servicio
        public const string CatalogoServicioVer = "Catalogo.Servicio.Ver";
        public const string CatalogoServicioCrear = "Catalogo.Servicio.Crear";
        public const string CatalogoServicioEditar = "Catalogo.Servicio.Editar";
        public const string CatalogoServicioEliminar = "Catalogo.Servicio.Eliminar";
        //catalogo /tecnico
        public const string CatalogoTecnicoVer = "Catalogo.Tecnico.Ver";
        public const string CatalogoTecnicoCrear = "Catalogo.Tecnico.Crear";
        public const string CatalogoTecnicoEditar = "Catalogo.Tecnico.Editar";
        public const string CatalogoTecnicoEliminar = "Catalogo.Tecnico.Eliminar";

        // Clientes
        public const string ClienteVer = "Cliente.Ver";
        public const string ClienteCrear = "Cliente.Crear";
        public const string ClienteEditar = "Cliente.Editar";
        public const string ClienteEliminar = "Cliente.Eliminar";

        // Ventas
        public const string VentaVer = "Venta.Ver";
        public const string VentaCrear = "Venta.Crear";
        public const string VentaEditar = "Venta.Editar";
        public const string VentaEliminar = "Venta.Eliminar";

        // Flota
        public const string FlotaVer = "Flota.Ver";
        public const string FlotaVerHistorial = "Flota.VerHistorial";

        // Citas
        public const string CitaVer = "Cita.Ver";
        public const string CitaCrear = "Cita.Crear";
        public const string CitaEditar = "Cita.Editar";
        public const string CitaEliminar = "Cita.Eliminar";

        // Capacidad del Taller
        public const string CapacidadVer = "Capacidad.Ver";
        public const string CapacidadCrear = "Capacidad.Crear";
        public const string CapacidadEditar = "Capacidad.Editar";
        public const string CapacidadEliminar = "Capacidad.Eliminar";

        // Recepción
        public const string RecepcionVer = "Recepcion.Ver";
        public const string RecepcionCrear = "Recepcion.Crear";
        public const string RecepcionEditar = "Recepcion.Editar";

        // Técnicos / Asignaciones
        public const string TecnicoVer = "Tecnico.Ver";
        public const string TecnicoAsignar = "Tecnico.Asignar";

        // Reportes
        public const string ReporteVer = "Reporte.Ver";

        // Dashboard
        public const string DashboardVer = "Dashboard.Ver";

        // Usuarios y Roles (admin)
        public const string UsuarioVer = "Usuario.Ver";
        public const string UsuarioCrear = "Usuario.Crear";
        public const string UsuarioEditar = "Usuario.Editar";
        public const string RolVer = "Rol.Ver";
        public const string RolCrear = "Rol.Crear";
        public const string RolEditar = "Rol.Editar";
        public const string PermisoVer = "Permiso.Ver";
        public const string PermisoEditar = "Permiso.Editar";
          
        // CRM - Leads
        public const string LeadVer = "Lead.Ver";
        public const string LeadCrear = "Lead.Crear";
        public const string LeadEditar = "Lead.Editar";
        public const string LeadEliminar = "Lead.Eliminar";
        public const string LeadAsignar = "Lead.Asignar";

        // CRM - Oportunidades
        public const string OportunidadVer = "Oportunidad.Ver";
        public const string OportunidadCrear = "Oportunidad.Crear";
        public const string OportunidadEditar = "Oportunidad.Editar";
        public const string OportunidadCerrar = "Oportunidad.Cerrar";

        // CRM - Actividades
        public const string ActividadCrmVer = "ActividadCrm.Ver";
        public const string ActividadCrmCrear = "ActividadCrm.Crear";
        public const string ActividadCrmEditar = "ActividadCrm.Editar";

        // CRM - Cotizaciones
        public const string CotizacionVer = "Cotizacion.Ver";
        public const string CotizacionCrear = "Cotizacion.Crear";

        // CRM - Dashboard
        public const string DashboardCrmVer = "DashboardCrm.Ver";

        public static List<string> All => new()
        {
            OrdenServicioVer, OrdenServicioCrear, OrdenServicioEditar, OrdenServicioEliminar,
            InventarioVer, InventarioCrear, InventarioEditar, InventarioEliminar,
            ClienteVer, ClienteCrear, ClienteEditar, ClienteEliminar,
            VentaVer, VentaCrear, VentaEditar, VentaEliminar,
            FlotaVer, FlotaVerHistorial,
            CapacidadVer, CapacidadCrear, CapacidadEditar, CapacidadEliminar,
            CitaVer, CitaCrear, CitaEditar, CitaEliminar,
            RecepcionVer, RecepcionCrear, RecepcionEditar,
            TecnicoVer, TecnicoAsignar,
            ReporteVer,
            DashboardVer,
            UsuarioVer, UsuarioCrear, UsuarioEditar,
            RolVer, RolCrear, RolEditar,
            PermisoVer, PermisoEditar,
            CatalogoMarcaVer, CatalogoMarcaCrear, CatalogoMarcaEditar, CatalogoMarcaEliminar,
            CatalogoModeloVer, CatalogoModeloCrear, CatalogoModeloEditar, CatalogoModeloEliminar,
            CatalogoVersionVer, CatalogoVersionCrear, CatalogoVersionEditar, CatalogoVersionEliminar,
            CatalogoServicioVer, CatalogoServicioCrear, CatalogoServicioEditar, CatalogoServicioEliminar,
            CatalogoTecnicoVer, CatalogoTecnicoCrear, CatalogoTecnicoEditar, CatalogoTecnicoEliminar,
             // CRM
            LeadVer, LeadCrear, LeadEditar, LeadEliminar, LeadAsignar,
            OportunidadVer, OportunidadCrear, OportunidadEditar, OportunidadCerrar,
            ActividadCrmVer, ActividadCrmCrear, ActividadCrmEditar,
            CotizacionVer, CotizacionCrear,
            DashboardCrmVer

        };
    }
}
