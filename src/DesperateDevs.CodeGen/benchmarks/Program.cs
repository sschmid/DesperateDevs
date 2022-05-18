﻿using BenchmarkDotNet.Running;

namespace DesperateDevs.CodeGen.Benchmarks
{
    static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(CodeGeneratorDataBenchmarks));
        }
    }
}