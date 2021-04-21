using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct PushInstruction : IAssemblyInstruction
	{
		internal OpCodes OpCode { get; private init; }

		internal int Register { get; private init; }
		internal int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte((byte)this.OpCode);

			switch (this.OpCode)
			{
				case OpCodes.PushImmediate8:
					writer.WriteByte((byte)this.Value);
					break;
				case OpCodes.PushImmediate32:
					writer.WriteInt32(this.Value);
					break;
				case OpCodes.PushMemory:
					writer.WriteByte((byte)this.Register);
					writer.WriteByte((byte)this.Value);
					break;
			}
		}

		internal static PushInstruction FromRegister(int register) => new()
		{
			OpCode = (OpCodes)((int)OpCodes.PushRegister | register),
		};

		internal static PushInstruction PushRbp() => new()
		{
			OpCode = OpCodes.PushRbp
		};

		internal static PushInstruction FromRsp(int value) => new()
		{
			OpCode = OpCodes.PushMemory,
			Register = 0x74 | 0x24 << 8,
			Value = value
		};

		internal static PushInstruction FromRbp(int value) => new()
		{
			OpCode = OpCodes.PushMemory,
			Register = 0x75,
			Value = value
		};

		internal static PushInstruction Constant(sbyte value) => new()
		{
			OpCode = OpCodes.PushImmediate8,
			Value = value
		};

		internal static PushInstruction Constant(int value) => new()
		{
			OpCode = OpCodes.PushImmediate32,
			Value = value
		};

		internal static PushInstruction FromRax(int value) => new()
		{
			OpCode = OpCodes.PushMemory,
			Register = 0x70,
			Value = value
		};
	}
}
