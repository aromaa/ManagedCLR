using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct CallInstruction : IAssemblyInstruction
	{
		internal OpCodes OpCode { get; private init; }
		internal int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte((byte)this.OpCode);
			writer.WriteByte((byte)this.Value);
		}

		internal static CallInstruction NearRelative(int offset) => new()
		{
			OpCode = OpCodes.CallNearRelative,
			Value = offset
		};

		internal static CallInstruction NearAbsolute(int to) => new()
		{
			OpCode = OpCodes.CallNearAbsolute,
			Value = 0b11_000_000 | 0b010 << 3 | to
		};
	}
}
