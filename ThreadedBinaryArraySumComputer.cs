using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedArraySum
{
    internal class ThreadedBinaryArraySumComputer
    {
        private class ThreadData
        {
            public int[] targetArray;
            public int index1;
            public int index2;
            public bool doesWork = false;
            public bool shouldExit = false;

            public void Process()
            {
                while (true)
                {
                    if (doesWork)
                    {
                        targetArray[index1] += targetArray[index2];
                        doesWork = false;
                    }
                    if (shouldExit)
                        break;
                }
            }
        }
        public readonly int ThreadCount;
        private readonly ThreadData[] _threadData;
        private readonly Thread[] _threads;
        public ThreadedBinaryArraySumComputer(int threadCount)
        {
            ThreadCount = threadCount;
            _threadData = new ThreadData[threadCount];
            _threads = new Thread[threadCount];
            
            
            for (int i = 0; i < threadCount; i++)
            {
                _threadData[i] = new ThreadData();
                _threads[i] = new Thread(_threadData[i].Process);
            }
        }

        public int Compute(int[] targetArray)
        {
            int[] arrCopy = new int[targetArray.Length];
            Array.Copy(targetArray, arrCopy, targetArray.Length);

            for (int i = 0; i < ThreadCount; i++)
            {
                var threadData = _threadData[i];
                threadData.targetArray = arrCopy;
                _threads[i].Start();
            }

            int n = targetArray.Length;
            while (n > 1)
            {
                int threadIndex = 0;
                for (int i = 0; i < n / 2; i++)
                {
                    while (_threadData[threadIndex].doesWork)
                    {
                        threadIndex = (threadIndex + 1) % ThreadCount;
                    }

                    _threadData[threadIndex].index1 = i;
                    _threadData[threadIndex].index2 = n - i - 1;
                    _threadData[threadIndex].doesWork = true;
                }

                foreach (var threadData in _threadData)
                    while (threadData.doesWork);

                n = (n + 1) / 2;
            }

            for (int i = 0; i < ThreadCount; i++)
            {
                _threadData[i].shouldExit = true;
                _threads[i].Join();
            }
            return arrCopy[0];
        }

        public static int SingleThreadSum(int[] array)
        {
            int[] arr = new int[array.Length];
            Array.Copy(array, arr, array.Length);

            int n = arr.Length;
            while (n > 1)
            {
                for (int i = 0; i < n / 2; i++)
                {
                    arr[i] += arr[n - i - 1];
                }
                n = (n + 1) / 2;
                Console.WriteLine(String.Join(' ', arr));
            }
            return arr[0];
        }

        public static int TasksSum(int[] array)
        {
            int[] arr = new int[array.Length];
            Array.Copy(array, arr, array.Length);

            int n = arr.Length;
            while (n > 1)
            {
                Task[] tasks = new Task[n / 2];
                for (int i = 0; i < n / 2; i++)
                {
                    int iCopy = i;
                    tasks[i] = Task.Run(() => arr[iCopy] += arr[n - iCopy - 1]);
                }
                Task.WaitAll(tasks);
                n = (n + 1) / 2;
                //Console.WriteLine(String.Join(' ', arr));
            }
            return arr[0];
        }

    }
}
