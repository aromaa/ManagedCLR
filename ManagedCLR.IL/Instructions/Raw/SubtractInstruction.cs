namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct SubtractInstruction : ILInstruction
	{
		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static SubtractInstruction Subtract() => new();
	}
}
