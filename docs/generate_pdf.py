#!/usr/bin/env python3
"""
Genera el PDF: Diseño del Módulo de Repuestos, Inventario y Compras
"""

from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.lib.colors import HexColor, white
from reportlab.lib.enums import TA_CENTER, TA_JUSTIFY
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle,
    PageBreak, Preformatted
)
import os

output_path = os.path.join(os.path.dirname(os.path.abspath(__file__)),
    'Diseno_Modulo_Repuestos_Inventario_Compras.pdf')

doc = SimpleDocTemplate(
    output_path, pagesize=letter,
    rightMargin=0.75*inch, leftMargin=0.75*inch,
    topMargin=0.75*inch, bottomMargin=0.75*inch,
)

BLUE_DARK = HexColor('#1B3A5C')
BLUE_LIGHT = HexColor('#E8EFF7')
GRAY_LIGHT = HexColor('#F5F5F5')
GRAY_MED = HexColor('#CCCCCC')
GRAY_TEXT = HexColor('#666666')

styles = getSampleStyleSheet()
styles.add(ParagraphStyle('CoverTitle', parent=styles['Title'], fontSize=36, textColor=BLUE_DARK, spaceAfter=10, alignment=TA_CENTER, fontName='Helvetica-Bold'))
styles.add(ParagraphStyle('CoverSubtitle', parent=styles['Title'], fontSize=20, textColor=BLUE_DARK, spaceAfter=6, alignment=TA_CENTER, fontName='Helvetica'))
styles.add(ParagraphStyle('CoverMeta', parent=styles['Normal'], fontSize=12, textColor=GRAY_TEXT, alignment=TA_CENTER))
styles.add(ParagraphStyle('H1', parent=styles['Heading1'], fontSize=20, textColor=BLUE_DARK, spaceBefore=20, spaceAfter=12, fontName='Helvetica-Bold'))
styles.add(ParagraphStyle('H2', parent=styles['Heading2'], fontSize=15, textColor=BLUE_DARK, spaceBefore=16, spaceAfter=8, fontName='Helvetica-Bold'))
styles.add(ParagraphStyle('H3', parent=styles['Heading3'], fontSize=12, textColor=BLUE_DARK, spaceBefore=12, spaceAfter=6, fontName='Helvetica-Bold'))
styles.add(ParagraphStyle('Body', parent=styles['Normal'], fontSize=10, spaceAfter=6, alignment=TA_JUSTIFY, leading=14, fontName='Helvetica'))
styles.add(ParagraphStyle('BulletCustom', parent=styles['Normal'], fontSize=10, spaceAfter=3, leftIndent=20, leading=14, bulletIndent=10, fontName='Helvetica'))
styles.add(ParagraphStyle('CodeCustom', parent=styles['Normal'], fontSize=7.5, fontName='Courier', leftIndent=10, spaceAfter=8, spaceBefore=4, leading=9.5, backColor=GRAY_LIGHT, borderPadding=6))
styles.add(ParagraphStyle('Note', parent=styles['Normal'], fontSize=9, fontName='Helvetica-Oblique', textColor=GRAY_TEXT, spaceAfter=6, leading=12))
styles.add(ParagraphStyle('TC', parent=styles['Normal'], fontSize=8, fontName='Helvetica', leading=10))
styles.add(ParagraphStyle('TH', parent=styles['Normal'], fontSize=8, fontName='Helvetica-Bold', textColor=white, leading=10))


def tbl(headers, rows, cw=None):
    data = [[Paragraph(h, styles['TH']) for h in headers]]
    for r in rows:
        data.append([Paragraph(str(c), styles['TC']) for c in r])
    t = Table(data, colWidths=cw, repeatRows=1)
    t.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), BLUE_DARK), ('TEXTCOLOR', (0,0), (-1,0), white),
        ('ROWBACKGROUNDS', (0,1), (-1,-1), [white, GRAY_LIGHT]),
        ('FONTNAME', (0,0), (-1,-1), 'Helvetica'), ('FONTSIZE', (0,0), (-1,-1), 8),
        ('TOPPADDING', (0,0), (-1,-1), 3), ('BOTTOMPADDING', (0,0), (-1,-1), 3),
        ('GRID', (0,0), (-1,-1), 0.5, GRAY_MED), ('VALIGN', (0,0), (-1,-1), 'TOP'),
    ]))
    return t


def enum_section(name, values, story):
    story.append(Paragraph(name, styles['H3']))
    story.append(tbl(['Valor','ID','Descripción'], values, [1.5*inch, 0.5*inch, 4.5*inch]))
    story.append(Spacer(1, 12))


def entity(title, desc, fields, story):
    story.append(Paragraph(title, styles['H2']))
    story.append(Paragraph(desc, styles['Note']))
    story.append(tbl(['Campo','Tipo','Req.','Descripción'], fields, [1.7*inch, 1.2*inch, 0.5*inch, 3.1*inch]))
    story.append(Spacer(1, 12))


S = []

# COVER
S.append(Spacer(1, 2.5*inch))
S.append(Paragraph('PremierFlow', styles['CoverTitle']))
S.append(Spacer(1, 0.3*inch))
S.append(Paragraph('Diseño del Módulo de<br/>Repuestos, Inventario y Compras', styles['CoverSubtitle']))
S.append(Spacer(1, 0.5*inch))
S.append(Paragraph('Documento de Arquitectura de Entidades', styles['CoverMeta']))
S.append(Spacer(1, 2*inch))
S.append(Paragraph('Versión 1.0  •  Marzo 2026', styles['CoverMeta']))
S.append(PageBreak())

