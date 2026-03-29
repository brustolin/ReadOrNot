namespace ReadOrNot.Application.DTOs.Reports;

public sealed record TokenReportDto(
    int TotalOpens,
    int UniqueOpensApproximation,
    DateTime? FirstOpenUtc,
    DateTime? LastOpenUtc,
    IReadOnlyCollection<ChartPointDto> DailyCounts,
    IReadOnlyCollection<OpenEventDto> Events);
