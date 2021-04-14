using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct JumpInstruction : IAssemblyInstruction
	{
		internal OpCodes OpCode { get; private init; }
		internal int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte((byte)this.OpCode);

			if (this.OpCode == OpCodes.JumpShort)
			{
				writer.WriteByte((byte)this.Value);
			}
			else
			{
				writer.WriteInt32(this.Value);
			}
		}

		internal static JumpInstruction JumpShort(int to) => new()
		{
			OpCode = OpCodes.JumpShort,

			Value = to
		};

		internal static JumpInstruction JumpNear(int to) => new()
		{
			OpCode = OpCodes.JumpNearRelative,

			Value = to
		};
	}
}