# TOC
S.append(Paragraph('Tabla de Contenido', styles['H1']))
for item in [
    '1. Contexto y Situación Actual',
    '2. Principio Fundamental: Un Repuesto, Tres Canales',
    '3. Diagrama UML de Clases',
    '4. Detalle de Entidades Nuevas',
    '    4.1 CategoriaProducto    4.7 DetalleVenta',
    '    4.2 Producto              4.8 Proveedor',
    '    4.3 InventarioSucursal    4.9 OrdenCompra',
    '    4.4 MovimientoInventario  4.10 OrdenCompraDetalle',
    '    4.5 OsRepuesto            4.11 RecepcionCompra',
    '    4.6 VentaMostrador        4.12 RecepcionCompraDetalle',
    '5. Enumeraciones Nuevas (8 enums)',
    '6. Integración con Entidades Existentes',
    '7. Flujos de Negocio',
    '8. Resumen de Archivos a Crear/Modificar',
]:
    S.append(Paragraph(item, styles['Body']))
S.append(PageBreak())

# 1. CONTEXTO
S.append(Paragraph('1. Contexto y Situación Actual', styles['H1']))
S.append(Paragraph(
    'PremierFlow es un DMS automotriz (ASP.NET Core 10.0, EF Core, SQL Server). '
    'Gestiona taller (OrdenServicio, OsServicio, técnicos, recepción) y venta de vehículos '
    '(con tracking de importación). <b>NO tiene módulo de repuestos.</b> El campo '
    '<font face="Courier" size="9">TotalRepuestos</font> en OrdenServicio siempre vale 0, con comentario: '
    '<i>"TotalRepuestos se calcularía desde otra tabla de repuestos"</i>.', styles['Body']))

S.append(Paragraph('Entidades existentes relevantes', styles['H2']))
S.append(tbl(['Entidad', 'Propósito', 'Relación con repuestos'], [
    ('OrdenServicio', 'Documento principal taller', 'Se le agregan líneas de repuestos (OsRepuesto)'),
    ('OsServicio', 'Línea de mano de obra', 'OsRepuesto será su gemelo para repuestos'),
    ('Cliente', 'Registro de clientes', 'Puede comprar repuestos en mostrador'),
    ('Sucursal', 'Sedes (SPS, TGU)', 'Cada sucursal tiene su propio inventario'),
    ('Vehiculo', 'Vehículos individuales', 'Ya tiene campos de importación (patrón a seguir)'),
    ('Tecnico', 'Técnicos del taller', 'Puede solicitar repuestos para una OS'),
    ('Ubicacion', 'Ubicaciones físicas', 'Tipos: Bodega, DepositoFiscal, Transito'),
], [1.2*inch, 1.6*inch, 3.7*inch]))
S.append(PageBreak())

# 2. PRINCIPIO FUNDAMENTAL
S.append(Paragraph('2. Principio Fundamental: Un Repuesto, Tres Canales', styles['H1']))
S.append(Paragraph(
    'En un DMS el repuesto es <b>UNO SOLO</b>. No existen "repuestos de servicio" y "repuestos de venta". '
    'Es el mismo producto del mismo inventario. Lo que cambia es el <b>CANAL</b>:', styles['Body']))
S.append(Paragraph('• <b>Canal Servicio (Taller):</b> Técnico consume repuesto en una OS.', styles['BulletCustom']))
S.append(Paragraph('• <b>Canal Venta (Mostrador):</b> Cliente compra directo sin OS.', styles['BulletCustom']))
S.append(Paragraph('• <b>Canal Compra (Proveedores):</b> Repuesto entra al inventario desde proveedores.', styles['BulletCustom']))
S.append(Spacer(1, 8))
S.append(Paragraph('<b>Los tres canales comparten el mismo Producto y el mismo InventarioSucursal. Todo movimiento se audita en MovimientoInventario.</b>', styles['Body']))
S.append(Spacer(1, 8))

diagram = (
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
    '                        └────────────────┘'
)
S.append(Preformatted(diagram, styles['CodeCustom']))
S.append(PageBreak())

# 3. UML
S.append(Paragraph('3. Diagrama UML de Clases', styles['H1']))
S.append(Paragraph('Las 12 entidades nuevas y su relación con las existentes:', styles['Body']))

