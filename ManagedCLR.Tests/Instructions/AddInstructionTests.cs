using ManagedCLR.Tests.Helpers;
using Xunit;

namespace ManagedCLR.Tests.Instructions
{
	[Collection("Instructions")]
	public abstract class AddInstructionTests : AssertHelper
	{
		protected AddInstructionTests(IAssertHelper helper) : base(helper)
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
				return a + b;
			}

			this.Assert(Test, a, b);
		}
		
		[Theory]
		[InlineData(-1, -1)]
		[InlineData(0, 0)]
		[InlineData(1, 1)]
		[InlineData(1.1, 1.3)]
		public void Single(float a, float b)
		{
			static float Test(float a, float b)
			{
				return a + b;
			}

			this.Assert(Test, a, b);
		}
	}
}
