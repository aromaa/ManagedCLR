namespace ManagedCLR.JIT
{
	public abstract class BaseJIT
	{
		public abstract void WriteStub(nuint slot, nuint memory, int maxStack);
	}
}