uml = (
    '══════════════════════════════════════════════════════════════════════\n'
    '                    DIAGRAMA UML - PREMIERFLOW\n'
    '              Módulo de Repuestos, Inventario y Compras\n'
    '══════════════════════════════════════════════════════════════════════\n'
    '\n'
    '┌─────────────────────────────────────────────────────────────────┐\n'
    '│ ENTIDADES EXISTENTES                                           │\n'
    '│                                                                │\n'
    '│ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐          │\n'
    '│ │ Sucursal │ │ Cliente  │ │ Tecnico  │ │ Vehiculo │          │\n'
    '│ │──────────│ │──────────│ │──────────│ │──────────│          │\n'
    '│ │Id     PK │ │ClienteId │ │TecnicoId │ │VehiculoId│          │\n'
    '│ │Nombre    │ │Nombre    │ │Nombre    │ │Vin       │          │\n'
    '│ │Codigo    │ │Telefono  │ │Especial. │ │Placa     │          │\n'
    '│ └────┬─────┘ └────┬─────┘ └────┬─────┘ └──────────┘          │\n'
    '│      │            │            │                               │\n'
    '│      │   ┌──────────────────────────────────────────────┐     │\n'
    '│      │   │       OrdenServicio (EXISTENTE)              │     │\n'
    '│      │   │──────────────────────────────────────────────│     │\n'
    '│      │   │ OsId PK    ClienteId FK   VehiculoId FK     │     │\n'
    '│      │   │ TotalManoObra  (suma OsServicio)             │     │\n'
    '│      │   │ TotalRepuestos ◄── AHORA SE CALCULA REAL    │     │\n'
    '│      │   │ TotalGeneral = ManoObra + Repuestos          │     │\n'
    '│      │   │──────────────────────────────────────────────│     │\n'
    '│      │   │ Servicios: OsServicio[]  (YA EXISTE)         │     │\n'
    '│      │   │ Repuestos: OsRepuesto[]  ★ NUEVO             │     │\n'
    '│      │   └────────────────┬─────────────────────────────┘     │\n'
    '└──────┼────────────────────┼────────────────────────────────────┘\n'
    '       │                    │\n'
    '═══════╪════════════════════╪════════════════════════════════════\n'
    '       │  ENTIDADES NUEVAS  │\n'
    '═══════╪════════════════════╪════════════════════════════════════\n'
    '       │                    │\n'
    '       │ ┌──────────────┐   │   ┌──────────────────┐\n'
    '       │ │Categoria     │   │   │    Producto       │\n'
    '       │ │Producto      │   │   │──────────────────│\n'
    '       │ │──────────────│   │   │ ProductoId    PK  │\n'
    '       │ │CategoriaId PK│◄──┤   │ Codigo (SKU)      │\n'
    '       │ │Nombre     1:N│   │   │ NumeroParteOEM    │\n'
    '       │ │CategPadreIdFK│   │   │ CategoriaIdFK     │\n'
    '       │ └──────────────┘   │   │ PrecioCosto       │\n'
    '       │                    │   │ PrecioVenta       │\n'
    '       │                    │   │ UnidadMedida      │\n'
    '       │                    │   └────────┬──────────┘\n'
    '       │                    │            │\n'
    '       │                    │   ┌────────┼──────────────┐\n'
    '       │                    │   │        │              │\n'
    '       ▼                    │   ▼        ▼              ▼\n'
    '┌────────────────┐          │ ┌──────────────┐ ┌──────────────┐\n'
    '│Inventario      │          │ │ OsRepuesto   │ │DetalleVenta  │\n'
    '│Sucursal        │          │ │──────────────│ │──────────────│\n'
    '│────────────────│          │ │OsRepuestoIdPK│ │DetalleVtaIdPK│\n'
    '│InvSucursalIdPK │          │ │OsId       FK │ │VtaMostrIdFK  │\n'
    '│ProductoId   FK │          │ │ProductoId FK │ │ProductoId FK │\n'
    '│SucursalId   FK │          │ │Cantidad      │ │Cantidad      │\n'
    '│CantDisponible  │          │ │PrecioUnit    │ │PrecioUnit    │\n'
    '│CantReservada   │          │ │Subtotal      │ │Subtotal      │\n'
    '│StockMinimo     │          │ │Estado (enum) │ └──────┬───────┘\n'
    '│StockMaximo     │          │ └──────────────┘        │\n'
    '└────────────────┘          │                ┌────────▼───────┐\n'
    ' UNIQUE(Prod,Suc)           │                │VentaMostrador  │\n'
    '                            │                │────────────────│\n'
    '┌────────────────┐          │                │VtaMostradorIdPK│\n'
    '│Movimiento      │          │                │NumeroVenta     │\n'
    '│Inventario      │          │                │ClienteId  FK?  │\n'
    '│────────────────│          │                │SucursalId FK   │\n'
    '│MovimientoIdPK  │          │                │VendedorId FK   │\n'
    '│ProductoId   FK │          │                │Total           │\n'
    '│SucursalId   FK │          │                │EstadoVenta     │\n'
    '│TipoMovimiento  │          │                └────────────────┘\n'
    '│Cantidad (+/-)  │          │\n'
    '│CantAnterior    │          │  CANAL COMPRA (ENTRADA)\n'
    '│CantDespues     │          │  ════════════════════════\n'
    '│OrigenDocumento │          │\n'
    '│ReferenciaId    │          │ ┌──────────────┐\n'
    '└────────────────┘          │ │ Proveedor    │\n'
    '                            │ │──────────────│\n'
    '(Audita TODO movimiento:    │ │ProveedorIdPK │\n'
    ' compra, consumo, venta,    │ │RazonSocial   │\n'
    ' reserva, ajuste,           │ │TipoProveedor │\n'
    ' transferencia)             │ │DiasCredito   │\n'
    '                            │ └──────┬───────┘\n'
    '                            │        │ 1:N\n'
    '                            │        ▼\n'
    '                            │ ┌──────────────┐\n'
    '                            │ │ OrdenCompra  │\n'
    '                            │ │──────────────│\n'
    '                            │ │OrdCompraIdPK │\n'
    '                            │ │NumeroOC      │\n'
    '                            │ │ProveedorIdFK │\n'
    '                            │ │SucDestIdFK   │\n'
    '                            │ │Estado (enum) │\n'
    '                            │ │Subtotal      │\n'
    '                            │ │CostoFlete    │\n'
    '                            │ │CostoAduana   │\n'
    '                            │ │TotalGeneral  │\n'
    '                            │ │─ Importación─│\n'
    '                            │ │NumContenedor │\n'
    '                            │ │NumeroBL      │\n'
    '                            │ │PuertoOrigen  │\n'
    '                            │ │FechaEmbarque │\n'
    '                            │ └──┬────────┬──┘\n'
    '                            │    │1:N     │1:N\n'
    '                            │    ▼        ▼\n'
    '                            │ ┌────────┐┌────────────┐\n'
    '                            │ │OCDetall││RecepcionCom│\n'
    '                            │ │────────││────────────│\n'
    '                            │ │OCDetIdP││RecCompIdPK │\n'
    '                            │ │OCId  FK││OCId     FK │\n'
    '                            │ │ProdIdFK││SucursalIdFK│\n'
    '                            │ │CantPed ││FechaRecep  │\n'
    '                            │ │CantRec ││Estado(enum)│\n'
    '                            │ │PrecioU ││RecibidoPorF│\n'
    '                            │ │Subtotal│└─────┬──────┘\n'
    '                            │ └────────┘      │ 1:N\n'
    '                            │     ▲           ▼\n'
    '                            │     │  ┌────────────┐\n'
    '                            │     │  │RecepCompDet│\n'
    '                            │     │  │────────────│\n'
    '                            │     │  │RecCmpDetIdP│\n'
    '                            │     └──│OCDetIdFK   │\n'
    '                            │ (ref.) │ProductoIdFK│\n'
    '                            │        │CantRecibida│\n'
    '                            │        │CantDanada  │\n'
    '                            │        │CantAceptada│\n'
    '                            │        └────────────┘\n'
    '══════════════════════════════════════════════════════════════════════\n'
)
S.append(Preformatted(uml, styles['CodeCustom']))

