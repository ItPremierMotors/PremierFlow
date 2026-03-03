#!/usr/bin/env python3
"""
Genera el documento Word: Diseño del Módulo de Repuestos, Inventario y Compras
"""

from docx import Document
from docx.shared import Inches, Pt, Cm, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.enum.style import WD_STYLE_TYPE
from docx.oxml.ns import qn
import os

doc = Document()

# ── Estilos ──────────────────────────────────────────────────
style = doc.styles['Normal']
font = style.font
font.name = 'Calibri'
font.size = Pt(11)

for level in range(1, 4):
    heading_style = doc.styles[f'Heading {level}']
    heading_style.font.color.rgb = RGBColor(0x1B, 0x3A, 0x5C)

# ── Helper functions ─────────────────────────────────────────
def add_entity_table(doc, title, fields, notes=None):
    """Adds a formatted entity table."""
    doc.add_heading(title, level=3)

    table = doc.add_table(rows=1, cols=4)
    table.style = 'Light Grid Accent 1'
    table.alignment = WD_TABLE_ALIGNMENT.CENTER

    hdr = table.rows[0].cells
    headers = ['Campo', 'Tipo', 'Requerido', 'Descripción']
    for i, h in enumerate(headers):
        hdr[i].text = h
        for p in hdr[i].paragraphs:
            for run in p.runs:
                run.bold = True
                run.font.size = Pt(9)

    for field in fields:
        row = table.add_row().cells
        for i, val in enumerate(field):
            row[i].text = str(val)
            for p in row[i].paragraphs:
                for run in p.runs:
                    run.font.size = Pt(9)

    if notes:
        p = doc.add_paragraph()
        run = p.add_run(notes)
        run.font.size = Pt(9)
        run.italic = True

    doc.add_paragraph()  # spacing


def add_enum_table(doc, name, values):
    """Adds enum definition table."""
    p = doc.add_paragraph()
    run = p.add_run(name)
    run.bold = True
    run.font.size = Pt(10)

    table = doc.add_table(rows=1, cols=3)
    table.style = 'Light Grid Accent 1'
    table.alignment = WD_TABLE_ALIGNMENT.CENTER

    hdr = table.rows[0].cells
    for i, h in enumerate(['Valor', 'ID', 'Descripción']):
        hdr[i].text = h
        for p in hdr[i].paragraphs:
            for run in p.runs:
                run.bold = True
                run.font.size = Pt(9)

    for val in values:
        row = table.add_row().cells
        for i, v in enumerate(val):
            row[i].text = str(v)
            for p in row[i].paragraphs:
                for run in p.runs:
                    run.font.size = Pt(9)

    doc.add_paragraph()


# ══════════════════════════════════════════════════════════════
# PORTADA
# ══════════════════════════════════════════════════════════════
for _ in range(6):
    doc.add_paragraph()

p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
run = p.add_run('PremierFlow')
run.font.size = Pt(36)
run.bold = True
run.font.color.rgb = RGBColor(0x1B, 0x3A, 0x5C)

p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
run = p.add_run('Diseño del Módulo de\nRepuestos, Inventario y Compras')
run.font.size = Pt(22)
run.font.color.rgb = RGBColor(0x1B, 0x3A, 0x5C)

doc.add_paragraph()

p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
run = p.add_run('Documento de Arquitectura de Entidades')
run.font.size = Pt(14)
run.font.color.rgb = RGBColor(0x66, 0x66, 0x66)

for _ in range(4):
    doc.add_paragraph()

p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
run = p.add_run('Versión 1.0  •  Marzo 2026')
run.font.size = Pt(11)
run.font.color.rgb = RGBColor(0x99, 0x99, 0x99)

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# TABLA DE CONTENIDO
# ══════════════════════════════════════════════════════════════
doc.add_heading('Tabla de Contenido', level=1)
toc_items = [
    '1. Contexto y Situación Actual',
    '2. Principio Fundamental: Un Repuesto, Tres Canales',
    '3. Diagrama UML de Clases',
    '4. Detalle de Entidades Nuevas',
    '   4.1 Catálogo: CategoriaProducto',
    '   4.2 Catálogo: Producto',
    '   4.3 Almacén: InventarioSucursal',
    '   4.4 Almacén: MovimientoInventario',
    '   4.5 Canal Servicio: OsRepuesto',
    '   4.6 Canal Venta: VentaMostrador',
    '   4.7 Canal Venta: DetalleVenta',
    '   4.8 Canal Compra: Proveedor',
    '   4.9 Canal Compra: OrdenCompra',
    '   4.10 Canal Compra: OrdenCompraDetalle',
    '   4.11 Canal Compra: RecepcionCompra',
    '   4.12 Canal Compra: RecepcionCompraDetalle',
    '5. Enumeraciones Nuevas',
    '6. Integración con Entidades Existentes',
    '7. Flujos de Negocio',
    '8. Resumen de Archivos a Crear/Modificar',
]
for item in toc_items:
    p = doc.add_paragraph(item)
    p.paragraph_format.space_after = Pt(2)

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# 1. CONTEXTO
# ══════════════════════════════════════════════════════════════
doc.add_heading('1. Contexto y Situación Actual', level=1)

doc.add_paragraph(
    'PremierFlow es un sistema de gestión para concesionarios automotrices (DMS) '
    'construido con ASP.NET Core 10.0, Entity Framework Core y SQL Server. '
    'Actualmente gestiona el ciclo completo de taller (OrdenServicio, OsServicio, '
    'asignación de técnicos, recepción de vehículos) y la venta de vehículos nuevos '
    '(con tracking de importación).'
)

doc.add_heading('Lo que falta', level=2)
doc.add_paragraph(
    'El sistema NO tiene módulo de repuestos. El campo TotalRepuestos en OrdenServicio '
    'siempre vale 0, y hay un comentario en el código que dice: "TotalRepuestos se '
    'calcularía desde otra tabla de repuestos". Este documento diseña esa tabla y '
    'todo el ecosistema que la rodea.'
)

doc.add_heading('Entidades existentes relevantes', level=2)

table = doc.add_table(rows=1, cols=3)
table.style = 'Light Grid Accent 1'
hdr = table.rows[0].cells
hdr[0].text = 'Entidad'
hdr[1].text = 'Propósito'
hdr[2].text = 'Relación con repuestos'
for cell in hdr:
    for p in cell.paragraphs:
        for run in p.runs:
            run.bold = True
            run.font.size = Pt(9)

existing = [
    ('OrdenServicio', 'Documento principal de taller', 'Se le agregan líneas de repuestos (OsRepuesto)'),
    ('OsServicio', 'Línea de mano de obra en OS', 'OsRepuesto será su gemelo para repuestos'),
    ('Cliente', 'Registro de clientes', 'Puede comprar repuestos en mostrador'),
    ('Sucursal', 'Sedes de Premier (SPS, TGU)', 'Cada sucursal tiene su propio inventario'),
    ('Vehiculo', 'Vehículos individuales', 'Ya tiene campos de importación que sirven de patrón'),
    ('Tecnico', 'Técnicos del taller', 'Puede solicitar repuestos para una OS'),
    ('Ubicacion', 'Ubicaciones físicas', 'Tipos: Bodega, DepositoFiscal, Transito'),
    ('ApplicationUser', 'Usuarios del sistema', 'Vendedor, aprobador, receptor de mercancía'),
]
for row_data in existing:
    row = table.add_row().cells
    for i, val in enumerate(row_data):
        row[i].text = val
        for p in row[i].paragraphs:
            for run in p.runs:
                run.font.size = Pt(9)

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# 2. PRINCIPIO FUNDAMENTAL
# ══════════════════════════════════════════════════════════════
doc.add_heading('2. Principio Fundamental: Un Repuesto, Tres Canales', level=1)

