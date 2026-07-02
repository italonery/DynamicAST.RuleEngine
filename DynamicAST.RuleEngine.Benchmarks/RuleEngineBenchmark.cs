using System.Data;
using BenchmarkDotNet.Attributes;
using DynamicAST.RuleEngine.AST;
using DynamicAST.RuleEngine.Compilation;
using DynamicAST.RuleEngine.Lexing;

namespace DynamicAST.RuleEngine.Benchmarks;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class RuleEngineBenchmark
{
    private readonly Func<Dictionary<string, object>, bool> _compiledRule;
    private readonly Dictionary<string, object> _context;
    
    private readonly DataTable _dataTable;
    private readonly string _dataTableFilter;

    private readonly Func<Dictionary<string, object>, bool> _nativeCSharpRule = 
        ctx => Convert.ToDecimal(ctx["age"]) >= 18 && 
               (Convert.ToDecimal(ctx["score"]) >= 90 || Convert.ToDecimal(ctx["bonus"]) == 0);

    public RuleEngineBenchmark()
    {
        var rule = "age >= 18 AND (score >= 90 OR bonus == 0)";
        var lexer = new Lexer(rule);
        var parser = new Parser(lexer.Tokenize());
        var compiler = new RuleCompiler();
        _compiledRule = compiler.CompileRule(parser.Parse());

        _context = new Dictionary<string, object>
        {
            { "age", 25m },
            { "score", 80m },
            { "bonus", 0m }
        };

        _dataTable = new DataTable();
        _dataTable.Columns.Add("age", typeof(decimal));
        _dataTable.Columns.Add("score", typeof(decimal));
        _dataTable.Columns.Add("bonus", typeof(decimal));
        _dataTable.Rows.Add(25m, 80m, 0m);
        _dataTableFilter = "age >= 18 AND (score >= 90 OR bonus = 0)";
    }

    [Benchmark(Baseline = true)]
    public bool NativeCSharp_Baseline()
    {
        return _nativeCSharpRule(_context);
    }

    [Benchmark]
    public bool Our_Compiled_AST()
    {
        return _compiledRule(_context);
    }

    [Benchmark]
    public bool Naive_DataTable_Compute()
    {
        var rows = _dataTable.Select(_dataTableFilter);
        return rows.Length > 0;
    }
}