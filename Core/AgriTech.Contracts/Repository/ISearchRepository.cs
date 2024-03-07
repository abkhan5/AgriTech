
namespace AgriTech;

public record SearchMetadataDto
{
    public string SearchId { get; set; }
    public string PreviousPageToken { get; set; }
    public string NextPageToken { get; set; }

    public int TotalRecords { get; set; }
    public int MaxRecords { get; set; }
    public int TotalPages => TotalRecords == 0 || MaxRecords == 0 ? 0 : TotalRecords / MaxRecords;
    public int PageNumber { get; set; }
    public int PageRecordCount { get; set; }
    public string UserId { get; set; }
    public string RequestId { get; set; }
    public string GetCacheKey() => $"SearchRequestKey~{SearchId}";
}

public record SearchPartFieldDto
{
    public string JoinFilter { get; set; }
    public string WhereFilter { get; set; }
    public string Projection { get; set; }
    public string OrderQuery { get; set; }
    public int MaxRecords { get; set; } = 20;

    public string GetQuery() => $"{Projection} {WhereFilter} {OrderQuery}";

}