doc.add_paragraph(
    'En un DMS, el repuesto es UNO SOLO. No existen "repuestos de servicio" y '
    '"repuestos de venta" — es el mismo producto del mismo inventario. '
    'Lo que cambia es el CANAL por donde sale:'
)

doc.add_paragraph('Canal Servicio (Taller): El técnico consume el repuesto durante una reparación. '
    'Sale a través de una Orden de Servicio.', style='List Bullet')
doc.add_paragraph('Canal Venta (Mostrador): El cliente compra el repuesto directamente. '
    'Sale a través de una Venta de Mostrador.', style='List Bullet')
doc.add_paragraph('Canal Compra (Proveedores): El repuesto entra al inventario. '
    'Viene de proveedores locales o internacionales a través de Órdenes de Compra.', style='List Bullet')

doc.add_paragraph()
p = doc.add_paragraph()
run = p.add_run('Los tres canales comparten el mismo Producto y el mismo InventarioSucursal. '
    'Todo movimiento (entrada o salida) se audita en MovimientoInventario.')
run.bold = True

doc.add_paragraph()

# Diagram as text
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
run = p.add_run(
    '┌─────────────┐        ┌────────────────┐        ┌──────────────────┐\n'
    '│  PROVEEDOR   │───────►│  INVENTARIO    │───────►│  ORDEN SERVICIO  │\n'
    '│  (entrada)   │  +stock│  SUCURSAL      │ -stock │  (canal taller)  │\n'
    '└─────────────┘        │  (almacén)     │        └──────────────────┘\n'
    '                        │                │\n'
    '                        │                │───────►┌──────────────────┐\n'
    '                        │                │ -stock │  VENTA MOSTRADOR │\n'
    '                        └────────────────┘        │  (canal venta)   │\n'
    '                                │                 └──────────────────┘\n'
    '                                ▼\n'
    '                        ┌────────────────┐\n'
    '                        │  MOVIMIENTO    │\n'
    '                        │  INVENTARIO    │\n'
    '                        │  (auditoría)   │\n'
    '                        └────────────────┘\n'
)
run.font.name = 'Consolas'
run.font.size = Pt(9)

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# 3. DIAGRAMA UML
# ══════════════════════════════════════════════════════════════
doc.add_heading('3. Diagrama UML de Clases', level=1)

doc.add_paragraph(
    'A continuación se presenta el diagrama UML completo mostrando las 12 entidades '
    'nuevas (en azul conceptual) y su relación con las entidades existentes (en gris). '
    'Las cardinalidades se indican en cada relación.'
)

doc.add_paragraph()

