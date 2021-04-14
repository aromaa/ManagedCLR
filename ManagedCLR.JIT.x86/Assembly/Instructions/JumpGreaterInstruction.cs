using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct JumpGreaterInstruction : IAssemblyInstruction
	{
		internal int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			//writer.WriteByte(0x0F);
			writer.WriteByte((byte)OpCodes.JumpGreater);
			writer.WriteByte((byte)this.Value);
		}

		public static JumpGreaterInstruction JumpTo(int to) => new()
		{
			Value = to
		};
	}
}
