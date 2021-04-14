using ManagedCLR.Tests.Helpers;
using Xunit;

namespace ManagedCLR.Tests.Instructions
{
	[Collection("Instructions")]
	public abstract class SubtractionInstructionTests : AssertHelper
	{
		protected SubtractionInstructionTests(IAssertHelper helper) : base(helper)
		{
		}

		[Theory]
		[InlineData(int.MinValue, -1)]
		[InlineData(-1, -1)]
		[InlineData(0, 0)]
		[InlineData(1, 1)]
		[InlineData(int.MaxValue, 1)]
		public void Int32(int a, int b)
		{
			static int Test(int a, int b)
			{
				return a - b;
			}
			
			this.Assert(Test, a, b);
		}
	}
}
