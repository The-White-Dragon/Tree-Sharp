using System.Threading;
using System;
using SharpTree;

var p = new Parsing();
p.SetupData();

GC.Collect();

Console.WriteLine("Waiting for start");

Thread.Sleep(1000);

var tree = p.TreeSharp();

Console.ReadKey();