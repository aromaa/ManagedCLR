using System.Reflection.Metadata;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct PopInstruction : IAssemblyInstruction
	{
		internal OpCodes Register { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte((byte)this.Register);
		}

		internal static PopInstruction ToRegister(int register) => new()
		{
			Register = OpCodes.PopRegister | (OpCodes)register
		};

		internal static PopInstruction PopRbp() => new()
		{
			Register = OpCodes.PopRbp
		};
	}
}
