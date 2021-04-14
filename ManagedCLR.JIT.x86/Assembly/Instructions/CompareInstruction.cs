using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct CompareInstruction : IAssemblyInstruction
	{
		private OpCodes OpCode { get; init; }
		public int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte(0x48); //REX.W
			writer.WriteByte((byte)this.OpCode);
			writer.WriteByte((byte)this.Value);
		}

		public static CompareInstruction Registers(int x, int y) => new()
		{
			OpCode = OpCodes.CompareRegisters,
			
			Value = 0b11_000_000 | x << 3 | y
		};
	}
}
