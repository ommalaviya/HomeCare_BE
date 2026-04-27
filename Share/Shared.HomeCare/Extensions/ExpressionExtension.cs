using System.Linq.Expressions;
namespace Shared.Extensions
{
    public static class ExpressionExtension
    {
        public static Expression<Func<TEntity, bool>> Add<TEntity>(this Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, bool>> condition)
        {
            try
            {
                var toInvoke = Expression.Invoke(condition, expression.Parameters);
                return Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(expression.Body, toInvoke), expression.Parameters);
            }
            catch
            {
                return Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(expression, condition));
            }
        }
    }
}

 