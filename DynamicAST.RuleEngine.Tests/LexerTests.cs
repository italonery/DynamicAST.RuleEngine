using DynamicAST.RuleEngine.Lexing;

namespace DynamicAST.RuleEngine.Tests;

public class LexerTests
{
    [Fact]
    public void Tokenize_ShouldExtract_BasicLogicalExpression()
    {
        var rule = "age >= 18 AND score == 10";
        var lexer = new Lexer(rule);

        var tokens = lexer.Tokenize();

        Assert.Equal(8, tokens.Count); 

        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("age", tokens[0].Value);

        Assert.Equal(TokenType.GreaterThanOrEqual, tokens[1].Type);
        Assert.Equal(">=", tokens[1].Value);

        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal("18", tokens[2].Value);

        Assert.Equal(TokenType.And, tokens[3].Type);
        Assert.Equal("AND", tokens[3].Value);

        Assert.Equal(TokenType.Identifier, tokens[4].Type);
        Assert.Equal("score", tokens[4].Value);

        Assert.Equal(TokenType.Equals, tokens[5].Type);
        Assert.Equal("==", tokens[5].Value);

        Assert.Equal(TokenType.Number, tokens[6].Type);
        Assert.Equal("10", tokens[6].Value);

        Assert.Equal(TokenType.EndOfFile, tokens[7].Type);
    }

    [Theory]
    [InlineData("dicountRate > 50", TokenType.GreaterThan, ">")]
    [InlineData("dicountRate < 50", TokenType.LessThan, "<")]
    [InlineData("dicountRate <= 50", TokenType.LessThanOrEqual, "<=")]
    [InlineData("dicountRate != 50", TokenType.NotEquals, "!=")]
    public void Tokenize_ShouldExtract_LookaheadOperators(string rule, TokenType expectedType, string expectedValue)
    {
        var lexer = new Lexer(rule);

        var tokens = lexer.Tokenize();

        Assert.Equal(expectedType, tokens[1].Type);
        Assert.Equal(expectedValue, tokens[1].Value);
    }
    
    [Fact]
    public void Tokenize_ShouldHandle_ParenthesesAndWhitespace()
    {
        var rule = "  ( weight >= 90 )  ";
        var lexer = new Lexer(rule);

        var tokens = lexer.Tokenize();

        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.OpenParenthesis, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal(TokenType.GreaterThanOrEqual, tokens[2].Type);
        Assert.Equal(TokenType.Number, tokens[3].Type);
        Assert.Equal(TokenType.CloseParenthesis, tokens[4].Type);
        Assert.Equal(TokenType.EndOfFile, tokens[5].Type);
    }
}
