using BenchmarkDotNet.Running;

BenchmarkRunner.Run<Performance>();

Console.ReadKey();

//public class Program
//{
//    public static void Main()
//    {

//        var p = new Performance();
//        p.SetupTreeText();
//        Console.WriteLine("Ready for profiling");

//        Console.ReadKey();
//        Console.WriteLine(Tree.FromText(p.TreeText));

//        Console.WriteLine("Profiling done");
//        Console.ReadKey();
//    }
//}