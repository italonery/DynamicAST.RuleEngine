# DynamicAST.RuleEngine

A high-performance, dynamic rule engine for .NET. Parses text-based logic into an Abstract Syntax Tree (AST) and compiles it at runtime using `System.Linq.Expressions` for zero-reflection, near-native execution.

## 🚀 The Engineering Challenge

Modern applications often require dynamic business rules (e.g., dynamic questionnaires, custom pricing engines, or medical triage) that change at runtime. The naive approach—using `DataTable.Compute` or heavy Reflection—leads to massive memory allocations, Garbage Collector (GC) pressure, and poor CPU utilization.

This project solves that by implementing a custom **Lexer**, a recursive descent **Parser**, and an **IL Compiler** that generates optimized `Func<T, bool>` delegates on the fly. 

## 📊 Performance Benchmarks

This engine was designed with a strict **Zero-Allocation** philosophy for rule evaluation. By utilizing `ReadOnlySpan<char>` during the lexing phase and native Expression Trees during compilation, the engine evaluates dynamic rules at near-native speeds without polluting the Heap.

**Environment:** .NET 10.0 | X64 RyuJIT | 13th Gen Intel Core i7

| Method                  | Mean        | Ratio | Gen0   | Allocated | Alloc Ratio |
|------------------------ |------------:|------:|-------:|----------:|------------:|
| **NativeCSharp_Baseline**| 15.52 ns    |  1.00 |      - |         - |          NA |
| **Our_Compiled_AST** | **18.55 ns**|**1.20**|      **-** |     **-** |      **NA** |
| Naive_DataTable_Compute | 1,207.51 ns | 77.84 | 0.4444 |    5592 B |          NA |

### Key Takeaways:
* **Zero Garbage:** The compiled AST executes with **0 bytes** of memory allocated per operation, entirely eliminating GC pauses during high-throughput rule evaluation.
* **Extreme Throughput:** At `18.55 ns` per execution, the engine is approximately **65x faster** than traditional dynamic evaluation methods.
* **Near-Native Speed:** The dynamic compiled delegate runs only `~3 ns` slower than hardcoded, compile-time C# logic.

## 🛠️ Architecture

1. **Lexing:** A `ref struct` zero-allocation tokenizer that slices strings using `ReadOnlySpan<char>`.
2. **Parsing:** A Recursive Descent Parser that strictly enforces mathematical and logical operator precedence.
3. **Compilation:** Maps the immutable AST to `System.Linq.Expressions`, utilizing short-circuit evaluation (`AndAlso` / `OrElse`) and reflection-emit for dictionary property access.

## 📜 License
This project is licensed under the MIT License.