namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct AddInstruction : ILInstruction
	{
		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static AddInstruction Add() => new();
	}
}