S.append(Paragraph('Cardinalidades', styles['H2']))
S.append(tbl(['Desde','Hacia','Cardinalidad'], [
    ('CategoriaProducto','CategoriaProducto (padre)','N:1 (auto-ref)'),
    ('CategoriaProducto','Producto','1:N'),
    ('Producto','InventarioSucursal','1:N (uno por sucursal)'),
    ('Producto','MovimientoInventario','1:N'),
    ('Producto','OsRepuesto','1:N'),
    ('Producto','DetalleVenta','1:N'),
    ('Producto','OrdenCompraDetalle','1:N'),
    ('Sucursal (existente)','InventarioSucursal','1:N'),
    ('OrdenServicio (existente)','OsRepuesto','1:N'),
    ('Tecnico (existente)','OsRepuesto','1:N (opcional)'),
    ('Cliente (existente)','VentaMostrador','1:N (opcional)'),
    ('VentaMostrador','DetalleVenta','1:N'),
    ('Proveedor','OrdenCompra','1:N'),
    ('OrdenCompra','OrdenCompraDetalle','1:N'),
    ('OrdenCompra','RecepcionCompra','1:N (parciales)'),
    ('RecepcionCompra','RecepcionCompraDetalle','1:N'),
    ('OrdenCompraDetalle','RecepcionCompraDetalle','1:N (ref)'),
], [2.2*inch, 2.2*inch, 2.1*inch]))
S.append(PageBreak())

# 4. ENTIDADES
S.append(Paragraph('4. Detalle de Entidades Nuevas', styles['H1']))

entity('4.1 CategoriaProducto — Clasificación de repuestos',
    '<b>Propósito:</b> Árbol jerárquico (ej: Filtros → Filtro Aceite, Filtro Aire). Auto-referencia para subcategorías. <b>Hereda de:</b> SoftDeletableEntity.', [
    ('CategoriaProductoId','int, PK','Sí','Identificador único'),
    ('Nombre','string(100)','Sí','"Filtros", "Aceites", "Frenos"'),
    ('Descripcion','string(500)','No','Descripción opcional'),
    ('CategoriaPadreId','int?, FK','No','FK a sí misma. null = raíz'),
], S)

entity('4.2 Producto — Catálogo maestro de repuestos',
    '<b>Propósito:</b> Registro maestro. Centro de todo — todos apuntan aquí. Un solo registro sin importar canal. <b>Hereda de:</b> SoftDeletableEntity. <b>Métodos:</b> MargenGanancia, TieneMargenSaludable.', [
    ('ProductoId','int, PK','Sí','Identificador único'),
    ('Codigo','string(50), único','Sí','SKU interno: "FLT-ACE-001"'),
    ('NumeroParteOEM','string(50)','No','Número fabricante: "15400-RTA-003"'),
    ('CodigoBarras','string(50)','No','Para escaneo rápido'),
    ('Nombre','string(200)','Sí','"Filtro de Aceite Honda Civic"'),
    ('Descripcion','string(2000)','No','Detalle extendido'),
    ('CategoriaProductoId','int, FK','Sí','A qué categoría pertenece'),
    ('MarcaRepuesto','string(100)','No','"Mann Filter", "Denso", "NGK"'),
    ('UnidadMedida','enum','Sí','Pieza, Litro, Galón, Juego, Metro, Kg'),
    ('PrecioCosto','decimal(10,2)','Sí','Lo que paga Premier al proveedor'),
    ('PrecioVenta','decimal(10,2)','Sí','Precio de lista al público'),
    ('EsOriginal','bool','Sí','true=OEM, false=aftermarket'),
    ('Peso','decimal(8,3)','No','En kg (para flete importación)'),
    ('ImagenUrl','string(500)','No','URL de foto'),
], S)

entity('4.3 InventarioSucursal — Stock real por sucursal',
    '<b>Propósito:</b> Stock por sucursal. UNIQUE(ProductoId, SucursalId). <b>Regla:</b> Stock Físico = Disponible + Reservada. <b>Hereda de:</b> AuditableEntity. <b>Métodos:</b> NecesitaReorden, Reservar, Consumir, Recibir.', [
    ('InventarioSucursalId','int, PK','Sí','Identificador único'),
    ('ProductoId','int, FK','Sí','Qué producto'),
    ('SucursalId','int, FK','Sí','En qué sucursal'),
    ('CantidadDisponible','int','Sí','Stock libre para usar/vender'),
    ('CantidadReservada','int','Sí','Apartado para OS aprobadas'),
    ('StockMinimo','int','Sí','Punto de reorden (alerta)'),
    ('StockMaximo','int','Sí','Capacidad máxima'),
    ('UbicacionAlmacen','string(50)','No','"Estante A3-B2"'),
], S)

