namespace DynamicAST.RuleEngine.Lexing;

public enum TokenType
{
    // Literals
    Number,
    String,
    Identifier,

    // Logical & Comparision Operators
    Equals,
    NotEquals,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    And,
    Or,

    // Punctuation
    OpenParenthesis,
    CloseParenthesis,

    // Control
    EndOfFile,
    Unknown,
}
