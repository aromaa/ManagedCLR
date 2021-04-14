using ManagedCLR.Interpreter.Tests.Helpers;
using ManagedCLR.Tests.Instructions;

namespace ManagedCLR.Interpreter.Tests.Instructions
{
	public class InterpreterCallInstructionTests : CallInstructionTests
	{
		public InterpreterCallInstructionTests() : base(InterpreterHelper.Instance)
		{
		}
	}
}
