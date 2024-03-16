namespace AgriTech.Dto;

public record PagedRequestResponse
{
    public string CurrentPageToken { get; set; }
    public string NextPageToken { get; set; }
    public int TotalRecords { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
