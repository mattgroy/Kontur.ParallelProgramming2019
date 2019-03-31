/*
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace FindTimeQuant
{
	class Program
	{
		static void Main(string[] args)
		{
			var processorNum = args.Length > 0 ? int.Parse(args[0]) - 1 : 0;
			Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << processorNum);

//			var deltSum = 0L;
//			var cnt = 0;
//			var sw = Stopwatch.StartNew();
//			var id = 1;
//			var lastTime = 0L;
//			var thread1 = new Thread(() =>
//			{
//				for (var i = 0; i < 1e9; i++)
//					if (id == 0)
//					{
//						cnt += 1;
//						id = (id + 1) % 2;
//						lastTime = sw.ElapsedMilliseconds;
//					}
//
//				
//			});
//			thread1.Start();
//			for (var i = 0; i < 1e9; i++)
//				if (id == 1)
//				{
//					id = (id + 1) % 2;
//					deltSum += (sw.ElapsedMilliseconds - lastTime);
//					lastTime = sw.ElapsedMilliseconds;
//				}
//			
//			Console.WriteLine(deltSum / cnt);

			var cnt = 0;
			var l = new List<Thread>();
			var sw = Stopwatch.StartNew();
			while (sw.ElapsedMilliseconds < 1000)
			{
				var t = new Thread(() => Thread.Sleep(2));
				l.Add(t);
				t.Start();
//				cnt++;
			}

			Console.WriteLine(l.Count);
		}
	}
}
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace FindTimeQuant
{
	class Program
	{	
		private static void Main()
		{
			var sw = new Stopwatch();
			var length = 1000;
			var col = GetIntsCollection(length);
			
//			sw.Start();
//			var colRes = col
//				.AsParallel()
//				.WithDegreeOfParallelism(4)
//				.Select(i => Thread.CurrentThread.ManagedThreadId);
//			
//			sw.Stop();
//			foreach (var t in colRes)
//			{
//				Console.Write(t);
//			}
			
			var ien = GetIntsIEnumerable(col);
			sw.Start();
			var ienRes = ien
				.AsParallel()
//				.WithDegreeOfParallelism(4)
				.Select(i => Thread.CurrentThread.ManagedThreadId);
			
			sw.Stop();
			foreach (var t in ienRes)
			{
				Console.Write(t + " ");
			}
		}

//		private IEnumerable<int> GetIntsIEnumerable(int length)
//		{
//			var rand = new Random();
//			for (var i = 0; i < length; i++)
//			{
//				yield return rand.Next();
//			}
//		}

		private static IEnumerable<int> GetIntsIEnumerable(IEnumerable<int> arr)
		{
			foreach (var e in arr)
			{
				yield return e;
			}
		}

		private static int[] GetIntsCollection(int length)
		{
			var arr = new int[length];
			var rand = new Random();
			for (var i = 0; i < length; i++)
			{
				arr[i] = rand.Next(10);
			}

			return arr;
		}
	}
}
