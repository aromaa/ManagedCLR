using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ManagedCLR.JIT.Assembly.Instructions;

namespace ManagedCLR.JIT.x86.Assembly.Instructions
{
	internal readonly struct LoadEffectiveAddressInstruction : IAssemblyInstruction
	{
		internal OpCodes OpCode { get; private init; }

		internal int Register { get; private init; }
		internal int Value { get; private init; }

		public void Write(ref BlobWriter writer)
		{
			writer.WriteByte((byte)this.OpCode);
			writer.WriteByte((byte)this.Register);
			writer.WriteByte((byte)this.Value);
		}

		public static LoadEffectiveAddressInstruction Ebp(int offset) => new()
		{
			OpCode = OpCodes.LoadEffectiveAddress,

			Register = 0x4D,
			Value = offset
		};
	}
}
