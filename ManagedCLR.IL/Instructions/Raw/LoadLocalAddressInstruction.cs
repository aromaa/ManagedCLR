using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct LoadLocalAddressInstruction : ILInstruction
	{
		public int Index { get; internal init; }

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static LoadLocalAddressInstruction FromIndex(int index) => new()
		{
			Index = index
		};
	}
}