entity('4.4 MovimientoInventario — Auditoría de movimientos',
    '<b>Propósito:</b> "Libro diario" del inventario. Inmutable. <b>Hereda de:</b> AuditableEntity (NO SoftDeletableEntity).', [
    ('MovimientoId','int, PK','Sí','Identificador único'),
    ('ProductoId','int, FK','Sí','Qué producto se movió'),
    ('SucursalId','int, FK','Sí','En qué sucursal'),
    ('TipoMovimiento','enum','Sí','Compra, Consumo, Venta, Reserva, Ajuste...'),
    ('Cantidad','int','Sí','Positivo=entrada, Negativo=salida'),
    ('CantidadAnterior','int','Sí','Stock ANTES'),
    ('CantidadDespues','int','Sí','Stock DESPUÉS'),
    ('OrigenDocumento','enum','Sí','OrdenServicio, VentaMostrador, OrdenCompra...'),
    ('ReferenciaId','int','Sí','ID del documento origen'),
    ('CostoUnitario','decimal(10,2)','No','Costo al momento'),
    ('Observaciones','string(500)','No',''),
], S)

S.append(PageBreak())

entity('4.5 OsRepuesto — Repuesto en Orden de Servicio',
    '<b>Propósito:</b> Gemelo de OsServicio (mano de obra). <b>Hereda de:</b> SoftDeletableEntity. <b>Métodos:</b> CalcularSubtotal, Reservar, Entregar, Cancelar.', [
    ('OsRepuestoId','int, PK','Sí','Identificador único'),
    ('OsId','int, FK','Sí','A qué OS pertenece'),
    ('ProductoId','int, FK','Sí','Qué producto/repuesto'),
    ('Cantidad','int','Sí','Cuántas unidades'),
    ('PrecioUnitario','decimal(10,2)','Sí','Precio al momento'),
    ('Descuento','decimal(10,2)','Sí','Monto descuento'),
    ('Subtotal','decimal(10,2)','Sí','(Precio × Cant) - Descuento'),
    ('Estado','enum','Sí','Pendiente/Reservado/Entregado/Cancelado'),
    ('TecnicoSolicitaId','int?, FK','No','Técnico que pidió'),
    ('Observaciones','string(500)','No',''),
], S)

S.append(Paragraph('Ciclo de vida del OsRepuesto:', styles['H3']))
S.append(tbl(['Estado de la OS','Estado OsRepuesto','Qué pasa con inventario'], [
    ('DIAGNÓSTICO/COTIZADA','Pendiente','Stock NO se toca'),
    ('APROBADA','Reservado','Disponible -=N, Reservada +=N'),
    ('COMPLETADA','Entregado','Reservada -=N (ya salió)'),
    ('CANCELADA','Cancelado','Si Reservado: Disponible +=N, Reservada -=N'),
], [1.8*inch, 1.5*inch, 3.2*inch]))
S.append(Spacer(1, 12))

entity('4.6 VentaMostrador — Venta directa de repuestos',
    '<b>Propósito:</b> Venta sin OS. ClienteId nullable (público general). <b>Hereda de:</b> SoftDeletableEntity.', [
    ('VentaMostradorId','int, PK','Sí','Identificador único'),
    ('NumeroVenta','string(20), único','Sí','"VTA-2026-0001"'),
    ('ClienteId','int?, FK','No','nullable: público general'),
    ('VehiculoId','int?, FK','No','nullable: no siempre aplica'),
    ('SucursalId','int, FK','Sí','En qué sucursal'),
    ('VendedorId','string, FK','Sí','ApplicationUser vendedor'),
    ('FechaVenta','DateTime','Sí',''),
    ('Subtotal','decimal(10,2)','Sí','Suma de líneas'),
    ('Impuesto','decimal(10,2)','Sí','ISV'),
    ('Total','decimal(10,2)','Sí','Subtotal + ISV'),
    ('EstadoVenta','enum','Sí','Pendiente/Completada/Cancelada/Devuelta'),
    ('Observaciones','string(500)','No',''),
], S)

entity('4.7 DetalleVenta — Líneas de la venta directa',
    '<b>Propósito:</b> Cada producto vendido. Mismo patrón que OsServicio:OrdenServicio. <b>Hereda de:</b> SoftDeletableEntity.', [
    ('DetalleVentaId','int, PK','Sí','Identificador único'),
    ('VentaMostradorId','int, FK','Sí','A qué venta pertenece'),
    ('ProductoId','int, FK','Sí','Qué producto'),
    ('Cantidad','int','Sí',''),
    ('PrecioUnitario','decimal(10,2)','Sí',''),
    ('Descuento','decimal(10,2)','Sí',''),
    ('Subtotal','decimal(10,2)','Sí','(Precio × Cant) - Descuento'),
], S)

S.append(PageBreak())

