using System.Diagnostics;

using Benchmarks;

using static Benchmarks.Utils;

try
{
    var parsing = RunBenchmark<Parsing>();
    var deserialization = RunBenchmark<Deserialization>();

    WriteResults();
}
catch { }

Console.WriteLine("All is done. Press any key to close this window");

//var p = new Parsing();
//p.SetupData();

//GC.Collect();

//Console.WriteLine("Waiting for start");

//Thread.Sleep(1000);

//var tree = p.TreeSharp();

Console.ReadKey();