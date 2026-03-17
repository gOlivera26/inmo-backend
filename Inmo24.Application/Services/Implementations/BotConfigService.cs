using Inmo24.Application.RequestDto.Bot;
using Inmo24.Application.ResponseDto.Bot;
using Inmo24.Application.Services.Interfaces;
using Inmo24.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;

namespace Inmo24.Application.Services.Implementations;

public class BotConfigService : BaseService, IBotConfigService
{
    public BotConfigService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<BotConfigResponseDto>> ObtenerConfiguracionAsync()
    {
        var tenantId = GetCurrentTenantId();
        var config = await _context.Set<BotConfiguracion>().FirstOrDefaultAsync(b => b.TenantId == tenantId && !b.IsDeleted);

        // 👇 AQUI PONEMOS TUS VALORES EXACTOS POR DEFECTO 👇
        if (config == null)
        {
            config = new BotConfiguracion
            {
                TenantId = tenantId,
                Activo = true,
                NombreBot = "Juan",
                TonoConversacion = "Argentino, amable, profesional y resolutivo.",
                SaludoInicial = "Hola buenas ✌🏼 Soy {NombreBot} de {NombreInmobiliaria}, tu inmobiliaria de confianza, en qué puedo ayudarte hoy? Decime lo que necesites y te doy una mano!"
            };
            PrepareAuditableEntity(config, isNew: true);
            _context.Set<BotConfiguracion>().Add(config);
            await _context.SaveChangesAsync();
        }

        return Ok(new BotConfigResponseDto
        {
            Activo = config.Activo,
            NombreBot = config.NombreBot,
            TonoConversacion = config.TonoConversacion,
            SaludoInicial = config.SaludoInicial,
            TelefonoDerivacion = config.TelefonoDerivacion ?? "",
            DirectricesExtra = config.DirectricesExtra ?? ""
        });
    }

    public async Task<OperationResponse<BotConfigResponseDto>> ActualizarConfiguracionAsync(BotConfigUpdateRequestDto request)
    {
        var tenantId = GetCurrentTenantId();
        var config = await _context.Set<BotConfiguracion>().FirstOrDefaultAsync(b => b.TenantId == tenantId && !b.IsDeleted);

        if (config == null) return NotFound<BotConfigResponseDto>();

        config.Activo = request.Activo;
        config.NombreBot = request.NombreBot;
        config.TonoConversacion = request.TonoConversacion;
        config.SaludoInicial = request.SaludoInicial;
        config.TelefonoDerivacion = request.TelefonoDerivacion;
        config.DirectricesExtra = request.DirectricesExtra;

        PrepareAuditableEntity(config, isNew: false);
        _context.Set<BotConfiguracion>().Update(config);
        await _context.SaveChangesAsync();

        return await ObtenerConfiguracionAsync();
    }

    public async Task<OperationResponse<BotPromptActivoDto>> ObtenerConfiguracionPorInstanciaAsync(string instanceName)
    {
        // 1. Buscamos el tenant que tenga esa instancia vinculada
        var tenant = await _context.Set<Tenant>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.InstanciaWa == instanceName && !t.IsDeleted);

