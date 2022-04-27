using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
	public sealed class SimpleTextReader : IDisposable
	{
		private readonly string[] lines;
		private readonly Dictionary<string, string> stringKeys;
		private readonly Dictionary<string, object> stringValues;

		private readonly Regex regex = new Regex("(^.*?): (.*)");

		public SimpleTextReader(string path)
		{
			lines = File.ReadAllLines(path);
			stringKeys = new Dictionary<string, string>(lines.Length);
			stringValues = new Dictionary<string, object>(lines.Length);

			SetupStringKeys();
		}

		private void SetupStringKeys()
		{
			for (int i = 0; i < lines.Length; i++)
			{
				Match match = regex.Match(lines[i]);
				if (match.Groups.Count != 3)
				{
					continue;
				}

				stringKeys.Add(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
			}
		}

		public bool ReadBoolean(string name, bool defaultValue = false)
		{
			if (stringValues.TryGetValue(name, out object result))
			{
				return (bool) result;
			}

			if (!stringKeys.TryGetValue(name, out string stringResult))
			{
				Debug.LogWarning($"There's no string key called {name}.");
				return defaultValue;
			}

			switch (stringResult.ToLowerInvariant())
			{
				case "true":
					stringValues.Add(name, true);
					return true;
				case "false":
					stringValues.Add(name, false);
					return false;
				default:
					return defaultValue;
			}
		}

		public int ReadInt(string name, int defaultValue = 0)
		{
			if (stringValues.TryGetValue(name, out object result))
			{
				return (int) result;
			}

			if (!stringKeys.TryGetValue(name, out string stringResult))
			{
				Debug.LogWarning($"There's no string key called {name}.");
				return defaultValue;
			}

			if (int.TryParse(stringResult, out int intResult))
			{
				stringValues.Add(name, intResult);
				return intResult;
			}

			return defaultValue;
		}

		public string ReadString(string name, string defaultValue = "")
		{
			if (stringValues.TryGetValue(name, out object result))
			{
				return (string) result;
			}

			if (!stringKeys.TryGetValue(name, out string stringResult))
			{
				Debug.LogWarning($"There's no string key called {name}.");
				return defaultValue;
			}

			if (stringResult.StartsWith("'") && stringResult.EndsWith("'"))
			{
				return stringResult.Substring(1, stringResult.Length - 2);
			}

			return defaultValue;
		}

		public void Dispose()
		{
			stringKeys?.Clear();
			stringValues?.Clear();
		}
	}
}