namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct PushInt64Instruction : ILInstruction
	{
		public long Value { get; internal init; }

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static PushInt64Instruction Constant(long value) => new()
		{
			Value = value
		};
	}
}