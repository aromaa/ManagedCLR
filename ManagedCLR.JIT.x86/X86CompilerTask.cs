using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using ManagedCLR.IL;
using ManagedCLR.IL.Instructions;
using ManagedCLR.Runtime.Type;
using ManagedCLR.Runtime.Type.Method;
using CIL = ManagedCLR.IL.Instructions.Raw;
using X86 = ManagedCLR.JIT.x86.Assembly.Instructions;
using AppDomain = ManagedCLR.Runtime.Domains.AppDomain;

namespace ManagedCLR.JIT.x86
{
	internal sealed class X86CompilerTask : ILConsumer
	{
		private readonly X86JIT jit;

		private readonly AppDomain appDomain;
		private readonly TypeMethodHandle method;

		internal BlobWriter writer; //Mutating struct

		private Dictionary<int, int> blocksStart;
		private Dictionary<int, (int, BranchType)> backpatching;

		private int localsSize = 0;

		internal X86CompilerTask(X86JIT jit, AppDomain appDomain, TypeMethodHandle method)
		{
			this.jit = jit;

			this.appDomain = appDomain;
			this.method = method;

			this.writer = new BlobWriter(new byte[1024]);

			this.blocksStart = new Dictionary<int, int>();
			this.backpatching = new Dictionary<int, (int, BranchType)>();
		}

		public void Compile()
		{
			X86.PushInstruction.PushRbp().Write(ref this.writer);
			X86.MoveInstruction.RegisterToRegister(4, 5).Write(ref this.writer);

			X86.SubtractInstruction.ConstantFrom(this.method.IL.MaxStack * 4, 4).Write(ref this.writer);

			foreach ((int offset, BasicBlock block) in this.method.IL.Blocks)
			{
				this.blocksStart.Add(offset, this.writer.Offset);

				foreach (ILInstruction instruction in block.instructions)
				{
					instruction.Consume(this);
				}
			}

			int end = this.writer.Offset;

			foreach ((int offset, (int jumpTo, BranchType type)) in this.backpatching)
			{
				this.writer.Offset = offset;

				int jumpTarget = this.blocksStart[jumpTo] - this.writer.Offset - 2;

				if (type == BranchType.Unconditional)
				{
					X86.JumpInstruction.JumpShort(jumpTarget).Write(ref this.writer);
				}
				else if (type == BranchType.Grater)
				{
					X86.JumpGreaterInstruction.JumpTo(jumpTarget).Write(ref this.writer);
				}
			}

			this.writer.Offset = end;

			Console.WriteLine("Method: " + BitConverter.ToString(this.writer.ToArray()).Replace("-", ""));
		}

