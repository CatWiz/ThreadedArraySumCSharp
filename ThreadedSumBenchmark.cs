using BenchmarkDotNet.Attributes;
using ThreadedArraySum;

namespace ThreadingTest
{
    public class ThreadedSumBenchmark
    {
        public int[] array;

        [Params(1000000)] // 1 mil
        public int N;

        [Params(1, 2, 4, 8, 16, 32, 64)]
        public int threadCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            array = new int[N];
            Random rng = new();
            for (int i = 0; i < N; i++)
                array[i] = rng.Next(1, 100);
        }

        [Benchmark]
        public void MultithreadedSumBenchmark()
        {
            SumComputeThread.MultithreadedSum(array, threadCount);
        }

        [Benchmark]
        public void MultithreadedSumTasksBenchmark()
        {
            SumComputeThread.MultithreadedSumTasks(array, threadCount);
        }
    }

    public class BinaryThreadedSumBenchmark
    {
        public int[] array;

        [Params(1000000)] // 1 mil
        public int n { get; set; }

        [Params(1, 2, 4, 8, 16)]
        public int threadCount { get; set; }

        private ThreadedBinaryArraySumComputer comp;

        [GlobalSetup]
        public void Setup()
        {
            array = new int[n];
            Array.Fill(array, 1);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            comp = new ThreadedBinaryArraySumComputer(threadCount);
        }

        [Benchmark]
        public void ThreadsBenchmark()
        {
            comp.Compute(array);
        }

        [Benchmark]
        public void ThreadedBinaryArraySumComputerBenchmark2()
        {
            ThreadedBinaryArraySumComputer.TasksSum(array);
        }
    }
}