entity('4.8 Proveedor — Catálogo de proveedores',
    '<b>Propósito:</b> Empresas que venden repuestos a Premier. Local o internacional. <b>Hereda de:</b> SoftDeletableEntity. <b>Métodos:</b> EsInternacional, NombreMostrar.', [
    ('ProveedorId','int, PK','Sí','Identificador único'),
    ('Codigo','string(20), único','Sí','"PROV-001"'),
    ('RazonSocial','string(200)','Sí','Nombre legal'),
    ('NombreComercial','string(200)','No','Nombre conocido'),
    ('TipoProveedor','enum','Sí','Local o Internacional'),
    ('Pais','string(100)','No','"Honduras", "China", "Japón"'),
    ('Ciudad','string(100)','No',''),
    ('Direccion','string(500)','No',''),
    ('Telefono','string(20)','No',''),
    ('Email','string(100)','No',''),
    ('SitioWeb','string(200)','No',''),
    ('ContactoPrincipal','string(200)','No','Persona de contacto'),
    ('TelefonoContacto','string(20)','No',''),
    ('RTN','string(20)','No','Solo proveedores locales'),
    ('DiasCredito','int','Sí','0=contado, 30, 60, 90 días'),
    ('MonedaPredeterminada','string(5)','Sí','"HNL" o "USD"'),
    ('Observaciones','string(2000)','No',''),
], S)

entity('4.9 OrdenCompra — Pedido a proveedor',
    '<b>Propósito:</b> Documento principal de compra. Equivalente a OrdenServicio pero para compras. Campos de importación siguen patrón de Vehiculo. <b>Hereda de:</b> SoftDeletableEntity. <b>Métodos:</b> EsImportacion, PuedeModificarse, CalcularTotales, PorcentajeRecibido.', [
    ('OrdenCompraId','int, PK','Sí','Identificador único'),
    ('NumeroOrdenCompra','string(20), único','Sí','"OC-2026-0001"'),
    ('ProveedorId','int, FK','Sí','A quién se compra'),
    ('SucursalDestinoId','int, FK','Sí','Dónde se recibirá'),
    ('Estado','enum','Sí','Ver EstadoOrdenCompra'),
    ('FechaEmision','DateTime','Sí','Cuándo se creó'),
    ('FechaAprobacion','DateTime?','No','Cuándo se aprobó'),
    ('FechaEstimadaEntrega','DateTime?','No','ETA general'),
    ('Moneda','string(5)','Sí','"HNL" o "USD"'),
    ('TipoCambio','decimal(10,4)','No','USD→HNL al momento'),
    ('Subtotal','decimal(18,2)','Sí','Suma de líneas'),
    ('CostoFlete','decimal(10,2)','Sí','Shipping'),
    ('CostoSeguro','decimal(10,2)','Sí','Seguro de carga'),
    ('CostoAduana','decimal(10,2)','Sí','Aranceles (solo intl.)'),
    ('OtrosCostos','decimal(10,2)','Sí','Handling, almacenaje'),
    ('TotalGeneral','decimal(18,2)','Sí','Subtotal + costos'),
    ('── Solo importación ──','','',''),
    ('NumeroContenedor','string(50)','No','"MSKU1234567"'),
    ('NumeroBL','string(50)','No','Bill of Lading'),
    ('NumeroFacturaProveedor','string(50)','No','Invoice # proveedor'),
    ('PuertoOrigen','string(100)','No','"Shanghai"'),
    ('PuertoDestino','string(100)','No','"Puerto Cortés"'),
    ('FechaEmbarque','DateTime?','No','Salió del origen'),
    ('FechaEstimadaArribo','DateTime?','No','ETA al puerto'),
    ('FechaLlegadaPuerto','DateTime?','No','Llegó al puerto'),
    ('FechaLiberacionAduana','DateTime?','No','Liberado de aduanas'),
    ('── Responsables ──','','',''),
    ('SolicitadoPorId','string, FK','Sí','Quién pidió'),
    ('AprobadoPorId','string?, FK','No','Quién aprobó'),
    ('Observaciones','string(2000)','No',''),
], S)

S.append(PageBreak())

entity('4.10 OrdenCompraDetalle — Líneas del pedido',
    '<b>Propósito:</b> Productos pedidos. CantidadRecibida se incrementa con cada recepción. <b>Hereda de:</b> SoftDeletableEntity. <b>Métodos:</b> CantidadPendiente, EstaCompleto.', [
    ('OrdenCompraDetalleId','int, PK','Sí','Identificador único'),
    ('OrdenCompraId','int, FK','Sí','A qué OC pertenece'),
    ('ProductoId','int, FK','Sí','Qué producto'),
    ('CantidadPedida','int','Sí','Cuántas se pidieron'),
    ('CantidadRecibida','int','Sí','Cuántas recibidas (default 0)'),
    ('PrecioUnitario','decimal(10,2)','Sí','Precio costo proveedor'),
    ('Subtotal','decimal(10,2)','Sí','PrecioUnit × CantPedida'),
    ('Observaciones','string(500)','No',''),
], S)

entity('4.11 RecepcionCompra — Recepción física de mercancía',
    '<b>Propósito:</b> Cuando llega mercancía. Una OC puede tener MÚLTIPLES recepciones (parciales). Equivalente a Recepcion (vehículos). <b>Hereda de:</b> SoftDeletableEntity.', [
    ('RecepcionCompraId','int, PK','Sí','Identificador único'),
    ('NumeroRecepcion','string(20), único','Sí','"REC-2026-0001"'),
    ('OrdenCompraId','int, FK','Sí','De qué pedido viene'),
    ('SucursalId','int, FK','Sí','Dónde se recibe'),
    ('FechaRecepcion','DateTime','Sí','Cuándo llegó'),
    ('Estado','enum','Sí','Pendiente/Verificada/Completada/Rechazada'),
    ('RecibidoPorId','string, FK','Sí','Quién recibió'),
    ('NumeroGuia','string(50)','No','Guía de transporte'),
    ('Observaciones','string(2000)','No','Novedades, daños'),
], S)

