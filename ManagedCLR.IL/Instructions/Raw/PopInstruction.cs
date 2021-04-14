namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct PopInstruction : ILInstruction
	{
		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static PopInstruction Pop() => new();
	}
}
