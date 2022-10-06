using System.Collections.Generic;

namespace Hertzole.CecilAttributes.CodeGen
{
	internal static class ListPool<T> 
	{
		private static readonly WeaverPool<List<T>> pool = new WeaverPool<List<T>>(() => new List<T>(), null, OnRelease);

		private static void OnRelease(List<T> list)
		{
			list.Clear();
		}

		public static List<T> Get()
		{
			return pool.GetInternal();
		}

		public static void Release(List<T> item)
		{
			pool.ReleaseInternal(item);
		}
	}
}