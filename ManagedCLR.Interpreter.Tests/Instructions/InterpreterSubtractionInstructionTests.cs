using ManagedCLR.Interpreter.Tests.Helpers;
using ManagedCLR.Tests.Instructions;

namespace ManagedCLR.Interpreter.Tests.Instructions
{
	public class InterpreterSubtractionInstructionTests : SubtractionInstructionTests
	{
		public InterpreterSubtractionInstructionTests() : base(InterpreterHelper.Instance)
		{
		}
	}
}
