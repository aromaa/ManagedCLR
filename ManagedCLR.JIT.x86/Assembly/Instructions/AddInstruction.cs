using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	public readonly struct AddInstruction : IAssemblyInstruction
	{
		private OpCodes OpCode { get; init; }

		public byte Extension { get; private init; }
		public int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte(0x48); //REX.W
			writer.WriteByte((byte)this.OpCode);
			writer.WriteByte(this.Extension);

			if (this.OpCode == OpCodes.AddImmediate)
			{
				writer.WriteInt32(this.Value);
			}
		}

		public static AddInstruction ConstantFrom(int value, int register) => new()
		{
			OpCode = OpCodes.AddImmediate,
			Value = value,
			Extension = (byte)(0b11_000_000 | 0b000 << 3 | register)
		};

		public static AddInstruction Register(int to, int from) => new()
		{
			OpCode = OpCodes.AddRegister,
			Extension = (byte)(0b11_000_000 | to << 3 | from)
		};
	}
}
