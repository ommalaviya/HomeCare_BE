namespace Shared.HomeCare.DataModel.Response
{

    public class FilteredDataQueryResponse<TEntity> : DataQueryResponse<TEntity>
    {
        public FilterRangeMeta FilterMeta { get; set; } = new();
    }
}