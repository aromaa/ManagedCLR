namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct LoadLocalInstruction : ILInstruction
	{
		public int Index { get; internal init; }

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static LoadLocalInstruction FromIndex(int index) => new()
		{
			Index = index
		};
	}
}
