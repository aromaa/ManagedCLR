namespace ManagedCLR.IL.Instructions
{
	public interface ILConsumer
	{
		public void Consume<T>(in T instruction) where T : ILInstruction;
	}
}
