namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct StoreLocalInstruction : ILInstruction
	{
		public int Index { get; internal init; }

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static StoreLocalInstruction ToIndex(int index) => new()
		{
			Index = index
		};
	}
}
