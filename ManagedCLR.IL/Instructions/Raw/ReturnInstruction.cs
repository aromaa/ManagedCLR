namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct ReturnInstruction : ILInstruction
	{
		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static ReturnInstruction Return() => new();
	}
}
