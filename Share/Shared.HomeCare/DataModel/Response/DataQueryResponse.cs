namespace Shared.HomeCare.DataModel.Response
{
    public class DataQueryResponse<TEntity>
    {
        public IEnumerable<TEntity> Records { get; set; } = new List<TEntity>();
        public int TotalRecords { get; set; }
    }
}