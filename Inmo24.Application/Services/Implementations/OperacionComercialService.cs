using Inmo24.Application.RequestDto.Operaciones;
using Inmo24.Application.ResponseDto.Operaciones;
using Inmo24.Application.Services.Interfaces;
using Inmo24.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using AutoMapper;

namespace Inmo24.Application.Services.Implementations;

public class OperacionComercialService : BaseService, IOperacionComercialService
{
    public OperacionComercialService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<bool>> CerrarOperacionAsync(CerrarOperacionRequestDto request)
    {
        var propiedad = await _context.Set<Propiedades>().FirstOrDefaultAsync(p => p.Id == request.PropiedadId);
        if (propiedad == null) return NotFound<bool>();

        var cliente = await _context.Set<Cliente>().FirstOrDefaultAsync(c => c.Id == request.ClienteId);
        if (cliente == null) return NotFound<bool>();

        var tenantId = GetCurrentTenantId();

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (request.TipoOperacion.ToUpper() == "VENTA")
            {
                if (propiedad.EstadoComercialId == 4) return BadRequest<bool>("La propiedad ya está vendida.");
                await ProcesarVentaAsync(request, propiedad, cliente, tenantId); 
            }
            else if (request.TipoOperacion.ToUpper() == "ALQUILER")
            {
                if (propiedad.EstadoComercialId == 3) return BadRequest<bool>("La propiedad ya está alquilada.");
                await ProcesarAlquilerAsync(request, propiedad, cliente, tenantId);
            }
            else
            {
                return BadRequest<bool>("Tipo de operación no válido.");
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return InternalServerError<bool>($"Error al cerrar la operación: {ex.Message}");
        }
    }

    public async Task<OperationResponse<OperacionDetalleResponseDto>> ObtenerOperacionPorPropiedadAsync(Guid propiedadId)
    {
        var propiedad = await _context.Set<Propiedades>().FirstOrDefaultAsync(p => p.Id == propiedadId);
        if (propiedad == null) return NotFound<OperacionDetalleResponseDto>();

        var response = new OperacionDetalleResponseDto();

        if (propiedad.EstadoComercialId == 4) // Vendida
        {
            var venta = await _context.Set<Venta>()
                .Include(v => v.Comprador)
                .Where(v => v.PropiedadId == propiedadId && !v.IsDeleted)
                .OrderByDescending(v => v.CreadoEl)
                .FirstOrDefaultAsync();

            if (venta == null) return NotFound<OperacionDetalleResponseDto>();

            response.TipoOperacion = "VENTA";
            response.ClienteId = venta.CompradorId;
            response.ClienteNombre = $"{venta.Comprador.Nombre} {venta.Comprador.Apellido}".Trim();
            response.ClienteTelefono = venta.Comprador.Telefono;
            response.Moneda = venta.Moneda;
            response.FechaOperacion = venta.FechaVenta;
            response.MontoTotal = venta.MontoTotal;
            response.ComisionInmobiliaria = venta.ComisionInmobiliaria;
            response.Escribania = venta.Escribania;
        }
        else if (propiedad.EstadoComercialId == 3) // Alquilada
        {
            var contrato = await _context.Set<Contrato>()
                .Include(c => c.Inquilino)
                .Where(c => c.PropiedadId == propiedadId && c.Estado == "vigente" && !c.IsDeleted)
                .OrderByDescending(c => c.CreadoEl)
                .FirstOrDefaultAsync();

            if (contrato == null) return NotFound<OperacionDetalleResponseDto>();

            response.TipoOperacion = "ALQUILER";
            response.ClienteId = contrato.InquilinoId;
            response.ClienteNombre = $"{contrato.Inquilino.Nombre} {contrato.Inquilino.Apellido}".Trim();
            response.ClienteTelefono = contrato.Inquilino.Telefono;
            response.Moneda = contrato.Moneda;
            response.FechaOperacion = contrato.FechaInicio;
            response.MontoMensual = contrato.MontoMensual;
            response.FechaFin = contrato.FechaFin;
            response.DiaVencimientoPago = contrato.DiaVencimientoPago;
            response.EstadoContrato = contrato.Estado;
        }
        else
        {
            return BadRequest<OperacionDetalleResponseDto>("La propiedad no tiene operaciones cerradas activas.");
        }

        return Ok(response);
    }

