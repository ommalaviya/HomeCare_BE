using System.Linq.Expressions;
namespace Shared.HomeCare.DataModel.Response
{
    public class ExpressionIncluder<TEntity> where TEntity : class
    {
        public Expression<Func<TEntity, object>>[] Includes { get; set; } = null!;

        public List<Func<IQueryable<TEntity>, IQueryable<TEntity>>>? ThenIncludes { get; set; }
    }
}
