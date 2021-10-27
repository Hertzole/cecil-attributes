//#if !UNITY_2020_2_OR_NEWER
using System.IO;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
{
	public class ILPostProcessCompiledAssembly : ICompiledAssembly
	{
		private readonly string assemblyFilename;
		private readonly string outputPath;
		private InMemoryAssembly inMemoryAssembly;

		public string Name { get; }
		public string[] References { get; }
		public string[] Defines { get; }

		public InMemoryAssembly InMemoryAssembly
		{
			get
			{
				if (inMemoryAssembly == null)
				{
					inMemoryAssembly = new InMemoryAssembly(
						File.ReadAllBytes(Path.Combine(outputPath, assemblyFilename)),
						File.ReadAllBytes(Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(assemblyFilename)}.pdb")));
				}

				return inMemoryAssembly;
			}
		}

		public ILPostProcessCompiledAssembly(string asmName, string[] refs, string[] defines, string outputPath)
		{
			assemblyFilename = asmName;
			Name = Path.GetFileNameWithoutExtension(assemblyFilename);
			References = refs;
			Defines = defines;
			this.outputPath = outputPath;

			Debug.Log($"{assemblyFilename} Refs:");
			for (int i = 0; i < refs.Length; i++)
			{
				Debug.Log(refs[i]);
			}
		}
	}
}
// #endif