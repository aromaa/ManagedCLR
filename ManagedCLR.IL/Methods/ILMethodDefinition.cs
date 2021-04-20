using System.Collections.Generic;
using System.Collections.Immutable;
using ManagedCLR.IL.Instructions;

namespace ManagedCLR.IL.Methods
{
	public sealed class ILMethodDefinition
	{
		public ImmutableArray<ILInstruction> Instructions { get; }

		public ImmutableDictionary<int, int> ILOffsets { get; init; }

		public Dictionary<int, BasicBlock> Blocks { get; init; }

		public int ArgumentsCount { get; init; }

		public int MaxStack { get; init; }

		public ILMethodDefinition(IEnumerable<ILInstruction> instructions)
		{
			this.Instructions = instructions.ToImmutableArray();

			this.ILOffsets = ImmutableDictionary<int, int>.Empty;
		}
	}
}
