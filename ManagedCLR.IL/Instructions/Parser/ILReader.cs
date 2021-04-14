using System;
using System.Reflection.Metadata;
using ManagedCLR.IL.Extensions;
using ManagedCLR.IL.Instructions.Raw;

namespace ManagedCLR.IL.Instructions.Parser
{
	public abstract class ILReader
	{
		public void Read(MethodBodyBlock body)
		{
			BlobReader ilReader = body.GetILReader();

			this.Read(ref ilReader);
		}

		public void Read(ref BlobReader reader)
		{
			this.ReadStart();

			while (reader.RemainingBytes > 0)
			{
				int offset = reader.Offset;

				ILOpCode opCode = reader.DecodeILOpCode();
				switch (opCode)
				{
					case ILOpCode.Nop:
						this.Read(NopInstruction.Nop(), offset);
						break;

					case ILOpCode.Ldarg_0:
						this.Read(LoadArgumentInstruction.FromIndex(0), offset);
						break;
					case ILOpCode.Ldarg_1:
						this.Read(LoadArgumentInstruction.FromIndex(1), offset);
						break;

					case ILOpCode.Ldloc_0:
						this.Read(LoadLocalInstruction.FromIndex(0), offset);
						break;
					case ILOpCode.Ldloc_1:
						this.Read(LoadLocalInstruction.FromIndex(1), offset);
						break;

					case ILOpCode.Stloc_0:
						this.Read(StoreLocalInstruction.ToIndex(0), offset);
						break;
					case ILOpCode.Stloc_1:
						this.Read(StoreLocalInstruction.ToIndex(1), offset);
						break;

					case ILOpCode.Ldc_i4_0:
						this.Read(PushInt32Instruction.Constant(0), offset);
						break;
					case ILOpCode.Ldc_i4_1:
						this.Read(PushInt32Instruction.Constant(1), offset);
						break;
					case ILOpCode.Ldc_i4_2:
						this.Read(PushInt32Instruction.Constant(2), offset);
						break;
					case ILOpCode.Ldc_i4_3:
						this.Read(PushInt32Instruction.Constant(3), offset);
						break;
					case ILOpCode.Ldc_i4_s:
						this.Read(PushInt32Instruction.Constant(reader.ReadSByte()), offset);
						break;

					case ILOpCode.Ldc_i4:
						this.Read(PushInt32Instruction.Constant(reader.ReadInt32()), offset);
						break;
					case ILOpCode.Ldc_i8:
						this.Read(PushInt64Instruction.Constant(reader.ReadInt64()), offset);
						break;

					case ILOpCode.Br_s:
						this.Read(BranchInstruction.ILIndex(reader.ReadSByte() + reader.Offset), offset);
						break;
					case ILOpCode.Bgt_s:
						this.Read(BranchGreaterInstruction.ILIndex(reader.ReadSByte() + reader.Offset), offset);
						break;
					case ILOpCode.Brfalse_s:
						this.Read(BranchFalseInstruction.ILIndex(reader.ReadSByte() + reader.Offset), offset);
						break;

					case ILOpCode.Call:
						this.Read(CallInstruction.Handle(reader.ReadInt32()), offset);
						break;

					case ILOpCode.Pop:
						this.Read(PopInstruction.Pop(), offset);
						break;
					case ILOpCode.Ret:
						this.Read(ReturnInstruction.Return(), offset);
						break;

					case ILOpCode.Add:
						this.Read(AddInstruction.Add(), offset);
						break;
					case ILOpCode.Sub:
						this.Read(SubtractInstruction.Subtract(), offset);
						break;

					default:
						throw new NotSupportedException("Opcode: " + opCode);
				}
			}

			this.ReadEnd();
		}

		protected abstract void Read<T>(T instruction, int offset) where T : struct, ILInstruction;

		protected abstract void ReadStart();
		protected abstract void ReadEnd();
	}
}
