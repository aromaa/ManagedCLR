using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using ManagedCLR.Runtime.Type.Method;
using AppDomain = ManagedCLR.Runtime.Domains.AppDomain;

namespace ManagedCLR.JIT.x86
{
	public sealed class X86JIT : BaseJIT
	{
		[DllImport("kernel32.dll")]
		static extern IntPtr VirtualAlloc(IntPtr lpAddress,
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

		internal unsafe IntPtr Allocate(BlobWriter writer)
		{
			IntPtr buffer = X86JIT.VirtualAlloc(default, 1024, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);

			Span<byte> bufferSpan = new((void*)buffer, 1024);
			writer.ToArray().CopyTo(bufferSpan);

			return buffer;
		}

		private IntPtr CompileMethod(TypeMethodHandle method, AppDomain appDomain)
		{
			X86CompilerTask task = new(this, appDomain, method);
			task.Compile();

			return this.Allocate(task.writer);
		}
	}
}