    private async Task ProcesarVentaAsync(CerrarOperacionRequestDto request, Propiedades propiedad, Cliente cliente, Guid tenantId)
    {
        var nuevaVenta = new Venta
        {
            TenantId = tenantId,
            PropiedadId = request.PropiedadId,
            CompradorId = request.ClienteId,
            MontoTotal = request.MontoTotal ?? 0,
            ComisionInmobiliaria = request.ComisionInmobiliaria ?? 0,
            Moneda = request.Moneda,
            FechaVenta = request.FechaVenta?.ToUniversalTime() ?? DateTime.UtcNow,
            Escribania = request.Escribania,
            Notas = request.Notas
        };
        PrepareAuditableEntity(nuevaVenta, isNew: true);
        await _context.Set<Venta>().AddAsync(nuevaVenta);

        // 👇 ARMAMOS LA OBSERVACIÓN LIMPIA 👇
        string nombreCliente = $"{cliente.Nombre} {cliente.Apellido}".Trim();
        string observacion = $"Venta cerrada por {request.Moneda} {request.MontoTotal}. Comprador: {nombreCliente} | Tel: {cliente.Telefono} (ID: {cliente.Id})";

        RegistrarHistorial(propiedad, 4, observacion);

        propiedad.EstadoComercialId = 4; // Vendida
        propiedad.NotificarLeads = false;

        PrepareAuditableEntity(propiedad, isNew: false);
        _context.Set<Propiedades>().Update(propiedad);
    }

    private async Task ProcesarAlquilerAsync(CerrarOperacionRequestDto request, Propiedades propiedad, Cliente cliente, Guid tenantId)
    {
        var nuevoContrato = new Contrato
        {
            TenantId = tenantId,
            PropiedadId = request.PropiedadId,
            InquilinoId = request.ClienteId,
            FechaInicio = request.FechaInicio?.ToUniversalTime() ?? DateTime.UtcNow,
            FechaFin = request.FechaFin?.ToUniversalTime() ?? DateTime.UtcNow.AddYears(1),
            MontoMensual = request.MontoMensual ?? 0,
            Moneda = request.Moneda,
            DiaVencimientoPago = request.DiaVencimientoPago ?? 10,
            Estado = "vigente"
        };
        PrepareAuditableEntity(nuevoContrato, isNew: true);
        await _context.Set<Contrato>().AddAsync(nuevoContrato);

        // 👇 ARMAMOS LA OBSERVACIÓN LIMPIA 👇
        string nombreCliente = $"{cliente.Nombre} {cliente.Apellido}".Trim();
        string observacion = $"Contrato de alquiler firmado. Inquilino: {nombreCliente} | Tel: {cliente.Telefono} (ID: {cliente.Id})";

        RegistrarHistorial(propiedad, 3, observacion);

        propiedad.EstadoComercialId = 3; // Alquilada
        propiedad.NotificarLeads = false;

        PrepareAuditableEntity(propiedad, isNew: false);
        _context.Set<Propiedades>().Update(propiedad);
    }

    private void RegistrarHistorial(Propiedades propiedad, short nuevoEstadoId, string observacion)
    {
        var historial = new PropiedadHistorialCambio
        {
            PropiedadId = propiedad.Id,
            EstadoComAntId = propiedad.EstadoComercialId,
            EstadoComNuevoId = nuevoEstadoId,
            Observacion = observacion
        };
        PrepareAuditableEntity(historial, isNew: true);
        _context.Set<PropiedadHistorialCambio>().Add(historial);
    }
}