# UML Diagram as formatted text
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.LEFT
run = p.add_run(
    '═══════════════════════════════════════════════════════════════════════════════\n'
    '                        DIAGRAMA UML - PREMIERFLOW\n'
    '                  Módulo de Repuestos, Inventario y Compras\n'
    '═══════════════════════════════════════════════════════════════════════════════\n'
    '\n'
    '┌─────────────────────────────────────────────────────────────────────────────┐\n'
    '│ ENTIDADES EXISTENTES                                                       │\n'
    '│                                                                            │\n'
    '│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │\n'
    '│  │   Sucursal    │  │   Cliente    │  │   Tecnico    │  │   Vehiculo   │   │\n'
    '│  │──────────────│  │──────────────│  │──────────────│  │──────────────│   │\n'
    '│  │ Id        PK │  │ ClienteId PK │  │ TecnicoId PK │  │ VehiculoId PK│   │\n'
    '│  │ Nombre       │  │ Nombre       │  │ Nombre       │  │ Vin          │   │\n'
    '│  │ Codigo       │  │ Telefono     │  │ Especialidad │  │ Placa        │   │\n'
    '│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘  └──────────────┘   │\n'
    '│         │                 │                  │                              │\n'
    '│         │      ┌──────────────────────────────────────────────────┐        │\n'
    '│         │      │          OrdenServicio (EXISTENTE)               │        │\n'
    '│         │      │─────────────────────────────────────────────────│        │\n'
    '│         │      │ OsId PK                                         │        │\n'
    '│         │      │ NumeroOs          ClienteId FK                   │        │\n'
    '│         │      │ TotalManoObra     VehiculoId FK                  │        │\n'
    '│         │      │ TotalRepuestos ◄── AHORA SE CALCULA REAL        │        │\n'
    '│         │      │ TotalGeneral = ManoObra + Repuestos              │        │\n'
    '│         │      │─────────────────────────────────────────────────│        │\n'
    '│         │      │ Servicios: OsServicio[]  (YA EXISTE)            │        │\n'
    '│         │      │ Repuestos: OsRepuesto[]  ★ NUEVO                │        │\n'
    '│         │      └───────────────────┬──────────────────────────────┘        │\n'
    '│         │                          │                                       │\n'
    '└─────────┼──────────────────────────┼───────────────────────────────────────┘\n'
    '          │                          │\n'
    '══════════╪══════════════════════════╪════════════════════════════════════════\n'
    '          │     ENTIDADES NUEVAS     │\n'
    '══════════╪══════════════════════════╪════════════════════════════════════════\n'
    '          │                          │\n'
    '          │    ┌─────────────────────┼────────────────────────────┐\n'
    '          │    │     CATÁLOGO        │                            │\n'
    '          │    │                     │                            │\n'
    '          │    │ ┌────────────────┐  │  ┌──────────────────────┐  │\n'
    '          │    │ │CategoriaProducto│ │  │     Producto         │  │\n'
    '          │    │ │────────────────│  │  │──────────────────────│  │\n'
    '          │    │ │CategoriaId  PK │  │  │ ProductoId       PK  │  │\n'
    '          │    │ │Nombre          │◄─┤  │ Codigo (SKU)         │  │\n'
    '          │    │ │CategoriaPadreId│  │  │ NumeroParteOEM       │  │\n'
    '          │    │ │(auto-ref) FK   │  │  │ Nombre               │  │\n'
    '          │    │ └────────────────┘  │  │ CategoriaProductoIdFK│  │\n'
    '          │    │                 1:N │  │ PrecioCosto          │  │\n'
    '          │    │                     │  │ PrecioVenta          │  │\n'
    '          │    │                     │  │ UnidadMedida (enum)  │  │\n'
    '          │    │                     │  │ EsOriginal           │  │\n'
    '          │    │                     │  └──────────┬───────────┘  │\n'
    '          │    └─────────────────────┼────────────┼───────────────┘\n'
    '          │                          │            │\n'
    '          │                          │   ┌────────┼──────────────────┐\n'
    '          │                          │   │        │                  │\n'
    '          ▼                          │   ▼        ▼                  ▼\n'
    ' ┌──────────────────┐               │ ┌────────────────┐  ┌────────────────┐\n'
    ' │InventarioSucursal│               │ │OsRepuesto      │  │DetalleVenta    │\n'
    ' │──────────────────│               │ │────────────────│  │────────────────│\n'
    ' │InventSucursalIdPK│               │ │OsRepuestoId PK │  │DetalleVentaIdPK│\n'
    ' │ProductoId     FK │               │ │OsId         FK │──┘VentaMostrId FK │\n'
    ' │SucursalId     FK │               │ │ProductoId   FK │  │ProductoId   FK │\n'
    ' │CantidadDisponible│               │ │Cantidad        │  │Cantidad        │\n'
    ' │CantidadReservada │               │ │PrecioUnitario  │  │PrecioUnitario  │\n'
    ' │StockMinimo       │               │ │Descuento       │  │Subtotal        │\n'
    ' │StockMaximo       │               │ │Subtotal        │  └───────┬────────┘\n'
    ' │UbicacionAlmacen  │               │ │Estado (enum)   │          │\n'
    ' └──────────────────┘               │ │TecnicoSolIdFK? │          ▼\n'
    '  UNIQUE(Producto,Sucursal)         │ └────────────────┘  ┌────────────────┐\n'
    '                                    │                     │VentaMostrador  │\n'
    ' ┌──────────────────┐               │                     │────────────────│\n'
    ' │MovimientoInvent. │               │                     │VentaMostIdPK   │\n'
    ' │──────────────────│               │                     │NumeroVenta     │\n'
    ' │MovimientoId   PK │               │                     │ClienteId   FK? │\n'
    ' │ProductoId     FK │               │                     │SucursalId  FK  │\n'
    ' │SucursalId     FK │               │                     │VendedorId  FK  │\n'
    ' │TipoMovimiento   │               │                     │Total           │\n'
    ' │Cantidad (+/-)    │               │                     │EstadoVenta     │\n'
    ' │CantidadAnterior  │               │                     └────────────────┘\n'
    ' │CantidadDespues   │               │\n'
    ' │OrigenDocumento   │               │    CANAL COMPRA (ENTRADA)\n'
    ' │ReferenciaId      │               │    ═══════════════════════\n'
    ' └──────────────────┘               │\n'
    '                                    │ ┌────────────────┐\n'
    '                                    │ │  Proveedor     │\n'
    '                                    │ │────────────────│\n'
    '                                    │ │ProveedorId  PK │\n'
    '                                    │ │RazonSocial     │\n'
    '                                    │ │TipoProveedor   │\n'
    '                                    │ │DiasCredito     │\n'
    '                                    │ │Moneda          │\n'
    '                                    │ └───────┬────────┘\n'
    '                                    │         │ 1:N\n'
    '                                    │         ▼\n'
    '                                    │ ┌────────────────┐\n'
    '                                    │ │ OrdenCompra    │\n'
    '                                    │ │────────────────│\n'
    '                                    │ │OrdenCompraIdPK │\n'
    '                                    │ │NumeroOC        │\n'
    '                                    │ │ProveedorId  FK │\n'
    '                                    │ │SucursalDest FK │\n'
    '                                    │ │Estado (enum)   │\n'
    '                                    │ │Subtotal        │\n'
    '                                    │ │CostoFlete      │\n'
    '                                    │ │CostoAduana     │\n'
    '                                    │ │TotalGeneral    │\n'
    '                                    │ │── Importación──│\n'
    '                                    │ │NumeroContenedor│\n'
    '                                    │ │NumeroBL        │\n'
    '                                    │ │PuertoOrigen    │\n'
    '                                    │ │FechaEmbarque   │\n'
    '                                    │ └──┬─────────┬───┘\n'
    '                                    │    │1:N      │1:N\n'
    '                                    │    ▼         ▼\n'
    '                                    │ ┌────────┐ ┌──────────────┐\n'
    '                                    │ │OC      │ │Recepcion     │\n'
    '                                    │ │Detalle │ │Compra        │\n'
    '                                    │ │────────│ │──────────────│\n'
    '                                    │ │OCDetIdP│ │RecCompraIdPK │\n'
    '                                    │ │OCId FK │ │OCId       FK │\n'
    '                                    │ │ProdIdFK│ │SucursalId FK │\n'
    '                                    │ │CantPed │ │FechaRecepc.  │\n'
    '                                    │ │CantRec │ │Estado (enum) │\n'
    '                                    │ │PrecioU │ │RecibidoPorFK │\n'
    '                                    │ │Subtotal│ └──────┬───────┘\n'
    '                                    │ └────────┘        │ 1:N\n'
    '                                    │     ▲             ▼\n'
    '                                    │     │    ┌──────────────┐\n'
    '                                    │     │    │RecepcionComp │\n'
    '                                    │     │    │Detalle       │\n'
    '                                    │     │    │──────────────│\n'
    '                                    │     │    │RecCompDetIdPK│\n'
    '                                    │     └────│OCDetalleIdFK │\n'
    '                                    │  (ref.)  │ProductoIdFK  │\n'
    '                                    │          │CantRecibida  │\n'
    '                                    │          │CantDanada    │\n'
    '                                    │          │CantAceptada  │\n'
    '                                    │          └──────────────┘\n'
    '                                    │\n'
    '══════════════════════════════════════════════════════════════════\n'
)
run.font.name = 'Consolas'
run.font.size = Pt(7)

doc.add_page_break()

# ── Cardinalidades ──
doc.add_heading('Cardinalidades', level=2)

card_table = doc.add_table(rows=1, cols=3)
card_table.style = 'Light Grid Accent 1'
hdr = card_table.rows[0].cells
for i, h in enumerate(['Desde', 'Hacia', 'Cardinalidad']):
    hdr[i].text = h
    for p in hdr[i].paragraphs:
        for run in p.runs:
            run.bold = True
            run.font.size = Pt(9)

cardinalities = [
    ('CategoriaProducto', 'CategoriaProducto (padre)', 'N:1 (auto-referencia)'),
    ('CategoriaProducto', 'Producto', '1:N'),
    ('Producto', 'InventarioSucursal', '1:N (uno por sucursal)'),
    ('Producto', 'MovimientoInventario', '1:N'),
    ('Producto', 'OsRepuesto', '1:N'),
    ('Producto', 'DetalleVenta', '1:N'),
    ('Producto', 'OrdenCompraDetalle', '1:N'),
    ('Sucursal (existente)', 'InventarioSucursal', '1:N'),
    ('OrdenServicio (existente)', 'OsRepuesto', '1:N'),
    ('Tecnico (existente)', 'OsRepuesto', '1:N (opcional)'),
    ('Cliente (existente)', 'VentaMostrador', '1:N (opcional)'),
    ('VentaMostrador', 'DetalleVenta', '1:N'),
    ('Proveedor', 'OrdenCompra', '1:N'),
    ('OrdenCompra', 'OrdenCompraDetalle', '1:N'),
    ('OrdenCompra', 'RecepcionCompra', '1:N (entregas parciales)'),
    ('RecepcionCompra', 'RecepcionCompraDetalle', '1:N'),
    ('OrdenCompraDetalle', 'RecepcionCompraDetalle', '1:N (ref.)'),
]
for row_data in cardinalities:
    row = card_table.add_row().cells
    for i, val in enumerate(row_data):
        row[i].text = val
        for p in row[i].paragraphs:
            for run in p.runs:
                run.font.size = Pt(9)

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# 4. DETALLE DE ENTIDADES
# ══════════════════════════════════════════════════════════════
doc.add_heading('4. Detalle de Entidades Nuevas', level=1)