entity('4.12 RecepcionCompraDetalle — Detalle de lo recibido',
    '<b>Propósito:</b> Control exacto: llegó vs pidió, incluyendo daños. CantidadAceptada entra al inventario. <b>Hereda de:</b> SoftDeletableEntity.', [
    ('RecepcionCompraDetalleId','int, PK','Sí','Identificador único'),
    ('RecepcionCompraId','int, FK','Sí','A qué recepción pertenece'),
    ('OrdenCompraDetalleId','int, FK','Sí','Qué línea del pedido'),
    ('ProductoId','int, FK','Sí','Qué producto'),
    ('CantidadRecibida','int','Sí','Total que llegó físicamente'),
    ('CantidadDanada','int','Sí','Cuántas dañadas (default 0)'),
    ('CantidadAceptada','int','Sí','Recibida - Dañada (entra al inv.)'),
    ('Observaciones','string(500)','No','Detalle de daños'),
], S)

S.append(PageBreak())

# 5. ENUMS
S.append(Paragraph('5. Enumeraciones Nuevas', styles['H1']))
S.append(Paragraph('Se agregan 8 enumeraciones a EnumsDomain.cs:', styles['Body']))

enum_section('UnidadMedida', [
    ('Pieza','1','Unidad individual'), ('Litro','2','Aceites, refrigerantes'),
    ('Galon','3','Aceites al por mayor'), ('Juego','4','Kit (pastillas, etc.)'),
    ('Metro','5','Mangueras, cables'), ('Kilogramo','6','Materiales a granel'), ('Otro','99',''),
], S)
enum_section('TipoProveedor', [('Local','1','Nacional (Honduras)'), ('Internacional','2','Extranjero')], S)
enum_section('EstadoOrdenCompra', [
    ('Borrador','1','Editable'), ('Aprobada','2','Aprobada internamente'),
    ('Enviada','3','Enviada al proveedor'), ('Confirmada','4','Proveedor confirmó'),
    ('Embarcada','5','En tránsito (solo intl.)'), ('EnAduana','6','En nacionalización (solo intl.)'),
    ('RecibidaParcial','7','Parte recibida'), ('RecibidaTotal','8','Todo recibido'), ('Cancelada','9','Cancelada'),
], S)
enum_section('EstadoRecepcionCompra', [
    ('Pendiente','1','Por verificar'), ('Verificada','2','Contada'),
    ('Completada','3','Inventario incrementado'), ('Rechazada','4','Rechazada'),
], S)
enum_section('EstadoVenta', [
    ('Pendiente','1','En proceso'), ('Completada','2','Pagada, stock descontado'),
    ('Cancelada','3','Cancelada'), ('Devuelta','4','Stock restaurado'),
], S)
enum_section('EstadoOsRepuesto', [
    ('Pendiente','1','Identificado (stock intacto)'), ('Reservado','2','Stock apartado (OS aprobada)'),
    ('Entregado','3','Consumido (OS completada)'), ('Cancelado','4','Cancelado'),
], S)
enum_section('TipoMovimiento', [
    ('Compra','1','+ Entró por recepción'), ('ConsumoServicio','2','- Salió por OS'),
    ('VentaMostrador','3','- Salió por venta'), ('Reserva','4','± Apartado para OS'),
    ('DevolucionReserva','5','+ Reserva cancelada'), ('DevolucionVenta','6','+ Devolución cliente'),
    ('DevolucionServicio','7','+ Devolución OS'), ('AjustePositivo','8','+ Inv. físico (+)'),
    ('AjusteNegativo','9','- Inv. físico (-)'), ('TransferenciaOut','10','- A otra sucursal'),
    ('TransferenciaIn','11','+ Desde otra sucursal'),
], S)
enum_section('OrigenDocumento', [
    ('OrdenServicio','1','Desde OS'), ('VentaMostrador','2','Desde venta'),
    ('OrdenCompra','3','Desde compra'), ('AjusteManual','4','Ajuste'),
    ('Transferencia','5','Entre sucursales'),
], S)

S.append(PageBreak())

# 6. INTEGRACIÓN
S.append(Paragraph('6. Integración con Entidades Existentes', styles['H1']))
S.append(Paragraph('Modificaciones menores a entidades existentes:', styles['Body']))

S.append(Paragraph('6.1 OrdenServicio.cs — Agregar colección de repuestos', styles['H2']))
S.append(Paragraph('Archivo: PremierFlow.Domain/Entities/OrdenServicio.cs', styles['Note']))
S.append(Preformatted(
    '// AGREGAR navegación (junto a Servicios):\n'
    'public virtual ICollection<OsRepuesto> Repuestos { get; set; }\n'
    '    = new List<OsRepuesto>();\n'
    '\n'
    '// ACTUALIZAR CalcularTotales():\n'
    'public void CalcularTotales()\n'
    '{\n'
    '    TotalManoObra = Servicios\n'
    '        .Where(s => s.Estado != EstadoServicioOS.Cancelado)\n'
    '        .Sum(s => s.Subtotal);\n'
    '\n'
    '    TotalRepuestos = Repuestos                    // NUEVO\n'
    '        .Where(r => r.Estado != EstadoOsRepuesto.Cancelado)\n'
    '        .Sum(r => r.Subtotal);\n'
    '\n'
    '    TotalGeneral = TotalManoObra + TotalRepuestos;\n'
    '}', styles['CodeCustom']))

S.append(Paragraph('6.2 Cliente.cs', styles['H2']))
S.append(Preformatted('public virtual ICollection<VentaMostrador> VentasMostrador { get; set; }\n    = new List<VentaMostrador>();', styles['CodeCustom']))

S.append(Paragraph('6.3 Sucursal.cs', styles['H2']))
S.append(Preformatted('public virtual ICollection<InventarioSucursal> Inventarios { get; set; }\n    = new List<InventarioSucursal>();', styles['CodeCustom']))

