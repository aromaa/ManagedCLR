namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct LoadArgumentInstruction : ILInstruction
	{
		public int Index { get; internal init; }

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static LoadArgumentInstruction FromIndex(int index) => new()
		{
			Index = index
		};
	}
}
