using DynamicAST.RuleEngine.Lexing;

namespace DynamicAST.RuleEngine.AST;

public abstract record ExpressionNode;

public record NumberExpressionNode(decimal Value) : ExpressionNode;

public record IdentifierExpressionNode(string Name) : ExpressionNode;

public record BinaryExpressionNode(ExpressionNode Left, TokenType Operator, ExpressionNode Right) : ExpressionNode;
