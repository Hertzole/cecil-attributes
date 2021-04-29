using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.Editor
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
    }
}
