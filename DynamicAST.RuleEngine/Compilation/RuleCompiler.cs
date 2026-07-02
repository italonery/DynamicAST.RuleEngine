using System.Linq.Expressions;
using DynamicAST.RuleEngine.AST;
using DynamicAST.RuleEngine.Lexing;

namespace DynamicAST.RuleEngine.Compilation;

public class RuleCompiler
{
    private static readonly ParameterExpression ContextParameter = Expression.Parameter(typeof(Dictionary<string, object>), "context");

    public Func<Dictionary<string, object>, bool> CompileRule(ExpressionNode rootNode)
    {
        var body = BuildExpression(rootNode);

        if (body.Type != typeof(bool))
        {
            throw new InvalidOperationException("The root expression must evaluate to a boolean.");
        }

        var lambda = Expression.Lambda<Func<Dictionary<string, object>, bool>>(body, ContextParameter);
        
        return lambda.Compile(); 
    }

    private Expression BuildExpression(ExpressionNode node)
    {
        return node switch
        {
            NumberExpressionNode numberNode => Expression.Constant(numberNode.Value, typeof(decimal)),
            
            IdentifierExpressionNode idNode => BuildIdentifierExpression(idNode),
            
            BinaryExpressionNode binaryNode => BuildBinaryExpression(binaryNode),
            
            _ => throw new NotSupportedException($"Node type {node.GetType().Name} is not supported.")
        };
    }

    private Expression BuildIdentifierExpression(IdentifierExpressionNode node)
    {
        var keyExpr = Expression.Constant(node.Name);
        var getItemMethod = typeof(Dictionary<string, object>).GetMethod("get_Item")!;
        var dictCall = Expression.Call(ContextParameter, getItemMethod, keyExpr);
        var convertMethod = typeof(Convert).GetMethod(nameof(Convert.ToDecimal), new[] { typeof(object) })!;
        
        return Expression.Call(convertMethod, dictCall);
    }

    private Expression BuildBinaryExpression(BinaryExpressionNode node)
    {
        var left = BuildExpression(node.Left);
        var right = BuildExpression(node.Right);

        return node.Operator switch
        {
            TokenType.Equals => Expression.Equal(left, right),
            TokenType.NotEquals => Expression.NotEqual(left, right),
            TokenType.GreaterThan => Expression.GreaterThan(left, right),
            TokenType.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
            TokenType.LessThan => Expression.LessThan(left, right),
            TokenType.LessThanOrEqual => Expression.LessThanOrEqual(left, right),            
            TokenType.And => Expression.AndAlso(left, right), 
            TokenType.Or => Expression.OrElse(left, right),   
            _ => throw new NotSupportedException($"Operator {node.Operator} is not supported.")
        };
    }
}
