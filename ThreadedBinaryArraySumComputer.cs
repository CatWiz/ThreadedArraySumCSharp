using Perfolizer.Horology;
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
            public long workingStatus = 0; // 0 - not working, 1 - working
            public object lockObject = new();
            public bool shouldExit = false;

            public void Process()
            {
                while (true)
                {
                    lock (lockObject)
                    {
                        while (workingStatus == 0 && !shouldExit)
                            Monitor.Wait(lockObject);
                        if (shouldExit)
                            break;

                        targetArray[index1] += targetArray[index2];
                        workingStatus = 0;
                    }
                }
            }
        }
        public readonly int ThreadCount;
        private readonly ThreadData[] _threadData;
        private readonly Thread[] _threads;
        private bool started = false;
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

        public void StartThreads()
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                _threads[i].Start();
            }
            started = true;
        }

        public int Compute(int[] targetArray)
        {
            if (!started)
                StartThreads();

            int[] arrCopy = new int[targetArray.Length];
            Array.Copy(targetArray, arrCopy, targetArray.Length);

            for (int i = 0; i < ThreadCount; i++)
            {
                _threadData[i].targetArray = arrCopy;
            }

            int n = targetArray.Length;
            while (n > 1)
            {
                int threadIndex = 0;
                for (int i = 0; i < n / 2; i++)
                {
                    while (Interlocked.Read(ref _threadData[threadIndex].workingStatus) != 0)
                    {
                        threadIndex = (threadIndex + 1) % ThreadCount;
                    }

                    var threadData = _threadData[threadIndex];
                    lock (threadData.lockObject)
                    {
                        threadData.index1 = i;
                        threadData.index2 = n - i - 1;
                        threadData.workingStatus = 1;
                        Monitor.Pulse(threadData.lockObject);
                    }
                }

                foreach (var threadData in _threadData)
                {
                    while (Interlocked.Read(ref threadData.workingStatus) != 0) { }
                }

                n = (n + 1) / 2;
            }
            
            return arrCopy[0];
        }

        public void Shutdown()
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                _threadData[i].shouldExit = true;
                lock (_threadData[i].lockObject)
                    Monitor.Pulse(_threadData[i].lockObject);

                _threads[i].Join();
            }
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
