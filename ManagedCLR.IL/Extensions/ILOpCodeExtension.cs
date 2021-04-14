using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace ManagedCLR.IL.Extensions
{
	internal static class ILOpCodeExtension
	{
		private static readonly OpCode[] opCodes = ILOpCodeExtension.GetOpCodes();

		private static OpCode[] GetOpCodes()
		{
			OpCode[] array = new OpCode[ushort.MaxValue];
			foreach (FieldInfo field in typeof(OpCodes).GetFields())
			{
				OpCode opCode = (OpCode)field.GetValue(null)!;
				
				array[(ushort)opCode.Value] = opCode;
			}

			return array;
		}

		internal static OperandType GetOperandType(this ILOpCode opCode) => ILOpCodeExtension.opCodes[(int)opCode].OperandType;
	}
}
