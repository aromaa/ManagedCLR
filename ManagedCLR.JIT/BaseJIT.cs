namespace ManagedCLR.JIT
{
	public abstract class BaseJIT
	{
		public abstract void WriteStub(uint slot, nuint memory, int maxStack);
	}
}
