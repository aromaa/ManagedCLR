using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	public readonly struct SubtractInstruction : IAssemblyInstruction
	{
		public byte Extension { get; private init; }
		public int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte(0x48); //REX.W
			writer.WriteByte((byte)OpCodes.SubtractImmediate);
			writer.WriteByte(this.Extension);
			writer.WriteInt32(this.Value);
		}

		public static SubtractInstruction ConstantFrom(int value, int register) => new()
		{
			Value = value,
			Extension = (byte)(0b11_000_000 | 0b101 << 3 | register)
		};
	}
}