# 4.1 CategoriaProducto
add_entity_table(doc, '4.1 CategoriaProducto — Clasificación de repuestos', [
    ('CategoriaProductoId', 'int, PK', 'Sí', 'Identificador único'),
    ('Nombre', 'string(100)', 'Sí', '"Filtros", "Aceites", "Frenos", "Suspensión"'),
    ('Descripcion', 'string(500)', 'No', 'Descripción opcional'),
    ('CategoriaPadreId', 'int?, FK', 'No', 'FK a sí misma. null = categoría raíz'),
],
'Propósito: Organizar el catálogo de repuestos en árbol jerárquico. '
'Ejemplo: Filtros → Filtro Aceite, Filtro Aire, Filtro Combustible. '
'Hereda de: SoftDeletableEntity.')

# 4.2 Producto
add_entity_table(doc, '4.2 Producto — Catálogo maestro de repuestos', [
    ('ProductoId', 'int, PK', 'Sí', 'Identificador único'),
    ('Codigo', 'string(50), único', 'Sí', 'SKU interno: "FLT-ACE-001"'),
    ('NumeroParteOEM', 'string(50)', 'No', 'Número del fabricante: "15400-RTA-003"'),
    ('CodigoBarras', 'string(50)', 'No', 'Para escaneo rápido en almacén/mostrador'),
    ('Nombre', 'string(200)', 'Sí', '"Filtro de Aceite Honda Civic"'),
    ('Descripcion', 'string(2000)', 'No', 'Detalle extendido, especificaciones'),
    ('CategoriaProductoId', 'int, FK', 'Sí', 'A qué categoría pertenece'),
    ('MarcaRepuesto', 'string(100)', 'No', 'Marca del repuesto: "Mann", "Denso", "NGK"'),
    ('UnidadMedida', 'enum', 'Sí', 'Pieza, Litro, Galón, Juego, Metro, Kg'),
    ('PrecioCosto', 'decimal(10,2)', 'Sí', 'Lo que paga Premier al proveedor'),
    ('PrecioVenta', 'decimal(10,2)', 'Sí', 'Precio de lista al público'),
    ('EsOriginal', 'bool', 'Sí', 'true = OEM genuino, false = aftermarket'),
    ('Peso', 'decimal(8,3)', 'No', 'En kg, útil para calcular flete de importación'),
    ('ImagenUrl', 'string(500)', 'No', 'URL de la foto del producto'),
],
'Propósito: Registro maestro del repuesto. Es el centro de todo el módulo — todos los '
'demás apuntan aquí. Un solo registro sin importar si se usa en taller o se vende en '
'mostrador. Hereda de: SoftDeletableEntity. '
'Métodos de dominio: MargenGanancia, TieneMargenSaludable.')

# 4.3 InventarioSucursal
add_entity_table(doc, '4.3 InventarioSucursal — Stock real por sucursal', [
    ('InventarioSucursalId', 'int, PK', 'Sí', 'Identificador único'),
    ('ProductoId', 'int, FK', 'Sí', 'Qué producto'),
    ('SucursalId', 'int, FK', 'Sí', 'En qué sucursal'),
    ('CantidadDisponible', 'int', 'Sí', 'Stock libre para usar/vender'),
    ('CantidadReservada', 'int', 'Sí', 'Apartado para OS aprobadas (no disponible)'),
    ('StockMinimo', 'int', 'Sí', 'Punto de reorden — alerta cuando baja de aquí'),
    ('StockMaximo', 'int', 'Sí', 'Capacidad máxima de almacenamiento'),
    ('UbicacionAlmacen', 'string(50)', 'No', 'Ubicación física: "Estante A3-B2"'),
],
'Propósito: Saber cuántas unidades de cada producto hay en cada sucursal. '
'Índice UNIQUE en (ProductoId, SucursalId) — un solo registro por combinación. '
'Regla: Stock Físico Total = CantidadDisponible + CantidadReservada. '
'Hereda de: AuditableEntity. '
'Métodos: NecesitaReorden, Reservar(n), Consumir(n), Recibir(n).')

# 4.4 MovimientoInventario
add_entity_table(doc, '4.4 MovimientoInventario — Auditoría de todo movimiento', [
    ('MovimientoId', 'int, PK', 'Sí', 'Identificador único'),
    ('ProductoId', 'int, FK', 'Sí', 'Qué producto se movió'),
    ('SucursalId', 'int, FK', 'Sí', 'En qué sucursal'),
    ('TipoMovimiento', 'enum', 'Sí', 'Compra, Consumo, Venta, Reserva, Ajuste, etc.'),
    ('Cantidad', 'int', 'Sí', 'Positivo = entrada, Negativo = salida'),
    ('CantidadAnterior', 'int', 'Sí', 'Foto del stock ANTES del movimiento'),
    ('CantidadDespues', 'int', 'Sí', 'Foto del stock DESPUÉS del movimiento'),
    ('OrigenDocumento', 'enum', 'Sí', 'OrdenServicio, VentaMostrador, OrdenCompra, AjusteManual'),
    ('ReferenciaId', 'int', 'Sí', 'ID del documento que causó el movimiento'),
    ('CostoUnitario', 'decimal(10,2)', 'No', 'Costo al momento (para valorización)'),
    ('Observaciones', 'string(500)', 'No', 'Notas'),
],
'Propósito: El "libro diario" del inventario. Cada entrada, salida, reserva, '
'ajuste o transferencia queda registrada. Es INMUTABLE — no se edita ni borra. '
'Hereda de: AuditableEntity (NO SoftDeletableEntity, porque nunca se "desactiva").')

doc.add_page_break()

# 4.5 OsRepuesto
add_entity_table(doc, '4.5 OsRepuesto — Repuesto usado en una Orden de Servicio', [
    ('OsRepuestoId', 'int, PK', 'Sí', 'Identificador único'),
    ('OsId', 'int, FK', 'Sí', 'A qué Orden de Servicio pertenece'),
    ('ProductoId', 'int, FK', 'Sí', 'Qué producto/repuesto'),
    ('Cantidad', 'int', 'Sí', 'Cuántas unidades se necesitan'),
    ('PrecioUnitario', 'decimal(10,2)', 'Sí', 'Precio al momento de agregar'),
    ('Descuento', 'decimal(10,2)', 'Sí', 'Monto de descuento'),
    ('Subtotal', 'decimal(10,2)', 'Sí', '(Precio × Cantidad) - Descuento'),
    ('Estado', 'enum', 'Sí', 'Pendiente, Reservado, Entregado, Cancelado'),
    ('TecnicoSolicitaId', 'int?, FK', 'No', 'Qué técnico pidió el repuesto'),
    ('Observaciones', 'string(500)', 'No', ''),
],
'Propósito: Es el GEMELO de OsServicio. Así como OsServicio es la línea de mano de obra, '
'OsRepuesto es la línea de repuesto dentro de la OS. Juntos forman la cotización completa. '
'Hereda de: SoftDeletableEntity. '
'Métodos: CalcularSubtotal(), Reservar(), Entregar(), Cancelar().')

doc.add_paragraph()
p = doc.add_paragraph()
run = p.add_run('Ciclo de vida del OsRepuesto dentro de la OS:')
run.bold = True

