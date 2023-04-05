using System;
using System.Collections.Concurrent;

namespace Hertzole.CecilAttributes.CodeGen
{
	internal class WeaverPool<T>
	{
		private readonly ConcurrentStack<T> stack;

		private readonly Func<T> createFunc;

		private readonly Action<T> onGet;
		private readonly Action<T> onRelease;

		public WeaverPool(Func<T> createFunc, Action<T> onGet, Action<T> onRelease)
		{
			this.createFunc = createFunc;
			this.onGet = onGet;
			this.onRelease = onRelease;
			stack = new ConcurrentStack<T>();
		}

		public T GetInternal()
		{
			if (stack.Count == 0 || !stack.TryPop(out T obj))
			{
				obj = createFunc();
			}

			onGet?.Invoke(obj);
			return obj;
		}
		
		public PoolScope<T> GetInternal(out T obj)
		{
			if (stack.Count == 0 || !stack.TryPop(out obj))
			{
				obj = createFunc();
			}

			onGet?.Invoke(obj);
			return new PoolScope<T>(obj, this);
		}

		public void ReleaseInternal(T obj)
		{
			onRelease?.Invoke(obj);
			stack.Push(obj);
		}
	}
}