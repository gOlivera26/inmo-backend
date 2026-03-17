using Inmo24.Application.ResponseDto.Dashboard;
namespace Inmo24.Application.Services.Implementations;

public class DashboardService : BaseService, IDashboardService
{
    public DashboardService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<DashboardResponseDto>> ObtenerResumenAsync()
    {
        var response = new DashboardResponseDto();
        var tenantId = GetCurrentTenantId();

        // 👇 SOLUCIÓN: Usamos UTC porque PostgreSQL ahora espera timestamptz en todas las tablas
        var hoy = DateTime.UtcNow;
        var inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var inicioMesPasado = inicioMesActual.AddMonths(-1);

        // 1. BIENVENIDA: Propiedades publicadas este mes
        response.PropiedadesPublicadasMes = await _context.Set<Propiedades>()
            .CountAsync(p => p.FaseCargaId == 3 && p.CreadoEl >= inicioMesActual);

        // 2. KPI: Ingresos Totales (Mes Actual vs Mes Pasado)
        // Pagos de alquileres
        var pagosActual = await _context.Set<Pago>()
            .Where(p => p.Contrato.TenantId == tenantId && p.Estado == "pagado" && p.FechaPago >= inicioMesActual)
            .SumAsync(p => p.MontoAbonado ?? 0);

        // Comisiones de ventas
        var comisionesActual = await _context.Set<Venta>()
            .Where(v => v.TenantId == tenantId && v.FechaVenta >= inicioMesActual)
            .SumAsync(v => v.ComisionInmobiliaria);

        var ingresosActual = pagosActual + comisionesActual;

        // Repetimos para el mes pasado
        var pagosPasado = await _context.Set<Pago>()
            .Where(p => p.Contrato.TenantId == tenantId && p.Estado == "pagado" && p.FechaPago >= inicioMesPasado && p.FechaPago < inicioMesActual)
            .SumAsync(p => p.MontoAbonado ?? 0);

        var comisionesPasado = await _context.Set<Venta>()
            .Where(v => v.TenantId == tenantId && v.FechaVenta >= inicioMesPasado && v.FechaVenta < inicioMesActual)
            .SumAsync(v => v.ComisionInmobiliaria);

        var ingresosPasado = pagosPasado + comisionesPasado;

        response.IngresosTotales = CalcularKpi("Ingresos Totales", ingresosActual, ingresosPasado, true);

        // 3. KPI: Stock Activo
        var stockActual = await _context.Set<Propiedades>()
            .CountAsync(p => p.EstadoComercialId == 1 && p.FaseCargaId == 3); // Disponible y Publicada
        response.StockActivo = CalcularKpi("Stock Activo", stockActual, 0, false); // No medimos tendencia de stock aquí, o ponemos 0.

        // 4. KPI: Base Clientes (Mes Actual vs Mes Pasado)
        var clientesActual = await _context.Set<Cliente>().CountAsync();
        var clientesMesPasado = await _context.Set<Cliente>().CountAsync(c => c.CreadoEl < inicioMesActual);
        response.BaseClientes = CalcularKpi("Base Clientes", clientesActual, clientesMesPasado, false);

        // 5. KPI: Visitas del Mes (Mes Actual vs Mes Pasado)
        var visitasActual = await _context.Set<Visita>().CountAsync(v => v.CreadoEl >= inicioMesActual);
        var visitasPasado = await _context.Set<Visita>().CountAsync(v => v.CreadoEl >= inicioMesPasado && v.CreadoEl < inicioMesActual);
        response.VisitasMes = CalcularKpi("Visitas Mes", visitasActual, visitasPasado, false);

        // 6. PROPIEDADES DESTACADAS (Últimas 3)
        response.PropiedadesDestacadas = await _context.Set<Propiedades>()
            .Include(p => p.PropiedadImagenes)
            .Include(p => p.EstadoComercial)
            .Include(p => p.Zona)
            .Where(p => p.Destacada == true && p.FaseCargaId == 3)
            .OrderByDescending(p => p.CreadoEl)
            .Take(3)
            .Select(p => new PropiedadDestacadaDashboardDto
            {
                Id = p.Id,
                Titulo = string.IsNullOrEmpty(p.Titulo) ? p.Direccion : p.Titulo,
                Ubicacion = $"{p.Zona.Nombre}, {p.Direccion}",
                Precio = p.Moneda == "USD" ? (p.PrecioUsd ?? 0) : (p.PrecioArs ?? 0),
                Moneda = p.Moneda,
                Estado = p.EstadoComercial.Nombre,
                ImagenUrl = p.PropiedadImagenes.Where(i => !i.IsDeleted && i.EsPrincipal).Select(i => i.Url).FirstOrDefault()
                            ?? p.PropiedadImagenes.Where(i => !i.IsDeleted).Select(i => i.Url).FirstOrDefault() ?? ""
            }).ToListAsync();

        // 7. DISTRIBUCIÓN POR ESTADOS (Gráfico Donut)
        var distribucion = await _context.Set<Propiedades>()
            .Include(p => p.EstadoComercial)
            .GroupBy(p => p.EstadoComercial.Nombre)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .ToListAsync();

        response.DistribucionEstados = distribucion.ToDictionary(x => x.Estado, x => x.Cantidad);

        // 8. PRÓXIMAS VISITAS
        response.ProximasVisitas = await _context.Set<Visita>()
            .Include(v => v.Cliente)
            .Include(v => v.Propiedad)
            .Where(v => v.FechaVisita >= hoy && v.Estado == "agendada")
            .OrderBy(v => v.FechaVisita)
            .Take(4)
            .Select(v => new VisitaDashboardDto
            {
                Id = v.Id,
                ClienteNombre = v.Cliente.Nombre + " " + v.Cliente.Apellido,
                PropiedadTitulo = v.Propiedad.Titulo ?? v.Propiedad.Direccion,
                FechaHora = v.FechaVisita
            }).ToListAsync();

        // 9. ACTIVIDAD RECIENTE (Timeline)
        // Obtenemos los últimos eventos de diferentes tablas y los combinamos
        var ultimasPropiedades = await _context.Set<Propiedades>()
            .OrderByDescending(p => p.CreadoEl)
            .Take(3)
            .Select(p => new ActividadRecienteDto
            {
                Tipo = "propiedad",
                Titulo = "Nueva propiedad registrada",
                Descripcion = $"Se ha registrado la propiedad {p.Codigo} en el sistema.",
                Fecha = p.CreadoEl
            }).ToListAsync();

        var ultimosClientes = await _context.Set<Cliente>()
            .OrderByDescending(c => c.CreadoEl)
            .Take(3)
            .Select(c => new ActividadRecienteDto
            {
                Tipo = "lead",
                Titulo = "Nuevo Lead recibido",
                Descripcion = $"El cliente {c.Nombre} {c.Apellido} ha sido registrado.",
                Fecha = c.CreadoEl
            }).ToListAsync();

        var ultimasVisitas = await _context.Set<Visita>()
            .Include(v => v.Propiedad)
            .OrderByDescending(v => v.CreadoEl)
            .Take(3)
            .Select(v => new ActividadRecienteDto
            {
                Tipo = "visita",
                Titulo = "Nueva visita agendada",
                Descripcion = $"Visita programada para {v.Propiedad.Codigo}.",
                Fecha = v.CreadoEl
            }).ToListAsync();

        var ultimasVentas = await _context.Set<Venta>()
            .Include(v => v.Propiedad)
            .OrderByDescending(v => v.CreadoEl)
            .Take(3)
            .Select(v => new ActividadRecienteDto
            {
                Tipo = "contrato",
                Titulo = "Propiedad Vendida 💰",
                Descripcion = $"Se cerró la venta de la propiedad {v.Propiedad.Codigo}.",
                Fecha = v.CreadoEl
            }).ToListAsync();

        var ultimosAlquileres = await _context.Set<Contrato>()
            .Include(c => c.Propiedad)
            .OrderByDescending(c => c.CreadoEl)
            .Take(3)
            .Select(c => new ActividadRecienteDto
            {
                Tipo = "contrato",
                Titulo = "Propiedad Alquilada 🔑",
                Descripcion = $"Se firmó contrato de alquiler para {c.Propiedad.Codigo}.",
                Fecha = c.CreadoEl
            }).ToListAsync();

        // Juntamos todo, ordenamos por fecha y tomamos los 5 más recientes
        response.ActividadReciente = ultimasPropiedades
            .Concat(ultimosClientes)
            .Concat(ultimasVisitas)
            .Concat(ultimasVentas)      // <-- Agregado
            .Concat(ultimosAlquileres)  // <-- Agregado
            .OrderByDescending(a => a.Fecha)
            .Take(5)
            .ToList();

        return Ok(response);
    }

    // Helper para calcular % de crecimiento y formatear la moneda si es necesario
    private KpiDto CalcularKpi(string label, decimal valorActual, decimal valorPasado, bool isMoneda)
    {
        var kpi = new KpiDto
        {
            Label = label,
            Value = valorActual,
            ValueFormatted = isMoneda ? $"${valorActual:N0}" : valorActual.ToString("N0")
        };

        if (valorPasado == 0)
        {
            kpi.Trend = "+100%";
            kpi.TrendingUp = true;
        }
        else
        {
            var porcentaje = ((valorActual - valorPasado) / valorPasado) * 100;
            kpi.TrendingUp = porcentaje >= 0;
            kpi.Trend = $"{(kpi.TrendingUp ? "+" : "")}{porcentaje:F1}%";
        }

        return kpi;
    }
}