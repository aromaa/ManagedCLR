using ManagedCLR.JIT.Tests.Helpers;
using ManagedCLR.Tests.Instructions;

namespace ManagedCLR.JIT.Tests.Instructions
{
	public class JITSubtractionInstructionTests : SubtractionInstructionTests
	{
		public JITSubtractionInstructionTests() : base(JITHelper.Instance)
		{
		}
	}
}
