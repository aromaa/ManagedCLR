namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct NopInstruction : ILInstruction
	{
		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static NopInstruction Nop() => new();
	}
}
