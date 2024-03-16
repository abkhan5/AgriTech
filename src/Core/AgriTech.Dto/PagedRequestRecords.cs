namespace AgriTech.Dto;

/// <summary>
///     Paged list interface
/// </summary>
public record PagedRequestRecords<T>
{
    public PagedRequestRecords()
    {
        Response = new PagedRequestResponse();
    }

    public string RequestedPageToken { get; set; }
    public int MaxItems { get; set; } = 50;
    public PagedRequestResponse Response { get; }
}
