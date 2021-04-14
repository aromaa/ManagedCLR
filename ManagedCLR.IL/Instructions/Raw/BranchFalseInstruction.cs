using System;

namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct BranchFalseInstruction : ILInstruction
	{
		public int ToIndex { get; internal init; }

		public bool Branch => true;

		public T GetValue<T>()
		{
			if (typeof(T) == typeof(int))
			{
				return (T)(object)this.ToIndex;
			}

			throw new NotSupportedException();
		}

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static BranchFalseInstruction ILIndex(int index) => new()
		{
			ToIndex = index
		};
	}
}
