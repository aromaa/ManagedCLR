using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ManagedCLR.Runtime.Type.Method;
using X86 = ManagedCLR.JIT.x86.Assembly.Instructions;
using AppDomain = ManagedCLR.Runtime.Domains.AppDomain;

namespace ManagedCLR.JIT.x86
{
	public sealed class X86JIT : BaseJIT
	{
		//static X86JIT()
		//{
		//	RuntimeHelpers.PrepareMethod(typeof(X86JIT).GetMethod("WriteStub").MethodHandle); //Prepare the method, otherwise the stub somehow fucks up everything, dunno why
		//}

		[DllImport("kernel32.dll")]
		static extern nuint VirtualAlloc(IntPtr lpAddress,
		                                 int dwSize,
		                                 AllocationType flAllocationType,
		                                 MemoryProtection flProtect);

		[DllImport("kernel32.dll")]
		static extern bool VirtualProtect(IntPtr lpAddress,
										  UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

		[Flags]
		public enum AllocationType
		{
			Commit = 0x1000,
			Reserve = 0x2000,
			Decommit = 0x4000,
			Release = 0x8000,
			Reset = 0x80000,
			Physical = 0x400000,
			TopDown = 0x100000,
			WriteWatch = 0x200000,
			LargePages = 0x20000000
		}

		[Flags]
		public enum MemoryProtection
		{
			Execute = 0x10,
			ExecuteRead = 0x20,
			ExecuteReadWrite = 0x40,
			ExecuteWriteCopy = 0x80,
			NoAccess = 0x01,
			ReadOnly = 0x02,
			ReadWrite = 0x04,
			WriteCopy = 0x08,
			GuardModifierflag = 0x100,
			NoCacheModifierflag = 0x200,
			WriteCombineModifierflag = 0x400
		}

		public unsafe delegate* unmanaged<void> LoadMethod(AppDomain appDomain, TypeMethodHandle method)
		{
			return (delegate* unmanaged<void>)this.CompileMethod(method, appDomain);
		}

		internal unsafe nuint Allocate(BlobWriter writer)
		{
			nuint buffer = X86JIT.VirtualAlloc(default, 1024, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);

			Span<byte> bufferSpan = new((void*)buffer, 1024);
			writer.ToArray().CopyTo(bufferSpan);

			return buffer;
		}

		private nuint CompileMethod(TypeMethodHandle method, AppDomain appDomain)
		{
			X86CompilerTask task = new(this, appDomain, method);
			task.Compile();

			return this.Allocate(task.writer);
		}

		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		public static nuint JitCallback(nuint slot)
		{
			TypeMethodHandle handle = AppDomain.Instance.TypeLoader.GetMethodSlot(slot);

			X86CompilerTask task = new((X86JIT)handle.Jit, AppDomain.Instance, handle);
			task.Compile();

			handle.EntryPointer = ((X86JIT)handle.Jit).Allocate(task.writer);

			return handle.EntryPointer;
		}
		
		public override unsafe void WriteStub(nuint slot, nuint memory, int maxStack)
		{
			BlobWriter writer = new(new byte[1024]);
			
			delegate* unmanaged[Stdcall]<nuint, nuint> callback = &JitCallback;

			//Prep stub stack
			X86.PushInstruction.PushRbp().Write(ref writer);
			X86.MoveInstruction.RegisterToRegister(4, 5).Write(ref writer);

			//Stub stack push
			X86.PushInstruction.FromRegister(3).Write(ref writer);
			X86.PushInstruction.Constant((int)slot).Write(ref writer);

			//Stub call
			X86.MoveInstruction.ConstTo((int)callback, 3).Write(ref writer);
			X86.CallInstruction.NearAbsolute(3).Write(ref writer);

			//Get back the original stack
			X86.PopInstruction.ToRegister(3).Write(ref writer);
			X86.PopInstruction.PopRbp().Write(ref writer);

			//Call method
			X86.JumpInstruction.JumpNearAbsolute(0).Write(ref writer);

			//Not actually never called
			X86.ReturnInstruction.NearReturnPop(0).Write(ref writer);

			Span<byte> bufferSpan = new((void*)memory, 1024);
			writer.ToArray().CopyTo(bufferSpan);

			Console.WriteLine("Stub: " + BitConverter.ToString(writer.ToArray()).Replace("-", ""));
		}
	}
}
