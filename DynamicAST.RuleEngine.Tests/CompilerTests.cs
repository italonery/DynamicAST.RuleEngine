using DynamicAST.RuleEngine.AST;
using DynamicAST.RuleEngine.Compilation;
using DynamicAST.RuleEngine.Lexing;

namespace DynamicAST.RuleEngine.Tests;

public class CompilerTests
{
    [Theory]
    [InlineData(25, 80, 0, true)]   
    [InlineData(17, 100, 0, false)] 
    [InlineData(30, 50, 1, false)]  
    public void CompileRule_ShouldEvaluate_DynamicContextCorrectly(decimal age, decimal score, decimal bonus, bool expectedResult)
    {
        var rule = "age >= 18 AND (score >= 90 OR bonus == 0)";
        
        var lexer = new Lexer(rule);
        var tokens = lexer.Tokenize();
        
        var parser = new Parser(tokens);
        var ast = parser.Parse();
        
        var compiler = new RuleCompiler();

        var compiledRule = compiler.CompileRule(ast);

        var context = new Dictionary<string, object>
        {
            { "age", age },
            { "score", score },
            { "bonus", bonus }
        };

        var result = compiledRule(context);

        Assert.Equal(expectedResult, result);
    }
}
