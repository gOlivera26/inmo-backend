namespace Inmo24.Application.Services.Implementations;

public class CloudflareR2StorageService : IStorageService
{
    private readonly string _bucketName;
    private readonly string _publicUrlBase;
    private readonly AmazonS3Client _s3Client;

    public CloudflareR2StorageService(IConfiguration configuration)
    {
        var r2Config = configuration.GetSection("CloudflareR2");

        _bucketName = r2Config["BucketName"] ?? throw new ArgumentNullException("BucketName no configurado");
        _publicUrlBase = r2Config["PublicUrlBase"]?.TrimEnd('/') ?? throw new ArgumentNullException("PublicUrlBase no configurado");

        var accessKey = r2Config["AccessKey"];
        var secretKey = r2Config["SecretKey"];
        var serviceUrl = r2Config["ServiceURL"];

        var config = new AmazonS3Config
        {
            ServiceURL = serviceUrl,
        };

        _s3Client = new AmazonS3Client(accessKey, secretKey, config);
    }

    public async Task<string> SubirImagenAsync(IFormFile file, Guid tenantId, string prefix = "propiedades")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("El archivo está vacío.");

        var ext = Path.GetExtension(file.FileName).ToLower();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowedExtensions.Contains(ext))
            throw new InvalidOperationException("Formato de archivo no permitido. Solo imágenes JPG, PNG o WEBP.");

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var fileName = $"{tenantId}/{prefix}/{timestamp}_{Guid.NewGuid().ToString()[..8]}{ext}";

        using var stream = file.OpenReadStream();

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = file.ContentType,
            DisablePayloadSigning = true
        };

        var response = await _s3Client.PutObjectAsync(putRequest);

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return $"{_publicUrlBase}/{fileName}";
        }

        throw new Exception("Error al subir el archivo a Cloudflare R2.");
    }

    public async Task<bool> BorrarImagenAsync(string fileUrl)
    {
        try
        {
            var fileKey = fileUrl.Replace($"{_publicUrlBase}/", "");

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileKey
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> SubirDocumentoAsync(IFormFile file, Guid tenantId, string prefix = "documentos")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("El archivo está vacío.");

        var ext = Path.GetExtension(file.FileName).ToLower();
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".webp" };

        if (!allowedExtensions.Contains(ext))
            throw new InvalidOperationException("Formato no permitido. Use PDF, Word, Excel o Imágenes.");

        var safeOriginalName = System.Text.RegularExpressions.Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^a-zA-Z0-9_\-]", "_");
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        // Formato final: /tenant_id/documentos/20260315123000_MiContrato.pdf
        var fileName = $"{tenantId}/{prefix}/{timestamp}_{safeOriginalName}{ext}";

        using var stream = file.OpenReadStream();

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = file.ContentType,
            DisablePayloadSigning = true
        };

        var response = await _s3Client.PutObjectAsync(putRequest);

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return $"{_publicUrlBase}/{fileName}";
        }

        throw new Exception("Error al subir el documento a Cloudflare R2.");
    }
}