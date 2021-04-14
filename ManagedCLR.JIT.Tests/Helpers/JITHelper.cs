using ManagedCLR.JIT.x86;
using ManagedCLR.Tests.Helpers;
using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using AppDomain = ManagedCLR.Runtime.Domains.AppDomain;

namespace ManagedCLR.JIT.Tests.Helpers
{
	internal sealed class JITHelper : IAssertHelper
	{
		internal static readonly JITHelper Instance = new();

		public void Result(Action method) => JITHelper.JitInternal<ValueTuple>(method.Method);

		public TOut Result<TOut>(Func<TOut> method) => JITHelper.JitInternal<TOut>(method.Method);
		public TOut Result<T1, TOut>(Func<T1, TOut> method, T1 arg1) => JITHelper.JitInternal<TOut>(method.Method, arg1);
		public TOut Result<T1, T2, TOut>(Func<T1, T2, TOut> method, T1 arg1, T2 arg2) => JITHelper.JitInternal<TOut>(method.Method, arg1, arg2);

		private static unsafe TOut JitInternal<TOut>(MethodInfo methodInfo, params object[] arguments)
		{
			MethodDefinitionHandle methodDefinitionHandle = MetadataTokens.MethodDefinitionHandle(methodInfo.MetadataToken);

			AppDomain appDomain = new();
			appDomain.Load(methodInfo.DeclaringType!.Assembly.Location);

			X86JIT jit = new();

			delegate* unmanaged<void> method = jit.LoadMethod(appDomain, appDomain.GetMethod());
			if (typeof(TOut) == typeof(ValueTuple)) //"Void"
			{
				method();

				return default;
			}

			if (typeof(TOut) == typeof(int))
			{
				return (TOut)(object)((delegate* unmanaged<int>)method)();
			}
			else if (typeof(TOut) == typeof(long))
			{
				return (TOut)(object)((delegate* unmanaged<long>)method)();
			}
			else
			{
				throw new NotImplementedException("Bad type: " + typeof(TOut));
			}
		}
	}
}
