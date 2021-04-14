using ManagedCLR.JIT.Tests.Helpers;
using ManagedCLR.Tests.Instructions;

namespace ManagedCLR.JIT.Tests.Instructions
{
	public class JITAddInstructionTests : AddInstructionTests
	{
		public JITAddInstructionTests() : base(JITHelper.Instance)
		{
		}
	}
}
