namespace Inmo24.Application;

public abstract class AppSettings
{

    /// <summary>
    /// Clase que contiene la configuración de la base de datos.
    /// </summary>
    public class DataBase
    {
        /// <summary>
        /// Cadena de conexión para la base de datos de FacturacionArca n8n.
        /// </summary>
        public static string? FacturacionArca { get; set; }
    }

    /// <summary>
    /// Clase que contiene la configuración de JWT.
    /// </summary>
    public class Jwt
    {
        /// <summary>
        /// Clave secreta para la generación y validación de tokens JWT.
        /// </summary>
        public static string SecretKey { get; set; } = " ";

        /// <summary>
        /// Emisor del token JWT.
        /// </summary>
        public static string? Issuer { get; set; }

        /// <summary>
        /// Audiencia del token JWT.
        /// </summary>
        public static string? Audience { get; set; }

        /// <summary>
        /// Duración en minutos del token JWT.
        /// </summary>
        public static int Minutes { get; set; }
    }
}