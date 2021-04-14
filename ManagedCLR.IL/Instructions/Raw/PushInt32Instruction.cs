namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct PushInt32Instruction : ILInstruction
	{
		public int Value { get; internal init; }

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static PushInt32Instruction Constant(int value) => new()
		{
			Value = value
		};
	}
}
