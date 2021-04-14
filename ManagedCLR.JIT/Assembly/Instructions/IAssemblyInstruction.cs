using System.Reflection.Metadata;

namespace ManagedCLR.JIT.Assembly.Instructions
{
	public interface IAssemblyInstruction
	{
		public void Write(ref BlobWriter writer);
	}
}
