namespace Shared.HomeCare.DataModel.Request
{
    public class PageRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
    }
}