        if (tenant == null) return NotFound<BotPromptActivoDto>();
        return await ObtenerPromptActivoParaN8nAsync(tenant.Id);
    }

    public async Task<OperationResponse<BotPromptActivoDto>> ObtenerPromptActivoParaN8nAsync(Guid tenantId)
    {
        var config = await _context.Set<BotConfiguracion>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.TenantId == tenantId && !b.IsDeleted);

        var tenant = await _context.Set<Tenant>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);

        if (config == null || tenant == null) return NotFound<BotPromptActivoDto>();

        if (!config.Activo)
        {
            return Ok(new BotPromptActivoDto { BotActivo = false });
        }

        var saludoPersonalizado = config.SaludoInicial
            .Replace("{NombreBot}", config.NombreBot)
            .Replace("{NombreInmobiliaria}", tenant.Nombre);

        var directricesExtra = string.IsNullOrWhiteSpace(config.DirectricesExtra)
            ? ""
            : $"\n# DIRECTRICES ESPECÍFICAS DE LA INMOBILIARIA\n{config.DirectricesExtra}\n";

        // 👇 CALCULAMOS LA HORA EXACTA EN C# PARA QUE N8N NO TENGA QUE HACERLO 👇
        // Usamos AddHours(-3) para Argentina de forma segura en cualquier servidor (Windows/Linux)
        string fechaActualStr = DateTime.UtcNow.AddHours(-3).ToString("cccc d 'de' MMMM 'de' yyyy HH:mm", new System.Globalization.CultureInfo("es-AR"));

        string promptMaestro = $@"
        FECHA ACTUAL: {fechaActualStr}

        # ROL Y CONTEXTO
        Sos {config.NombreBot}, un Asesor Inmobiliario Virtual de {tenant.Nombre}, con más de 15 años de experiencia especializado en **ALQUILERES Y VENTAS**. Vas a atender consultas por WhatsApp de personas interesadas en alquilar o comprar propiedades (casas, departamentos, PH, dúplex, etc.) en nombre de la inmobiliaria.

        Tu objetivo es:
        - Responder dudas reales de las personas.
        - Guiar hacia el siguiente paso (ver fotos, coordinar visita, dejar datos o cancelar citas).
        - CAPTURAR LEADS: Guardar en el CRM lo que la gente busca y su estado actual.
        - Mantener un tono humano, natural y profesional.
        - Usar herramientas SOLO cuando corresponde y sin romper reglas.
        {directricesExtra}
        # REGLAS FUNDAMENTALES (ANTI-LOOPS Y ANTI-ERRORES)
        1) PROHIBIDO ENVIAR IMÁGENES si el cliente no lo pide explícitamente.
           Palabras clave válidas: ""mandame fotos"", ""pasame imágenes"", ""quiero ver fotos"", ""podés enviarme fotos"", ""enviame fotos de la casa"".
           Si no aparece un pedido explícito, NO envíes ni ofrezcas imágenes.
        2) NO repetir fotos.
        3) NO inventar herramientas ni acciones. Continuá la conversación normalmente con texto humano.
        4) NO inventar información sobre propiedades. Toda descripción debe venir de informacion_casas.
        5) NO entrar en loops. Si no sabés qué hacer, hacé una pregunta corta.
        6) VERIFICACIÓN OBLIGATORIA. Nunca agendes una visita sin antes haber usado consultar_disponibilidad.
        7) REGLA DE CANCELACIÓN. Para cancelar, es OBLIGATORIO pedir primero la fecha/hora y el nombre.
        8) PROTOCOLO DE DERIVACIÓN A HUMANO. Si el cliente se enoja o pide un humano, usá pedir_ayuda_humana.
        9) REGLA DE ORO (CRM). Mantener el CRM actualizado usando guardar_contacto_hubspot de forma silenciosa.
        10) COHERENCIA TEMPORAL: Al calcular fechas para visitas, usa siempre el año provisto en 'FECHA ACTUAL'.

        # RESPONSABILIDADES
        - Consultar SIEMPRE informacion_casas antes de responder características.
        - Usar enviar_imagen SOLO cuando el cliente lo pide explícitamente.
        - Cuando el cliente quiera visitar: PRIMERO pedir día/hora y consultar disponibilidad.
        - Si el horario está LIBRE: Recién ahí pedir nombre y teléfono.
        - Usar agendar_visita SOLO una vez que tengas: disponibilidad + nombre + teléfono + día/hora.   

        # FLUJO DE CONVERSACIÓN
        1) Saludo inicial obligatorio:
           ""{saludoPersonalizado}""

        2) Si el cliente busca propiedad:
           - Preguntar si busca Alquilar o Comprar, zona y presupuesto.
           - AL OBTENER ESTOS DATOS: Ejecutar guardar_contacto_hubspot (Etapa: IN_PROGRESS).
           - Mostrar opciones UNA POR VEZ usando informacion_casas.

        3) Si quiere visitar (LÓGICA DE AGENDA):
           - A: Preguntar ""¿Qué día y horario te quedaría cómodo para ir?"".
           - B: EJECUTAR consultar_disponibilidad.
           - C: Si está LIBRE -> Pedir nombre y teléfono. Si está OCUPADO -> Pedir otro horario.
           - D: Una vez con los datos -> EJECUTAR agendar_visita.
           - E: Inmediatamente después -> EJECUTAR guardar_contacto_hubspot (Etapa: OPEN_DEAL).

        # FORMATO OBLIGATORIO PARA USAR enviar_imagen
        El formato del ID SIEMPRE comienza con INMO- seguido de 4 caracteres alfanuméricos (Ejemplo: INMO-A1B2).
        Extrae este código exacto de la información de la propiedad. No inventar ConversationID ni enviar texto extra.
        {{
          ""ID"": ""INMO-XXXX"",
          ""ConversationID"": ""{{{{conversationId}}}}""
        }}

        # REGLAS DE ESTILO
        - Tono: {config.TonoConversacion}
        - Frases simples, breves, fáciles de leer. Emoticones moderados.";

        return Ok(new BotPromptActivoDto
        {
            TenantId = tenantId,
            BotActivo = true,
            PromptArmado = promptMaestro,
            TelefonoDerivacion = config.TelefonoDerivacion ?? ""
        });
    }
}