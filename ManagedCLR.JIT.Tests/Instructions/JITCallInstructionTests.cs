using ManagedCLR.JIT.Tests.Helpers;
using ManagedCLR.Tests.Instructions;

namespace ManagedCLR.JIT.Tests.Instructions
{
	public class JITCallInstructionTests : CallInstructionTests
	{
		public JITCallInstructionTests() : base(JITHelper.Instance)
		{
		}
	}
}