cycle_data = [
    ('DIAGNÓSTICO / COTIZADA', 'Pendiente', 'Se identifican repuestos. Stock NO se toca.'),
    ('APROBADA', 'Reservado', 'Disponible -= N, Reservada += N. Se genera MovimientoInventario tipo Reserva.'),
    ('COMPLETADA', 'Entregado', 'Reservada -= N. El repuesto ya salió. MovimientoInventario tipo ConsumoServicio.'),
    ('CANCELADA', 'Cancelado', 'Si estaba Reservado: Disponible += N, Reservada -= N. MovimientoInventario tipo DevolucionReserva.'),
]
cycle_table = doc.add_table(rows=1, cols=3)
cycle_table.style = 'Light Grid Accent 1'
hdr = cycle_table.rows[0].cells
for i, h in enumerate(['Estado de la OS', 'Estado OsRepuesto', 'Qué pasa con el inventario']):
    hdr[i].text = h
    for p in hdr[i].paragraphs:
        for run in p.runs:
            run.bold = True
            run.font.size = Pt(9)
for row_data in cycle_data:
    row = cycle_table.add_row().cells
    for i, val in enumerate(row_data):
        row[i].text = val
        for p in row[i].paragraphs:
            for run in p.runs:
                run.font.size = Pt(9)

doc.add_paragraph()

# 4.6 VentaMostrador
add_entity_table(doc, '4.6 VentaMostrador — Venta directa de repuestos', [
    ('VentaMostradorId', 'int, PK', 'Sí', 'Identificador único'),
    ('NumeroVenta', 'string(20), único', 'Sí', '"VTA-2026-0001"'),
    ('ClienteId', 'int?, FK', 'No', 'nullable: público general no registrado'),
    ('VehiculoId', 'int?, FK', 'No', 'nullable: no siempre saben para qué carro'),
    ('SucursalId', 'int, FK', 'Sí', 'En qué sucursal se vendió'),
    ('VendedorId', 'string, FK', 'Sí', 'ApplicationUser que vendió'),
    ('FechaVenta', 'DateTime', 'Sí', ''),
    ('Subtotal', 'decimal(10,2)', 'Sí', 'Suma de líneas'),
    ('Impuesto', 'decimal(10,2)', 'Sí', 'ISV'),
    ('Total', 'decimal(10,2)', 'Sí', 'Subtotal + ISV'),
    ('EstadoVenta', 'enum', 'Sí', 'Pendiente, Completada, Cancelada, Devuelta'),
    ('Observaciones', 'string(500)', 'No', ''),
],
'Propósito: Cuando un cliente compra repuestos directamente sin OS. ClienteId es nullable '
'porque puede ser venta a público general. Hereda de: SoftDeletableEntity.')

# 4.7 DetalleVenta
add_entity_table(doc, '4.7 DetalleVenta — Líneas de la venta directa', [
    ('DetalleVentaId', 'int, PK', 'Sí', 'Identificador único'),
    ('VentaMostradorId', 'int, FK', 'Sí', 'A qué venta pertenece'),
    ('ProductoId', 'int, FK', 'Sí', 'Qué producto se vendió'),
    ('Cantidad', 'int', 'Sí', ''),
    ('PrecioUnitario', 'decimal(10,2)', 'Sí', ''),
    ('Descuento', 'decimal(10,2)', 'Sí', ''),
    ('Subtotal', 'decimal(10,2)', 'Sí', '(Precio × Cantidad) - Descuento'),
],
'Propósito: Cada producto individual vendido en la venta de mostrador. '
'Sigue el mismo patrón que OsServicio es a OrdenServicio. '
'Hereda de: SoftDeletableEntity.')

doc.add_page_break()

# 4.8 Proveedor
add_entity_table(doc, '4.8 Proveedor — Catálogo de proveedores', [
    ('ProveedorId', 'int, PK', 'Sí', 'Identificador único'),
    ('Codigo', 'string(20), único', 'Sí', '"PROV-001"'),
    ('RazonSocial', 'string(200)', 'Sí', 'Nombre legal de la empresa'),
    ('NombreComercial', 'string(200)', 'No', 'Nombre con el que se le conoce'),
    ('TipoProveedor', 'enum', 'Sí', 'Local o Internacional'),
    ('Pais', 'string(100)', 'No', '"Honduras", "China", "Japón", "USA"'),
    ('Ciudad', 'string(100)', 'No', ''),
    ('Direccion', 'string(500)', 'No', ''),
    ('Telefono', 'string(20)', 'No', ''),
    ('Email', 'string(100)', 'No', ''),
    ('SitioWeb', 'string(200)', 'No', ''),
    ('ContactoPrincipal', 'string(200)', 'No', 'Nombre de la persona de contacto'),
    ('TelefonoContacto', 'string(20)', 'No', ''),
    ('RTN', 'string(20)', 'No', 'Solo proveedores locales (Honduras)'),
    ('DiasCredito', 'int', 'Sí', 'Plazo de pago: 0 = contado, 30, 60, 90 días'),
    ('MonedaPredeterminada', 'string(5)', 'Sí', '"HNL" o "USD"'),
    ('Observaciones', 'string(2000)', 'No', ''),
],
'Propósito: Registro de cada empresa a la que Premier le compra. Puede ser local '
'(Honduras) o internacional (China, Japón). Hereda de: SoftDeletableEntity. '
'Métodos: EsInternacional, NombreMostrar.')

# 4.9 OrdenCompra
add_entity_table(doc, '4.9 OrdenCompra — Pedido a proveedor', [
    ('OrdenCompraId', 'int, PK', 'Sí', 'Identificador único'),
    ('NumeroOrdenCompra', 'string(20), único', 'Sí', '"OC-2026-0001"'),
    ('ProveedorId', 'int, FK', 'Sí', 'A quién se le compra'),
    ('SucursalDestinoId', 'int, FK', 'Sí', 'Dónde se recibirá'),
    ('Estado', 'enum', 'Sí', 'Ver EstadoOrdenCompra'),
    ('FechaEmision', 'DateTime', 'Sí', 'Cuándo se creó'),
    ('FechaAprobacion', 'DateTime?', 'No', 'Cuándo la aprobaron internamente'),
    ('FechaEstimadaEntrega', 'DateTime?', 'No', 'Cuándo se espera recibir todo'),
    ('Moneda', 'string(5)', 'Sí', '"HNL" o "USD"'),
    ('TipoCambio', 'decimal(10,4)', 'No', 'Tipo de cambio USD→HNL al momento'),
    ('Subtotal', 'decimal(18,2)', 'Sí', 'Suma de líneas (costo productos)'),
    ('CostoFlete', 'decimal(10,2)', 'Sí', 'Transporte / shipping'),
    ('CostoSeguro', 'decimal(10,2)', 'Sí', 'Seguro de carga'),
    ('CostoAduana', 'decimal(10,2)', 'Sí', 'Aranceles, DAI, ISV importación'),
    ('OtrosCostos', 'decimal(10,2)', 'Sí', 'Handling, almacenaje, etc.'),
    ('TotalGeneral', 'decimal(18,2)', 'Sí', 'Subtotal + todos los costos'),
    ('───', '── Importación ──', '──', '── Solo para proveedor internacional ──'),
    ('NumeroContenedor', 'string(50)', 'No', '"MSKU1234567"'),
    ('NumeroBL', 'string(50)', 'No', 'Bill of Lading (conocimiento de embarque)'),
    ('NumeroFacturaProveedor', 'string(50)', 'No', 'Invoice # del proveedor'),
    ('PuertoOrigen', 'string(100)', 'No', '"Shanghai", "Shenzhen", "Yokohama"'),
    ('PuertoDestino', 'string(100)', 'No', '"Puerto Cortés"'),
    ('FechaEmbarque', 'DateTime?', 'No', 'Cuándo salió del país de origen'),
    ('FechaEstimadaArribo', 'DateTime?', 'No', 'ETA al puerto destino'),
    ('FechaLlegadaPuerto', 'DateTime?', 'No', 'Cuándo llegó físicamente'),
    ('FechaLiberacionAduana', 'DateTime?', 'No', 'Cuándo se liberó de aduanas'),
    ('───', '── Responsables ──', '──', '──'),
    ('SolicitadoPorId', 'string, FK', 'Sí', 'ApplicationUser que hizo el pedido'),
    ('AprobadoPorId', 'string?, FK', 'No', 'ApplicationUser que aprobó'),
    ('Observaciones', 'string(2000)', 'No', ''),
],
'Propósito: Documento principal de compra. Es a proveedores lo que OrdenServicio es '
'a clientes. Los campos de importación siguen el mismo patrón que Vehiculo '
'(NumeroImportacion, CostoImportacion). Hereda de: SoftDeletableEntity. '
'Métodos: EsImportacion, PuedeModificarse, CalcularTotales(), PorcentajeRecibido.')