		public void Consume<T>(in T instruction) where T : ILInstruction
		{
			if (instruction is CIL.NopInstruction nop)
			{
				//Nothing for now
			}
			else if (instruction is CIL.PushInt32Instruction push32)
			{
				X86.PushInstruction.Constant(push32.Value).Write(ref this.writer);
			}
			else if (instruction is CIL.StoreLocalInstruction storeLocal)
			{
				X86.PopInstruction.ToRegister(storeLocal.Index).Write(ref this.writer);
			}
			else if (instruction is CIL.BranchInstruction branch)
			{
				this.backpatching.Add(this.writer.Offset, (branch.ToIndex, BranchType.Unconditional));

				X86.JumpInstruction.JumpShort(0).Write(ref this.writer);
			}
			else if (instruction is CIL.LoadLocalInstruction loadLocal)
			{
				X86.PushInstruction.FromRegister(loadLocal.Index).Write(ref this.writer);
			}
			else if (instruction is CIL.ReturnInstruction @return)
			{
				X86.PopInstruction.ToRegister(0).Write(ref this.writer);
				X86.AddInstruction.ConstantFrom(this.method.IL.MaxStack * 4, 4).Write(ref this.writer);
				X86.PopInstruction.PopRbp().Write(ref this.writer);
				X86.ReturnInstruction.NearReturnPop(this.method.IL.ArgumentsCount * 4).Write(ref this.writer);
			}
			else if (instruction is CIL.CallInstruction call)
			{
				TypeMethodHandle callMethod = this.method.Loader.ReadMethod(this.jit, call.Target);

				unsafe
				{
					X86.MoveInstruction.Indirect((int)Unsafe.AsPointer(ref callMethod.EntryPointerRef)).Write(ref this.writer);
				}

				X86.CallInstruction.NearAbsolute(0).Write(ref this.writer);

				//Return val
				X86.PushInstruction.FromRegister(0).Write(ref this.writer);
			}
			else if (instruction is CIL.PopInstruction pop)
			{
				X86.PopInstruction.ToRegister(0).Write(ref this.writer);
			}
			else if (instruction is CIL.LoadArgumentInstruction loadArgument)
			{
				X86.PushInstruction.FromRbp((2 + loadArgument.Index) * 4).Write(ref this.writer);
			}
			else if (instruction is CIL.AddInstruction add)
			{
				X86.PopInstruction.ToRegister(0).Write(ref this.writer);
				X86.PopInstruction.ToRegister(1).Write(ref this.writer);

				X86.AddInstruction.Register(1, 0).Write(ref this.writer);
				X86.PushInstruction.FromRegister(0).Write(ref this.writer);
			}
			else if (instruction is CIL.BranchGreaterInstruction branchGreater)
			{
				X86.PopInstruction.ToRegister(0).Write(ref this.writer);
				X86.PopInstruction.ToRegister(1).Write(ref this.writer);

				X86.CompareInstruction.Registers(0, 1).Write(ref this.writer);

				this.backpatching.Add(this.writer.Offset, (branchGreater.ToIndex, BranchType.Grater));

				X86.JumpGreaterInstruction.JumpTo(0).Write(ref this.writer);
			}
			else if (instruction is CIL.LoadLocalAddressInstruction loadLocalAddress)
			{
				X86.LoadEffectiveAddressInstruction.Ebp((loadLocalAddress.Index - 1) * 4).Write(ref this.writer);
				X86.PushInstruction.FromRegister(1).Write(ref this.writer);
			}
			else if (instruction is CIL.InitObjectInstruction initObject)
			{
				X86.PopInstruction.ToRegister(0).Write(ref this.writer);
			}
			else if (instruction is CIL.LoadFieldValueInstruction loadFieldValue)
			{
				FieldDefinition field = this.method.Loader.metadata.GetFieldDefinition(loadFieldValue.Target);

				TypeHandle type = this.appDomain.TypeLoader.LoadType(this.method.Loader.metadata, this.method.Loader.metadata.GetTypeDefinition(field.GetDeclaringType()));

				X86.PopInstruction.ToRegister(0).Write(ref this.writer);
				X86.PushInstruction.FromRax(type.Offsets[this.method.Loader.metadata.GetString(field.Name)]).Write(ref this.writer);
			}
			else if (instruction is CIL.SaveFieldValueInstruction saveFieldValue)
			{
				FieldDefinition field = this.method.Loader.metadata.GetFieldDefinition(saveFieldValue.Target);

				TypeHandle type = this.appDomain.TypeLoader.LoadType(this.method.Loader.metadata, this.method.Loader.metadata.GetTypeDefinition(field.GetDeclaringType()));

				X86.PopInstruction.ToRegister(3).Write(ref this.writer);
				X86.PopInstruction.ToRegister(0).Write(ref this.writer);
				X86.MoveInstruction.ToEax(type.Offsets[this.method.Loader.metadata.GetString(field.Name)]).Write(ref this.writer);
			}
			else
			{
				throw new InvalidOperationException($"Unknown IL: {typeof(T)}");
			}
		}

		private enum BranchType
		{
			Unconditional,
			Grater,
		}
	}
}
