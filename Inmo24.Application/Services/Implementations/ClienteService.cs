using Inmo24.Application.RequestDto.Clientes;
using Inmo24.Application.ResponseDto.Clientes;

namespace Inmo24.Application.Services.Implementations;

public class ClienteService : BaseService, IClienteService
{
    public ClienteService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<CrmResumenResponseDto>> ObtenerResumenCrmAsync()
    {
        var tenantId = GetCurrentTenantId();
        var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalClientes = await _context.Set<Cliente>().CountAsync();

        var clientesActivos = await _context.Set<Cliente>()
            .CountAsync(c => c.EtapaCrm == "IN_PROGRESS" || c.EtapaCrm == "OPEN_DEAL");

        var ventasMes = await _context.Set<Venta>()
            .CountAsync(v => v.TenantId == tenantId && v.FechaVenta >= inicioMes);

        var alquileresMes = await _context.Set<Contrato>()
            .CountAsync(c => c.TenantId == tenantId && c.FechaInicio >= inicioMes);

        return Ok(new CrmResumenResponseDto
        {
            TotalClientes = totalClientes,
            ClientesActivos = clientesActivos,
            CierresDelMes = ventasMes + alquileresMes
        });
    }

    public async Task<OperationResponse<List<ClienteResponseDto>>> ObtenerClientesAsync(ClienteFilterRequest request)
    {
        var query = _context.Set<Cliente>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Busqueda))
        {
            var texto = request.Busqueda.ToLower().Trim();
            query = query.Where(c =>
                c.Nombre.ToLower().Contains(texto) ||
                (c.Apellido != null && c.Apellido.ToLower().Contains(texto)) ||
                c.Telefono.Contains(texto) ||
                (c.Email != null && c.Email.ToLower().Contains(texto)));
        }

        if (!string.IsNullOrWhiteSpace(request.EtapaCrm) && request.EtapaCrm != "TODOS")
        {
            query = query.Where(c => c.EtapaCrm == request.EtapaCrm);
        }

        query = query.OrderByDescending(c => c.CreadoEl);

        return await GetPagedDataAsync<Cliente, ClienteResponseDto>(request.Page, request.PageSize, query);
    }

    public async Task<OperationResponse<ClienteDetalleResponseDto>> ObtenerClientePorIdAsync(Guid id)
    {
        var cliente = await _context.Set<Cliente>()
            .Include(c => c.Visita)
                .ThenInclude(v => v.Propiedad)
                    .ThenInclude(p => p.PropiedadImagenes)
            .Include(c => c.Venta)
                .ThenInclude(v => v.Propiedad)
            .Include(c => c.Contratos)
                .ThenInclude(co => co.Propiedad)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null) return NotFound<ClienteDetalleResponseDto>();

        var dto = new ClienteDetalleResponseDto
        {
            Id = cliente.Id,
            NombreCompleto = string.IsNullOrWhiteSpace(cliente.Apellido) ? cliente.Nombre : $"{cliente.Nombre} {cliente.Apellido}",
            Telefono = cliente.Telefono,
            Email = cliente.Email,
            EtapaCrm = cliente.EtapaCrm,
            Notas = cliente.Notas ?? string.Empty,
            CreadoEl = cliente.CreadoEl,

            // 👇 FILTRAMOS LAS VISITAS DONDE LA PROPIEDAD FUE ELIMINADA 👇
            Visitas = cliente.Visita
                .Where(v => v.Propiedad != null)
                .OrderByDescending(v => v.FechaVisita).Select(v => new ClienteVisitaDto
                {
                    VisitaId = v.Id,
                    PropiedadId = v.PropiedadId ?? Guid.Empty,
                    PropiedadTitulo = v.Propiedad?.Titulo ?? v.DireccionCasa,
                    Estado = v.Estado,
                    FechaVisita = v.FechaVisita,
                    PropiedadImagenUrl = v.Propiedad?.PropiedadImagenes.FirstOrDefault(i => i.EsPrincipal && !i.IsDeleted)?.Url
                                     ?? v.Propiedad?.PropiedadImagenes.FirstOrDefault(i => !i.IsDeleted)?.Url
                                     ?? ""
                }).ToList(),

            // 👇 FILTRAMOS LAS OPERACIONES DONDE LA PROPIEDAD FUE ELIMINADA 👇
            Operaciones = cliente.Venta
                .Where(v => v.Propiedad != null)
                .Select(v => new ClienteOperacionDto
                {
                    OperacionId = v.Id,
                    PropiedadId = v.PropiedadId,
                    Tipo = "VENTA",
                    PropiedadTitulo = v.Propiedad.Titulo ?? v.Propiedad.Direccion,
                    Monto = v.MontoTotal,
                    Moneda = v.Moneda,
                    Fecha = v.FechaVenta
                }).Concat(cliente.Contratos
                .Where(c => c.Propiedad != null)
                .Select(c => new ClienteOperacionDto
                {
                    OperacionId = c.Id,
                    PropiedadId = c.PropiedadId,
                    Tipo = "ALQUILER",
                    PropiedadTitulo = c.Propiedad.Titulo ?? c.Propiedad.Direccion,
                    Monto = c.MontoMensual,
                    Moneda = c.Moneda,
                    Fecha = c.FechaInicio
                })).OrderByDescending(o => o.Fecha).ToList()
        };

        return Ok(dto);
    }

    public async Task<OperationResponse<ClienteResponseDto>> CrearClienteAsync(ClienteCreateRequestDto request)
    {
        var tenantId = GetCurrentTenantId();

        var clienteExistente = await _context.Set<Cliente>()
            .FirstOrDefaultAsync(c => c.Telefono == request.Telefono && c.TenantId == tenantId);

        if (clienteExistente != null)
        {
            if (!string.IsNullOrEmpty(request.EtapaCrm))
                clienteExistente.EtapaCrm = request.EtapaCrm;

            if (!string.IsNullOrEmpty(request.Notas))
            {
                clienteExistente.Notas = string.IsNullOrEmpty(clienteExistente.Notas)
                    ? $"[{DateTime.UtcNow:dd/MM/yyyy HH:mm}] {request.Notas}"
                    : $"{clienteExistente.Notas}\n[{DateTime.UtcNow:dd/MM/yyyy HH:mm}] {request.Notas}";
            }

            PrepareAuditableEntity(clienteExistente, isNew: false);
            _context.Set<Cliente>().Update(clienteExistente);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ClienteResponseDto>(clienteExistente));
        }
        else
        {
            var nuevoCliente = _mapper.Map<Cliente>(request);
            nuevoCliente.EtapaCrm = string.IsNullOrEmpty(request.EtapaCrm) ? "NEW" : request.EtapaCrm;

            if (!string.IsNullOrEmpty(request.Notas))
            {
                nuevoCliente.Notas = $"[{DateTime.UtcNow:dd/MM/yyyy HH:mm}] {request.Notas}";
            }

            PrepareAuditableEntity(nuevoCliente, isNew: true);
            _context.Set<Cliente>().Add(nuevoCliente);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ClienteResponseDto>(nuevoCliente));
        }
    }

    public async Task<OperationResponse<ClienteResponseDto>> ActualizarClienteAsync(Guid id, ClienteUpdateRequestDto request)
    {
        var cliente = await _context.Set<Cliente>().FirstOrDefaultAsync(c => c.Id == id);
        if (cliente == null) return NotFound<ClienteResponseDto>();

        var partesNombre = request.NombreCompleto.Trim().Split(' ', 2);
        cliente.Nombre = partesNombre[0];
        cliente.Apellido = partesNombre.Length > 1 ? partesNombre[1] : string.Empty;

        cliente.Email = request.Email;
        cliente.EtapaCrm = request.EtapaCrm;

        PrepareAuditableEntity(cliente, isNew: false);
        _context.Set<Cliente>().Update(cliente);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<ClienteResponseDto>(cliente));
    }
}