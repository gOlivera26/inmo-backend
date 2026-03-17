namespace Inmo24.Application.ResponseDto.Dashboard;

public class KpiDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string ValueFormatted { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty;
    public bool TrendingUp { get; set; }
}