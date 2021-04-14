using ManagedCLR.Tests.Helpers;
using Xunit;

namespace ManagedCLR.Tests.Instructions
{
	[Collection("Instructions")]
	public abstract class CallInstructionTests : AssertHelper
	{
		protected CallInstructionTests(IAssertHelper helper) : base(helper)
		{
		}

		[Fact]
		public void CallVoidMethod()
		{
			static void Test()
			{
			}

			this.Assert(Test);
		}

		[Fact]
		public void ReturnInt()
		{
			static int Test()
			{
				return 0x33_33_33_33;
			}

			this.Assert(Test);
		}

		[Fact]
		public void ReturnLong()
		{
			static long Test()
			{
				return 0x33_33_33_33__33_33_33_33;
			}

			this.Assert(Test);
		}

		[Theory]
		[InlineData(3)] //Int
		[InlineData(3L)] //Long
		[InlineData("string")] //String
		public void ReturnsFirstArgument<T>(T a)
		{
			static T Test(T a)
			{
				return a;
			}

			this.Assert(Test, a);
		}
	}
}
