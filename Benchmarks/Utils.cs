
using System.Text;

using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks;
public class Utils
{
    private static Dictionary<string, Summary> _results { get; } = new();

    public static void WriteResults()
    {
        ConsoleLogger.Default.Flush();
        Console.Clear();

        foreach (var (group, summary) in _results)
        {
            ConsoleLogger.Default.WriteLineHeader(group);

            MarkdownExporter.Console.ExportToLog(summary, ConsoleLogger.Default);
        }
    }

    public static void PrintSummary(Summary summary!!)
    {
        MarkdownExporter.Console.ExportToLog(summary, ConsoleLogger.Default);
        //ConclusionHelper.Print(ConsoleLogger.Default, DefaultConfig.Instance.GetAnalysers().SelectMany(x => x.Analyse(summary)).ToList());
    }

    public static Summary RunBenchmark<T>()
    {
        var summary = BenchmarkRunner.Run<T>();

        ConsoleLogger.Default.Flush();
        _results.Add(typeof(T).Name, summary);

        return summary;
    }
}
