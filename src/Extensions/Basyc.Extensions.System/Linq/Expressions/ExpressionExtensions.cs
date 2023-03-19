using System.Linq.Expressions;

namespace Basyc.Extensions.System.Linq.Expressions;

public static class ExpressionExtensions
{
	public static string GetMemberName<TClass, TMember>(
		this Expression<Func<TClass, TMember>> expression)
	{
		var body = expression.Body;
		return GetMemberName(body);
	}

	public static string GetMemberName<TMember>(
		this Expression<Func<TMember>> expression)
	{
		var body = expression.Body;
		return GetMemberName(body);
	}

	private static string GetMemberName(
		Expression expression)
	{
		if (expression is MemberExpression memberExpression)
			return memberExpression.Expression != null && memberExpression.Expression.NodeType ==
				ExpressionType.MemberAccess
				? GetMemberName(memberExpression.Expression) + "." + memberExpression.Member.Name
				: memberExpression.Member.Name;

		return expression is UnaryExpression unaryExpression
			? unaryExpression.NodeType != ExpressionType.Convert
				? throw new Exception($"Cannot interpret member from {expression}")
				: GetMemberName(unaryExpression.Operand)
			: throw new Exception($"Could not determine member from {expression}");
	}
}