doc.add_page_break()

# 4.10 OrdenCompraDetalle
add_entity_table(doc, '4.10 OrdenCompraDetalle — Líneas del pedido', [
    ('OrdenCompraDetalleId', 'int, PK', 'Sí', 'Identificador único'),
    ('OrdenCompraId', 'int, FK', 'Sí', 'A qué OC pertenece'),
    ('ProductoId', 'int, FK', 'Sí', 'Qué producto se pide'),
    ('CantidadPedida', 'int', 'Sí', 'Cuántas unidades se pidieron'),
    ('CantidadRecibida', 'int', 'Sí', 'Se actualiza con cada recepción (default 0)'),
    ('PrecioUnitario', 'decimal(10,2)', 'Sí', 'Precio de costo del proveedor'),
    ('Subtotal', 'decimal(10,2)', 'Sí', 'PrecioUnitario × CantidadPedida'),
    ('Observaciones', 'string(500)', 'No', ''),
],
'Propósito: Cada producto pedido en la OC. CantidadRecibida se incrementa con '
'cada RecepcionCompra para saber qué falta. '
'Hereda de: SoftDeletableEntity. '
'Métodos: CantidadPendiente (Pedida - Recibida), EstaCompleto (Recibida >= Pedida).')

# 4.11 RecepcionCompra
add_entity_table(doc, '4.11 RecepcionCompra — Recepción física de mercancía', [
    ('RecepcionCompraId', 'int, PK', 'Sí', 'Identificador único'),
    ('NumeroRecepcion', 'string(20), único', 'Sí', '"REC-2026-0001"'),
    ('OrdenCompraId', 'int, FK', 'Sí', 'De qué pedido viene'),
    ('SucursalId', 'int, FK', 'Sí', 'Dónde se recibe'),
    ('FechaRecepcion', 'DateTime', 'Sí', 'Cuándo llegó'),
    ('Estado', 'enum', 'Sí', 'Pendiente, Verificada, Completada, Rechazada'),
    ('RecibidoPorId', 'string, FK', 'Sí', 'ApplicationUser que recibió'),
    ('NumeroGuia', 'string(50)', 'No', 'Guía de transporte / tracking'),
    ('Observaciones', 'string(2000)', 'No', 'Novedades, daños, faltantes'),
],
'Propósito: Documento de recepción. Una OC puede tener MÚLTIPLES recepciones '
'(entregas parciales). Es el equivalente a Recepcion (check-in de vehículos) '
'pero para repuestos. Hereda de: SoftDeletableEntity.')

# 4.12 RecepcionCompraDetalle
add_entity_table(doc, '4.12 RecepcionCompraDetalle — Detalle de lo recibido', [
    ('RecepcionCompraDetalleId', 'int, PK', 'Sí', 'Identificador único'),
    ('RecepcionCompraId', 'int, FK', 'Sí', 'A qué recepción pertenece'),
    ('OrdenCompraDetalleId', 'int, FK', 'Sí', 'Qué línea del pedido original'),
    ('ProductoId', 'int, FK', 'Sí', 'Qué producto (redundante pero útil para queries)'),
    ('CantidadRecibida', 'int', 'Sí', 'Total que llegó físicamente'),
    ('CantidadDanada', 'int', 'Sí', 'Cuántas llegaron dañadas (default 0)'),
    ('CantidadAceptada', 'int', 'Sí', 'Recibida - Dañada (entran al inventario)'),
    ('Observaciones', 'string(500)', 'No', 'Detalle de daños si aplica'),
],
'Propósito: Permite control exacto de lo que llegó vs lo que se pidió, '
'incluyendo daños. Al completar, las CantidadAceptada se suman al inventario. '
'Hereda de: SoftDeletableEntity.')

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# 5. ENUMERACIONES
# ══════════════════════════════════════════════════════════════
doc.add_heading('5. Enumeraciones Nuevas', level=1)
doc.add_paragraph('Se agregan 8 enumeraciones nuevas al archivo EnumsDomain.cs existente:')

add_enum_table(doc, 'UnidadMedida', [
    ('Pieza', '1', 'Unidad individual'),
    ('Litro', '2', 'Para aceites, refrigerantes'),
    ('Galon', '3', 'Para aceites al por mayor'),
    ('Juego', '4', 'Kit completo (ej: juego de pastillas de freno)'),
    ('Metro', '5', 'Para mangueras, cables'),
    ('Kilogramo', '6', 'Para materiales a granel'),
    ('Otro', '99', ''),
])

add_enum_table(doc, 'TipoProveedor', [
    ('Local', '1', 'Proveedor nacional (Honduras)'),
    ('Internacional', '2', 'Proveedor extranjero (China, Japón, USA, etc.)'),
])

add_enum_table(doc, 'EstadoOrdenCompra', [
    ('Borrador', '1', 'Recién creada, se pueden editar líneas'),
    ('Aprobada', '2', 'Aprobada internamente, lista para enviar'),
    ('Enviada', '3', 'Enviada al proveedor'),
    ('Confirmada', '4', 'Proveedor confirmó que la puede surtir'),
    ('Embarcada', '5', 'En tránsito marítimo/aéreo (solo internacional)'),
    ('EnAduana', '6', 'En proceso de nacionalización (solo internacional)'),
    ('RecibidaParcial', '7', 'Se recibió parte del pedido'),
    ('RecibidaTotal', '8', 'Se recibió todo el pedido'),
    ('Cancelada', '9', 'Pedido cancelado'),
])

add_enum_table(doc, 'EstadoRecepcionCompra', [
    ('Pendiente', '1', 'Recepción creada, pendiente de verificar'),
    ('Verificada', '2', 'Mercancía contada y verificada'),
    ('Completada', '3', 'Aceptada — inventario incrementado'),
    ('Rechazada', '4', 'Rechazada — no entra al inventario'),
])

add_enum_table(doc, 'EstadoVenta', [
    ('Pendiente', '1', 'Venta en proceso'),
    ('Completada', '2', 'Pagada y entregada — stock descontado'),
    ('Cancelada', '3', 'Cancelada antes de completar'),
    ('Devuelta', '4', 'Cliente devolvió — stock devuelto'),
])

add_enum_table(doc, 'EstadoOsRepuesto', [
    ('Pendiente', '1', 'Identificado pero no reservado (stock intacto)'),
    ('Reservado', '2', 'Stock apartado (OS aprobada)'),
    ('Entregado', '3', 'Consumido físicamente (OS completada)'),
    ('Cancelado', '4', 'Cancelado — si estaba reservado, se devuelve'),
])

