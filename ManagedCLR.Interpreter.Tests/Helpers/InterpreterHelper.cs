using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using ManagedCLR.Tests.Helpers;
using AppDomain = ManagedCLR.Runtime.Domains.AppDomain;

namespace ManagedCLR.Interpreter.Tests.Helpers
{
	internal sealed class InterpreterHelper : IAssertHelper
	{
		internal static readonly InterpreterHelper Instance = new();

		public void Result(Action method) => InterpreterHelper.InterpretInternal<ValueTuple>(method.Method);

		public TOut Result<TOut>(Func<TOut> method) => InterpreterHelper.InterpretInternal<TOut>(method.Method);
		public TOut Result<T1, TOut>(Func<T1, TOut> method, T1 arg1) => InterpreterHelper.InterpretInternal<TOut>(method.Method, arg1);
		public TOut Result<T1, T2, TOut>(Func<T1, T2, TOut> method, T1 arg1, T2 arg2) => InterpreterHelper.InterpretInternal<TOut>(method.Method, arg1, arg2);

		private static TOut InterpretInternal<TOut>(MethodInfo methodInfo, params object[] arguments)
		{
			MethodDefinitionHandle methodDefinitionHandle = MetadataTokens.MethodDefinitionHandle(methodInfo.MetadataToken);

			AppDomain appDomain = new();

			appDomain.Load(methodInfo.DeclaringType!.Assembly.Location);

			ILInterpreter ilInterpreter = new();
			throw new NotSupportedException();
			//ilInterpreter.Interpret(appDomain, appDomain.GetMethod(), arguments);

			if (typeof(TOut) == typeof(ValueTuple)) //"Void"
			{
				return default;
			}

			return (TOut)ilInterpreter.GetStackValue();
		}
	}
}
