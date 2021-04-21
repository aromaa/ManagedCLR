using System;
using System.Collections.Generic;
using ManagedCLR.IL;
using ManagedCLR.IL.Instructions;
using ManagedCLR.IL.Instructions.Parser;
using ManagedCLR.IL.Instructions.Raw;

namespace ManagedCLR.Runtime.Assembly.Parser
{
	internal sealed class ILMethodReader : ILReader
	{
		internal List<ILInstruction> instructions;

		internal Dictionary<int, int> ilOffset;
		internal Dictionary<int, int> instructionOffset;

		private SortedSet<int> leaders;

		internal Dictionary<int, BasicBlock> blocks;

		internal ILMethodReader()
		{
			this.instructions = new List<ILInstruction>();

			this.ilOffset = new Dictionary<int, int>();
			this.instructionOffset = new Dictionary<int, int>();

			this.leaders = new SortedSet<int>();

			this.blocks = new Dictionary<int, BasicBlock>();
		}

		protected override void ReadStart()
		{
			this.instructions.Clear();
			this.leaders.Clear();
			this.ilOffset.Clear();
			this.instructionOffset.Clear();
			this.blocks.Clear();
		}

		protected override void Read<T>(T instruction, int offset)
		{
			int index = this.instructions.Count;
			if (index > 0 && this.instructions[index - 1].Branch)
			{
				this.leaders.Add(offset);
			}

			this.instructions.Add(instruction);
			this.ilOffset.Add(offset, index);
			this.instructionOffset.Add(index, offset);

			if (instruction.Branch)
			{
				this.leaders.Add(instruction.GetValue<int>());
			}
		}

		protected override void ReadEnd()
		{
			//Workaround for empty list bcs I'm lazy
			this.leaders.Add(int.MaxValue);

			//Construct basic blocks, this could be optimized
			for (int i = 0; i < this.instructions.Count; )
			{
				BasicBlock block = new();

				int cutOff = this.leaders.Min;

				this.leaders.Remove(cutOff);

				int offset = this.instructionOffset[i];

				while (i < this.instructions.Count)
				{
					if (this.instructionOffset[i] == cutOff)
					{
						break;
					}

					block.Add(this.instructions[i++]);
				}

				this.blocks.Add(offset, block);
			}
		}
	}
}
