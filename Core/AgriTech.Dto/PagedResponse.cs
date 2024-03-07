namespace AgriTech.Dto;

public record PagedResponse<T>
{
    public PagedResponse()
    {
        Records = new List<T>();
    }

    public string PreviousPageToken { get; set; }
    public string NextPageToken { get; set; }
    public List<T> Records { get; set; }
    public int TotalRecords { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}