S.append(Paragraph('6.4 EnumsDomain.cs — Agregar los 8 enums de la sección 5', styles['H2']))
S.append(Paragraph('6.5 PremierFlowDbContext.cs — Agregar 12 DbSets', styles['H2']))
S.append(Preformatted(
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
    'public DbSet<RecepcionCompraDetalle> RecepcionesCompraDetalle { get; set; }', styles['CodeCustom']))

S.append(PageBreak())

# 7. FLUJOS
S.append(Paragraph('7. Flujos de Negocio', styles['H1']))

S.append(Paragraph('7.1 Compra internacional (ej: China)', styles['H2']))
for t, d in [
    ('1. Crear pedido','OC-2026-0042 a "AutoParts Shanghai". 200 filtros @ $3.50 USD. Estado: Borrador.'),
    ('2. Aprobación','Gerente aprueba → Enviada al proveedor.'),
    ('3. Confirmación','Proveedor confirma → Confirmada.'),
    ('4. Embarque','Contenedor MSKU1234567 sale de Shanghai. NumeroBL registrado. Estado: Embarcada.'),
    ('5. Aduana','Llega a Puerto Cortés. CostoAduana=L.15,000. Estado: EnAduana.'),
    ('6. Liberación','Aduanas libera. FechaLiberacionAduana registrada.'),
    ('7. Recepción','REC-2026-0078: 200 llegan, 5 dañados → 195 aceptados. Inventario +=195. Mov. tipo Compra. OC: RecibidaParcial.'),
]:
    S.append(Paragraph(f'<b>{t}:</b> {d}', styles['Body']))

S.append(Spacer(1, 8))
S.append(Paragraph('7.2 Compra local', styles['H2']))
S.append(Paragraph('Igual sin embarque/aduana: <b>Borrador → Aprobada → Enviada → Confirmada → RecibidaParcial/Total</b>.', styles['Body']))

S.append(Spacer(1, 8))
S.append(Paragraph('7.3 Consumo en servicio (taller)', styles['H2']))
for t, d in [
    ('1. Diagnóstico','Técnico: Honda CRV necesita 1 filtro + 4L aceite.'),
    ('2. Cotización','OsRepuesto: Filtro $290 + Aceite $1,200 = $1,490. Estado: Pendiente.'),
    ('3. Aprobación','Cliente aprueba. Reservado. Stock: Disponible -=N, Reservada +=N.'),
    ('4. Ejecución','Técnico instala. Entregado. Reservada -=N. Mov: ConsumoServicio.'),
    ('5. Facturación','TotalManoObra + TotalRepuestos = TotalGeneral.'),
]:
    S.append(Paragraph(f'<b>{t}:</b> {d}', styles['Body']))

S.append(Spacer(1, 8))
S.append(Paragraph('7.4 Venta en mostrador', styles['H2']))
for t, d in [
    ('1. Solicitud','Cliente pide 2 filtros.'),
    ('2. Stock','Verificar InventarioSucursal.'),
    ('3. Crear','VTA-2026-0200: 2 filtros @ $290.'),
    ('4. Completar','Cobrar. Disponible -=2. Mov: VentaMostrador -2.'),
]:
    S.append(Paragraph(f'<b>{t}:</b> {d}', styles['Body']))

S.append(PageBreak())

# 8. RESUMEN
S.append(Paragraph('8. Resumen de Archivos a Crear y Modificar', styles['H1']))

S.append(Paragraph('<b>24 archivos nuevos:</b> 12 entidades (Domain/Entities/) + 12 configuraciones EF Core (Infrastructure/Configurations/).', styles['Body']))
S.append(Paragraph('<b>5 archivos a modificar:</b>', styles['Body']))
for m in [
    'OrdenServicio.cs — Agregar ICollection&lt;OsRepuesto&gt;, actualizar CalcularTotales()',
    'Cliente.cs — Agregar ICollection&lt;VentaMostrador&gt;',
    'Sucursal.cs — Agregar ICollection&lt;InventarioSucursal&gt;',
    'EnumsDomain.cs — Agregar 8 enums nuevos',
    'PremierFlowDbContext.cs — Agregar 12 DbSets',
]:
    S.append(Paragraph(f'   • {m}', styles['Body']))

S.append(Spacer(1, 16))
S.append(Paragraph('Resumen de las 12 entidades nuevas', styles['H2']))
S.append(tbl(['#','Entidad','Grupo','Propósito'], [
    ('1','CategoriaProducto','Catálogo','Clasificación jerárquica'),
    ('2','Producto','Catálogo','Registro maestro del repuesto'),
    ('3','InventarioSucursal','Almacén','Stock por sucursal'),
    ('4','MovimientoInventario','Almacén','Auditoría de movimientos'),
    ('5','OsRepuesto','Canal Servicio','Repuesto en OS de taller'),
    ('6','VentaMostrador','Canal Venta','Venta directa cabecera'),
    ('7','DetalleVenta','Canal Venta','Líneas de venta'),
    ('8','Proveedor','Canal Compra','Catálogo de proveedores'),
    ('9','OrdenCompra','Canal Compra','Pedido a proveedor'),
    ('10','OrdenCompraDetalle','Canal Compra','Líneas del pedido'),
    ('11','RecepcionCompra','Canal Compra','Recepción de mercancía'),
    ('12','RecepcionCompraDetalle','Canal Compra','Detalle de lo recibido'),
], [0.4*inch, 2*inch, 1.3*inch, 2.8*inch]))

doc.build(S)
print(f'PDF generado: {output_path}')
print(f'Tamano: {os.path.getsize(output_path) / 1024:.0f} KB')
