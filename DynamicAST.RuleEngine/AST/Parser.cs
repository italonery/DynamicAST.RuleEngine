using DynamicAST.RuleEngine.Lexing;

namespace DynamicAST.RuleEngine.AST;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _position;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    private Token Current => _position < _tokens.Count ? _tokens[_position] : _tokens[^1];

    private Token Advance()
    {
        var current = Current;
        _position++;
        return current;
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Current.Type == type)
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    public ExpressionNode Parse()
    {
        return ParseExpression();
    }

    private ExpressionNode ParseExpression()
    {
        return ParseOr();
    }

    private ExpressionNode ParseOr()
    {
        var left = ParseAnd();

        while (Current.Type == TokenType.Or)
        {
            var operatorToken = Advance();
            var right = ParseAnd();
            left = new BinaryExpressionNode(left, operatorToken.Type, right);
        }

        return left;
    }

    private ExpressionNode ParseAnd()
    {
        var left = ParseComparison();

        while (Current.Type == TokenType.And)
        {
            var operatorToken = Advance();
            var right = ParseComparison();
            left = new BinaryExpressionNode(left, operatorToken.Type, right);
        }

        return left;
    }

    private ExpressionNode ParseComparison()
    {
        var left = ParsePrimary();

        if (Match(TokenType.GreaterThan, TokenType.GreaterThanOrEqual,
                   TokenType.LessThan, TokenType.LessThanOrEqual,
                   TokenType.Equals, TokenType.NotEquals))
        {
            var operatorToken = _tokens[_position - 1];
            var right = ParsePrimary();
            return new BinaryExpressionNode(left, operatorToken.Type, right);
        }

        return left;
    }

    private ExpressionNode ParsePrimary()
    {
        if (Match(TokenType.Number))
        {
            var token = _tokens[_position - 1];
            return new NumberExpressionNode(decimal.Parse(token.Value, System.Globalization.CultureInfo.InvariantCulture));
        }

        if (Match(TokenType.Identifier))
        {
            var token = _tokens[_position - 1];
            return new IdentifierExpressionNode(token.Value);
        }

        if (Match(TokenType.OpenParenthesis))
        {
            var expression = ParseExpression();
            if (!Match(TokenType.CloseParenthesis))
            {
                throw new Exception($"Syntax Error: Expected closing parenthesis at position {_position}");
            }
            return expression;
        }

        throw new Exception($"Syntax Error: Unexpected token '{Current.Value}' of type {Current.Type} at position {_position}");
    }
}
