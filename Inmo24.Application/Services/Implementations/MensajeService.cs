using Inmo24.Application.ResponseDto.Mensajes;


namespace Inmo24.Application.Services.Implementations;

public class MensajeService : BaseService, IMensajeService
{
    public MensajeService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<List<ChatSessionResponseDto>>> ObtenerSesionesChatAsync()
    {
        var tenantId = GetCurrentTenantId();

        var sesionesMaxId = await _context.Set<N8nChatHistorialBot>()
            .GroupBy(m => m.SessionId)
            .Select(g => g.Max(m => m.Id))
            .ToListAsync();

        var ultimosMensajes = await _context.Set<N8nChatHistorialBot>()
            .Where(m => sesionesMaxId.Contains(m.Id))
            .OrderByDescending(m => m.Id)
            .ToListAsync();

        var clientes = await _context.Set<Cliente>()
            .Where(c => c.TenantId == tenantId && !c.IsDeleted)
            .Select(c => new { c.Nombre, c.Apellido, c.Telefono })
            .ToListAsync();

        var listaSesiones = new List<ChatSessionResponseDto>();

        foreach (var msg in ultimosMensajes)
        {
            // El bot guarda la sesión como "549351000000@s.whatsapp.net"
            var telefonoLimpio = msg.SessionId.Split('@')[0];

            // Buscamos si ese número coincide con algún cliente
            var cliente = clientes.FirstOrDefault(c => c.Telefono.Contains(telefonoLimpio) || telefonoLimpio.Contains(c.Telefono));

            // Parseamos el JSON para sacar el extracto del último mensaje
            var contenidoUltimo = "Mensaje multimedia/herramienta";
            try
            {
                using var doc = JsonDocument.Parse(msg.Message);
                if (doc.RootElement.TryGetProperty("content", out var contentProp))
                {
                    contenidoUltimo = contentProp.GetString() ?? contenidoUltimo;

                    if (contenidoUltimo.Contains("contexto: ["))
                    {
                        var extract = contenidoUltimo.Substring(contenidoUltimo.IndexOf("contexto: [") + 11);
                        extract = extract.Substring(0, extract.IndexOf("]"));
                        contenidoUltimo = extract.Replace("\"", "");
                    }

                    if (contenidoUltimo.Length > 40) contenidoUltimo = contenidoUltimo.Substring(0, 37) + "...";
                }
            }
            catch { }

            listaSesiones.Add(new ChatSessionResponseDto
            {
                SessionId = msg.SessionId,
                Telefono = telefonoLimpio,
                NombreCliente = cliente != null ? $"{cliente.Nombre} {cliente.Apellido}".Trim() : "Desconocido",
                UltimoMensaje = contenidoUltimo
            });
        }

        return Ok(listaSesiones);
    }

    public async Task<OperationResponse<List<ChatMessageResponseDto>>> ObtenerMensajesPorSesionAsync(string sessionId)
    {
        var mensajesDb = await _context.Set<N8nChatHistorialBot>()
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.Id)
            .ToListAsync();

        var listaMensajes = new List<ChatMessageResponseDto>();

        foreach (var msg in mensajesDb)
        {
            var tipo = "unknown";
            var contenido = "";

            try
            {
                using var doc = JsonDocument.Parse(msg.Message);

                if (doc.RootElement.TryGetProperty("type", out var typeProp))
                    tipo = typeProp.GetString() ?? "unknown";

                if (doc.RootElement.TryGetProperty("content", out var contentProp))
                    contenido = contentProp.GetString() ?? "";

                // Limpieza del contexto del Humano
                if (tipo == "human" && contenido.Contains("contexto: ["))
                {
                    var extract = contenido.Substring(contenido.IndexOf("contexto: [") + 11);
                    extract = extract.Substring(0, extract.IndexOf("]"));
                    contenido = extract.Replace("\"", "").Replace("\\n", "\n");
                }

                // Si el agente usa una herramienta y no manda texto
                if (string.IsNullOrWhiteSpace(contenido) && doc.RootElement.TryGetProperty("tool_calls", out var tools) && tools.GetArrayLength() > 0)
                {
                    contenido = "⚙️ [Ejecutando proceso interno del sistema]";
                }
            }
            catch { }

            if (!string.IsNullOrWhiteSpace(contenido))
            {
                listaMensajes.Add(new ChatMessageResponseDto
                {
                    Id = msg.Id,
                    Tipo = tipo, // human | ai
                    Contenido = contenido
                });
            }
        }

        return Ok(listaMensajes);
    }
}