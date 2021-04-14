using ManagedCLR.Interpreter.Tests.Helpers;
using ManagedCLR.Tests.Instructions;

namespace ManagedCLR.Interpreter.Tests.Instructions
{
	public class InterpreterAddInstructionTests : AddInstructionTests
	{
		public InterpreterAddInstructionTests() : base(InterpreterHelper.Instance)
		{
		}
	}
}
