using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct MoveInstruction : IAssemblyInstruction
	{
		internal bool RexW { get; private init; }

		internal OpCodes OpCode { get; private init; }
		internal long Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			if (this.RexW)
			{
				writer.WriteByte(0x48);
			}

			writer.WriteByte((byte)this.OpCode);

			if (this.OpCode == OpCodes.MoveToRegister)
			{
				writer.WriteByte((byte)this.Value);
			}
			else
			{
				writer.WriteInt64(this.Value);
			}
		}

		internal static MoveInstruction RegisterToRegister(int to, int from) => new()
		{
			RexW = true,

			OpCode = OpCodes.MoveToRegister,
			Value = 0b11_000_000 | to << 3 | from
		};

		internal static MoveInstruction ConstTo(long value, int to) => new()
		{
			RexW = true,
			OpCode = (OpCodes)((int)OpCodes.MoveToRegisterImmediate | to),

			Value = value
		};
	}
}
