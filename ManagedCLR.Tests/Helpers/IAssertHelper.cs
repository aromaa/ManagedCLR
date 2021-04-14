using System;

namespace ManagedCLR.Tests.Helpers
{
	public interface IAssertHelper
	{
		public void Result(Action method);

		public TOut Result<TOut>(Func<TOut> method);
		public TOut Result<T1, TOut>(Func<T1, TOut> method, T1 arg1);
		public TOut Result<T1, T2, TOut>(Func<T1, T2, TOut> method, T1 arg1, T2 arg2);
	}
}
