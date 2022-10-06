namespace Hertzole.CecilAttributes.CodeGen
{
	internal static class StaticPool<T> where T : class, new()
	{
		private static readonly WeaverPool<T> pool = new WeaverPool<T>(() => new T(), null, null);

		public static T Get()
		{
			return pool.GetInternal();
		}

		public static void Release(T item)
		{
			pool.ReleaseInternal(item);
		}
	}
}