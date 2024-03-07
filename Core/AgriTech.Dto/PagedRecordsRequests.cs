namespace AgriTech.Dto;

public record PagedRecordsRequests<T>
{
    public PagedRecordsRequests()
    {
        Response = new PagedResponse<T>
        {
            PageSize = 50
        };
    }

    public string RequestedPageToken { get; set; }
    public PagedResponse<T> Response { get; }
}
