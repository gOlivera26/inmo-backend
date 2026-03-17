namespace Inmo24.Application.RequestDto.Common;

public class PaginatedRequest
{
    [Required]
    public int Page { get; set; } = 1;

    [Required]
    public int PageSize { get; set; } = 10;
}