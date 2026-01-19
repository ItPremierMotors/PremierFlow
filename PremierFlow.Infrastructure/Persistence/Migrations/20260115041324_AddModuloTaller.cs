using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloTaller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Sucursales_SucursalID",
                table: "Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSucursales_Sucursales_SucursalID",
                table: "UsuarioSucursales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ubicaciones",
                table: "Ubicaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sucursales",
                table: "Sucursales");

            migrationBuilder.RenameTable(
                name: "Ubicaciones",
                newName: "ubicaciones");

            migrationBuilder.RenameTable(
                name: "Sucursales",
                newName: "sucursales");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "ubicaciones",
                newName: "tipo");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "ubicaciones",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Activa",
                table: "ubicaciones",
                newName: "activa");

            migrationBuilder.RenameColumn(
                name: "SucursalID",
                table: "ubicaciones",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_Ubicaciones_SucursalID",
                table: "ubicaciones",
                newName: "IX_ubicaciones_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "sucursales",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "sucursales",
                newName: "direccion");

            migrationBuilder.RenameColumn(
                name: "Codigo",
                table: "sucursales",
                newName: "codigo");

            migrationBuilder.RenameColumn(
                name: "Ciudad",
                table: "sucursales",
                newName: "ciudad");

            migrationBuilder.RenameColumn(
                name: "Activa",
                table: "sucursales",
                newName: "activa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "sucursales",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "tipo",
                table: "ubicaciones",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "ubicaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "activa",
                table: "ubicaciones",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "sucursales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "direccion",
                table: "sucursales",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "codigo",
                table: "sucursales",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ciudad",
                table: "sucursales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "activa",
                table: "sucursales",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ubicaciones",
                table: "ubicaciones",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sucursales",
                table: "sucursales",
                column: "id");

            migrationBuilder.CreateTable(
                name: "capacidad_taller",
                columns: table => new
                {
                    capacidad_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    turno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    tecnicos_disponibles = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    bahias_disponibles = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    minutos_disponibles = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    minutos_reservados = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    minutos_utilizados = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    permite_agendamiento = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    observaciones = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    sucursal_id = table.Column<int>(type: "int", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_capacidad_taller", x => x.capacidad_id);
                    table.ForeignKey(
                        name: "FK_capacidad_taller_sucursales_sucursal_id",
                        column: x => x.sucursal_id,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    cliente_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo_cliente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    documento_identidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    direccion = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    no_show_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    usuario_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.cliente_id);
                });

            migrationBuilder.CreateTable(
                name: "estados_os",
                columns: table => new
                {
                    estado_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    orden_secuencial = table.Column<int>(type: "int", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estados_os", x => x.estado_id);
                });

            migrationBuilder.CreateTable(
                name: "marcas",
                columns: table => new
                {
                    marca_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    pais_origen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    es_marca_propia = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    logo_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marcas", x => x.marca_id);
                });

            migrationBuilder.CreateTable(
                name: "tecnicos",
                columns: table => new
                {
                    tecnico_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bahia_asignada = table.Column<int>(type: "int", nullable: true),
                    usuario_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    sucursal_id = table.Column<int>(type: "int", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tecnicos", x => x.tecnico_id);
                    table.ForeignKey(
                        name: "FK_tecnicos_sucursales_sucursal_id",
                        column: x => x.sucursal_id,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tipos_servicio",
                columns: table => new
                {
                    tipo_servicio_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    duracion_estimada_min = table.Column<int>(type: "int", nullable: false),
                    clasificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    permite_walk_in = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    requiere_cita = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    precio_base = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    stock_requerido = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_servicio", x => x.tipo_servicio_id);
                });

            migrationBuilder.CreateTable(
                name: "bloques_horario",
                columns: table => new
                {
                    bloque_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    capacidad_id = table.Column<int>(type: "int", nullable: false),
                    hora_inicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    hora_fin = table.Column<TimeSpan>(type: "time", nullable: false),
                    capacidad_maxima_vehiculos = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    vehiculos_agendados = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    tipo_bloque = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bloques_horario", x => x.bloque_id);
                    table.ForeignKey(
                        name: "FK_bloques_horario_capacidad_taller_capacidad_id",
                        column: x => x.capacidad_id,
                        principalTable: "capacidad_taller",
                        principalColumn: "capacidad_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modelos",
                columns: table => new
                {
                    modelo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    marca_id = table.Column<int>(type: "int", nullable: false),
                    codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    segmento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    anio_inicio = table.Column<int>(type: "int", nullable: true),
                    anio_fin = table.Column<int>(type: "int", nullable: true),
                    descripcion = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    imagen_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modelos", x => x.modelo_id);
                    table.ForeignKey(
                        name: "FK_modelos_marcas_marca_id",
                        column: x => x.marca_id,
                        principalTable: "marcas",
                        principalColumn: "marca_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "versiones",
                columns: table => new
                {
                    version_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    modelo_id = table.Column<int>(type: "int", nullable: false),
                    codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    motor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    transmision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    traccion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    num_puertas = table.Column<int>(type: "int", nullable: true),
                    num_pasajeros = table.Column<int>(type: "int", nullable: true),
                    tipo_combustible = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    cilindraje = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    potencia_hp = table.Column<int>(type: "int", nullable: true),
                    torque_nm = table.Column<int>(type: "int", nullable: true),
                    precio_base = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    anio_version = table.Column<int>(type: "int", nullable: true),
                    caracteristicas_principales = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_versiones", x => x.version_id);
                    table.ForeignKey(
                        name: "FK_versiones_modelos_modelo_id",
                        column: x => x.modelo_id,
                        principalTable: "modelos",
                        principalColumn: "modelo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vehiculos",
                columns: table => new
                {
                    vehiculo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vin = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    placa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    numero_motor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    numero_chasis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    marca_id = table.Column<int>(type: "int", nullable: false),
                    modelo_id = table.Column<int>(type: "int", nullable: false),
                    version_id = table.Column<int>(type: "int", nullable: true),
                    anio = table.Column<int>(type: "int", nullable: false),
                    color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ubicacion_id = table.Column<int>(type: "int", nullable: true),
                    sucursal_id = table.Column<int>(type: "int", nullable: true),
                    procedencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    numero_importacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    numero_poliza = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    fecha_ingreso_pais = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_recepcion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    costo_importacion = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    cliente_id = table.Column<int>(type: "int", nullable: true),
                    precio_lista = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    precio_venta = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    fecha_venta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_entrega = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vendedor_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    kilometraje_actual = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    fecha_primera_matricula = table.Column<DateTime>(type: "datetime2", nullable: true),
                    garantia_hasta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehiculos", x => x.vehiculo_id);
                    table.ForeignKey(
                        name: "FK_vehiculos_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "cliente_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehiculos_marcas_marca_id",
                        column: x => x.marca_id,
                        principalTable: "marcas",
                        principalColumn: "marca_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehiculos_modelos_modelo_id",
                        column: x => x.modelo_id,
                        principalTable: "modelos",
                        principalColumn: "modelo_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehiculos_sucursales_sucursal_id",
                        column: x => x.sucursal_id,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehiculos_ubicaciones_ubicacion_id",
                        column: x => x.ubicacion_id,
                        principalTable: "ubicaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehiculos_versiones_version_id",
                        column: x => x.version_id,
                        principalTable: "versiones",
                        principalColumn: "version_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    cita_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo_cita = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    cliente_id = table.Column<int>(type: "int", nullable: false),
                    vehiculo_id = table.Column<int>(type: "int", nullable: false),
                    tipo_servicio_id = table.Column<int>(type: "int", nullable: false),
                    fecha_hora_inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_hora_fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    tipo_ingreso = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    motivo_visita = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    motivo_cancelacion = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    pre_orden_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    sucursal_id = table.Column<int>(type: "int", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citas", x => x.cita_id);
                    table.ForeignKey(
                        name: "FK_citas_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "cliente_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_citas_sucursales_sucursal_id",
                        column: x => x.sucursal_id,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_citas_tipos_servicio_tipo_servicio_id",
                        column: x => x.tipo_servicio_id,
                        principalTable: "tipos_servicio",
                        principalColumn: "tipo_servicio_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_citas_vehiculos_vehiculo_id",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "vehiculo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ordenes_servicio",
                columns: table => new
                {
                    os_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numero_os = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    cita_id = table.Column<int>(type: "int", nullable: true),
                    vehiculo_id = table.Column<int>(type: "int", nullable: false),
                    cliente_id = table.Column<int>(type: "int", nullable: false),
                    fecha_apertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_cierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    estado_id = table.Column<int>(type: "int", nullable: false),
                    kilometraje_ingreso = table.Column<int>(type: "int", nullable: false),
                    nivel_combustible = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    tipo_ingreso = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    es_garantia = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    observaciones_apertura = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    observaciones_cierre = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    asesor_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    coordinador_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    total_mano_obra = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    total_repuestos = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    total_general = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    sucursal_id = table.Column<int>(type: "int", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordenes_servicio", x => x.os_id);
                    table.ForeignKey(
                        name: "FK_ordenes_servicio_citas_cita_id",
                        column: x => x.cita_id,
                        principalTable: "citas",
                        principalColumn: "cita_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_servicio_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "cliente_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_servicio_estados_os_estado_id",
                        column: x => x.estado_id,
                        principalTable: "estados_os",
                        principalColumn: "estado_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_servicio_sucursales_sucursal_id",
                        column: x => x.sucursal_id,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_servicio_vehiculos_vehiculo_id",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "vehiculo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historial_ats",
                columns: table => new
                {
                    ats_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vehiculo_id = table.Column<int>(type: "int", nullable: false),
                    os_id = table.Column<int>(type: "int", nullable: false),
                    fecha_servicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tipo_servicio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    kilometraje = table.Column<int>(type: "int", nullable: false),
                    trabajos_realizados = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    monto_total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    observaciones_tecnicas = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    proxima_revision = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_ats", x => x.ats_id);
                    table.ForeignKey(
                        name: "FK_historial_ats_ordenes_servicio_os_id",
                        column: x => x.os_id,
                        principalTable: "ordenes_servicio",
                        principalColumn: "os_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historial_ats_vehiculos_vehiculo_id",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "vehiculo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "os_servicios",
                columns: table => new
                {
                    os_servicio_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    os_id = table.Column<int>(type: "int", nullable: false),
                    tipo_servicio_id = table.Column<int>(type: "int", nullable: false),
                    descripcion_trabajo = table.Column<string>(type: "nvarchar(3000)", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    tecnico_asignado_id = table.Column<int>(type: "int", nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_os_servicios", x => x.os_servicio_id);
                    table.ForeignKey(
                        name: "FK_os_servicios_ordenes_servicio_os_id",
                        column: x => x.os_id,
                        principalTable: "ordenes_servicio",
                        principalColumn: "os_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_os_servicios_tecnicos_tecnico_asignado_id",
                        column: x => x.tecnico_asignado_id,
                        principalTable: "tecnicos",
                        principalColumn: "tecnico_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_os_servicios_tipos_servicio_tipo_servicio_id",
                        column: x => x.tipo_servicio_id,
                        principalTable: "tipos_servicio",
                        principalColumn: "tipo_servicio_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recepciones",
                columns: table => new
                {
                    recepcion_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    os_id = table.Column<int>(type: "int", nullable: false),
                    fecha_hora_recepcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    recibido_por_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    entregado_por = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    estado_carroceria = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    accesorios_recibidos = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    llanta_repuesto = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    gato = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    triangulos = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    extintor = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    herramientas = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    radio = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    tapetes = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    observaciones_generales = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    firma_cliente_base64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    checklist_completado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recepciones", x => x.recepcion_id);
                    table.ForeignKey(
                        name: "FK_recepciones_ordenes_servicio_os_id",
                        column: x => x.os_id,
                        principalTable: "ordenes_servicio",
                        principalColumn: "os_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asignaciones_tecnico",
                columns: table => new
                {
                    asignacion_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    os_id = table.Column<int>(type: "int", nullable: false),
                    tecnico_id = table.Column<int>(type: "int", nullable: false),
                    os_servicio_id = table.Column<int>(type: "int", nullable: true),
                    fecha_asignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asignaciones_tecnico", x => x.asignacion_id);
                    table.ForeignKey(
                        name: "FK_asignaciones_tecnico_ordenes_servicio_os_id",
                        column: x => x.os_id,
                        principalTable: "ordenes_servicio",
                        principalColumn: "os_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asignaciones_tecnico_os_servicios_os_servicio_id",
                        column: x => x.os_servicio_id,
                        principalTable: "os_servicios",
                        principalColumn: "os_servicio_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_asignaciones_tecnico_tecnicos_tecnico_id",
                        column: x => x.tecnico_id,
                        principalTable: "tecnicos",
                        principalColumn: "tecnico_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evidencias",
                columns: table => new
                {
                    evidencia_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    os_id = table.Column<int>(type: "int", nullable: false),
                    recepcion_id = table.Column<int>(type: "int", nullable: true),
                    tipo_evidencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    url_archivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    fecha_captura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_registro_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidencias", x => x.evidencia_id);
                    table.ForeignKey(
                        name: "FK_evidencias_ordenes_servicio_os_id",
                        column: x => x.os_id,
                        principalTable: "ordenes_servicio",
                        principalColumn: "os_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_evidencias_recepciones_recepcion_id",
                        column: x => x.recepcion_id,
                        principalTable: "recepciones",
                        principalColumn: "recepcion_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "estados_os",
                columns: new[] { "estado_id", "activo", "codigo", "descripcion", "fecha_creacion", "fecha_modificacion", "nombre", "orden_secuencial", "usuario_crea_id", "usuario_modifica_id" },
                values: new object[,]
                {
                    { 1, true, "ABIERTA", "OS creada, pendiente de diagnóstico", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Abierta", 1, null, null },
                    { 2, true, "DIAGNOSTICO", "Técnico evaluando el vehículo", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "En Diagnóstico", 2, null, null },
                    { 3, true, "COTIZADA", "Presupuesto generado", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cotizada", 3, null, null },
                    { 4, true, "APROBADA", "Cliente aprobó cotización", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aprobada", 4, null, null },
                    { 5, true, "EN_TRABAJO", "Técnico trabajando", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "En Trabajo", 5, null, null },
                    { 6, true, "PAUSADA", "Trabajo detenido", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pausada", 6, null, null },
                    { 7, true, "COMPLETADA", "Trabajo terminado", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Completada", 7, null, null },
                    { 8, true, "FACTURADA", "Factura generada", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Facturada", 8, null, null },
                    { 9, true, "CERRADA", "OS cerrada y pagada", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cerrada", 9, null, null },
                    { 99, true, "CANCELADA", "OS cancelada", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cancelada", 99, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_sucursales_codigo",
                table: "sucursales",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_tecnico_os_id",
                table: "asignaciones_tecnico",
                column: "os_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_tecnico_os_servicio_id",
                table: "asignaciones_tecnico",
                column: "os_servicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_tecnico_tecnico_id",
                table: "asignaciones_tecnico",
                column: "tecnico_id");

            migrationBuilder.CreateIndex(
                name: "IX_bloques_horario_capacidad_id",
                table: "bloques_horario",
                column: "capacidad_id");

            migrationBuilder.CreateIndex(
                name: "IX_capacidad_taller_fecha_turno_sucursal_id",
                table: "capacidad_taller",
                columns: new[] { "fecha", "turno", "sucursal_id" },
                unique: true,
                filter: "[sucursal_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_capacidad_taller_sucursal_id",
                table: "capacidad_taller",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_citas_cliente_id",
                table: "citas",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_citas_codigo_cita",
                table: "citas",
                column: "codigo_cita",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_citas_estado",
                table: "citas",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_citas_fecha_hora_inicio",
                table: "citas",
                column: "fecha_hora_inicio");

            migrationBuilder.CreateIndex(
                name: "IX_citas_sucursal_id",
                table: "citas",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_citas_tipo_servicio_id",
                table: "citas",
                column: "tipo_servicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_citas_vehiculo_id",
                table: "citas",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_documento_identidad",
                table: "clientes",
                column: "documento_identidad",
                unique: true,
                filter: "[documento_identidad] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_email",
                table: "clientes",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_telefono",
                table: "clientes",
                column: "telefono");

            migrationBuilder.CreateIndex(
                name: "IX_estados_os_codigo",
                table: "estados_os",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_os_id",
                table: "evidencias",
                column: "os_id");

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_recepcion_id",
                table: "evidencias",
                column: "recepcion_id");

            migrationBuilder.CreateIndex(
                name: "IX_historial_ats_os_id",
                table: "historial_ats",
                column: "os_id");

            migrationBuilder.CreateIndex(
                name: "IX_historial_ats_vehiculo_id",
                table: "historial_ats",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_historial_ats_vehiculo_id_fecha_servicio",
                table: "historial_ats",
                columns: new[] { "vehiculo_id", "fecha_servicio" });

            migrationBuilder.CreateIndex(
                name: "IX_marcas_codigo",
                table: "marcas",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modelos_codigo",
                table: "modelos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modelos_marca_id",
                table: "modelos",
                column: "marca_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_servicio_cita_id",
                table: "ordenes_servicio",
                column: "cita_id",
                unique: true,
                filter: "[cita_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_servicio_cliente_id",
                table: "ordenes_servicio",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_servicio_estado_id",
                table: "ordenes_servicio",
                column: "estado_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_servicio_fecha_apertura",
                table: "ordenes_servicio",
                column: "fecha_apertura");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_servicio_numero_os",
                table: "ordenes_servicio",
                column: "numero_os",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_servicio_sucursal_id",
                table: "ordenes_servicio",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_servicio_vehiculo_id",
                table: "ordenes_servicio",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_os_servicios_os_id",
                table: "os_servicios",
                column: "os_id");

            migrationBuilder.CreateIndex(
                name: "IX_os_servicios_tecnico_asignado_id",
                table: "os_servicios",
                column: "tecnico_asignado_id");

            migrationBuilder.CreateIndex(
                name: "IX_os_servicios_tipo_servicio_id",
                table: "os_servicios",
                column: "tipo_servicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_recepciones_os_id",
                table: "recepciones",
                column: "os_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tecnicos_codigo",
                table: "tecnicos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tecnicos_sucursal_id",
                table: "tecnicos",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_servicio_codigo",
                table: "tipos_servicio",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_cliente_id",
                table: "vehiculos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_estado",
                table: "vehiculos",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_marca_id",
                table: "vehiculos",
                column: "marca_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_modelo_id",
                table: "vehiculos",
                column: "modelo_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_placa",
                table: "vehiculos",
                column: "placa");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_sucursal_id",
                table: "vehiculos",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_ubicacion_id",
                table: "vehiculos",
                column: "ubicacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_version_id",
                table: "vehiculos",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_vin",
                table: "vehiculos",
                column: "vin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_versiones_codigo",
                table: "versiones",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_versiones_modelo_id",
                table: "versiones",
                column: "modelo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ubicaciones_sucursales_sucursal_id",
                table: "ubicaciones",
                column: "sucursal_id",
                principalTable: "sucursales",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSucursales_sucursales_SucursalID",
                table: "UsuarioSucursales",
                column: "SucursalID",
                principalTable: "sucursales",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ubicaciones_sucursales_sucursal_id",
                table: "ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSucursales_sucursales_SucursalID",
                table: "UsuarioSucursales");

            migrationBuilder.DropTable(
                name: "asignaciones_tecnico");

            migrationBuilder.DropTable(
                name: "bloques_horario");

            migrationBuilder.DropTable(
                name: "evidencias");

            migrationBuilder.DropTable(
                name: "historial_ats");

            migrationBuilder.DropTable(
                name: "os_servicios");

            migrationBuilder.DropTable(
                name: "capacidad_taller");

            migrationBuilder.DropTable(
                name: "recepciones");

            migrationBuilder.DropTable(
                name: "tecnicos");

            migrationBuilder.DropTable(
                name: "ordenes_servicio");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "estados_os");

            migrationBuilder.DropTable(
                name: "tipos_servicio");

            migrationBuilder.DropTable(
                name: "vehiculos");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "versiones");

            migrationBuilder.DropTable(
                name: "modelos");

            migrationBuilder.DropTable(
                name: "marcas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ubicaciones",
                table: "ubicaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sucursales",
                table: "sucursales");

            migrationBuilder.DropIndex(
                name: "IX_sucursales_codigo",
                table: "sucursales");

            migrationBuilder.RenameTable(
                name: "ubicaciones",
                newName: "Ubicaciones");

            migrationBuilder.RenameTable(
                name: "sucursales",
                newName: "Sucursales");

            migrationBuilder.RenameColumn(
                name: "tipo",
                table: "Ubicaciones",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Ubicaciones",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "activa",
                table: "Ubicaciones",
                newName: "Activa");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "Ubicaciones",
                newName: "SucursalID");

            migrationBuilder.RenameIndex(
                name: "IX_ubicaciones_sucursal_id",
                table: "Ubicaciones",
                newName: "IX_Ubicaciones_SucursalID");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Sucursales",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "direccion",
                table: "Sucursales",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "codigo",
                table: "Sucursales",
                newName: "Codigo");

            migrationBuilder.RenameColumn(
                name: "ciudad",
                table: "Sucursales",
                newName: "Ciudad");

            migrationBuilder.RenameColumn(
                name: "activa",
                table: "Sucursales",
                newName: "Activa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Sucursales",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Ubicaciones",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Ubicaciones",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Activa",
                table: "Ubicaciones",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Sucursales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "Sucursales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Sucursales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Ciudad",
                table: "Sucursales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Activa",
                table: "Sucursales",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ubicaciones",
                table: "Ubicaciones",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sucursales",
                table: "Sucursales",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Sucursales_SucursalID",
                table: "Ubicaciones",
                column: "SucursalID",
                principalTable: "Sucursales",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSucursales_Sucursales_SucursalID",
                table: "UsuarioSucursales",
                column: "SucursalID",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
