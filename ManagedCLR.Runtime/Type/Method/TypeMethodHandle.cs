using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
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
		
		private readonly UnmanagedDataHolder unmanagedData;

		public AssemblyLoader Loader { get; init; }
		public ILMethodDefinition IL { get; }

		public BaseJIT Jit { get; }

		public ImmutableArray<TypeHandle> Locals { get; init; }

		public TypeMethodHandle(BaseJIT jit, ILMethodDefinition il)
		{
			this.unmanagedData = new UnmanagedDataHolder(this);

			this.Jit = jit;

			this.IL = il;

			nuint stub = TypeMethodHandle.VirtualAlloc(default, 1024, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);

			jit.WriteStub((nuint)(nint)this.PinnedData.Handle, stub, il.MaxStack + il.ArgumentsCount);

			this.EntryPointer = stub;
		}

		private ref PinnedDataHolder PinnedData => ref this.unmanagedData.PinnedData;

		public nuint EntryPointer
		{
			get => this.PinnedData.codeSection;
			set => this.PinnedData.codeSection = value;
		}

		public ref nuint EntryPointerRef => ref this.PinnedData.codeSection;

		private readonly unsafe struct UnmanagedDataHolder
		{
			private readonly PinnedDataHolder* pinnedData;

			internal UnmanagedDataHolder(TypeMethodHandle handle)
			{
				//Well... We can't allocate a single non-array objects in the POH directly
				//And the API doesn't directly allow reference objects in the POH arrays (however this is supported by the GC and can be easily worked around)
				//But we don't want to fragment the gen 0 heap, so instead, lets create a length 1 array in the POH with blittable struct which contains the pinned data
				ref PinnedDataHolder pinnedData = ref MemoryMarshal.GetArrayDataReference(GC.AllocateArray<PinnedDataHolder>(length: 1, pinned: true));

				pinnedData = new PinnedDataHolder(handle);

				this.pinnedData = (PinnedDataHolder*)Unsafe.AsPointer(ref pinnedData);
			}

			internal ref PinnedDataHolder PinnedData => ref Unsafe.AsRef<PinnedDataHolder>(this.pinnedData);
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct PinnedDataHolder
		{
			internal GCHandle Handle { get; }

			internal nuint codeSection;

			internal PinnedDataHolder(TypeMethodHandle handle) : this()
			{
				this.Handle = GCHandle.Alloc(handle);
			}
		}
	}
}
