using BenchmarkDotNet.Running;

namespace DynamicAST.RuleEngine.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<RuleEngineBenchmark>();
    }
}