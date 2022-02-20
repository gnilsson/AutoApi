namespace AutoApi.Rest.Settings;

public class PaginationSettings
{
    public int DefaultPageSize { get; init; } = 20;
    public int MaxPageSize { get; init; } = 50;
}
