using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using ManagedCLR.IL.Methods;
using ManagedCLR.JIT;
using ManagedCLR.Runtime.Assembly.Loader;

namespace ManagedCLR.Runtime.Type.Method
{
	public sealed class TypeMethodHandle
	{
		[DllImport("kernel32.dll")]
		static extern nuint VirtualAlloc(IntPtr lpAddress,
		                                 int dwSize,
		                                 AllocationType flAllocationType,
		                                 MemoryProtection flProtect);

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

		public AssemblyLoader Loader { get; init; }
		public ILMethodDefinition IL { get; }

		public BaseJIT Jit { get; }

		internal PinnedData ClrData { get; }

		public ImmutableArray<TypeHandle> Locals { get; init; }

		public TypeMethodHandle(BaseJIT jit, uint slot, ILMethodDefinition il)
		{
			this.Jit = jit;

			this.IL = il;

			nuint stub = TypeMethodHandle.VirtualAlloc(default, 1024, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);

			jit.WriteStub(slot, stub, il.MaxStack + il.ArgumentsCount);

			this.ClrData = new PinnedData()
			{
				Slot = slot,

				codeSection = stub
			};

			GCHandle.Alloc(this.ClrData, GCHandleType.Pinned);
		}

		public nuint EntryPointer
		{
			get => this.ClrData.codeSection;
			set => this.ClrData.codeSection = value;
		}

		public ref nuint EntryPointerRef => ref this.ClrData.codeSection;

		[StructLayout(LayoutKind.Sequential)]
		internal sealed class PinnedData
		{
			internal uint Slot { get; init; }

			internal nuint codeSection;
		}
	}
}
