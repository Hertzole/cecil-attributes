using System;

namespace Hertzole.CecilAttributes.CodeGen
{
	public static class Logger
	{
		public static void Log(string message)
		{
#if UNITY_2020_2_OR_NEWER
			Console.WriteLine(message);
#else
			UnityEngine.Debug.Log(message);
#endif
		}
	}
}