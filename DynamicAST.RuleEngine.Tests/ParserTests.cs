using DynamicAST.RuleEngine.AST;
using DynamicAST.RuleEngine.Lexing;

namespace DynamicAST.RuleEngine.Tests;

public class ParserTests
{
    [Fact]
    public void Parse_ShouldBuild_CorrectAstForBasicExpression()
    {
        var rule = "weight >= 10 AND score == 5";
        var lexer = new Lexer(rule);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens);

        var ast = parser.Parse();

        var root = Assert.IsType<BinaryExpressionNode>(ast);
        Assert.Equal(TokenType.And, root.Operator);

        var leftNode = Assert.IsType<BinaryExpressionNode>(root.Left);
        Assert.Equal(TokenType.GreaterThanOrEqual, leftNode.Operator);
        
        var leftIdentifier = Assert.IsType<IdentifierExpressionNode>(leftNode.Left);
        Assert.Equal("weight", leftIdentifier.Name);
        
        var leftNumber = Assert.IsType<NumberExpressionNode>(leftNode.Right);
        Assert.Equal(10m, leftNumber.Value);

        var rightNode = Assert.IsType<BinaryExpressionNode>(root.Right);
        Assert.Equal(TokenType.Equals, rightNode.Operator);
        
        var rightIdentifier = Assert.IsType<IdentifierExpressionNode>(rightNode.Left);
        Assert.Equal("score", rightIdentifier.Name);
        
        var rightNumber = Assert.IsType<NumberExpressionNode>(rightNode.Right);
        Assert.Equal(5m, rightNumber.Value);
    }
}
