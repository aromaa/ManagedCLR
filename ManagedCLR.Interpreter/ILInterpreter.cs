using System;
using System.Collections.Generic;
using ManagedCLR.IL.Instructions;
using ManagedCLR.IL.Instructions.Raw;
using ManagedCLR.IL.Methods;
using AppDomain = ManagedCLR.Runtime.Domains.AppDomain;

namespace ManagedCLR.Interpreter
{
	public sealed class ILInterpreter
	{
		private Queue<object> Stack = new Queue<object>();
		
		public void Interpret(AppDomain appDomain, ILMethodDefinition method, params object[] arguments)
		{
			object[] locals = new object[20];

			for(int i = 0; i < method.Instructions.Length; i++)
			{
				ILInstruction instruction = method.Instructions[i];

				switch (instruction)
				{
					case NopInstruction nop:
						break;
					//case CallInstruction call:
					//	ILMethodDefinition callMethod = appDomain.Loader.ReadMethod(call.Target);

					//	object[] args = new object[callMethod.ArgumentsCount];
					//	for (int j = 0; j < args.Length; j++)
					//	{
					//		args[j] = this.Stack.Dequeue();
					//	}

					//	this.Interpret(appDomain, callMethod, args);

					//	break;
					case PushInt32Instruction push:
						this.Stack.Enqueue(push.Value);
						break;
					case PushInt64Instruction push:
						this.Stack.Enqueue(push.Value);
						break;
					case StoreLocalInstruction store:
						locals[store.Index] = this.Stack.Dequeue();
						break;
					case BranchInstruction branch:
						i = method.ILOffsets[branch.ToIndex] - 1;
						break;
					case LoadLocalInstruction load:
						this.Stack.Enqueue(locals[load.Index]);
						break;
					case ReturnInstruction @return:
						return;
					case LoadArgumentInstruction argument:
						this.Stack.Enqueue(arguments[argument.Index]);
						break;
					case BranchFalseInstruction branch:
						object value = this.Stack.Dequeue();
						if (value is int number && number == 0)
						{
							i = method.ILOffsets[branch.ToIndex] - 1;
						}
						break;
					case AddInstruction add:
						{
							int num = (int)this.Stack.Dequeue();
							int num2 = (int)this.Stack.Dequeue();

							this.Stack.Enqueue(num + num2);
							break;
						}
					case SubtractInstruction sub:
						{
							int num = (int)this.Stack.Dequeue();
							int num2 = (int)this.Stack.Dequeue();

							this.Stack.Enqueue(num - num2);
							break;
						}
					case BranchGreaterInstruction greater:
						{
							break;
						}
					default:
						throw new InvalidOperationException("Wtf is tihs: " + instruction.GetType());
				}
			}

			throw new InvalidOperationException("not like tihs");
		}

		public object GetStackValue() => this.Stack.Dequeue();
	}
}
