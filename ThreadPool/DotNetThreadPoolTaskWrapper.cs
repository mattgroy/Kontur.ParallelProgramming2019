using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPool
{
	public class DotNetThreadPoolTaskWrapper : IThreadPool
	{
		private const int PoolSize = 16;
		
//		private List<Thread> threadList = new List<Thread>();
		private readonly Thread[] threadArr = new Thread[PoolSize];
//		private Queue<int> threadQueue = new Queue<int>(PoolSize);
		private readonly ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();
		
		private readonly object locker = new object();

		private int cnt;

		public DotNetThreadPoolTaskWrapper()
		{
			for (var i = 0; i < threadArr.Length; i++)
			{
				var t = new Thread(() =>
				{
					Action action;
					while (true)
					{
						if (actionQueue.TryDequeue(out action))
						{
							action();
						}
						else
						{
							lock (locker)
							{
								Monitor.Wait(locker);								
							}
						}
					}
					
				}) {IsBackground = true};
				t.Start();
				threadArr[i] = t;
			}
		}
		
		public void EnqueueAction(Action action)
		{
//			Task.Run(action);
//			if (threadQueue.Count < 1)
//			{
//				threadQueue.Enqueue(new Thread(() =>
//				{
//					
//				}));
//			}
			actionQueue.Enqueue(action);
			lock (locker)
			{
				Monitor.Pulse(locker);				
			}
		}

		public long GetTasksProcessedCount()
		{
			return cnt;
		}

		public long GetWastedCycles()
		{
			return -1;
		}
	}
}
