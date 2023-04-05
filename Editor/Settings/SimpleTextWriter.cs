using System;
using System.IO;
using System.Text;

namespace Hertzole.CecilAttributes.Editor
{
	public sealed class SimpleTextWriter : IDisposable
	{
		private readonly string path;
		private readonly TextWriter writer;

		public SimpleTextWriter(string path)
		{
			this.path = path;
			writer = new StringWriter(new StringBuilder());
			WriteInt("version", 1);
		}

		public void WriteString(string name, string value)
		{
			WriteName(name);
			writer.WriteLine($"'{value}'");
		}

		public void WriteBoolean(string name, bool value)
		{
			WriteName(name);
			writer.WriteLine(value ? "true" : "false");
		}

		public void WriteInt(string name, int value)
		{
			WriteName(name);
			writer.WriteLine(value);
		}

		private void WriteName(string name)
		{
			writer.Write($"{name}: ");
		}

		public void Dispose()
		{
			File.WriteAllText(path, writer.ToString());

			writer?.Dispose();
		}
	}
}