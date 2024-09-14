using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Numerics;

namespace ThreadingTest
{
    public class Program
    {
        private static bool ValidateThreadCount(string? input, out int threadCount)
        {
            int count;
            
            if (string.IsNullOrWhiteSpace(input))
            {
                threadCount = -1;
                return false;
            }
            
            if (!int.TryParse(input, out count))
            {
                threadCount = -1;
                return false;
            }

            if (count < 1)
            {
                threadCount = -1;
                return false;
            }

            threadCount = count;
            return true;
        }


        public static void Main()
        {
            Console.WriteLine("1 - Manual test; 2 - Benchmark test");
            string input = Console.ReadLine()!;

            if (input == "1")
            {
                int N = 1000000000;
                Console.WriteLine($"Generating array of size {N}");

                int[] array = new int[N];
                Random rng = new();
                for (int i = 0; i < N; i++)
                    array[i] = rng.Next(1, 100);


                Console.WriteLine("Enter number of threads:");
                int threadCount;
                while (!ValidateThreadCount(Console.ReadLine(), out threadCount))
                {
                    Console.WriteLine("Invalid input. Please enter a valid number of threads:");
                }

                int time = Environment.TickCount;
                //BigInteger sum = SumComputeThread.MultithreadedSum(array, threadCount);
                Int64 sum = SumComputeThread.MultithreadedSumTasks(array, threadCount);
                int elapsed = Environment.TickCount - time;

                Console.WriteLine($"Sum computed by all threads: {sum}");
                Console.WriteLine($"Time elapsed: {elapsed} ms");
            }
            else if (input == "2")
            {
                // Benchmark Result: https://files.catbox.moe/1gpx63.png
                BenchmarkDotNet.Running.BenchmarkRunner.Run<ThreadedSumBenchmark>();
            }
            else
            {
                Console.WriteLine("What the hell man.");
            }
        }
    }
}