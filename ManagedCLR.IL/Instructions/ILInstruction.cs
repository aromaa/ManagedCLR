using System;

namespace ManagedCLR.IL.Instructions
{
	public interface ILInstruction
	{
		public bool Branch => false;

		public T GetValue<T>() => throw new NotSupportedException();

		public void Consume(ILConsumer reader) => throw new NotSupportedException();
	}
}
