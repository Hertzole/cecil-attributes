using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace Hertzole.CecilAttributes.CodeGen
{
    public static class WeaverHelpers
    {
        public static OpCode GetBoolOpCode(bool value)
        {
            return value == true ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
        }

        public static Instruction GetIntInstruction(int value)
        {
            if (value == 0)
            {
                return Instruction.Create(OpCodes.Ldc_I4_0);
            }
            else if (value == 1)
            {
                return Instruction.Create(OpCodes.Ldc_I4_1);
            }
            else if (value == 2)
            {
                return Instruction.Create(OpCodes.Ldc_I4_2);
            }
            else if (value == 3)
            {
                return Instruction.Create(OpCodes.Ldc_I4_3);
            }
            else if (value == 4)
            {
                return Instruction.Create(OpCodes.Ldc_I4_4);
            }
            else if (value == 5)
            {
                return Instruction.Create(OpCodes.Ldc_I4_5);
            }
            else if (value == 6)
            {
                return Instruction.Create(OpCodes.Ldc_I4_6);
            }
            else if (value == 7)
            {
                return Instruction.Create(OpCodes.Ldc_I4_7);
            }
            else if (value == 8)
            {
                return Instruction.Create(OpCodes.Ldc_I4_8);
            }
            else if (value > 8 && value < 127)
            {
                return Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)value);
            }
            else
            {
                return Instruction.Create(OpCodes.Ldc_I4, value);
            }
        }
        
        public static AssemblyDefinition AssemblyDefinitionFor(ICompiledAssembly compiledAssembly)
        {
            PostProcessorAssemblyResolver assemblyResolver = new PostProcessorAssemblyResolver(compiledAssembly);
            ReaderParameters readerParameters = new ReaderParameters
            {
                SymbolStream = new MemoryStream(compiledAssembly.InMemoryAssembly.PdbData),
                SymbolReaderProvider = new PortablePdbReaderProvider(),
                AssemblyResolver = assemblyResolver,
                ReflectionImporterProvider = new PostProcessorReflectionImporterProvider(),
                ReadingMode = ReadingMode.Immediate,
                ReadSymbols = true
            };

            AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(new MemoryStream(compiledAssembly.InMemoryAssembly.PeData), readerParameters);

            // Apparently, it will happen that when we ask to resolve a type that lives inside Hertzole.CecilAttributes, and we
            // are also postprocessing Hertzole.CecilAttributes, type resolving will fail, because we do not actually try to resolve
            // inside the assembly we are processing. Let's make sure we do that, so that we can use postprocessor features inside
            // Hertzole.CecilAttributes itself as well.
            assemblyResolver.AddAssemblyDefinitionBeingOperatedOn(assemblyDefinition);

            return assemblyDefinition;
        }
    }
}
