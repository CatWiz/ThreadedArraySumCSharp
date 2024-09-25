using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Numerics;
using ThreadedArraySum;

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
                int n = Convert.ToInt32(1e6);
                Console.WriteLine($"Generating array of {n} elements...");
                int[] array = new int[n];
                Array.Fill(array, 1);

                Console.WriteLine("Enter thread count:");
                int threadCount;
                while (!ValidateThreadCount(Console.ReadLine(), out threadCount))
                {
                    Console.WriteLine("Invalid thread count. Enter a positive integer:");
                }

                ThreadedBinaryArraySumComputer sumComputer = new(threadCount);

                int time = Environment.TickCount;
                int sum = sumComputer.Compute(array);
                time = Environment.TickCount - time;
                Console.WriteLine($"Sum: {sum}");
                Console.WriteLine($"Time: {time}ms");
            }
            else if (input == "2")
            {
                // Benchmark Result: https://files.catbox.moe/1gpx63.png
                BenchmarkDotNet.Running.BenchmarkRunner.Run<BinaryThreadedSumBenchmark>();
            }
            else
            {
                Console.WriteLine("What the hell man.");
            }
        }
    }
}