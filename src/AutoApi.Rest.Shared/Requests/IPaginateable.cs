namespace AutoApi.Rest.Shared.Requests;

public interface IPaginateable
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