add_enum_table(doc, 'TipoMovimiento', [
    ('Compra', '1', '+ Entró por recepción de compra'),
    ('ConsumoServicio', '2', '- Salió por OS del taller'),
    ('VentaMostrador', '3', '- Salió por venta directa'),
    ('Reserva', '4', '± Stock apartado para OS aprobada'),
    ('DevolucionReserva', '5', '+ Se canceló la reserva de OS'),
    ('DevolucionVenta', '6', '+ Cliente devolvió producto comprado'),
    ('DevolucionServicio', '7', '+ Se devolvió repuesto de OS'),
    ('AjustePositivo', '8', '+ Inventario físico encontró más'),
    ('AjusteNegativo', '9', '- Inventario físico encontró menos'),
    ('TransferenciaOut', '10', '- Salió hacia otra sucursal'),
    ('TransferenciaIn', '11', '+ Entró desde otra sucursal'),
])

add_enum_table(doc, 'OrigenDocumento', [
    ('OrdenServicio', '1', 'Movimiento originado por una OS'),
    ('VentaMostrador', '2', 'Movimiento originado por una venta directa'),
    ('OrdenCompra', '3', 'Movimiento originado por una compra'),
    ('AjusteManual', '4', 'Ajuste de inventario manual'),
    ('Transferencia', '5', 'Transferencia entre sucursales'),
])

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# 6. INTEGRACIÓN
# ══════════════════════════════════════════════════════════════
doc.add_heading('6. Integración con Entidades Existentes', level=1)

doc.add_paragraph(
    'Las siguientes entidades YA EXISTENTES requieren modificaciones menores '
    'para conectarse con el nuevo módulo de repuestos:'
)

doc.add_heading('6.1 OrdenServicio (Modificar)', level=2)
doc.add_paragraph('Archivo: PremierFlow.Domain/Entities/OrdenServicio.cs')
doc.add_paragraph(
    'Se agrega la colección de repuestos (paralela a la de servicios) y se actualiza '
    'el método CalcularTotales() para que TotalRepuestos se calcule desde datos reales:'
)

p = doc.add_paragraph()
run = p.add_run(
    '// AGREGAR navegación:\n'
    'public virtual ICollection<OsRepuesto> Repuestos { get; set; } \n'
    '    = new List<OsRepuesto>();\n'
    '\n'
    '// ACTUALIZAR CalcularTotales():\n'
    'public void CalcularTotales()\n'
    '{\n'
    '    TotalManoObra = Servicios\n'
    '        .Where(s => s.Estado != EstadoServicioOS.Cancelado)\n'
    '        .Sum(s => s.Subtotal);\n'
    '\n'
    '    TotalRepuestos = Repuestos                    // ← NUEVO\n'
    '        .Where(r => r.Estado != EstadoOsRepuesto.Cancelado)\n'
    '        .Sum(r => r.Subtotal);\n'
    '\n'
    '    TotalGeneral = TotalManoObra + TotalRepuestos;\n'
    '}\n'
)
run.font.name = 'Consolas'
run.font.size = Pt(9)

doc.add_heading('6.2 Cliente (Modificar)', level=2)
doc.add_paragraph('Archivo: PremierFlow.Domain/Entities/Cliente.cs')
doc.add_paragraph('Se agrega la navegación a ventas de mostrador:')
p = doc.add_paragraph()
run = p.add_run(
    '// AGREGAR navegación:\n'
    'public virtual ICollection<VentaMostrador> VentasMostrador { get; set; } \n'
    '    = new List<VentaMostrador>();\n'
)
run.font.name = 'Consolas'
run.font.size = Pt(9)

doc.add_heading('6.3 Sucursal (Modificar)', level=2)
doc.add_paragraph('Archivo: PremierFlow.Domain/Entities/Sucursal.cs')
doc.add_paragraph('Se agrega la navegación a inventarios:')
p = doc.add_paragraph()
run = p.add_run(
    '// AGREGAR navegación:\n'
    'public virtual ICollection<InventarioSucursal> Inventarios { get; set; } \n'
    '    = new List<InventarioSucursal>();\n'
)
run.font.name = 'Consolas'
run.font.size = Pt(9)

doc.add_heading('6.4 EnumsDomain.cs (Modificar)', level=2)
doc.add_paragraph('Archivo: PremierFlow.Domain/Enums/EnumsDomain.cs')
doc.add_paragraph('Se agregan los 8 enums detallados en la sección 5.')

doc.add_heading('6.5 PremierFlowDbContext.cs (Modificar)', level=2)
doc.add_paragraph('Archivo: PremierFlow.Infrastructure/Persistence/PremierFlowDbContext.cs')
doc.add_paragraph('Se agregan 12 DbSets nuevos:')
p = doc.add_paragraph()
run = p.add_run(
    'public DbSet<CategoriaProducto> CategoriasProducto { get; set; }\n'
    'public DbSet<Producto> Productos { get; set; }\n'
    'public DbSet<InventarioSucursal> InventarioSucursal { get; set; }\n'
    'public DbSet<MovimientoInventario> MovimientosInventario { get; set; }\n'
    'public DbSet<OsRepuesto> OsRepuestos { get; set; }\n'
    'public DbSet<VentaMostrador> VentasMostrador { get; set; }\n'
    'public DbSet<DetalleVenta> DetallesVenta { get; set; }\n'
    'public DbSet<Proveedor> Proveedores { get; set; }\n'
    'public DbSet<OrdenCompra> OrdenesCompra { get; set; }\n'
    'public DbSet<OrdenCompraDetalle> OrdenesCompraDetalle { get; set; }\n'
    'public DbSet<RecepcionCompra> RecepcionesCompra { get; set; }\n'
    'public DbSet<RecepcionCompraDetalle> RecepcionesCompraDetalle { get; set; }\n'
)
run.font.name = 'Consolas'
run.font.size = Pt(9)

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# 7. FLUJOS DE NEGOCIO
# ══════════════════════════════════════════════════════════════
doc.add_heading('7. Flujos de Negocio', level=1)

# 7.1 Compra internacional
doc.add_heading('7.1 Flujo de compra internacional (ej: China)', level=2)

steps = [
    ('1. Crear pedido',
     'Se crea OrdenCompra "OC-2026-0042" al Proveedor "AutoParts Shanghai". '
     'Se agregan líneas: 200 filtros de aceite a $3.50 USD c/u. Estado: Borrador.'),
    ('2. Aprobación',
     'Gerente de compras revisa y aprueba. Estado: Aprobada → Enviada al proveedor.'),
    ('3. Confirmación',
     'Proveedor confirma disponibilidad y precio. Estado: Confirmada.'),
    ('4. Embarque',
     'Sale contenedor MSKU1234567 de Shanghai. Se registra NumeroBL, FechaEmbarque. '
     'Estado: Embarcada.'),
    ('5. Aduana',
     'Contenedor llega a Puerto Cortés. Se registra FechaLlegadaPuerto, CostoAduana. '
     'Estado: EnAduana.'),
    ('6. Liberación',
     'Aduanas libera la mercancía. Se registra FechaLiberacionAduana.'),
    ('7. Recepción',
     'Se crea RecepcionCompra "REC-2026-0078" en sucursal SPS. '
     'Se reciben 200 unidades, 5 dañadas → 195 aceptadas. '
     'InventarioSucursal SPS: CantidadDisponible += 195. '
     'MovimientoInventario: tipo Compra, cantidad +195. '
     'OrdenCompraDetalle: CantidadRecibida = 195. '
     'OrdenCompra Estado: RecibidaParcial (faltan 5).'),
]
for title, desc in steps:
    p = doc.add_paragraph()
    run = p.add_run(title + ': ')
    run.bold = True
    run.font.size = Pt(10)
    run = p.add_run(desc)
    run.font.size = Pt(10)

