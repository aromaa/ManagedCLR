using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct ReturnInstruction : IAssemblyInstruction
	{
		public OpCodes OpCode { get; private init; }
		public int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte((byte)this.OpCode);

			if (this.OpCode == OpCodes.NearReturnPop)
			{
				writer.WriteInt16((short)this.Value);
			}
		}

		public static ReturnInstruction NearReturn() => new()
		{
			OpCode = OpCodes.NearReturn,
		};

		public static ReturnInstruction NearReturnPop(int amount) => new()
		{
			OpCode = OpCodes.NearReturnPop,

			Value = amount,
		};
	}
}
