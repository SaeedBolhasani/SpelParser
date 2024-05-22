using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using SpelParser.Generated;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using static SpelParser.Generated.SpelGrammerParser;

namespace SpelParser;

public class SpelGrammerCompiler<T> : SpelGrammerBaseVisitor<Expression>
{
    private static readonly ParameterExpression _param = Expression.Parameter(typeof(T), "p");

    private static string GetString(ConstantContext context) => context.GetText().Trim('"', '\'');

    public override Expression VisitString([NotNull] StringContext context)
    {
        return Expression.Constant(GetString(context), typeof(string));
    }

    public override Expression VisitNumber([NotNull] NumberContext context)
    {
        return Expression.Constant(GetString(context));
    }

    public override Expression VisitConstant([NotNull] ConstantContext context)
    {
        return base.Visit(context);
    }

    public override Expression VisitField([NotNull] FieldContext context)
    {
        var fieldNameToken = context.FIELD().GetText();
        var fieldExpression = typeof(T).GetProperty(fieldNameToken, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)!;

        ArgumentNullException.ThrowIfNull(fieldExpression, fieldNameToken);
        return Expression.Property(_param, fieldExpression.Name);
    }

    public override Expression VisitAnd([NotNull] AndContext context)
    {
        return Expression.AndAlso(Visit(context.expression(0)), Visit(context.expression(1)));
    }

    public override Expression VisitOr([NotNull] OrContext context)
    {
        return Expression.OrElse(Visit(context.expression(0)), Visit(context.expression(1)));
    }

    public override Expression VisitParenthesis([NotNull] ParenthesisContext context)
    {
        return Visit(context.expression());
    }

    public override Expression VisitEqualExpression([NotNull] EqualExpressionContext context)
    {
        return Expression.Equal(CreateCallCompareToMethodExpression(context.field(), context.constant()), Expression.Constant(0));
    }

    public override Expression VisitNotEqualExpression([NotNull] NotEqualExpressionContext context)
    {
        return Expression.NotEqual(CreateCallCompareToMethodExpression(context.field(), context.constant()), Expression.Constant(0));
    }

    public override Expression VisitGreaterThanExpression([NotNull] GreaterThanExpressionContext context)
    {
        return Expression.GreaterThan(CreateCallCompareToMethodExpression(context.field(), context.constant()), Expression.Constant(0));
    }

    public override Expression VisitGreaterThanOrEqualExpression([NotNull] GreaterThanOrEqualExpressionContext context)
    {
        return Expression.GreaterThanOrEqual(CreateCallCompareToMethodExpression(context.field(), context.constant()), Expression.Constant(0));
    }

    public override Expression VisitLessThanExpression([NotNull] LessThanExpressionContext context)
    {
        return Expression.LessThan(CreateCallCompareToMethodExpression(context.field(), context.constant()), Expression.Constant(0));
    }

    public override Expression VisitLessThanOrEqualExpression([NotNull] LessThanOrEqualExpressionContext context)
    {
        return Expression.LessThan(CreateCallCompareToMethodExpression(context.field(), context.constant()), Expression.Constant(0));
    }

    private MethodCallExpression CreateCallCompareToMethodExpression(FieldContext fieldContext, ConstantContext constantContext)
    {
        var fieldExpression = VisitField(fieldContext);
        var fieldType = fieldExpression.Type;

        Expression constantExpression;
        if (fieldType.BaseType == typeof(Enum))
        {
            var underlyingType = Enum.GetUnderlyingType(fieldType);
            var constant = Convert.ChangeType(Enum.Parse(fieldType, GetString(constantContext), true), underlyingType);
            fieldExpression = Expression.Convert(fieldExpression, underlyingType);
            fieldType = underlyingType;
            constantExpression = Expression.Constant(constant);           
        }
        else
        {
            constantExpression = CreateValueExpression(fieldType, constantContext);
        }

        return Expression.Call(fieldExpression, fieldType.GetMethod("CompareTo", [fieldType])!, constantExpression);
    }

    private Expression CreateValueExpression(Type fieldExpression, ConstantContext constantContext)
    {
        var constantExpression = Visit(constantContext);

        if (fieldExpression == typeof(string))
            return constantExpression;

        if (fieldExpression.GetMethod("Parse", [typeof(string)]) != null)
            return Expression.Call(fieldExpression, "Parse", Type.EmptyTypes, constantExpression);

        throw new ArgumentException("Unknown type", fieldExpression.ToString());
    }

    public Expression<Func<T, bool>> CreateFunc(string input)
    {
        var inputStream = new AntlrInputStream(input);
        var lexer = new SpelGrammerLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new SpelGrammerParser(tokenStream);
        var query = parser.query();

        var result = Visit(query);

        return Expression.Lambda<Func<T, bool>>(result, _param);
    }
}
