using System.Linq.Expressions;

namespace Application.Extensions;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>>? left, Expression<Func<T, bool>>? right)
    {
        if (left is null && right is null) return _ => true;
        if (left is null) return right!;
        if (right is null) return left!;

        var param = left.Parameters[0];
        var rightBody = new ReplaceParameterVisitor(right.Parameters[0], param).Visit(right.Body)!;
        var body = Expression.AndAlso(left.Body, rightBody);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>>? left, Expression<Func<T, bool>>? right)
    {
        if (left is null && right is null) return _ => false;
        if (left is null) return right!;
        if (right is null) return left!;

        var param = left.Parameters[0];
        var rightBody = new ReplaceParameterVisitor(right.Parameters[0], param).Visit(right.Body)!;
        var body = Expression.OrElse(left.Body, rightBody);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>>? expr)
    {
        if (expr is null) return _ => true;
        var param = expr.Parameters[0];
        var body = Expression.Not(expr.Body);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from, _to;
        public ReplaceParameterVisitor(ParameterExpression from, ParameterExpression to) => (_from, _to) = (from, to);

        protected override Expression VisitParameter(ParameterExpression node) => node == _from ? _to : base.VisitParameter(node);
    }
}
