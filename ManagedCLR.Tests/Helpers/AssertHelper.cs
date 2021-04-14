using System;

namespace ManagedCLR.Tests.Helpers
{
	public abstract class AssertHelper
	{
		private readonly IAssertHelper Helper;

		protected AssertHelper(IAssertHelper helper)
		{
			this.Helper = helper;
		}

		protected internal void Assert(Action method) => this.Helper.Result(method);

		protected internal void Assert<TOut>(Func<TOut> method) => Xunit.Assert.Equal(method(), this.Helper.Result(method));
		protected internal void Assert<T1, TOut>(Func<T1, TOut> method, T1 arg1) => Xunit.Assert.Equal(method(arg1), this.Helper.Result(method, arg1));
		protected internal void Assert<T1, T2, TOut>(Func<T1, T2, TOut> method, T1 arg1, T2 arg2) => Xunit.Assert.Equal(method(arg1, arg2), this.Helper.Result(method, arg1, arg2));
	}
}
