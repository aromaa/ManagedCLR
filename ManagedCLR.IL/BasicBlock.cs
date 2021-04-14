using System.Collections.Generic;
using ManagedCLR.IL.Instructions;

namespace ManagedCLR.IL
{
	public sealed class BasicBlock
	{
		public List<ILInstruction> instructions = new();

		public void Add(ILInstruction instruction)
		{
			this.instructions.Add(instruction);
		}
	}
}
