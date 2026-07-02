namespace DynamicAST.RuleEngine.Lexing;

public readonly record struct Token(TokenType Type, string Value)
{
    public override string ToString()
    {
        return $"[{Type}: '{Value}']";
    }
}
