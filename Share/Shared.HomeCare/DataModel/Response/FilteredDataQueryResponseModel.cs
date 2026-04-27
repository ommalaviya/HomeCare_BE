namespace Shared.HomeCare.DataModel.Response
{
    public class FilterRangeMeta
    {
        public decimal? MaxAmount { get; set; }
        public int?  MaxBookedServices { get; set; }
        public int? MaxBookingCount { get; set; }
        // Extend here for other range types that you need:
    }

    public class FilteredDataQueryResponseModel<TResponseModel> : DataQueryResponseModel<TResponseModel>
        where TResponseModel : class
    {
        public FilterRangeMeta FilterMeta { get; set; } = new();
    }
}