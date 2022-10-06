using System.Collections.Generic;

namespace Hertzole.CecilAttributes.CodeGen
{
	public static class DictionaryPool<TKey, TValue>
	{
		private static readonly WeaverPool<Dictionary<TKey, TValue>> pool = new WeaverPool<Dictionary<TKey, TValue>>(() => new Dictionary<TKey, TValue>(), null, OnRelease);

		private static void OnRelease(Dictionary<TKey, TValue> dict)
		{
			dict.Clear();
		}

		public static Dictionary<TKey, TValue> Get()
		{
			return pool.GetInternal();
		}

		public static void Release(Dictionary<TKey, TValue> item)
		{
			pool.ReleaseInternal(item);
		}
	}
}