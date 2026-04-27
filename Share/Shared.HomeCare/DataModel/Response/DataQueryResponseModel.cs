namespace Shared.HomeCare.DataModel.Response
{
    public class DataQueryResponseModel<TResponseModel> where TResponseModel : class
    {
        public IEnumerable<TResponseModel> Records { get; set; } = Enumerable.Empty<TResponseModel>();
        public int TotalRecords { get; set; }
         //Added by Pankil
        public string? Message { get; set; }
    }
}