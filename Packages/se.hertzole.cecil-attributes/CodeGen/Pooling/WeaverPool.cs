using System;
using System.Collections.Concurrent;

namespace Hertzole.CecilAttributes.CodeGen
{
	internal class WeaverPool<TObj>
	{
		private readonly ConcurrentStack<TObj> stack;

		private readonly Func<TObj> createFunc;

		private readonly Action<TObj> onGet;
		private readonly Action<TObj> onRelease;

		public WeaverPool(Func<TObj> createFunc, Action<TObj> onGet, Action<TObj> onRelease)
		{
			this.createFunc = createFunc;
			this.onGet = onGet;
			this.onRelease = onRelease;
			stack = new ConcurrentStack<TObj>();
		}

		public TObj GetInternal()
		{
			if (stack.Count == 0 || !stack.TryPop(out TObj obj))
			{
				obj = createFunc();
			}

			onGet?.Invoke(obj);
			return obj;
		}

		public void ReleaseInternal(TObj obj)
		{
			onRelease?.Invoke(obj);
			stack.Push(obj);
		}
	}
}