using System;

namespace Hertzole.CecilAttributes.CodeGen
{
	public readonly struct PoolScope<T> : IDisposable
	{
		private readonly T value;
		private readonly WeaverPool<T> pool;

		internal PoolScope(T value, WeaverPool<T> pool)
		{
			this.value = value;
			this.pool = pool;
		}

		void IDisposable.Dispose()
		{
			pool.ReleaseInternal(value);
		}
	}
}