doc.add_paragraph()

# 7.2 Compra local
doc.add_heading('7.2 Flujo de compra local', level=2)
doc.add_paragraph(
    'Igual que el internacional pero sin los pasos de embarque/aduana. '
    'La OC va directo de Confirmada a RecibidaParcial/Total.'
)

p = doc.add_paragraph()
run = p.add_run('Borrador → Aprobada → Enviada → Confirmada → RecibidaParcial/Total')
run.bold = True

doc.add_paragraph()

# 7.3 Consumo en servicio
doc.add_heading('7.3 Flujo de consumo en servicio (taller)', level=2)

steps_svc = [
    ('1. Diagnóstico',
     'Técnico diagnostica Honda CRV: necesita 1 filtro aceite + 4L aceite sintético.'),
    ('2. Cotización',
     'Se crean OsRepuesto: Filtro $290 + Aceite 4L $1,200 = TotalRepuestos $1,490. '
     'Estado: Pendiente. Inventario intacto.'),
    ('3. Aprobación cliente',
     'Cliente aprueba cotización. OS pasa a APROBADA. '
     'OsRepuesto → Reservado. Stock: Disponible -= N, Reservada += N.'),
    ('4. Ejecución',
     'Técnico instala los repuestos. OS pasa a COMPLETADA. '
     'OsRepuesto → Entregado. Stock: Reservada -= N. '
     'MovimientoInventario: tipo ConsumoServicio.'),
    ('5. Facturación',
     'OrdenServicio: TotalManoObra + TotalRepuestos = TotalGeneral. '
     'Se factura al cliente.'),
]
for title, desc in steps_svc:
    p = doc.add_paragraph()
    run = p.add_run(title + ': ')
    run.bold = True
    run.font.size = Pt(10)
    run = p.add_run(desc)
    run.font.size = Pt(10)

doc.add_paragraph()

# 7.4 Venta mostrador
doc.add_heading('7.4 Flujo de venta en mostrador', level=2)

steps_sale = [
    ('1. Cliente solicita',
     'Cliente pide 2 filtros de aceite en mostrador.'),
    ('2. Verificar stock',
     'Se consulta InventarioSucursal: ¿hay suficiente en esta sucursal?'),
    ('3. Crear venta',
     'VentaMostrador "VTA-2026-0200" con DetalleVenta: 2 filtros @ $290 c/u.'),
    ('4. Completar',
     'Se cobra y entrega. EstadoVenta → Completada. '
     'InventarioSucursal: Disponible -= 2. '
     'MovimientoInventario: tipo VentaMostrador, cantidad -2.'),
]
for title, desc in steps_sale:
    p = doc.add_paragraph()
    run = p.add_run(title + ': ')
    run.bold = True
    run.font.size = Pt(10)
    run = p.add_run(desc)
    run.font.size = Pt(10)

doc.add_page_break()

# ══════════════════════════════════════════════════════════════
# 8. RESUMEN DE ARCHIVOS
# ══════════════════════════════════════════════════════════════
doc.add_heading('8. Resumen de Archivos a Crear y Modificar', level=1)

doc.add_heading('Archivos nuevos (24)', level=2)

# Entities
doc.add_paragraph('Domain/Entities/ (12 archivos):', style='List Bullet')
new_entities = [
    'CategoriaProducto.cs', 'Producto.cs',
    'InventarioSucursal.cs', 'MovimientoInventario.cs',
    'OsRepuesto.cs',
    'VentaMostrador.cs', 'DetalleVenta.cs',
    'Proveedor.cs', 'OrdenCompra.cs', 'OrdenCompraDetalle.cs',
    'RecepcionCompra.cs', 'RecepcionCompraDetalle.cs',
]
for entity in new_entities:
    doc.add_paragraph(entity, style='List Bullet 2')

doc.add_paragraph('Infrastructure/Configurations/ (12 archivos):', style='List Bullet')
for entity in new_entities:
    name = entity.replace('.cs', 'Configuration.cs')
    doc.add_paragraph(name, style='List Bullet 2')

doc.add_heading('Archivos a modificar (4)', level=2)

modifications = [
    ('Domain/Entities/OrdenServicio.cs', 'Agregar ICollection<OsRepuesto> y actualizar CalcularTotales()'),
    ('Domain/Entities/Cliente.cs', 'Agregar ICollection<VentaMostrador>'),
    ('Domain/Entities/Sucursal.cs', 'Agregar ICollection<InventarioSucursal>'),
    ('Domain/Enums/EnumsDomain.cs', 'Agregar 8 enums nuevos'),
    ('Infrastructure/Persistence/PremierFlowDbContext.cs', 'Agregar 12 DbSets'),
]
for path, desc in modifications:
    p = doc.add_paragraph(style='List Bullet')
    run = p.add_run(path)
    run.bold = True
    run.font.size = Pt(10)
    p.add_run(f' — {desc}').font.size = Pt(10)

doc.add_paragraph()

# Summary table
doc.add_heading('Resumen de entidades nuevas', level=2)

summary_table = doc.add_table(rows=1, cols=4)
summary_table.style = 'Light Grid Accent 1'
hdr = summary_table.rows[0].cells
for i, h in enumerate(['#', 'Entidad', 'Grupo', 'Propósito']):
    hdr[i].text = h
    for p in hdr[i].paragraphs:
        for run in p.runs:
            run.bold = True
            run.font.size = Pt(9)

summary_data = [
    ('1', 'CategoriaProducto', 'Catálogo', 'Clasificación jerárquica'),
    ('2', 'Producto', 'Catálogo', 'Registro maestro del repuesto'),
    ('3', 'InventarioSucursal', 'Almacén', 'Stock por sucursal'),
    ('4', 'MovimientoInventario', 'Almacén', 'Auditoría de movimientos'),
    ('5', 'OsRepuesto', 'Canal Servicio', 'Repuesto en OS de taller'),
    ('6', 'VentaMostrador', 'Canal Venta', 'Venta directa cabecera'),
    ('7', 'DetalleVenta', 'Canal Venta', 'Líneas de venta directa'),
    ('8', 'Proveedor', 'Canal Compra', 'Catálogo de proveedores'),
    ('9', 'OrdenCompra', 'Canal Compra', 'Pedido a proveedor'),
    ('10', 'OrdenCompraDetalle', 'Canal Compra', 'Líneas del pedido'),
    ('11', 'RecepcionCompra', 'Canal Compra', 'Recepción de mercancía'),
    ('12', 'RecepcionCompraDetalle', 'Canal Compra', 'Detalle de lo recibido'),
]
for row_data in summary_data:
    row = summary_table.add_row().cells
    for i, val in enumerate(row_data):
        row[i].text = val
        for p in row[i].paragraphs:
            for run in p.runs:
                run.font.size = Pt(9)

# ── Guardar ──────────────────────────────────────────────────
output_path = os.path.join(os.path.dirname(__file__),
    'Diseno_Modulo_Repuestos_Inventario_Compras.docx')
doc.save(output_path)
print(f'Documento generado: {output_path}')
