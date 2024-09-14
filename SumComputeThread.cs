using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ThreadingTest
{
    internal class SumComputeThread
    {
        private readonly int _start;
        private readonly int _end;
        private int[] _array;
        private readonly Action<Int64> _callback;

        public SumComputeThread(int[] array, int start, int end, Action<Int64> callback)
        {
            _array = array;
            _start = start;
            _end = end;
            _callback = callback;
        }

        public void ThreadProc()
        {
            Int64 sum = _array[_start];
            for (int i = _start + 1; i < _end; i++)
            {
                sum += _array[i];
            }
            _callback(sum);
        }
        public static Int64 MultithreadedSum(int[] array, int threadCount)
        {
            int chunkSize = array.Length / threadCount;
            SumComputeThread[] threads = new SumComputeThread[threadCount];
            Int64 totalSum = 0;
            for (int i = 0; i < threads.Length; i++)
            {
                int start = i * chunkSize;
                int end = (i + 1) * chunkSize;
                if (i == threads.Length - 1)
                    end = array.Length;

                int iCopy = i;
                threads[i] = new SumComputeThread(array, start, end, threadSum =>
                {
                    //Console.WriteLine($"Thread {iCopy + 1} computed sum: {threadSum}");
                    Interlocked.Add(ref totalSum, threadSum);
                });
            }

            Thread[] threadObjects = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threadObjects[i] = new Thread(threads[i].ThreadProc);
                threadObjects[i].Start();
            }

            for (int i = 0; i < threadCount; i++)
            {
                threadObjects[i].Join();
            }

            return totalSum;
        }

        public static Int64 MultithreadedSumTasks(int[] array, int threadCount)
        {
            int chunkSize = array.Length / threadCount;
            Int64 totalSum = 0;
            Task<Int64>[] tasks = new Task<Int64>[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                int start = i * chunkSize;
                int end = (i + 1) * chunkSize;
                if (i == threadCount - 1)
                    end = array.Length;

                tasks[i] = Task.Run(() =>
                {
                    Int64 sum = array[start];
                    for (int j = start + 1; j < end; j++)
                        sum += array[j];
                    
                    return sum;
                });
            }

            Task.WaitAll(tasks);
            foreach (var task in tasks)
                totalSum += task.Result;
            
            return totalSum;
        }
    }
}
