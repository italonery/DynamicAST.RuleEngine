namespace DynamicAST.RuleEngine.Lexing;

public ref struct Lexer
{
    private ReadOnlySpan<char> _text;
    private int _position;

    public Lexer(string text)
    {
        _text = text.AsSpan();
        _position = 0;
    }

    private readonly char Current => _position < _text.Length ? _text[_position] : '\0';

    public void Advance()
    {
        _position++;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_position < _text.Length)
        {
            if (char.IsWhiteSpace(Current))
            {
                Advance();
                continue;
            }

            if (char.IsDigit(Current))
            {
                tokens.Add(ReadNumber());
                continue;
            }

            if (char.IsLetter(Current) || Current == '_')
            {
                tokens.Add(ReadIdentifierOrKeyword());
                continue;
            }

            if (Current is '>' or '<' or '=' or '!' or '(' or ')')
            {
                tokens.Add(ReadOperator());
                continue;
            }

            tokens.Add(new Token(TokenType.Unknown, Current.ToString()));
            Advance();
        }

        tokens.Add(new Token(TokenType.EndOfFile, string.Empty));
        return tokens;
    }

    private Token ReadNumber()
    {
        int start = _position;
        bool hasDot = false;

        while (char.IsDigit(Current) || (Current == '.' && !hasDot))
        {
            if (Current == '.') hasDot = true;
            Advance();
        }

        var span = _text.Slice(start, _position - start);
        return new Token(TokenType.Number, span.ToString());
    }

    private Token ReadIdentifierOrKeyword()
    {
        int start = _position;

        while (char.IsLetterOrDigit(Current) || Current == '_')
        {
            Advance();
        }

        var span = _text.Slice(start, _position - start);
        var value = span.ToString();

        return value.ToUpperInvariant() switch
        {
            "AND" => new Token(TokenType.And, value),
            "OR" => new Token(TokenType.Or, value),
            _ => new Token(TokenType.Identifier, value),
        };
    }

    private Token ReadOperator()
    {
        char current = Current;
        Advance();

        switch (current)
        {
            case '=':
                if (Current == '=')
                {
                    Advance();
                    return new Token(TokenType.Equals, "==");
                }
                return new Token(TokenType.Unknown, "=");

            case '!':
                if (Current == '=')
                {
                    Advance();
                    return new Token(TokenType.NotEquals, "!=");
                }
                return new Token(TokenType.Unknown, "!");

            case '>':
                if (Current == '=')
                {
                    Advance();
                    return new Token(TokenType.GreaterThanOrEqual, ">=");
                }
                return new Token(TokenType.GreaterThan, ">");

            case '<':
                if (Current == '=')
                {
                    Advance();
                    return new Token(TokenType.LessThanOrEqual, "<=");
                }
                return new Token(TokenType.LessThan, "<");

            case '(':
                return new Token(TokenType.OpenParenthesis, "(");

            case ')':
                return new Token(TokenType.CloseParenthesis, ")");

            default:
                return new Token(TokenType.Unknown, current.ToString());
        }
    }
}
